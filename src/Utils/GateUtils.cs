using static RippleFriends.Hooks.Tracker.FriendTrackerHooks;
using static RippleFriends.Utils.PlayerUtils;
using RippleFriends.Options;
using RWCustom;
using Watcher;

namespace RippleFriends.Utils;

internal static class ShelterDoorUtils
{
    public static bool IsInShelterDoor(ShelterDoor shelterDoor, Creature creature)
    {
        return shelterDoor.room == creature.room && !creature.inShortcut && creature.abstractCreature is { } abstractCreature && abstractCreature.pos.Tile is { } creatureTile && shelterDoor.room is { } room && room.LocalCoordinateOfNode(0).Tile is { } nodeTile && Custom.ManhattanDistance(creatureTile, nodeTile) > 6 && ShelterDoor.IsTileInsideShelterRange(room.abstractRoom, creatureTile);
    }
}

internal static class RegionGateUtils
{
    public static float GetTile(RegionGate regionGate, float x)
    {
        return (x / 20f - (regionGate.room?.TileWidth ?? 0) / 2) * (regionGate.letThroughDir ? 1 : -1);
    }

    public static float GetBodyChunkLeftTile(RegionGate regionGate, BodyChunk bodyChunk)
    {
        return GetTile(regionGate, bodyChunk.pos.x - bodyChunk.rad * (regionGate.letThroughDir ? 1 : -1));
    }

    public static float GetBodyChunkRightTile(RegionGate regionGate, BodyChunk bodyChunk)
    {
        return GetTile(regionGate, bodyChunk.pos.x + bodyChunk.rad * (regionGate.letThroughDir ? 1 : -1));
    }

    public static bool IsInRegionGate(RegionGate regionGate, Creature creature)
    {
        if (regionGate.room != creature.room)
        {
            return false;
        }

        foreach (var bodyChunk in creature.bodyChunks ?? [])
        {
            if (bodyChunk != null && GetBodyChunkLeftTile(regionGate, bodyChunk) < -8f)
            {
                return false;
            }
        }

        return true;
    }
}

internal static class WarpPointUtils
{
    public static bool IsInWarpPoint(WarpPoint warpPoint, Creature creature)
    {
        return creature.room == warpPoint.room && creature.mainBodyChunk?.pos is { } creturePos && Custom.DistLess(warpPoint.pos, creturePos, warpPoint.PullRadius * Config.WarpPointRadius.Value);
    }
}

internal static class GateUtils<T> where T : UpdatableAndDeletable
{
    public static bool CanActivate(T gate, List<AbstractCreature>? abstractPlayers, Func<T, Creature, bool> isInGate, float normalTime, bool force, float forceTime)
    {
        bool canActivate = true;

        foreach (var abstractCreature in GetTrackedFriends())
        {
            if (abstractCreature.realizedCreature is Creature creature && !creature.dead && !isInGate(gate, creature))
            {
                if (!force)
                {
                    return false;
                }

                canActivate = false;

                break;
            }
        }

        foreach (var abstractPlayer in abstractPlayers ?? [])
        {
            if (abstractPlayer.realizedCreature is Player player && IsPlayer(player))
            {
                if (!isInGate(gate, player) || !IsIdlePlayer(player, normalTime))
                {
                    return false;
                }
                if (force && !canActivate && !IsIdlePlayer(player, normalTime + forceTime))
                {
                    return false;
                }
            }

        }

        return true;
    }

    private static void Warp(AbstractCreature abstractCreature, AbstractRoom abstractRoom, WorldCoordinate position)
    {
        abstractCreature.realizedCreature?.room?.RemoveObject(abstractCreature.realizedCreature);
        abstractCreature.realizedCreature?.Destroy();
        abstractCreature.realizedCreature = null;
        abstractCreature.Room?.RemoveEntity(abstractCreature);

        abstractCreature.pos = position;
        abstractCreature.timeSpentHere = 0;
        abstractCreature.InDen = false;
        if (abstractCreature.state.dead)
        {
            abstractCreature.LoseAllStuckObjects();
        }

        abstractRoom.AddEntity(abstractCreature);
        abstractCreature.RealizeInRoom();

        if (abstractCreature.realizedCreature is Creature creature && abstractRoom.realizedRoom is Room room)
        {
            creature.NewRoom(room);
        }
    }

    private static void Revive(Creature creature)
    {
        creature.dead = false;
        creature.killTag = null;
        creature.killTagCounter = 0;
        if (creature.abstractCreature?.pos is { } creaturePos)
        {
            creature.abstractCreature?.abstractAI?.SetDestination(creaturePos);
        }

        if (creature.State is CreatureState creatureState)
        {
            creatureState.alive = true;
            if (creatureState is PlayerState playerState)
            {
                playerState.permaDead = false;
                playerState.permanentDamageTracking = 0f;
            }
            if (creatureState is HealthState healthState)
            {
                healthState.health = 1f;
            }
        }
    }

    public static void WarpAndRevive(T gate, Func<T, Creature, bool> isInGate, bool warp, bool revival)
    {
        if (gate.room is Room room && room.game is { } game && room.abstractRoom is { } abstractRoom)
        {
            WorldCoordinate position = new();

            foreach (var abstractPlayer in game.AlivePlayers ?? [])
            {
                if (abstractPlayer?.realizedCreature is Player player && isInGate(gate, player))
                {
                    position = abstractPlayer.pos;

                    break;
                }
            }

            foreach (var abstractCreature in GetTrackedFriends(game.Players, IsAnyFriend))
            {
                var creature = abstractCreature.realizedCreature;

                if (warp && (creature == null || !isInGate(gate, creature)) && (!abstractCreature.state.dead || revival))
                {
                    Warp(abstractCreature, abstractRoom, position);
                }
                if (revival && creature != null && creature.dead && abstractCreature.Room == abstractRoom)
                {
                    Revive(creature);
                }
            }
        }
    }
}
