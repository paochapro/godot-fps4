using System.Collections.Generic;
using System.Linq;

class PlayerWeaponManager
{
    List<Weapon> weapons;
	Dictionary<AmmoType, int> ammo;

    WeaponInstance? _equipedInstance;
    Node3D instanceRoot;
    Node3D modelRoot;
    Gui gui;

    WeaponInstance? equipedInstance {
        get => _equipedInstance;
        set {
            instanceRoot.AddChild(value);
            _equipedInstance = value;
            
            if(value != null) {
                AmmoType ammoType = value.Weapon.WeaponData.ammoType;
                
                var takeOutAmmo = (int takenAmmo) => {
                    ammo[ammoType] -= takenAmmo;
                };
                
                value.OnReloadEnd += takeOutAmmo;
            }
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
        weapons = new(startWeapons);

        equipedInstance = startWeapons.FirstOrDefault()?.CreateWeaponInstance(ss, modelRoot);

        var types = Enum.GetValues<AmmoType>();
        ammo = new(types.Select(t => new KeyValuePair<AmmoType,int>(t, 10)));
    }

    //Constructor without start weapons
    public PlayerWeaponManager(PhysicsDirectSpaceState3D ss, Gui gui, Node3D instanceRoot, Node3D modelRoot)
        : this(ss, gui, instanceRoot, modelRoot, Enumerable.Empty<Weapon>())
    {
    }

    public void Process(double dt)
    {
        if(equipedInstance == null) return;
        Controls(equipedInstance);
        GuiUpdate(equipedInstance);
    }

    void Controls(WeaponInstance instance)
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