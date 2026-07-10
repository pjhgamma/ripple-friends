using MonoMod.Cil;
using static RippleFriends.Hooks.Tracker.OwnerTrackerHooks;
using static RippleFriends.Utils.ILUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.General;

internal class ExplosionHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.Explosion.Value;

    [HookPatch(typeof(IL.Explosion), nameof(IL.Explosion.Update))]
    private static void IL_Explosion_Update(ILContext il)
    {
        IL_Branch_Ripple(il);
    }

    [HookPatch(typeof(On.Explosion), nameof(On.Explosion.ctor))]
    private static void On_Explosion_ctor(On.Explosion.orig_ctor orig, Explosion self, Room room, PhysicalObject sourceObject, UnityEngine.Vector2 pos, int lifeTime, float rad, float force, float damage, float stun, float deafen, Creature killTagHolder, float killTagHolderDmgFactor, float minStun, float backgroundNoise)
    {
        ChainOwner(sourceObject, self);
        ChainOwner(sourceObject, sourceObject);

        orig(self, room, sourceObject, pos, lifeTime, rad, force, damage, stun, deafen, killTagHolder, killTagHolderDmgFactor, minStun, backgroundNoise);
    }

    [HookPatch(typeof(IL.ExplosiveSpear), nameof(IL.ExplosiveSpear.HitByExplosion))]
    private static void IL_ExplosiveSpear_HitByExplosion(ILContext il)
    {
        IL_Explosion_HitByExplosion<ExplosiveSpear>(il, "Ignite");
    }

    [HookPatch(typeof(IL.ScavengerBomb), nameof(IL.ScavengerBomb.HitByWeapon))]
    private static void IL_ScavengerBomb_HitByWeapon(ILContext il)
    {
        IL_Explosion_HitByWeapon<ScavengerBomb>(il, "InitiateBurn");
    }

    [HookPatch(typeof(IL.ScavengerBomb), nameof(IL.ScavengerBomb.HitByExplosion))]
    private static void IL_ScavengerBomb_HitByExplosion(ILContext il)
    {
        IL_Explosion_HitByExplosion<ScavengerBomb>(il, "InitiateBurn");
    }
}
