using MonoMod.Cil;
using static RippleFriends.Hooks.Tracker.OwnerTrackerHooks;
using static RippleFriends.Utils.ILUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Items;

internal class TubeWormHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.TubeWorm.Value;

    [HookPatch(typeof(IL.TubeWorm.Tongue), nameof(IL.TubeWorm.Tongue.Update))]
    private static void IL_TubeWorm_Tongue_Update(ILContext il)
    {
        IL_Tongue<TubeWorm.Tongue>(
            il,
            tongue => GetGrabber(tongue.worm)
        );
    }
}
