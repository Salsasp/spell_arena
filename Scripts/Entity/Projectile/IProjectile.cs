using Godot;

public interface IProjectile
{
    SpellMovementComponent spellMovementComponent { get; }
    AnimationComponent animationComponent { get; }
    LifetimeComponent lifetimeComponent { get; }
    DespawnComponent despawnComponent { get; }

    public void _Ready();

    public void _PhysicsProcess(double delta);
}