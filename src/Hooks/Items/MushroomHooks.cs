using static RippleFriends.Hooks.Tracker.FriendTrackerHooks;
using static RippleFriends.Hooks.Tracker.OwnerTrackerHooks;
using static RippleFriends.Utils.PlayerUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Items;

internal class MushroomHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.Mushroom.Value;

    public static IEnumerable<AbstractCreature> GetOtherSlugcats(Player source)
    {
        foreach (var abstractCreature in GetTrackedFriends(source.room?.game?.Players, (abstractCreature) => IsSlugcat(abstractCreature) && IsFriend(abstractCreature, source)))
        {
            yield return abstractCreature;
        }
    }

    private static void MulticastMushroom(Player source)
    {
        foreach (var abstractCreature in GetOtherSlugcats(source))
        {
            if (abstractCreature.realizedCreature is Player player)
            {
                player.mushroomCounter += 320;
            }
        }
    }

    [HookPatch(typeof(On.Mushroom), nameof(On.Mushroom.BitByPlayer))]
    private static void On_Mushroom_BitByPlayer(On.Mushroom.orig_BitByPlayer orig, Mushroom self, Creature.Grasp grasp, bool eu)
    {
        if (grasp?.grabber is Player player)
        {
            MulticastMushroom(player);
        }

        orig(self, grasp, eu);
    }

    [HookPatch(typeof(On.Spear), nameof(On.Spear.HitSomethingWithoutStopping))]
    private static void On_Spear_HitSomethingWithoutStopping(On.Spear.orig_HitSomethingWithoutStopping orig, Spear self, PhysicalObject obj, BodyChunk chunk, PhysicalObject.Appendage appendage)
    {
        if (GetOwner(self)?.realizedCreature is Player player && obj is Mushroom)
        {
            MulticastMushroom(player);
        }

        orig(self, obj, chunk, appendage);
    }

    [HookPatch(typeof(On.Player), nameof(On.Player.Update))]
    private static void On_Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (!self.inShortcut)
        {
            return;
        }

        foreach (var abstractPlayer in GetOtherSlugcats(self))
        {
            if (abstractPlayer.realizedCreature is Player player && !player.inShortcut)
            {
                self.mushroomCounter = player.mushroomCounter;

                break;
            }
        }
    }
}
