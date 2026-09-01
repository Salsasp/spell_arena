using Godot;

public partial class BurnPool : Node2D
{
	[Export] public Sprite2D BurnPoolSprite;
	[Export] public Area2D BurnPoolArea2D;
	[Export] public Timer DamageCooldownTimer;
	[Export] public GpuParticles2D BurnPoolFireParticles;
	[Export] public double FirePoolDamageCooldown = 0.2;
	[Export] public DamageComponent damageComponent;
	[Export] public HitComponent hitComponent;
	[Export] public LifetimeComponent lifetimeComponent;
	[Export] public AnimationPlayer AnimPlayer;

	/// <summary> Added to the radius multiplier per fire innate stack beyond the first. </summary>
	private const float RADIUS_PER_EXTRA_STACK = 0.25f;
	/// <summary> Added to the tick damage multiplier per fire innate stack beyond the first. </summary>
	private const float TICK_DAMAGE_PER_EXTRA_STACK = 0.5f;

	/// <summary> Fire wand innate stacks behind this pool. Must be set before the node enters the tree. </summary>
	public int Stacks { get; set; } = 1;

	private bool hasAppliedInitialTick;

	public static float GetRadiusMultiplier(int stacks)
	{
		return 1f + (ExtraStacks(stacks) * RADIUS_PER_EXTRA_STACK);
	}

	public static float GetTickDamageMultiplier(int stacks)
	{
		return 1f + (ExtraStacks(stacks) * TICK_DAMAGE_PER_EXTRA_STACK);
	}

	private static int ExtraStacks(int stacks)
	{
		return Mathf.Max(0, stacks - 1);
	}

	public override void _Ready()
	{
		// Scaling the root covers the damage area, sprites and particles in one step, and
		// avoids mutating the CircleShape2D sub-resource, which every pool instance shares.
		Scale = Vector2.One * GetRadiusMultiplier(Stacks);
		damageComponent.BaseDamage = Mathf.RoundToInt(damageComponent.BaseDamage * GetTickDamageMultiplier(Stacks));

		DamageCooldownTimer.WaitTime = FirePoolDamageCooldown;
		DamageCooldownTimer.OneShot = false;
		DamageCooldownTimer.Timeout += OnDamageTimerTimeout;
		DamageCooldownTimer.Start();

		SortLayers.Apply(this, SortLayers.Floors);

		AnimPlayer.Play("spawn");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!hasAppliedInitialTick)
		{
			hasAppliedInitialTick = true;
			ApplyTickDamage();
		}

		if (lifetimeComponent.TimedOut)
		{
			QueueFree();
		}
	}

	public void OnDamageTimerTimeout()
	{
		ApplyTickDamage();
	}

	private void ApplyTickDamage()
	{
		foreach (Node2D body in BurnPoolArea2D.GetOverlappingBodies())
		{
			if (body is Enemy enemy)
			{
				bool isCrit = damageComponent.IsCriticalHit();
				int damage = damageComponent.CalculateDamage(isCrit);
				enemy.ApplyDamage(damage, isCrit, showHitAnimation: false);
			}
		}
	}
}
