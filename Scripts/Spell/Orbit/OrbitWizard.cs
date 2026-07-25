using Godot;

public partial class OrbitWizard : OrbitProjectile
{
	// TODO: Add wizard exclusive stuff

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Rotation = AngleToParent;
	}
}
