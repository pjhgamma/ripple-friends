using MonoMod.Cil;
using static RippleFriends.Utils.ILUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Items;

internal class FirecrackerPlantHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.FirecrackerPlant.Value;

    [HookPatch(typeof(IL.FirecrackerPlant), nameof(IL.FirecrackerPlant.PopLump))]
    private static void IL_FirecrackerPlant_PopLump(ILContext il)
    {
        IL_Branch_Ripple(il);
    }

    [HookPatch(typeof(IL.FirecrackerPlant), nameof(IL.FirecrackerPlant.HitByExplosion))]
    private static void IL_FirecrackerPlant_HitByExplosion(ILContext il)
    {
        IL_Explosion_HitByExplosion<FirecrackerPlant>(il, "Ignite");
    }

    [HookPatch(typeof(IL.JokeRifle), nameof(IL.JokeRifle.Use))]
    private static void IL_JokeRifle_Use(ILContext il)
    {
        IL_Branch_Ripple(il);
    }
}
