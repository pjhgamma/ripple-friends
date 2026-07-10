using MonoMod.Cil;
using static RippleFriends.Hooks.Tracker.OwnerTrackerHooks;
using static RippleFriends.Utils.ILUtils;
using static RippleFriends.Utils.PlayerUtils;
using RippleFriends.Options;
using Mono.Cecil.Cil;

namespace RippleFriends.Hooks.Items;

internal class SnailHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.Snail.Value;

    [HookPatch(typeof(IL.Snail), nameof(IL.Snail.Click))]
    private static void IL_Snail_Click(ILContext il)
    {
        IL_Branch_Ripple(il);
    }

    [HookPatch(typeof(On.Snail), nameof(On.Snail.Click))]
    private static void On_Snail_Click(On.Snail.orig_Click orig, Snail self)
    {
        orig(self);

        if (self.triggerTicker <= 0)
        {
            SetOwner(self);
        }
    }

    [HookPatch(typeof(On.Player), nameof(On.Player.TossObject))]
    private static void On_Player_TossObject(On.Player.orig_TossObject orig, Player self, int grasp, bool eu)
    {
        if (self.grasps[grasp]?.grabbed is Snail snail)
        {
            SetOwner(snail, self);
        }

        orig(self, grasp, eu);
    }

    [HookPatch(typeof(IL.Snail), nameof(IL.Snail.Violence))]
    private static void IL_PhysicalObject_HitByWeapon(ILContext il)
    {
        ILCursor c = new(il);

        if (c.TryGotoNext(
            i => i.MatchLdarg(0),
            i => i.MatchLdcI4(1),
            i => i.MatchStfld<Snail>("triggered")
        ))
        {
            c.Emit(OpCodes.Ldarg_1);
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate((BodyChunk bodyChunk, Snail snail) => ChainOwner(bodyChunk.owner, snail));
        }
    }

    [HookPatch(typeof(On.Snail), nameof(On.Snail.Die))]
    private static void On_Snail_Die(On.Snail.orig_Die orig, Snail self)
    {
        if (IsSlugcat(self.killTag))
        {
            SetOwner(self, self.killTag);
        }

        orig(self);
    }
}
