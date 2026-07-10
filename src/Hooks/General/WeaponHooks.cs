using static RippleFriends.Hooks.Tracker.FriendTrackerHooks;
using RippleFriends.Options;

namespace RippleFriends.Hooks.General;

internal class WeaponHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.Weapon.Value;

    [HookPatch(typeof(On.Weapon), nameof(On.Weapon.HitThisObject))]
    private static bool On_Weapon_HitThisObject(On.Weapon.orig_HitThisObject orig, Weapon self, PhysicalObject obj)
    {
        if (IsFriend(self, obj))
        {
            return false;
        }
        return orig(self, obj);
    }
}
