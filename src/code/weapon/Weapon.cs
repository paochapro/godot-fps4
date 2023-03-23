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

    public WeaponInstance CreateWeaponInstance(PhysicsDirectSpaceState3D ss, Node3D modelRoot)
    {
        WeaponInstance instance = new(this, data, modelRoot, ss);
        instance.OnFire += OnInstanceFire;
        instance.OnReloadEnd += OnInstanceReloadEnd;
        return instance;
    }

    void OnInstanceFire() => magazine--;
    
    void OnInstanceReloadEnd(int givenAmmo) => magazine += givenAmmo;
}