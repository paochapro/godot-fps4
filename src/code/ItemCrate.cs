using System;

public partial class ItemCrate : RigidBody3D, FireRayReact
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnFireRayHit()
	{
		GD.Print("Hello!");
		QueueFree();
	}
}