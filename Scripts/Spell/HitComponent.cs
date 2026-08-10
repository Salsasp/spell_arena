using Godot;
using System.Collections.Generic;

public partial class HitComponent : Component
{
	private ModifierManager modifierManager;
	private DespawnComponent despawnComponent;
	private SpellMovementComponent movementComponent;
	private Area2D chainRadius;
	private int chainCount = 4;
	private const int DOT_TICK_INTERVAL = 2;
	private const int DOT_DURATION = 10;
	[Export] public bool ShowDebugCircle { get; set; } = true;
	[Export] public bool ChainEnabled { get; set; } = true;

	public void Update(Node2D target)
	{
		ModifierContext context = ModifierContext.ForProjectile(GetParent<PlayerProjectile>(), target);
		ModifierEffectRegistry.Apply(ModifierPhase.OnHit, context);

		if (!context.HitResult.PreventDespawn && !context.HitResult.HandledChain)
		{
			despawnComponent.Despawn();
		}
	}

	public override void _Ready()
	{
		if (ChainEnabled)
		{
			chainRadius = GetNode<Area2D>("../ChainRadius");
		}
	}

	public void Initialize(ModifierManager modMan, DespawnComponent despawnComp, SpellMovementComponent moveComp)
	{
		modifierManager = modMan;
		despawnComponent = despawnComp;
		movementComponent = moveComp;
	}

	public void ChainSpell(Node2D currTarget)
	{
		if (chainCount <= 0)
		{
			return;
		}

		List<Node2D> chainTargets = new();

		foreach (Area2D area in chainRadius.GetOverlappingAreas())
		{
			if ((area.CollisionLayer & CollisionLayers.EnemyHurtbox) == 0)
			{
				continue;
			}

			Node2D enemyRoot = area.GetParent()?.GetParent() as Node2D;
			if (enemyRoot == null || enemyRoot == currTarget)
			{
				continue;
			}

			chainTargets.Add(enemyRoot);
		}

		if (chainTargets.Count == 0)
		{
			return;
		}

		Node2D closestEnemy = chainTargets[0];
		float currDistance = chainTargets[0].GlobalPosition.DistanceTo(currTarget.GlobalPosition);
		for (int i = 1; i < chainTargets.Count; i++)
		{
			float distance = chainTargets[i].GlobalPosition.DistanceTo(currTarget.GlobalPosition);
			if (distance < currDistance)
			{
				closestEnemy = chainTargets[i];
				currDistance = distance;
			}
		}

		ChainTo(currTarget, closestEnemy);
	}

	public void ChainTo(Node2D currTarget, Node2D enemy)
	{
		float dir = Mathf.Atan2(enemy.GlobalPosition.Y - currTarget.GlobalPosition.Y,
								enemy.GlobalPosition.X - currTarget.GlobalPosition.X);
		movementComponent.GetParent().GetNode<Area2D>("ProjArea2D").Rotation = dir;
		movementComponent.Direction = new Vector2(Mathf.Cos(dir), Mathf.Sin(dir));
		movementComponent.Speed = 1000f;
		chainCount -= 1;
	}

	public void ApplyDot(Node2D target, ModifierData modifier)
	{
		float dotDamage = modifier.dotDamage;
		DotComponent dot = new()
		{
			DamagePerTick = dotDamage * modifier.copies,
			TickInterval = DOT_TICK_INTERVAL,
			Duration = DOT_DURATION
		};

		if (!HasDotComponent(target))
		{
			target.AddChild(dot);
		}
	}

	private static bool HasDotComponent(Node2D target)
	{
		foreach (Node child in target.GetChildren())
		{
			if (child is DotComponent)
			{
				return true;
			}
		}

		return false;
	}

	public override void _Draw()
	{
		if (!ShowDebugCircle) return;

		var collisionShape = chainRadius.GetChild<CollisionShape2D>(0);
		if (collisionShape?.Shape is CircleShape2D circle)
		{
			Vector2 center = Vector2.Zero;
			Color circleColor = new Color(0.1f, 0.6f, 1.0f, 0.3f);
			DrawCircle(center, circle.Radius, circleColor);

			Color borderColor = new Color(0.1f, 0.6f, 1.0f, 0.8f);
			DrawArc(center, circle.Radius, 0, Mathf.Tau, 64, borderColor, 2.0f);
		}
	}
}
