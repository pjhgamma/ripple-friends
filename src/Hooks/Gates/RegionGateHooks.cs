using Mono.Cecil.Cil;
using MonoMod.Cil;
using static RippleFriends.Hooks.Tracker.FriendTrackerHooks;
using static RippleFriends.Utils.GateUtils<RegionGate>;
using static RippleFriends.Utils.RegionGateUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Gates;

internal class RegionGateHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.RegionGate.Value;

    [HookPatch(typeof(On.RegionGate), nameof(On.RegionGate.PlayersStandingStill))]
    private static bool On_RegionGate_PlayersStandingStill(On.RegionGate.orig_PlayersStandingStill orig, RegionGate self)
    {
        self.startCounter = 60;

        return CanActivate(
            self,
            ModManager.CoopAvailable ? self.room?.game?.PlayersToProgressOrWin : self.room?.game?.Players,
            IsInRegionGate,
            Config.RegionGateTime.Value,
            Config.RegionGateForce.Value,
            Config.RegionGateForceTime.Value
        );
    }

    [HookPatch(typeof(On.RegionGate), nameof(On.RegionGate.AllPlayersThroughToOtherSide))]
    private static bool On_RegionGate_AllPlayersThroughToOtherSide(On.RegionGate.orig_AllPlayersThroughToOtherSide orig, RegionGate self)
    {
        foreach (var creature in GetRoomFriends(self.room))
        {
            foreach (var bodyChunk in creature?.bodyChunks ?? [])
            {
                if (bodyChunk != null && GetBodyChunkLeftTile(self, bodyChunk) > -8f && GetBodyChunkRightTile(self, bodyChunk) < Config.RegionGateTile.Value)
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
        var l = c.DefineLabel();

        if (c.TryGotoNext(
            i => i.MatchLdarg(0),
            i => i.MatchLdcI4(0),
            i => i.MatchStfld<RegionGate>("startCounter"),
            i => i.MatchBr(out l)
        ) && c.TryGotoPrev(
            MoveType.After,
            i => i.MatchLdarg(0),
            i => i.MatchCall<RegionGate>("PlayersStandingStill")
        ))
        {
            c.Next.Operand = l;
        }
    }
}

internal class RegionGateResonanceHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.RegionGateWarp.Value || Config.RegionGateRevival.Value;

    [HookPatch(typeof(IL.RegionGate), nameof(IL.RegionGate.Update))]
    private static void IL_RegionGate_Update(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel l = c.DefineLabel();

        if (c.TryGotoNext(
            i => i.MatchLdarg(0),
            i => i.MatchLdsfld<RegionGate.Mode>("ClosingAirLock"),
            i => i.MatchStfld<RegionGate>("mode")
        ))
        {
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate((RegionGate regionGate) => WarpAndRevive(regionGate, IsInRegionGate, Config.RegionGateWarp.Value, Config.RegionGateRevival.Value));
        }
    }
}
