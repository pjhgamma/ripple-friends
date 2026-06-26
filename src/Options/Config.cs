using static RippleFriends.Options.RemixMenu;

namespace RippleFriends.Options;

internal static class Config
{
    public static Configurable<bool> FriendPlayer = null!;
    public static Configurable<bool> FriendCreature = null!;
    public static Configurable<bool> FriendNeutralCreature = null!;
    public static Configurable<bool> FriendIterator = null!;
    public static Configurable<bool> FriendGrabbed = null!;
    public static Configurable<bool> FriendArena = null!;

    public static Configurable<bool> Collision = null!;
    public static Configurable<bool> Weapon = null!;
    public static Configurable<bool> Explosion = null!;

    public static Configurable<bool> GrabPlayer = null!;
    public static Configurable<float> GrabPlayerTime = null!;
    public static Configurable<bool> NoStealing = null!;

    public static Configurable<bool> GourmandSlam = null!;
    public static Configurable<bool> ArtificerParry = null!;
    public static Configurable<bool> SaintTongue = null!;
    public static Configurable<bool> SaintAttunement = null!;

    public static Configurable<bool> Mushroom = null!;
    public static Configurable<bool> FirecrackerPlant = null!;
    public static Configurable<bool> Bee = null!;
    public static Configurable<bool> JellyFish = null!;
    public static Configurable<bool> Snail = null!;
    public static Configurable<bool> TubeWorm = null!;

    public static Configurable<bool> FireEgg = null!;
    public static Configurable<bool> SingularityBomb = null!;

    public static Configurable<bool> Pomegranate = null!;
    public static Configurable<bool> Frog = null!;

    public static Configurable<bool> Pebbles = null!;
    public static Configurable<bool> Moon = null!;

    public static Configurable<bool> RegionGate = null!;
    public static Configurable<float> RegionGateTile = null!;
    public static Configurable<float> RegionGateTime = null!;
    public static Configurable<float> RegionGateForceTime = null!;

    public static Configurable<bool> WarpPoint = null!;
    public static Configurable<float> WarpPointRadius = null!;
    public static Configurable<float> WarpPointTime = null!;
    public static Configurable<float> WarpPointForceTime = null!;

    public static void Bind(OptionInterface oi)
    {
        FriendPlayer = oi.config.Bind("FriendPlayer", true, new ConfigurableInfo("Includes players and slugpups as Ripple Friends."));
        FriendCreature = oi.config.Bind("FriendCreature", true, new ConfigurableInfo("Includes friendly creatures, such as tamed lizards and friendly scavengers, as Ripple Friends."));
        FriendNeutralCreature = oi.config.Bind("FriendNeutralCreature", false, new ConfigurableInfo("Includes ignoring creatures, such as neutralized lizards and rain deer, as Ripple Friends."));
        FriendIterator = oi.config.Bind("FriendIterator", true, new ConfigurableInfo("Includes Iterators as Ripple Friends."));
        FriendGrabbed = oi.config.Bind("FriendGrabbed", true, new ConfigurableInfo("Includes objects grabbed by Ripple Friends as well."));
        FriendArena = oi.config.Bind("FriendArena", false, new ConfigurableInfo("Activates the Ripple Friends relationship in the Arena."));

        Collision = oi.config.Bind("Collision", true, new ConfigurableInfo("Will not collide with Ripple Friends."));
        Weapon = oi.config.Bind("Weapon", true, new ConfigurableInfo("Will not be hit by most weapons thrown by Ripple Friends."));
        Explosion = oi.config.Bind("Explosion", true, new ConfigurableInfo("Will not be hit by most explosions caused by Ripple Friends. This also applies to chain explosions."));

        GrabPlayer = oi.config.Bind("GrabPlayer", true, new ConfigurableInfo("Cannot grab a player when they enter a control input."));
        GrabPlayerTime = oi.config.Bind("GrabPlayerTime", 3f, new ConfigurableInfo("Sets the maximum control input time (in seconds) during which a player cannot be grabbed."));
        NoStealing = oi.config.Bind("NoStealing", true, new ConfigurableInfo("Will not steal objects grabbed by Ripple Friends. If the No Stealing option is enabled in Jolly Co-op, players never steal grabbed objects from each other."));

        GourmandSlam = oi.config.Bind("GourmandSlam", true, new ConfigurableInfo("Will not take damage from a Gourmand's roll, slide, or slam. If the Spears Miss option is enabled in Jolly Co-op, players never damage each other."));
        ArtificerParry = oi.config.Bind("ArtificerParry", true, new ConfigurableInfo("Will not be stunned by a Artificer's parry. If the Spears Miss option is enabled in Jolly Co-op, players never stun each other."));
        SaintTongue = oi.config.Bind("SaintTongue", true, new ConfigurableInfo("Will not be caught by the tongue shot by a Saint."));
        SaintAttunement = oi.config.Bind("SaintAttunement", true, new ConfigurableInfo("Will not be instantly killed by a Saint's attunement."));

        Mushroom = oi.config.Bind("Mushroom", true, new ConfigurableInfo("Share the effects of mushrooms among players."));
        FirecrackerPlant = oi.config.Bind("FirecrackerPlant", true, new ConfigurableInfo("Will not be stunned by a cherrybomb thrown by Ripple Friends. However, this does not apply to the final explosion. This also applies to joke rifle bullets and chain explosions."));
        Bee = oi.config.Bind("Bee", true, new ConfigurableInfo("Will not be caught by bees triggered by Ripple Friends. However, this does not apply to thoese spawned by approaching a beehive. This also applies to joke rifle bullets."));
        JellyFish = oi.config.Bind("JellyFish", true, new ConfigurableInfo("Will not be caught or stunned by the tentacles of a jellyfish held by a Ripple Friends, and will not be stunned by a thrown jellyfish."));
        Snail = oi.config.Bind("Snail", true, new ConfigurableInfo("Will not be stunned by the next popping from a snail thrown, hit, or killed by Ripple Friends."));
        TubeWorm = oi.config.Bind("TubeWorm", true, new ConfigurableInfo("Will not be caught by the tongue of a grappling worm shot by Ripple Friends."));

        FireEgg = oi.config.Bind("FireEgg", true, new ConfigurableInfo("Will not be attached to by a fire egg thrown by Ripple Friends. However, this does not apply to the explosion."));
        SingularityBomb = oi.config.Bind("SingularityBomb", true, new ConfigurableInfo("Will not be sucked or instantly killed by a singularity bomb thrown by Ripple Friends. However, this does not apply to the explosion."));

        Pomegranate = oi.config.Bind("Pomegranate", true, new ConfigurableInfo("Will not take damage from a pomegranate dropped by Ripple Friends."));
        Frog = oi.config.Bind("Frog", true, new ConfigurableInfo("Will not be attached to by a frog thrown by Ripple Friends."));

        Pebbles = oi.config.Bind("Pebbles", true, new ConfigurableInfo("Pebbles cannot kill players."));
        Moon = oi.config.Bind("Moon", true, new ConfigurableInfo("Players cannot steal Moon's neurons."));

        RegionGate = oi.config.Bind("RegionGate", true, new ConfigurableInfo("The karma gate waits for Ripple Friends in the room and will not activate while any player is entering control inputs."));
        RegionGateTile = oi.config.Bind("RegionGateTile", 4f, new ConfigurableInfo("Sets the range within which the karma gate begins to close. A value of 0 represents the middle position of the karma gate, and 8 represents the shutter position on the opposite side."));
        RegionGateTime = oi.config.Bind("RegionGateTime", 3f, new ConfigurableInfo("Sets the minimum time (in seconds) of no control input from the player required for the karma gate to activate."));
        RegionGateForceTime = oi.config.Bind("RegionGateForceTime", 3f, new ConfigurableInfo("Sets the minimum time (in seconds) of no control input from the player required for the karma gate to forcefully activate, ignoring non-player Ripple Friends."));

        WarpPoint = oi.config.Bind("WarpPoint", true, new ConfigurableInfo("The warp point waits for Ripple Friends in the room and will not activate while any player is entering control inputs."));
        WarpPointRadius = oi.config.Bind("WarpPointRadius", 1f, new ConfigurableInfo("Sets the pull radius of the warp point. A value of 1 is similar to the visual radius of the warp point, and 6 is close to the maximum range at which the warp point can pull in creatures."));
        WarpPointTime = oi.config.Bind("WarpPointTime", 3f, new ConfigurableInfo("Sets the minimum time (in seconds) of no control input from the player required for the warp point to activate."));
        WarpPointForceTime = oi.config.Bind("WarpPointForceTime", 3f, new ConfigurableInfo("Sets the minimum time (in seconds) of no control input from the player required for the warp point to forcefully activate, ignoring non-player Ripple Friends."));
    }
}
