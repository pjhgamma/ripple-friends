using MonoMod.Cil;
using static RippleFriends.Core.ILUtils;
using RippleFriends.Options;

namespace RippleFriends.Hooks.General;

internal class CollisionHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.Collision.Value;

    [HookPatch(typeof(IL.Room), nameof(IL.Room.Update))]
    private static void IL_Room_Update(ILContext il)
    {
        IL_Branch_Ripple(il);
    }
}
