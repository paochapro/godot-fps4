abstract partial class FireService : Node3D
{
	float damage;

	static PackedScene decalScene;

	protected const int rayDistance = 300;
	protected float Damage => damage;
	protected PhysicsDirectSpaceState3D spaceState;

	static FireService() => decalScene = GD.Load<PackedScene>("uid://dle5bqyhjn142");

	public FireService(float damage, PhysicsDirectSpaceState3D spaceState)  {
		this.spaceState = spaceState;
		this.damage = damage;
	}

	public abstract void FireOutput();

	protected void PlaceBulletDecal(Node3D node, Vector3 pos, Vector3 normal)
	{
		var decal = decalScene.Instantiate<Decal>();

		node.AddChild(decal);

		Vector3 y = normal;
		Vector3 z = y.Cross(-GlobalTransform.Basis.Z).Normalized();
		Vector3 x = y.Cross(z).Normalized();

		Basis decalBasis = new Basis(x,y,z);

		decal.GlobalPosition = pos;
		decal.GlobalTransform = decal.GlobalTransform with { Basis = decalBasis };
	}

	void DebugShowDecalBasisVectors(Node3D node, Basis decalBasis, Vector3 pos)
	{
		const float length = 0.3f;
		Node3D basisVectors = new();
		basisVectors.AddChild(Utils.GenerateLine(Vector3.Zero, decalBasis.X * length, Color.Color8(255,0,0)));
		basisVectors.AddChild(Utils.GenerateLine(Vector3.Zero, decalBasis.Y * length, Color.Color8(0,255,0)));
		basisVectors.AddChild(Utils.GenerateLine(Vector3.Zero, decalBasis.Z * length, Color.Color8(0,0,255)));
		node.AddChild(basisVectors);
		basisVectors.GlobalPosition = pos;
	}
}