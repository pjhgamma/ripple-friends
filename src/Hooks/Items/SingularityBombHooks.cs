using MonoMod.Cil;
using static RippleFriends.Core.ILUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Items;

internal class SingularityBombHooks : DownpourHooks
{
    protected override bool IsOptionEnabled => Config.SaintAttunement.Value;

    [HookPatch(typeof(IL.MoreSlugcats.SingularityBomb), nameof(IL.MoreSlugcats.SingularityBomb.Update))]
    private static void IL_SingularityBomb_Update(ILContext il)
    {
        IL_Branch_Ripple(il);
    }

    [HookPatch(typeof(IL.MoreSlugcats.SingularityBomb), nameof(IL.MoreSlugcats.SingularityBomb.Explode))]
    private static void IL_SingularityBomb_Explode(ILContext il)
    {
        IL_Branch_Ripple(il);
    }
}
