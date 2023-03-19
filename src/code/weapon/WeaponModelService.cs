partial class WeaponModelService : Node3D
{
    AnimationPlayer anim;

    public WeaponModelService(Node3D model)
    {
        anim = model.GetNode<AnimationPlayer>("AnimationPlayer");
        AddChild(model);
    }

    public void OnFire()
    {

    }

    public void OnStartReloading()
    {

    }

    public void OnEndReloading()
    {

    }
}