using MonoMod.Cil;
using static RippleFriends.Core.ILUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Players;

internal class SaintTongueHooks : DownpourHooks
{
    protected override bool IsOptionEnabled => Config.SaintTongue.Value;

    [HookPatch(typeof(IL.Player.Tongue), nameof(IL.Player.Tongue.Update))]
    private static void IL_Player_Tongue_Update(ILContext il)
    {
        IL_Tongue<Player.Tongue>(
            il,
            tongue => tongue?.player
        );
    }
}
