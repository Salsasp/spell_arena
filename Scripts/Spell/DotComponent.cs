using Godot;
using System;

public partial class DotComponent : Component
{
	public float DamagePerTick { get; set; }
    public float TickInterval { get; set; }
    public float Duration { get; set; }
    
    private float timeRemaining;
    private float timeSinceLastTick;
	private HealthComponent healthComponent;
    
	public override void _Ready()
    {
		healthComponent = GetNode<HealthComponent>("../HealthComponent");
        timeRemaining = Duration;
    }

    /// <summary>
    /// Re-applies this DOT from a fresh hit. Takes the stronger of the two tick damages so a
    /// weaker source can never downgrade an existing burn, and restarts the duration.
    /// </summary>
    public void Refresh(float damagePerTick, float duration)
    {
        DamagePerTick = Mathf.Max(DamagePerTick, damagePerTick);
        Duration = duration;
        timeRemaining = duration;
    }

    public override void _PhysicsProcess(double delta)
    {
        timeRemaining -= (float)delta;
        if (timeRemaining <= 0)
        {
            QueueFree();
            return;
        }
        
        timeSinceLastTick += (float)delta;
        if (timeSinceLastTick >= TickInterval)
        {
            Tick();
            timeSinceLastTick = 0;
        }
    }
    
    private void Tick()
    {
        healthComponent.TakeDamage((int)DamagePerTick);
    }
}
