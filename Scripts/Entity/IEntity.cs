using Godot;

public interface IEntity
{
    MovementComponent movementComponent { get; }
    HealthComponent healthComponent { get; }
    HitboxComponent hitboxComponent { get; }
    AnimationComponent animationComponent { get; }
    bool IsFalling { get; set; }
    void OnHit(Area2D hitBy);
    void Fall(Pitfall pitfall);
}