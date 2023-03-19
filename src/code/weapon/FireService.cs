abstract partial class FireService : Node3D
{
    protected const int rayDistance = 300;
    protected Map map;

    public FireService(Map map) => this.map = map;

    public abstract void FireOutput();
}

partial class PistolFire : FireService
{
    PackedScene decalScene = GD.Load<PackedScene>("uid://dle5bqyhjn142");

    public PistolFire(Map map) : base(map)
    {}

    public override void FireOutput()
    {
        var ss = map.GetWorld3D().DirectSpaceState;

        PhysicsRayQueryParameters3D rayParams = new() { 
            From = GlobalPosition,
            To = GlobalPosition + (-GlobalTransform.Basis.Z * rayDistance),
            CollisionMask = 1
        };

        Godot.Collections.Dictionary result = ss.IntersectRay(rayParams);

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