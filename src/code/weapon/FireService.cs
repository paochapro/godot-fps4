abstract partial class FireService : Node3D
{
	float damage;

	protected const int rayDistance = 300;
	protected float Damage => damage;
	protected PhysicsDirectSpaceState3D spaceState;

	public FireService(float damage, PhysicsDirectSpaceState3D spaceState)  {
		this.spaceState = spaceState;
		this.damage = damage;
	}

	public abstract void FireOutput();
}

partial class PistolFire : FireService
{
	PackedScene decalScene;

	public PistolFire(float damage, PhysicsDirectSpaceState3D spaceState) : base(damage, spaceState)
	{
		//TODO: needs to be redone
		decalScene = GD.Load<PackedScene>("uid://dle5bqyhjn142");
	}

	public override void FireOutput()
	{
		PhysicsRayQueryParameters3D rayParams = new() { 
			From = GlobalPosition,
			To = GlobalPosition + (-GlobalTransform.Basis.Z * rayDistance),
			CollisionMask = 1
		};

		Godot.Collections.Dictionary result = spaceState.IntersectRay(rayParams);

		if(result.Count != 0)
		{
			var position = (Vector3)result["position"];
			var normal = (Vector3)result["normal"];
			var obj = (Node3D)result["collider"];

			if(obj is FireRayReact rayReact)
				rayReact.OnFireRayHit();

			PlaceBulletDecal(obj, position, normal);
		}
	}

	void PlaceBulletDecal(Node3D node, Vector3 pos, Vector3 normal)
	{
		var decal = decalScene.Instantiate<Decal>();

		node.AddChild(decal);

		Basis normalBasis = Utils.BasisFromVector(normal);
		Basis newBasis = Basis.Identity with { 
			X = normalBasis.X,
			Y = normalBasis.Z,
			Z = normalBasis.Y 
		};

		decal.GlobalPosition = pos;
		decal.GlobalTransform = decal.GlobalTransform with { Basis = newBasis };
	}
}