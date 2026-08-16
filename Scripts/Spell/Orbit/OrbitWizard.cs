using Godot;

public partial class OrbitWizard : OrbitProjectile
{
    // List of requirements
    // Shoot projectiles at closest enemy on 0.5s cooldown
    // Projectiles should be identical to player projectile
	[Export] public float AttackCooldown = 0.5f;
	private float cooldown = 0f;

    public override void _Ready()
    {
        base._Ready();
	}


	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		//Rotation = AngleToParent;
		Update(delta);
	}

	public void Update(double delta)
	{
		cooldown -= (float)delta;

		if (cooldown > 0)
		{
			return;
		}

		Enemy target = Util.FindNearestEnemy(this);
		if (target == null || ProjectileSpawner.Instance == null)
		{
			return;
		}

		Vector2 dir = GlobalPosition.DirectionTo(target.GlobalPosition);
		ProjectileSpawner.Instance.SpawnOrbitWizardProjectile(GlobalPosition, dir, castModifiers: CastModifiers);
		cooldown = AttackCooldown;
	}


}
