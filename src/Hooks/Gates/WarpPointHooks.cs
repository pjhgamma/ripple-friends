using Mono.Cecil.Cil;
using MonoMod.Cil;
using static RippleFriends.Utils.GateUtils<Watcher.WarpPoint>;
using static RippleFriends.Utils.WarpPointUtils;
using RippleFriends.Options;
using Watcher;

namespace RippleFriends.Hooks.Gates;

internal class WarpPointHooks : WatcherHooks
{
    protected override bool IsOptionEnabled => Config.WarpPoint.Value;

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
                if (warpPoint.guaranteeTrigger)
                {
                    return true;
                }

                if (CanActivate(
                    warpPoint,
                    warpPoint.room?.game?.AlivePlayers,
                    IsInWarpPoint,
                    Config.WarpPointTime.Value,
                    Config.WarpPointForce.Value,
                    Config.WarpPointForceTime.Value
                ))
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

internal class WarpPointResonanceHooks : WatcherHooks
{
    protected override bool IsOptionEnabled => Config.WarpPointWarp.Value || Config.WarpPointRevival.Value;

    [HookPatch(typeof(IL.Watcher.WarpPoint), nameof(IL.Watcher.WarpPoint.Update))]
    private static void IL_WarpPoint_Update(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel l = c.DefineLabel();

        if (c.TryGotoNext(
            MoveType.After,
            i => i.MatchLdarg(0),
            i => i.MatchLdfld<WarpPoint>("triggerTime"),
            i => i.MatchLdarg(0),
            i => i.MatchLdfld<WarpPoint>("triggerActivationTime"),
            i => i.MatchBltUn(out l)
        ))
        {
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate((WarpPoint warpPoint) => WarpAndRevive(warpPoint, IsInWarpPoint, Config.WarpPointWarp.Value, Config.WarpPointRevival.Value));
        }
    }
}
