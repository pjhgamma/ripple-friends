using MoreSlugcats;
using RippleFriends.Options;
using System.Runtime.CompilerServices;
using Watcher;

namespace RippleFriends.Hooks.Tracker;

internal class OwnerTrackerHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.FriendSlugcat.Value || Config.FriendCreature.Value || Config.FriendNeutralCreature.Value || Config.FriendGrabbed.Value || Config.FriendIterator.Value;

    private static ConditionalWeakTable<UpdatableAndDeletable, AbstractCreature> _ownerCWT = new();

    public static Creature? GetGrabber(PhysicalObject? source)
    {
        return source?.grabbedBy is { Count: > 0 } grasps ? grasps[0].grabber : null;
    }

    public static AbstractCreature? GetOwner(UpdatableAndDeletable? source)
    {
        if (source == null)
        {
            return null;
        }

        Creature? owner = source switch
        {
            PhysicalObject physicalObject when Config.FriendGrabbed.Value && GetGrabber(physicalObject) is { } grabber => grabber,
            Weapon weapon => weapon.thrownBy,
            Explosion explosion => explosion.killTagHolder,
            SporePlant.Bee bee => GetOwner(bee.owner)?.realizedCreature,
            FireEgg fireEgg => fireEgg.thrownBy,
            Pomegranate pomegranate => pomegranate.killTagHolder,
            Frog frog => frog.thrownBy ?? GetGrabber(frog),
            Player player => player,
            _ => null
        };

        return owner?.abstractCreature ?? (_ownerCWT.TryGetValue(source, out var abstractCreature) ? abstractCreature : null);
    }

    private static void SetOwner(UpdatableAndDeletable? source, object? target = null)
    {
        if (source == null)
        {
            return;
        }

        _ownerCWT.Remove(source);

        if (target is Creature creature)
        {
            target = creature.abstractCreature;
        }
        if (target is AbstractCreature abstractCreature)
        {
            _ownerCWT.Add(source, abstractCreature);
        }
    }

    public static void SetOwner(UpdatableAndDeletable? source) => SetOwner(source, (object?)null);

    public static void SetOwner(UpdatableAndDeletable? source, AbstractCreature? target) => SetOwner(source, (object?)target);

    public static void SetOwner(UpdatableAndDeletable? source, Creature? target) => SetOwner(source, (object?)target);

    public static void ChainOwner(UpdatableAndDeletable? source, UpdatableAndDeletable? target)
    {
        SetOwner(target, GetOwner(source));
    }

    [HookPatch(typeof(On.RainWorldGame), nameof(On.RainWorldGame.ctor))]
    private static void On_RainWorldGame_ctor(On.RainWorldGame.orig_ctor orig, RainWorldGame self, ProcessManager manager)
    {
        orig(self, manager);

        _ownerCWT = new();
    }
}
