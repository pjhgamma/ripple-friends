using MonoMod.Cil;
using static RippleFriends.Utils.ILUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Players;

internal class SaintAttunementHooks : DownpourHooks
{
    protected override bool IsOptionEnabled => Config.SaintAttunement.Value;

    [HookPatch(typeof(IL.Player), nameof(IL.Player.ClassMechanicsSaint))]
    private static void IL_Player_ClassMechanicsSaint(ILContext il)
    {
        IL_Branch_Ripple(il);
    }
}
