using Mono.Cecil.Cil;
using MonoMod.Cil;
using static RippleFriends.Core.PlayerUtils;
using static RippleFriends.Core.FriendTracker;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Gates;

internal class RegionGateHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.RegionGate.Value;

    public static IEnumerable<AbstractCreature?> GetFriendAbstractCreatures(UpdatableAndDeletable updatableAndDeletable)
    {
        var abstractPlayers = ModManager.CoopAvailable ? updatableAndDeletable.room?.game?.PlayersToProgressOrWin : updatableAndDeletable.room?.game?.Players;

        foreach (var abstractCreature in updatableAndDeletable.room?.abstractRoom?.creatures ?? [])
        {
            if (abstractCreature?.realizedCreature is Player)
            {
                yield return abstractCreature;
            }
            else
            {
                foreach (var abstractPlayer in abstractPlayers ?? [])
                {
                    if (abstractPlayer?.realizedCreature is Player player && IsFriend(abstractCreature?.realizedCreature, player))
                    {
                        yield return abstractCreature;

                        break;
                    }
                }
            }
        }
    }

    private static float GetTile(RegionGate regionGate, float x)
    {
        return (x / 20f - (regionGate.room?.TileWidth ?? 0) / 2) * (regionGate.letThroughDir ? 1 : -1);
    }

    private static float GetBodyChunkLeftTile(RegionGate regionGate, BodyChunk bodyChunk)
    {
        return GetTile(regionGate, bodyChunk.pos.x - bodyChunk.rad * (regionGate.letThroughDir ? 1 : -1));
    }

    private static float GetBodyChunkRightTile(RegionGate regionGate, BodyChunk bodyChunk)
    {
        return GetTile(regionGate, bodyChunk.pos.x + bodyChunk.rad * (regionGate.letThroughDir ? 1 : -1));
    }

    [HookPatch(typeof(On.RegionGate), nameof(On.RegionGate.PlayersStandingStill))]
    private static bool On_RegionGate_PlayersStandingStill(On.RegionGate.orig_PlayersStandingStill orig, RegionGate self)
    {
        var normal = true;
        var force = true;

        foreach (var abstractCreature in GetFriendAbstractCreatures(self))
        {
            foreach (var bodyChunk in abstractCreature?.realizedCreature?.bodyChunks ?? [])
            {
                if (bodyChunk == null)
                {
                    continue;
                }

                if (GetBodyChunkLeftTile(self, bodyChunk) < -8f)
                {
                    normal = false;

                    break;
                }
            }

            if (abstractCreature?.realizedCreature is Player player)
            {
                if (!IsIdlePlayer(player, Config.RegionGateTime.Value))
                {
                    normal = false;
                }
                if (!IsIdlePlayer(player, Config.RegionGateTime.Value + Config.RegionGateForceTime.Value))
                {
                    force = false;
                }
            }
        }

        return normal || force;
    }

    [HookPatch(typeof(On.RegionGate), nameof(On.RegionGate.AllPlayersThroughToOtherSide))]
    private static bool On_RegionGate_AllPlayersThroughToOtherSide(On.RegionGate.orig_AllPlayersThroughToOtherSide orig, RegionGate self)
    {
        foreach (var abstractCreature in GetFriendAbstractCreatures(self))
        {
            foreach (var bodyChunk in abstractCreature?.realizedCreature?.bodyChunks ?? [])
            {
                if (bodyChunk == null)
                {
                    continue;
                }

                if (GetBodyChunkLeftTile(self, bodyChunk) > -8f && GetBodyChunkRightTile(self, bodyChunk) < Config.RegionGateTile.Value)
                {
                    return false;
                }
            }
        }
        return true;
    }

    [HookPatch(typeof(IL.RegionGate), nameof(IL.RegionGate.Update))]
    private static void IL_RegionGate_Update(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel l = c.DefineLabel();

        if (c.TryGotoNext(
            MoveType.After,
            i => i.MatchLdarg(0),
            i => i.MatchLdfld<RegionGate>("startCounter"),
            i => i.MatchLdcI4(60),
            i => i.MatchBle(out _)
        ))
        {
            l = c.MarkLabel();

            if (c.TryGotoPrev(
                i => i.MatchLdarg(0),
                i => i.MatchCallvirt<RegionGate>("get_MeetRequirement"),
                i => i.MatchBrfalse(out _)
            ))
            {
                c.Emit(OpCodes.Br, l);
            }
        }
    }
}
