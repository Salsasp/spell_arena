using Godot;

public partial class TextProjectile : Node2D
{
	private DamageComponent damageComponent;
	private DespawnComponent despawnComponent;
	private LifetimeComponent lifetimeComponent;
	private SpellMovementComponent spellMovementComponent;
	private RichTextLabel richTextLabel;

	public string Text = "";

	public override void _Ready()
	{
		damageComponent = GetNode<DamageComponent>("DamageComponent");
		despawnComponent = GetNode<DespawnComponent>("DespawnComponent");
		lifetimeComponent = GetNode<LifetimeComponent>("LifetimeComponent");
		spellMovementComponent = GetNode<SpellMovementComponent>("SpellMovementComponent");
		richTextLabel = GetNode<RichTextLabel>("ProjArea2D/RichTextLabel");
		spellMovementComponent.SetHomingEnabled(true, 4.0f);
		SortLayers.Apply(this, SortLayers.Projectiles);

		if (richTextLabel != null)
		{
			richTextLabel.BbcodeEnabled = true;
			richTextLabel.Text = Text;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		spellMovementComponent.Update(delta);
		GlobalPosition += spellMovementComponent.Velocity * (float)delta;

		if (lifetimeComponent.TimedOut)
		{
			despawnComponent.Despawn();
		}
	}
}
