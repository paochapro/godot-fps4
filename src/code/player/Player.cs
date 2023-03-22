using System.Collections.Generic;

partial class Player : CharacterBody3D
{
	float sens = 0.0017f;
	readonly float MAX_YAW = Mathf.DegToRad(90);
	readonly float MIN_YAW = Mathf.DegToRad(-90);
	Vector3 spawnPos;
	[Export] float pushForce = 0.2f;

	//Initialization of these properties happen in _Ready instead of constructor
	//Thus we need to disable nullable to stop getting warnings
	#nullable disable
	PlayerMovement pm;
	PlayerWeaponManager weaponManager;

	Camera3D camera;
	Map map;
	Gui gui;
	#nullable restore

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

		WeaponData startWeaponData = new() { 
			ammoType = AmmoType.Pistol,
			fireType = FireType.Pistol,
			damage = 10,
			magazineCapacity = 5,
			modelUID = "uid://siry2qbvuvw4",
			reloadTime = 3f,
			//Useless for now
			fireRate = 0.5f,
			soundFire = "fire_snd",
			soundReload = "reload_snd",
		};

		var modelRoot = camera.GetNode<Node3D>("WeaponModelRoot");
		Weapon startWeapon = new(map, startWeaponData);
		weaponManager = new(map, gui, camera, modelRoot, new[] { startWeapon });
	}

	public override void _Process(double dt)
	{
		if(Input.IsActionJustPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured 
				? Input.MouseModeEnum.Visible 
				: Input.MouseModeEnum.Captured;
		}

		weaponManager.Process(dt);
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
