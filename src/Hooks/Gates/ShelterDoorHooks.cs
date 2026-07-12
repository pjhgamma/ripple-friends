using Mono.Cecil.Cil;
using MonoMod.Cil;
using static RippleFriends.Utils.GateUtils<ShelterDoor>;
using static RippleFriends.Utils.ShelterDoorUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Gates;

internal class ShelterDoorHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.ShelterDoor.Value;

    [HookPatch(typeof(IL.Player), nameof(IL.Player.Update))]
    private static void IL_Player_Update(ILContext il)
    {
        ILCursor c = new(il);
        var l = c.DefineLabel();

        if (c.TryGotoNext(
            MoveType.After,
            i => i.MatchLdfld<Player>("timeSinceInCorridorMode"),
            i => i.MatchLdcI4(10)
        ))
        {
            c.Prev.Operand = 0;

            if (c.TryGotoNext(
                MoveType.After,
                i => i.MatchLdcI4(20),
                i => i.MatchBr(out _),
                i => i.MatchLdcI4(40),
                i => i.MatchBle(out _)
            ))
            {
                l = c.MarkLabel();

                if (c.TryGotoPrev(
                    MoveType.After,
                    i => i.MatchLdarg(0),
                    i => i.MatchLdfld<Player>("readyForWin"),
                    i => i.MatchBrfalse(out _)
                ))
                {
                    c.Emit(OpCodes.Br, l);

                    if (c.TryGotoPrev(
                        MoveType.After,
                        i => i.MatchLdarg(0),
                        i => i.MatchLdfld<Player>("readyForWin"),
                        i => i.MatchBrfalse(out l)
                    ))
                    {
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate((Player player) => player.room?.shelterDoor is ShelterDoor shelterDoor && CanActivate(
                            shelterDoor,
                            shelterDoor.room?.game?.AlivePlayers,
                            IsInShelterDoor,
                            Config.ShelterDoorTime.Value,
                            Config.ShelterDoorForce.Value,
                            Config.ShelterDoorForceTime.Value
                        ));
                        c.Emit(OpCodes.Brfalse, l);
                    }
                }
            }
        }
    }
}

internal class ShelterDoorResonanceHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.ShelterDoorWarp.Value || Config.ShelterDoorRevival.Value;

    [HookPatch(typeof(IL.ShelterDoor), nameof(IL.ShelterDoor.Close))]
    private static void IL_ShelterDoor_Close(ILContext il)
    {
        ILCursor c = new(il);

        if (c.TryGotoNext(
            MoveType.After,
            i => i.MatchLdsfld<ModManager>("CoopAvailable")
        ))
        {
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate((ShelterDoor shelterDoor) =>
            {
                if (shelterDoor.closedFac == shelterDoor.closeSpeed)
                {
                    WarpAndRevive(shelterDoor, IsInShelterDoor, Config.ShelterDoorWarp.Value, Config.ShelterDoorRevival.Value);
                }
            });
        }
    }
}
