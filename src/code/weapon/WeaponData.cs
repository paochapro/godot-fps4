struct WeaponData {
    public string modelUID;
    public string soundFire;
    public string soundReload;
    public float reloadTime;
    public int magazineCapacity;
    public float fireRate;
    public float damage;
    public AmmoType ammoType;
    public FireType fireType;
}

enum AmmoType {
    Pistol,
    Shotgun,
    Rifle,
}

enum FireType {
    Pistol,
    Shotgun,
    Rifle
}