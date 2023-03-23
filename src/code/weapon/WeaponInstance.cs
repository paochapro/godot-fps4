partial class WeaponInstance : Node3D
{	
	Weapon weapon;
	FireService fire;
	Timer reloadTimer;
	AnimationPlayer modelAnims;

	int magazineReloadAmount; //not very safe

	bool isReloading => !reloadTimer.IsStopped();
	public Weapon Weapon => weapon;
	public double ReloadTimeLeft => reloadTimer.TimeLeft;
	
	public event Action? OnFire;
	public event Action<int>? OnReloadEnd;

	public WeaponInstance(Weapon weapon, WeaponData data, Node3D modelRoot, PhysicsDirectSpaceState3D ss)
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
				fireService = new PistolFire(dmg, ss);
				break;
			default:
				fireService = new PistolFire(dmg, ss);
				break;
		}
		
		this.fire = fireService;
		AddChild(fire);

		//Create model
		var modelScene = GD.Load<PackedScene>(data.modelUID);
		var model = modelScene.Instantiate<Node3D>();
		this.modelAnims = model.GetNode<AnimationPlayer>("AnimationPlayer");
		modelRoot.AddChild(model);

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
