using Mono.Cecil.Cil;
using MonoMod.Cil;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Players;

internal class GourmandSlamHooks : DownpourHooks
{
    protected override bool IsOptionEnabled => Config.GourmandSlam.Value;

    [HookPatch(typeof(IL.Player), nameof(IL.Player.Collide))]
    private static void IL_Player_Collide(ILContext il)
    {
        ILCursor c = new(il);
        var l = c.DefineLabel();

        if (c.TryGotoNext(
            i => i.MatchIsinst<Creature>()
        ) && c.TryGotoNext(
            i => i.MatchLdsfld<ModManager>("MSC"),
            i => i.MatchBrfalse(out l)
        ) && c.TryGotoPrev(
            i => i.MatchLdarg(0),
            i => i.MatchCall<Player>("get_isGourmand"),
            i => i.MatchBrtrue(out _)
        ))
        {
            c.Emit(OpCodes.Br, l);
        }
    }
}
