using Godot;

public partial class ArenaClampComponent : Component
{
	private EnemyContext context;

	public void Initialize(EnemyContext ctx)
	{
		context = ctx;
	}

	public void Apply()
	{
		if (context?.ArenaBounds == null || owner == null)
		{
			return;
		}

		owner.GlobalPosition = context.ArenaBounds.ClampPosition(owner.GlobalPosition);
	}
}
