using Godot;

public abstract partial class AIMovementBehaviorComponent : Component
{
	protected EnemyContext context;
	protected MovementComponent movement;

	public virtual void Initialize(EnemyContext ctx)
	{
		context = ctx;
		movement = ctx.Movement;
	}

	public abstract void Update(double delta);
}
