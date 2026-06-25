using MonoMod.Cil;
using static RippleFriends.Core.ILUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.Items;

internal class PomegranateHooks : WatcherHooks
{
    protected override bool IsOptionEnabled => Config.Pomegranate.Value;

    [HookPatch(typeof(IL.Pomegranate), nameof(IL.Pomegranate.Collide))]
    private static void IL_Pomegranate_Collide(ILContext il)
    {
        IL_Return_Creature<Pomegranate>(il);
    }
}
