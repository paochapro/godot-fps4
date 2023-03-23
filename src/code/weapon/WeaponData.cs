readonly record struct WeaponData (
    string modelUID,
    string soundFire,
    string soundReload,
    float reloadTime,
    int magazineCapacity,
    float fireRate,
    float damage,
    AmmoType ammoType,
    FireType fireType
);

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