class Weapon
{
    readonly WeaponData data;
    int _magazine;
    Map map;

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

    public Weapon(Map map, WeaponData weaponData, int startMagazine)
    {
        this.map = map;
        this.data = weaponData;
        magazine = startMagazine;
    }

    public Weapon(Map map, WeaponData weaponData)
        : this(map, weaponData, weaponData.magazineCapacity)
    {
    }

    public WeaponInstance CreateWeaponInstance(Map map, Node3D modelRoot)
    {
        WeaponInstance instance = new(this, data, modelRoot, map);
        instance.OnFire += OnInstanceFire;
        instance.OnReloadEnd += OnInstanceReloadEnd;
        return instance;
    }

    void OnInstanceFire() => magazine--;
    
    void OnInstanceReloadEnd(int givenAmmo) => magazine += givenAmmo;
}