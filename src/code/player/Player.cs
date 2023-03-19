partial class Player : CharacterBody3D
{
	PlayerMovement pm;
	Weapon currentWeapon;

	float sens = 0.0017f;
	readonly float MAX_YAW = Mathf.DegToRad(90);
	readonly float MIN_YAW = Mathf.DegToRad(-90);
	Vector3 spawnPos;
	[Export] float pushForce = 0.2f;

	//Nodes
	Camera3D camera;
	Map map;
	Gui gui;

	public override void _Ready()
	{
		spawnPos = GlobalPosition;
		base.FloorStopOnSlope = true;
		base.FloorSnapLength = 1.0f;
		base.WallMinSlideAngle = 0;

		pm = GetNode<PlayerMovement>("PlayerMovement");
		camera = GetNode<Camera3D>("Camera3D");
		map = GetNode<Map>("/root/Root/Map");
		gui = GetNode<Gui>("/root/Root/Gui");

		var weaponModel = GD.Load<PackedScene>("uid://siry2qbvuvw4").Instantiate<Node3D>();
		var weaponModelRoot = GetNode<Node3D>("Camera3D/WeaponModelRoot");
		var weaponModelService = new WeaponModelService(weaponModel);
		weaponModelRoot.AddChild(weaponModelService);

		WeaponData weaponData = new WeaponData() {
			AmmoService = new AmmoService(999, 999, 999),
			FireService = new PistolFire(map),
			ModelService = weaponModelService
		};

		currentWeapon = new Weapon(weaponData);
		currentWeapon.Name = "Weapon";
		camera.AddChild(currentWeapon);
	}

	public override void _Process(double dt)
	{
		if(Input.IsActionJustPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured 
				? Input.MouseModeEnum.Visible 
				: Input.MouseModeEnum.Captured;
		}

		//Weapon
		if(Input.IsActionJustPressed("fire"))
		{
			bool fired = currentWeapon.TryFire();
			DebugVars.Set("Fired", fired);
		}

		if(Input.IsActionJustPressed("reload"))
		{
			DebugVars.Set("Reloaded", currentWeapon.TryReload());
		}

		Label ammo = gui.GetNode<Label>("Ammo/Label");
		Label reloadTimer = gui.GetNode<Label>("ReloadTime/Label");
		ammo.Text = $"{currentWeapon.Ammo.Magazine}/{currentWeapon.Ammo.Ammo}";
		reloadTimer.Text = currentWeapon.ReloadTime.ToString("F2");
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		//Clamp
		if(GlobalPosition.Y < -200)
			GlobalPosition = spawnPos;

		//Movement
		Vector2 inputDir = Input.GetVector("go_left", "go_right", "go_forward", "go_backward");
		bool inputJump = Input.IsActionJustPressed("jump");

		Vector3 hVelocity = pm.HorizontalMovement(inputDir, camera.Rotation, GetWallNormal(), dt);
		float yVelocity = pm.VerticalMovement(inputJump, IsOnFloor(), IsOnCeiling(), dt);

		this.Velocity = new Vector3(hVelocity.X, yVelocity, hVelocity.Z);
		MoveAndSlide();
		Collision();
	}

	public override void _Input(InputEvent ev)
	{
		if(ev is InputEventMouseMotion motion)
			if(Input.MouseMode == Input.MouseModeEnum.Captured)
				Looking(motion.Relative);
	}

	void Collision()
	{
		foreach(KinematicCollision3D coll in this.GetAllSlideCollisions())
		{
			var collider = coll.GetCollider();
			if(collider is RigidBody3D rigid)
			{
				//rigid.ApplyImpulse(-coll.GetNormal() * pushForce, coll.GetPosition());
				rigid.ApplyCentralImpulse(-coll.GetNormal() * pushForce);
			}
		}
	}

	void Looking(Vector2 relative) {
		Vector3 newRotation = camera.Rotation;
		newRotation.Y -= relative.X * sens;
		newRotation.X -= relative.Y * sens;
		newRotation.X = Mathf.Clamp(newRotation.X, MIN_YAW, MAX_YAW);
		camera.Rotation = newRotation;
	}
}
