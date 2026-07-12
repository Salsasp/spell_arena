using Godot;

public abstract partial class AttackBehaviorComponent : Component
{
	protected EnemyContext context;

	public virtual void Initialize(EnemyContext ctx)
	{
		context = ctx;
	}

	public abstract void Update(double delta);
}
