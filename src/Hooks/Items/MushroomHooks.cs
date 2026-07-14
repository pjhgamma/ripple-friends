using static RippleFriends.Hooks.Tracker.FriendTrackerHooks;
using static RippleFriends.Utils.PlayerUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Items;

internal class MushroomHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.Mushroom.Value;

    [HookPatch(typeof(On.Player), nameof(On.Player.Update))]
    private static void On_Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        foreach (var abstractSlugcat in GetTrackedFriends(self.room?.game?.AlivePlayers, (abstractCreature) => IsSlugcat(abstractCreature) && IsFriend(abstractCreature, self)))
        {
            if (abstractSlugcat.realizedCreature is Player slugcat && !slugcat.dead && !slugcat.inShortcut)
            {
                self.mushroomCounter = Math.Max(slugcat.mushroomCounter, self.mushroomCounter - Convert.ToInt32(self.inShortcut));
            }
        }

        orig(self, eu);
    }
}
