using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace RippleFriends.Options;

internal abstract class RemixMenuBuilder : OptionInterface
{
    private const float _padding = 30f;
    private const float _spacing = 25f;
    private const float _gap = 5f;
    private readonly Vector2 MarginX = new(_padding, 600f - _padding);
    private float Width => MarginX.y - MarginX.x;

    private OpTab? _currentTab;
    private Vector2 _pos = new();

    private int _columns = 3;
    private float _currentColumn = 0f;
    private float ElementWidth => Width / _columns;

    protected void SetCurrentTab(OpTab? tab)
    {
        _currentTab = tab;
        _pos = new(MarginX.x, 600f);
        _currentColumn = 0;

        AddNewLine();
    }

    protected void SetColumns(int columns)
    {
        ResetColumn();

        _columns = columns;
    }

    protected void AddNewLine(float spacingModifier = 1f)
    {
        _pos.x = MarginX.x;
        _pos.y -= spacingModifier * _spacing;
        _currentColumn = 0;
    }

    protected void AddNewColumn(float span)
    {
        _pos.x += ElementWidth * span;
        if ((_currentColumn += span) > _columns + 0.5f)
        {
            ResetColumn();
        }
    }

    protected void ResetColumn()
    {
        if (_currentColumn > 0)
        {
            AddNewLine(1.5f);
        }
    }

    protected void AddLabel(string text, FLabelAlignment alignment = FLabelAlignment.Center, bool bigText = false)
    {
        if (_currentTab == null)
        {
            return;
        }

        ResetColumn();

        float height = (bigText ? 1.5f : 1f) * _spacing;

        if (bigText)
        {
            AddNewLine(0.5f);
        }

        OpLabel label = new(
            new(_pos.x, _pos.y - 6f),
            new(Width, height),
            Translate(text),
            alignment,
            bigText
        )
        {
            autoWrap = true,
        };

        _currentTab.AddItems(label);
        AddNewLine();
    }

    protected void AddTitle(string? title = null, string? description = null, FLabelAlignment alignment = FLabelAlignment.Center)
    {
        ResetColumn();
        AddNewLine();

        if (title != null)
        {
            AddLabel(title, alignment, true);
        }
        if (description != null)
        {
            AddLabel(description, alignment);
        }
    }

    protected OpCheckBox? AddCheckBox(Configurable<bool> configurable, string? text = null, OpCheckBox? master = null)
    {
        if (_currentTab == null)
        {
            return null;
        }

        string desc = Translate(configurable.info?.description ?? "");

        OpCheckBox checkBox = new(
            configurable,
            new Vector2(_pos.x, _pos.y - _spacing * 0.5f)
        )
        {
            description = desc
        };
        OpLabel label = new(
            new Vector2(_pos.x + _spacing + _gap, _pos.y - _spacing * 0.5f),
            new(ElementWidth - _spacing - _gap, _spacing),
            Translate(text ?? ""),
            FLabelAlignment.Left
        )
        {
            description = desc
        };

        if (master != null)
        {
            checkBox.greyedOut = !master.GetValueBool();
            master.OnChange += delegate
            {
                checkBox.greyedOut = !master.GetValueBool();
            };
        }

        if (text != null)
        {
            _currentTab.AddItems(label);
        }
        _currentTab.AddItems(checkBox);

        _pos.x += ElementWidth;
        if ((_currentColumn += 1) > _columns - 1)
        {
            AddNewLine(1.5f);
        }

        return checkBox;
    }

    protected void AddFloatSlider(Configurable<float> configurable, float span = 1, float min = 0f, float max = 5f, string? text = null, OpCheckBox? master = null)
    {
        if (_currentTab == null)
        {
            return;
        }

        if (_currentColumn > _columns - span + 0.5f)
        {
            ResetColumn();
        }

        string desc = Translate(configurable.info?.description ?? "");

        OpLabel label = new(
            new Vector2(_pos.x + _spacing + _gap, _pos.y - _spacing * 0.5f),
            new(ElementWidth - _spacing - _gap, _spacing),
            Translate(text ?? ""),
            FLabelAlignment.Left
        )
        {
            description = desc
        };
        OpFloatSlider slider = new(
            configurable,
            new Vector2(_pos.x + _gap + (text != null ? ElementWidth : 0), _pos.y - _spacing * 0.5f - 3f),
            (int)(ElementWidth * span - _gap * 2f - (text != null ? ElementWidth : 0))
        )
        {
            description = desc,
            min = min,
            max = max,
        };

        if (master != null)
        {
            slider.greyedOut = !master.GetValueBool();
            master.OnChange += delegate
            {
                slider.greyedOut = !master.GetValueBool();
            };
        }

        if (text != null)
        {
            _currentTab.AddItems(label);
        }
        _currentTab.AddItems(slider);

        _pos.x += ElementWidth * span;
        if ((_currentColumn += span) > _columns - 1)
        {
            AddNewLine(1.5f);
        }
    }
}

internal class RemixMenu : RemixMenuBuilder
{
    public static readonly RemixMenu Instance = new();

    public RemixMenu()
    {
        Config.Bind(this);
    }

    public override void Initialize()
    {
        base.Initialize();

        OpTab generalTab = new(this, Translate("General"));
        OpTab interactionsTab = new(this, Translate("Interactions"));
        OpTab gatesTab = new(this, Translate("Gates"));

        Tabs = [generalTab, interactionsTab, gatesTab];

        SetCurrentTab(generalTab);

        SetColumns(3);

        AddLabel("Ripple Friends", FLabelAlignment.Center, bigText: true);
        AddLabel("Ripple friends do not affect each other.");
        AddLabel("This option itself does nothing, but targets to be affected by the other options.");
        AddLabel("The Ripple Friends relationship applies bidirectionally, excluding oneself.");
        AddCheckBox(Config.FriendSlugcat, "Slugcats");
        AddCheckBox(Config.FriendCreature, "Friendly Creatures");
        AddCheckBox(Config.FriendNeutralCreature, "Neutral Creatures");
        AddCheckBox(Config.FriendIterator, "Iterators");
        AddCheckBox(Config.FriendGrabbed, "Grabbed Objects");
        AddCheckBox(Config.FriendArena, "Arena");

        AddTitle("General");

        AddCheckBox(Config.Collision, "Collisions");
        AddCheckBox(Config.Weapon, "Weapons");
        AddCheckBox(Config.Explosion, "Explosions");

        SetCurrentTab(interactionsTab);

        SetColumns(4);

        AddTitle("Players");

        var grabPlayerCheckBox = AddCheckBox(Config.GrabPlayer, "Grab Player");
        AddFloatSlider(Config.GrabPlayerTime, span: 2f, master: grabPlayerCheckBox);
        AddCheckBox(Config.NoStealing, "No Stealing");

        if (ModManager.MSC)
        {
            AddCheckBox(Config.GourmandSlam, "Gourmand Slam");
            AddCheckBox(Config.ArtificerParry, "Artificer Parry");
            AddCheckBox(Config.SaintTongue, "Saint Tongue");
            AddCheckBox(Config.SaintAttunement, "Saint Attunement");
        }

        AddTitle("Items");

        AddCheckBox(Config.Mushroom, "Mushroom");
        AddCheckBox(Config.FirecrackerPlant, "Cherrybomb");
        AddCheckBox(Config.Bee, "Beehive");
        AddCheckBox(Config.JellyFish, "Jellyfish");
        AddCheckBox(Config.Snail, "Snail");
        AddCheckBox(Config.TubeWorm, "Grappling Worm");

        if (ModManager.MSC)
        {
            AddCheckBox(Config.FireEgg, "Fire Egg");
            AddCheckBox(Config.SingularityBomb, "Singularity Bomb");
        }

        if (ModManager.Watcher)
        {
            AddCheckBox(Config.Pomegranate, "Pomegranate");
            AddCheckBox(Config.Frog, "Frog");
        }

        SetColumns(2);

        AddTitle("Iterators");

        AddCheckBox(Config.Pebbles, "Pebbles");
        AddCheckBox(Config.Moon, "Moon");

        SetCurrentTab(gatesTab);

        SetColumns(4);

        AddTitle("Shelters");

        var shelterDoorCheckBox = AddCheckBox(Config.ShelterDoor, "Activation Delay");
        AddFloatSlider(Config.ShelterDoorTime, span: 3f, master: shelterDoorCheckBox);
        var shelterDoorForceCheckBox = AddCheckBox(Config.ShelterDoorForce, "Force Activation", shelterDoorCheckBox);
        AddFloatSlider(Config.ShelterDoorForceTime, span: 3f, max: 10f, master: shelterDoorForceCheckBox);
        AddCheckBox(Config.ShelterDoorWarp, "Warp");
        AddCheckBox(Config.ShelterDoorRevival, "Revival");

        AddTitle("Karma Gates");

        var regionGateCheckBox = AddCheckBox(Config.RegionGate, "Activation Delay");
        AddFloatSlider(Config.RegionGateTime, span: 1.5f, master: regionGateCheckBox);
        AddFloatSlider(Config.RegionGateTile, span: 1.5f, max: 8f, master: regionGateCheckBox);
        var regionGateForceCheckBox = AddCheckBox(Config.RegionGateForce, "Force Activation", regionGateCheckBox);
        AddFloatSlider(Config.RegionGateForceTime, span: 3f, max: 10f, master: regionGateForceCheckBox);
        AddCheckBox(Config.RegionGateWarp, "Warp");
        AddCheckBox(Config.RegionGateRevival, "Revival");

        if (ModManager.Watcher)
        {
            AddTitle("Warp Points");

            var warpPointCheckBox = AddCheckBox(Config.WarpPoint, "Activation Delay");
            AddFloatSlider(Config.WarpPointTime, span: 1.5f, master: warpPointCheckBox);
            AddFloatSlider(Config.WarpPointRadius, span: 1.5f, max: 6f, master: warpPointCheckBox);
            var warpPointForceCheckBox = AddCheckBox(Config.WarpPointForce, "Force Activation", master: warpPointCheckBox);
            AddFloatSlider(Config.WarpPointForceTime, span: 3f, max: 10f, master: warpPointForceCheckBox);
            AddCheckBox(Config.WarpPointWarp, "Warp");
            AddCheckBox(Config.WarpPointRevival, "Revival");
        }
    }
}
