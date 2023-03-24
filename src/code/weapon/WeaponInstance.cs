partial class WeaponInstance : Node3D
{	
	Weapon weapon;
	FireService fire;
	Timer reloadTimer;
	Node3D model;
	AnimationPlayer modelAnims;

	int magazineReloadAmount; //not very safe

	bool isReloading => !reloadTimer.IsStopped();

	public Weapon Weapon => weapon;
	public Node3D Model => model;
	public double ReloadTimeLeft => reloadTimer.TimeLeft;
	
	public event Action? OnFire;
	public event Action<int>? OnReloadEnd;

	public WeaponInstance(Weapon weapon, WeaponData data, Node3D model, PhysicsDirectSpaceState3D ss)
	{
		this.weapon = weapon;

		//Create FireService
		FireService fireService;
		float dmg = data.damage;
		
		switch(data.fireType) {
			case FireType.Pistol:
				fireService = new PistolFire(dmg, ss);
				break;
			case FireType.Shotgun:
				fireService = new ShotgunFire(dmg, ss);
				break;
			default:
				fireService = new PistolFire(dmg, ss);
				break;
		}
		
		this.fire = fireService;
		AddChild(fire);

		//Get animations
		this.model = model;
		this.modelAnims = model.GetNode<AnimationPlayer>("AnimationPlayer");

		//Reload timer
		reloadTimer = new Timer();
		reloadTimer.WaitTime = data.reloadTime;
		reloadTimer.OneShot = true;
		reloadTimer.Timeout += ReloadEnd;
		AddChild(reloadTimer);
	}

	public bool TryFire()
	{
		if(!weapon.CanFire) return false;

		fire.FireOutput();
		modelAnims.PlayOrPass("fire");
		OnFire?.Invoke();

		return true;
	}

	public bool TryStartReloading(int givenAmmo)
	{
		bool cancel = (
			!weapon.CanReload ||
			givenAmmo == 0 ||
			isReloading
		);
		if(cancel) return false;

		modelAnims.PlayOrPass("reloading");
		reloadTimer.Start();
		magazineReloadAmount = givenAmmo;

		return true;
	}

	void ReloadEnd()
	{
		modelAnims.PlayOrPass("end_reloading");
		OnReloadEnd?.Invoke(magazineReloadAmount);
		magazineReloadAmount = 0;
	}
}
