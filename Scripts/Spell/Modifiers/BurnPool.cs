using Godot;
using System;

public partial class BurnPool : Node2D
{
	[Export] public AnimatedSprite2D BurnPoolSprite;
	[Export] public Area2D BurnPoolArea2D;
	[Export] public Timer DamageCooldownTimer;
	[Export] public GpuParticles2D BurnPoolFireParticles;
	[Export] public double FirePoolDamageCooldown = 0.2;
	[Export] public DamageComponent damageComponent;
	[Export] public HitComponent hitComponent;
	[Export] public LifetimeComponent lifetimeComponent;
	private bool damageTimerActive = false;

	public override void _Ready()
	{
		//BurnPoolArea2D.AreaEntered += OnBurnPoolEntered;
		DamageCooldownTimer.Timeout += OnDamageTimerTimeout;

		DamageCooldownTimer.Start(FirePoolDamageCooldown);
		damageTimerActive = true;

		SortLayers.Apply(this, SortLayers.Floors);
	}



    public override void _PhysicsProcess(double delta)
    {
		foreach (Node2D body in BurnPoolArea2D.GetOverlappingBodies())
		{
			if (body is Enemy enemy)
			{
				
			}
		}

        if (lifetimeComponent.TimedOut)
		{
			QueueFree();
		}
    }


	// public void OnBurnPoolEntered(Area2D area)
	// {
	// 	if (damageTimerActive) return;
	// }

	public void OnDamageTimerTimeout()
	{
		damageTimerActive = false;
	}

}
