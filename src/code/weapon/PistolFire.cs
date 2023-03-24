partial class PistolFire : FireService
{
	public PistolFire(float damage, PhysicsDirectSpaceState3D spaceState) 
		: base(damage, spaceState)
	{
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
				rayReact.OnFireRayHit(Damage);

			PlaceBulletDecal(obj, position, normal);
		}
	}
}