using MonoMod.Cil;
using static RippleFriends.Hooks.Tracker.OwnerTrackerHooks;
using static RippleFriends.Utils.ILUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Items;

internal class JellyFishHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.JellyFish.Value;

    [HookPatch(typeof(IL.JellyFish), nameof(IL.JellyFish.Update))]
    private static void IL_JellyFish_Update(ILContext il)
    {
        IL_Branch_Ripple(il);
    }

    [HookPatch(typeof(IL.JellyFish), nameof(IL.JellyFish.Collide))]
    private static void IL_JellyFish_Collide(ILContext il)
    {
        IL_Return_Creature<JellyFish>(il);
    }

    [HookPatch(typeof(On.JellyFish), nameof(On.JellyFish.Update))]
    private static void On_JellyFish_Update(On.JellyFish.orig_Update orig, JellyFish self, bool eu)
    {
        orig(self, eu);

        if (GetGrabber(self) is Creature creature)
        {
            SetOwner(self, creature);
        }
        else if (!self.Electric)
        {
            SetOwner(self);
        }
    }

    [HookPatch(typeof(On.JellyFish), nameof(On.JellyFish.Tossed))]
    private static void On_JellyFish_Tossed(On.JellyFish.orig_Tossed orig, JellyFish self, Creature tossedBy)
    {
        SetOwner(self, tossedBy);

        orig(self, tossedBy);
    }
}
