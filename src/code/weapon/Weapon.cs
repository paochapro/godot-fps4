class Weapon
{
    readonly WeaponData data;
    int _magazine;

    int magazine {
        get => _magazine;
        set {
            _magazine = Mathf.Clamp(value, 0, data.magazineCapacity);
        }
    }

    public WeaponData WeaponData => data;
    public int Magazine => magazine;
    public int MagazineRefill => data.magazineCapacity - magazine;

    public bool CanReload => magazine < data.magazineCapacity;
    public bool CanFire => magazine > 0;

    public Weapon(WeaponData weaponData, int startMagazine)
    {
        this.data = weaponData;
        magazine = startMagazine;
    }

    public Weapon(WeaponData weaponData)
        : this(weaponData, weaponData.magazineCapacity)
    {
    }

    public WeaponInstance CreateWeaponInstance(PhysicsDirectSpaceState3D ss, Node3D model)
    {
        WeaponInstance instance = new(this, data, model, ss);
        instance.OnFire += OnInstanceFire;
        instance.OnReloadEnd += OnInstanceReloadEnd;
        return instance;
    }

    public Node3D CreateModel()
    {
        var modelScene = GD.Load<PackedScene>(data.modelUID);
		var model = modelScene.Instantiate<Node3D>();
        return model;
    }

    void OnInstanceFire() => magazine--;
    
    void OnInstanceReloadEnd(int givenAmmo) => magazine += givenAmmo;
}