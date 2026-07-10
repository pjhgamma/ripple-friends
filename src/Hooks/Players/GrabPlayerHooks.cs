using static RippleFriends.Utils.PlayerUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Players;

internal class GrabPlayerHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.GrabPlayer.Value;

    [HookPatch(typeof(On.Player), nameof(On.Player.CanIPickThisUp))]
    private static bool On_Player_CanIPickThisUp(On.Player.orig_CanIPickThisUp orig, Player self, PhysicalObject obj)
    {
        if (IsPlayer(obj) && obj is Player player && !player.dead && !IsIdlePlayer(player, Config.GrabPlayerTime.Value))
        {
            return false;
        }
        return orig(self, obj);
    }
}
