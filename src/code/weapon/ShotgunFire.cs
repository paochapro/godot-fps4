partial class ShotgunFire : FireService
{
    const int BULLETS_PER_SHOT = 5;
    static readonly float SPREAD_ANGLE = Mathf.DegToRad(10);

    public ShotgunFire(float damage, PhysicsDirectSpaceState3D spaceState) 
        : base(damage, spaceState)
    {
    }

    public override void FireOutput()
    {
        for(int i = 0; i < BULLETS_PER_SHOT; i++)
        {
            Vector3 rayDir = GetRandomRayDirection();

            PhysicsRayQueryParameters3D rayParams = new() { 
                From = GlobalPosition,
                To = GlobalPosition + (rayDir * rayDistance),
                CollisionMask = 1
            };

            var result = spaceState.IntersectRay(rayParams);

            if(result.Count != 0)
            {
                Node3D ent = (Node3D)result["collider"];

                if(ent is FireRayReact rayReact)
                    rayReact.OnFireRayHit(Damage);

                PlaceBulletDecal(ent, (Vector3)result["position"], (Vector3)result["normal"]);
            }
        }
    }

    Vector3 GetRandomRayDirection()
    {
        Basis basis = GlobalTransform.Basis;
        Vector3 forward = -basis.Z;
        Vector3 right = basis.X;

        float axisRotateAngle = Mathf.Lerp(0,180,GD.Randf());
        float resultRotateAngle = Mathf.Lerp(-SPREAD_ANGLE, SPREAD_ANGLE, GD.Randf());
        Vector3 randomAxis = right.Rotated(forward, axisRotateAngle);
        Vector3 result = forward.Rotated(randomAxis.Normalized(), resultRotateAngle);

        return result;
    }
} 