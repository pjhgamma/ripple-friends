using Mono.Cecil.Cil;
using MonoMod.Cil;
using static RippleFriends.Hooks.Tracker.OwnerTrackerHooks;
using static RippleFriends.Utils.PlayerUtils;
using RippleFriends.Options;
using RWCustom;

namespace RippleFriends.Hooks.Tracker;

internal class FriendTrackerHooks : BaseHooks
{
    protected override bool IsOptionEnabled => Config.FriendSlugcat.Value || Config.FriendCreature.Value;

    private static readonly HashSet<AbstractCreature> _friendSet = [];

    private static AbstractPhysicalObject? GetAbstractPhysicalObject(object? source)
    {
        return source switch
        {
            PhysicalObject physicalObject => physicalObject.abstractPhysicalObject,
            AbstractPhysicalObject abstractPhysicalObject => abstractPhysicalObject,
            UpdatableAndDeletable updatableAndDeletable => GetOwner(updatableAndDeletable),
            _ => null
        };
    }

    private static bool IsFriend(object? source, object? target)
    {
        var sourceAbstractPhysicalObject = GetAbstractPhysicalObject(source);
        var targetAbstractPhysicalObject = GetAbstractPhysicalObject(target);

        if (sourceAbstractPhysicalObject == null || targetAbstractPhysicalObject == null)
        {
            return false;
        }

        if (!Config.FriendArena.Value && sourceAbstractPhysicalObject.Room?.realizedRoom?.game?.IsArenaSession != false)
        {
            return false;
        }

        AbstractCreature? abstractSlugcat = null;
        AbstractPhysicalObject? abstractPhysicalObject = null;

        if (IsSlugcat(sourceAbstractPhysicalObject))
        {
            abstractSlugcat = sourceAbstractPhysicalObject as AbstractCreature;
            abstractPhysicalObject = targetAbstractPhysicalObject;
        }
        else if (IsSlugcat(targetAbstractPhysicalObject))
        {
            abstractSlugcat = targetAbstractPhysicalObject as AbstractCreature;
            abstractPhysicalObject = sourceAbstractPhysicalObject;
        }

        if (abstractSlugcat == null || abstractPhysicalObject == null)
        {
            return false;
        }

        if ((GetOwner(abstractPhysicalObject.realizedObject) ?? abstractPhysicalObject as AbstractCreature) is { } abstractCreature)
        {
            if (IsSlugcat(abstractCreature))
            {
                return Config.FriendSlugcat.Value && abstractCreature != abstractSlugcat;
            }

            if (Config.FriendCreature.Value || Config.FriendNeutralCreature.Value)
            {
                var aiSource = abstractCreature.abstractAI?.RealAI;
                var friend = aiSource?.friendTracker?.friend;

                if ((aiSource is LizardAI && Custom.rainWorld?.options?.friendlyLizards == true && friend != null) || friend?.abstractCreature == abstractSlugcat)
                {
                    return Config.FriendCreature.Value;
                }

                if (aiSource != null)
                {
                    var representation = aiSource.tracker?.RepresentationForCreature(abstractSlugcat, false);
                    var relationship = representation != null ? aiSource.DynamicRelationship(representation) : aiSource.StaticRelationship(abstractSlugcat);

                    if (relationship.type == CreatureTemplate.Relationship.Type.Pack)
                    {
                        return Config.FriendCreature.Value;
                    }
                    if (relationship.type == CreatureTemplate.Relationship.Type.Ignores)
                    {
                        return Config.FriendNeutralCreature.Value;
                    }
                }
            }

        }

        if (Config.FriendIterator.Value)
        {
            if (abstractSlugcat.realizedObject is Player player && abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.Oracle)
            {
                return !player.monkAscension;
            }
            if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsFriend(AbstractPhysicalObject? source, AbstractPhysicalObject? target) => IsFriend((object?)source, target);

    public static bool IsFriend(AbstractPhysicalObject? source, UpdatableAndDeletable? target) => IsFriend((object?)source, target);

    public static bool IsFriend(UpdatableAndDeletable? source, AbstractPhysicalObject? target) => IsFriend((object?)source, target);

    public static bool IsFriend(UpdatableAndDeletable? source, UpdatableAndDeletable? target) => IsFriend((object?)source, target);

    private static bool IsAnyFriend(object? source)
    {
        Room? room = source switch
        {
            AbstractPhysicalObject abstractPhysicalObject => abstractPhysicalObject.Room?.realizedRoom,
            UpdatableAndDeletable updatableAndDeletable => updatableAndDeletable.room,
            _ => null
        };

        foreach (var abstractCreature in room?.game?.Players ?? [])
        {
            if (IsFriend(source, abstractCreature))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsAnyFriend(AbstractPhysicalObject? source) => IsAnyFriend((object?)source);

    public static bool IsAnyFriend(UpdatableAndDeletable? source) => IsAnyFriend((object?)source);

    public static IEnumerable<Creature> GetRoomFriends(Room? room)
    {
        foreach (var physicalObjects in room?.physicalObjects ?? [])
        {
            foreach (var physicalObject in physicalObjects)
            {
                if (physicalObject is Creature creature && IsAnyFriend(creature))
                {
                    yield return creature;
                }
            }
        }
    }

    public static IEnumerable<AbstractCreature> GetTrackedFriends()
    {
        foreach (var abstractCreature in _friendSet)
        {
            yield return abstractCreature;
        }
    }

    public static IEnumerable<AbstractCreature> GetTrackedFriends(List<AbstractCreature>? abstractPlayers, Func<AbstractCreature, bool> predicate)
    {
        foreach (var abstractPlayer in abstractPlayers ?? [])
        {
            if (predicate(abstractPlayer))
            {
                yield return abstractPlayer;
            }
        }

        foreach (var abstractCreature in GetTrackedFriends())
        {
            if (predicate(abstractCreature))
            {
                yield return abstractCreature;
            }
        }
    }

    private static AbstractCreature? GetTrakerFriend(FriendTracker friendTracker)
    {
        if (friendTracker.creature?.abstractCreature is AbstractCreature abstractCreature && IsSlugcat(friendTracker.friend))
        {
            return abstractCreature;
        }
        return null;
    }

    private static void AddTrackedFriend(FriendTracker friendTracker)
    {
        if (GetTrakerFriend(friendTracker) is AbstractCreature abstractCreature && !IsPlayer(abstractCreature))
        {
            _friendSet.Add(abstractCreature);
        }
    }

    private static void RemoveTrackedFriend(FriendTracker friendTracker)
    {
        if (GetTrakerFriend(friendTracker) is AbstractCreature abstractCreature)
        {
            _friendSet.Remove(abstractCreature);
        }
    }

    [HookPatch(typeof(On.RainWorldGame), nameof(On.RainWorldGame.ctor))]
    private void On_RainWorldGame_ctor(On.RainWorldGame.orig_ctor orig, RainWorldGame self, ProcessManager manager)
    {
        orig(self, manager);

        _friendSet.Clear();
    }

    [HookPatch(typeof(On.FriendTracker), nameof(On.FriendTracker.ctor))]
    private static void On_FriendTracker_ctor(On.FriendTracker.orig_ctor orig, FriendTracker self, ArtificialIntelligence AI)
    {
        orig(self, AI);

        AddTrackedFriend(self);
    }

    [HookPatch(typeof(IL.FriendTracker), nameof(IL.FriendTracker.Update))]
    private static void IL_FriendTracker_Update(ILContext il)
    {
        ILCursor c = new(il);

        if (c.TryGotoNext(
            MoveType.After,
            i => i.MatchStfld<FriendTracker>("friend")
        ))
        {
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(AddTrackedFriend);

            if (c.TryGotoNext(
                i => i.MatchStfld<FriendTracker>("friend")
            ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate(RemoveTrackedFriend);
            }
        }
    }
}
