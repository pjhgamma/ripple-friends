using MoreSlugcats;

namespace RippleFriends.Utils;

internal static class PlayerUtils
{
    private static bool IsPlayer(object player)
    {
        if (player is PhysicalObject physicalObject)
        {
            player = physicalObject.abstractPhysicalObject;
        }

        return player is AbstractCreature abstractCreature && abstractCreature.creatureTemplate?.type == CreatureTemplate.Type.Slugcat;
    }

    public static bool IsPlayer(AbstractPhysicalObject abstractPhysicalObject) => IsPlayer((object)abstractPhysicalObject);

    public static bool IsPlayer(PhysicalObject physicalObject) => IsPlayer((object)physicalObject);

    private static bool IsNPC(object player)
    {
        if (player is PhysicalObject physicalObject)
        {
            player = physicalObject.abstractPhysicalObject;
        }

        return player is AbstractCreature abstractCreature && abstractCreature.creatureTemplate?.type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC;
    }

    public static bool IsNPC(AbstractPhysicalObject abstractPhysicalObject) => IsNPC((object)abstractPhysicalObject);

    public static bool IsNPC(PhysicalObject physicalObject) => IsNPC((object)physicalObject);

    private static bool IsSlugcat(object player)
    {
        return IsPlayer(player) || IsNPC(player);
    }

    public static bool IsSlugcat(AbstractPhysicalObject abstractPhysicalObject) => IsSlugcat((object)abstractPhysicalObject);

    public static bool IsSlugcat(PhysicalObject physicalObject) => IsSlugcat((object)physicalObject);

    public static bool IsIdlePlayer(Player player, float seconds)
    {
        return player.touchedNoInputCounter > seconds * 40f;
    }
}
