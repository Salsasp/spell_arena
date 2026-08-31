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
	private bool hasAppliedInitialTick;

	public override void _Ready()
	{
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
