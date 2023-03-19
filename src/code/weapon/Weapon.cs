partial class Weapon : Node3D
{
    AmmoService ammo;
    FireService fire;
    WeaponModelService model;
    Timer timer;

    public AmmoService Ammo => ammo;
    public double ReloadTime => timer.TimeLeft;

    public Weapon(WeaponData data)
    {
        this.ammo = data.AmmoService;
        this.fire = data.FireService;
        this.model = data.ModelService;

        timer = new Timer();
        timer.WaitTime = 1;
        timer.OneShot = true;
        timer.Timeout += OnReload;

        AddChild(fire);
        AddChild(timer);
    }

    public bool TryFire()
    {
        if(ammo.TryFire())
        {
            fire.FireOutput();
            model.OnFire();
            return true;
        }

        return false;
    }

    public bool TryReload()
    {
        if(ammo.CanReload)
        {
            model.OnStartReloading();
            timer.Start();
            return true;
        }

        return false;
    }

    void OnReload()
    {
        ammo.TryReload();
        model.OnEndReloading();
    }
}

struct WeaponData {
    public AmmoService AmmoService; 
    public FireService FireService;
    public WeaponModelService ModelService;
}