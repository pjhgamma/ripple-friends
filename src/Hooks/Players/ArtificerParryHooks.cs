using MonoMod.Cil;
using static RippleFriends.Utils.ILUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Players;

internal class ArtificerParryHooks : DownpourHooks
{
    protected override bool IsOptionEnabled => Config.ArtificerParry.Value;

    [HookPatch(typeof(IL.Player), nameof(IL.Player.ClassMechanicsArtificer))]
    private static void IL_Player_ClassMechanicsArtificer(ILContext il)
    {
        IL_Branch_Ripple(il);
    }
}
