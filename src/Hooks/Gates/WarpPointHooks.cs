using Mono.Cecil.Cil;
using MonoMod.Cil;
using static RippleFriends.Hooks.Tracker.FriendTrackerHooks;
using static RippleFriends.Utils.PlayerUtils;
using RippleFriends.Options;
using RWCustom;
using Watcher;

namespace RippleFriends.Hooks.Gates;

internal class WarpPointHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.WarpPoint.Value;

    private static bool WithinWarp(WarpPoint warpPoint, Creature creature)
    {
        return creature.room == warpPoint.room && Custom.DistLess(warpPoint.pos, creature.mainBodyChunk?.pos ?? new(), warpPoint.PullRadius * Config.WarpPointRadius.Value);
    }

    [HookPatch(typeof(IL.Watcher.WarpPoint), nameof(IL.Watcher.WarpPoint.Update))]
    private static void IL_WarpPoint_Update(ILContext il)
    {
        ILCursor c = new(il);
        var l = c.DefineLabel();

        if (c.TryGotoNext(
            i => i.MatchLdarg(0),
            i => i.MatchLdfld<WarpPoint>("triggerTime"),
            i => i.MatchLdarg(0),
            i => i.MatchLdfld<WarpPoint>("triggerActivationTime"),
            i => i.MatchBltUn(out l)
        ) && c.TryGotoPrev(
            i => i.MatchLdloc(out _),
            i => i.MatchLdfld<UpdatableAndDeletable>("room"),
            i => i.MatchBrfalse(out _)
        ))
        {
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate((WarpPoint warpPoint) =>
            {
                if (warpPoint.strongPull)
                {
                    return true;
                }

                var normal = true;
                var force = true;

                foreach (var abstractPlayer in warpPoint.room?.game?.AlivePlayers ?? [])
                {
                    if (abstractPlayer?.realizedCreature is Player player)
                    {
                        if (!WithinWarp(warpPoint, player))
                        {
                            normal = false;
                            force = false;

                            break;
                        }
                        if (!IsIdlePlayer(player, Config.WarpPointTime.Value))
                        {
                            normal = false;
                        }
                        if (!IsIdlePlayer(player, Config.WarpPointTime.Value + Config.WarpPointForceTime.Value))
                        {
                            force = false;
                        }
                    }
                }

                foreach (var abstractCreature in warpPoint.room?.abstractRoom?.creatures ?? [])
                {
                    if (abstractCreature?.realizedCreature is Creature creature)
                    {
                        foreach (var abstractPlayer in warpPoint.room?.game?.Players ?? [])
                        {
                            if (abstractPlayer?.realizedCreature is Player player && IsFriend(creature, player))
                            {
                                if (!WithinWarp(warpPoint, creature))
                                {
                                    normal = false;
                                }

                                break;
                            }
                        }
                    }
                }

                if (normal || force)
                {
                    warpPoint.triggerTime = warpPoint.triggerActivationTime;

                    return true;
                }

                warpPoint.triggerTime = 0f;

                return false;
            });
            c.Emit(OpCodes.Brfalse, l);
        }
    }
}
