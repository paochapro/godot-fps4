public partial class Gui : Control
{
	VBoxContainer varsContainer;

	public override void _Ready()
	{
		varsContainer = GetNode<VBoxContainer>("vars");
		var fpsSpinbox = GetNode<SpinBox>("options/fps/SpinBox");
		fpsSpinbox.Value = Engine.PhysicsTicksPerSecond;
	}

	public void _on_spin_box_value_changed(float value)
	{
		Engine.PhysicsTicksPerSecond = (int)value;
	}

	public override void _Process(double delta)
	{
		UpdateDebugVars();
	}

	void UpdateDebugVars()
	{
		foreach(var pair in DebugVars.Vars)
		{
			Label pairLabel = varsContainer.GetNodeOrNull<Label>(pair.Key);

			if(pairLabel == null)
			{
				pairLabel = new Label();
				pairLabel.Name = pair.Key;
				varsContainer.AddChild(pairLabel);
			}

			pairLabel.Text = $"{pair.Key}: {pair.Value}";
		}
	}
}