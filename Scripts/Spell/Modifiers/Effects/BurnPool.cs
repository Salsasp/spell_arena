using Godot;
using System;

public partial class BurnPool : Node2D
{
	[Export] public AnimatedSprite2D BurnPoolSprite;
	[Export] public Area2D BurnPoolArea2D;
	[Export] public Timer DamageCooldownTimer;
	[Export] public GpuParticles2D BurnPoolFireParticles;
	[Export] public int FirePoolDamage;
	[Export] public double FirePoolDamageCooldown = 0.2;
	private bool damageTimerActive = false;

	public override void _Ready()
	{
		BurnPoolArea2D.AreaEntered += OnBurnPoolEntered;
		DamageCooldownTimer.Timeout += OnDamageTimerTimeout;
	}

	public void OnBurnPoolEntered(Area2D area)
	{
		if (damageTimerActive) return;

		Enemy entityRoot = area.GetParent<Enemy>();
		entityRoot.healthComponent.TakeDamage(FirePoolDamage);
		DamageCooldownTimer.Start(FirePoolDamageCooldown);
		damageTimerActive = true;
	}

	public void OnDamageTimerTimeout()
	{
		damageTimerActive = false;
	}

}
