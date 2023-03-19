class AmmoService
{
    readonly int magazineCapacity;
    int ammo;
    int magazine;

    public int Ammo => ammo;
    public int Magazine => magazine;

    public AmmoService(int startAmmo, int startMagazine, int magazineCapacity)
    {
        this.ammo = startAmmo;
        this.magazine = startMagazine;
        this.magazineCapacity = magazineCapacity;
    }

    public bool CanReload => ammo > 0 && magazine != magazineCapacity;
    public bool CanFire => magazine > 0;

    public bool TryFire()
    {
        if(magazine <= 0)
            return false;

        magazine -= 1;

        return true;
    }   

    public bool TryReload()
    {
        if(ammo <= 0)
            return false;

        int taken = magazineCapacity - magazine;

        if(ammo < taken)
            taken = ammo;

        ammo -= taken;
        magazine += taken;

        return true;
    }
}