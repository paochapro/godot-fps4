using System.Collections.Generic;
using System.Linq;

class PlayerWeaponManager
{
    List<Weapon> weapons;
	Dictionary<AmmoType, int> ammo;

    readonly StringName[] equipSlotActions;
    readonly Dictionary<FireType, string> fireTypeWeapons;

    WeaponInstance? _equipedInstance;
    Node3D instanceRoot;
    Node3D modelRoot;
    PhysicsDirectSpaceState3D spaceState;
    Gui gui;

    WeaponInstance? equipedInstance {
        get => _equipedInstance;
        set {
            if(_equipedInstance != null) {
                modelRoot.RemoveChild(_equipedInstance.Model);
                instanceRoot.RemoveChild(_equipedInstance);
                _equipedInstance.Model.QueueFree();
                _equipedInstance.QueueFree();
            }
            
            if(value != null)  {
                instanceRoot.AddChild(value);
                modelRoot.AddChild(value.Model);

                AmmoType ammoType = value.Weapon.WeaponData.ammoType;
                
                var takeOutAmmo = (int takenAmmo) => {
                    ammo[ammoType] -= takenAmmo;
                };
                
                value.OnReloadEnd += takeOutAmmo;
            }

            _equipedInstance = value;
        }
    }

    public WeaponInstance? EquipedInstance => equipedInstance;
    public bool FireSuccess { get; private set; }
    public bool StartReloadSuccess { get; private set; }

    public PlayerWeaponManager(PhysicsDirectSpaceState3D ss, Gui gui, Node3D instanceRoot, Node3D modelRoot, IEnumerable<Weapon> startWeapons)
    {
        this.instanceRoot = instanceRoot;
        this.modelRoot = modelRoot;
        this.gui = gui;
        this.spaceState = ss;
        weapons = new(startWeapons);

        equipSlotActions = InputMap.GetActions().Where(s => s.ToString().StartsWith("weapon_equip_slot_")).ToArray();

        fireTypeWeapons = new() {
            [FireType.Pistol] = "pistol",
            [FireType.Rifle] = "rifle",
            [FireType.Shotgun] = "shotgun"
        };

        var types = Enum.GetValues<AmmoType>();
        ammo = new(types.Select(t => new KeyValuePair<AmmoType,int>(t, 10)));

        Weapon? firstWp = weapons.FirstOrDefault();
        
        if(firstWp != null)
        {
            equipedInstance = firstWp.CreateWeaponInstance(ss, firstWp.CreateModel());
        }
    }

    //Constructor without start weapons
    public PlayerWeaponManager(PhysicsDirectSpaceState3D ss, Gui gui, Node3D instanceRoot, Node3D modelRoot)
        : this(ss, gui, instanceRoot, modelRoot, Enumerable.Empty<Weapon>())
    {
    }

    public void Process(double dt)
    {
        if(equipedInstance == null) return;
        EquipWeaponsControls();
        InstanceControls(equipedInstance);
        GuiUpdate(equipedInstance);
    }

    void EquipWeaponsControls()
    {
        foreach(string action in equipSlotActions)
        {
            string weaponName = action.Split('_').Last();

            if(Input.IsActionJustPressed(action))
            {
                GD.Print($"Action pressed: {action}");

                Predicate<Weapon> fireTypeSearch = (Weapon w) => {
                    string fireTypeName = fireTypeWeapons[w.WeaponData.fireType].ToLower();
                    return fireTypeName == weaponName;
                };

                Weapon? weapon = weapons?.Find(fireTypeSearch);

                if(weapon != null)
                    equipedInstance = weapon.CreateWeaponInstance(spaceState, weapon.CreateModel());
            }
        }
    }

    void InstanceControls(WeaponInstance instance)
    {
		if(Input.IsActionJustPressed("fire"))
            FireSuccess = TryFire(instance);

		if(Input.IsActionJustPressed("reload"))
            StartReloadSuccess = TryStartReloading(instance);         
    }
    
    bool TryFire(WeaponInstance instance)
    {
		return instance.TryFire();
    }

    bool TryStartReloading(WeaponInstance instance)
    {
        int giveAmmo = GetNeededAmmo(instance);
        return instance.TryStartReloading(giveAmmo);
    }

    int GetNeededAmmo(WeaponInstance instance)
    {
        AmmoType ammoType = instance.Weapon.WeaponData.ammoType;
        int ammoAmount = ammo[ammoType];
        int taken = instance.Weapon.MagazineRefill;

        if(ammoAmount < taken)
            taken = ammoAmount;

        return taken;
    }

    void GuiUpdate(WeaponInstance instance)
    {
        //Ammo
        WeaponData data = instance.Weapon.WeaponData;
        Label? ammoLabel = gui.GetNode<Label>("/root/Root/Gui/Ammo/Label");

        if(ammoLabel != null)
            ammoLabel.Text = $"{instance.Weapon.Magazine}/{ammo[data.ammoType]}";

        //Reload time
        Label? reloadTimeLabel = gui.GetNode<Label>("ReloadTime/Label");

        if(reloadTimeLabel != null)
            reloadTimeLabel.Text = $"{instance.ReloadTimeLeft:F2}";
    }
}