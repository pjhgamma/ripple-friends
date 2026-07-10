using MonoMod.Cil;
using MoreSlugcats;
using static RippleFriends.Utils.ILUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Items;

internal class FireEggHooks : DownpourHooks
{
    protected override bool IsOptionEnabled => Config.FireEgg.Value;

    [HookPatch(typeof(IL.MoreSlugcats.FireEgg), nameof(IL.MoreSlugcats.FireEgg.Update))]
    private static void IL_FireEgg_Update(ILContext il)
    {
        IL_Return_Creature<FireEgg>(il);
    }

    [HookPatch(typeof(IL.MoreSlugcats.FireEgg), nameof(IL.MoreSlugcats.FireEgg.Collide))]
    private static void IL_FireEgg_Collide(ILContext il)
    {
        IL_Return_Creature<FireEgg>(il);
    }
}
