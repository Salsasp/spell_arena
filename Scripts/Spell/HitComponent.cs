using Godot;
using System;
using System.Collections.Generic;

public partial class HitComponent : Component
{
	private ModifierManager modifierManager;
	private DespawnComponent despawnComponent;
	private SpellMovementComponent movementComponent;
	private Area2D chainRadius;
	private ProjectileSpawner projectileSpawner;
	private int chainCount = 4;
	private const int DOT_TICK_INTERVAL = 2;
	private const int DOT_DURATION = 10;
	[Export] public bool ShowDebugCircle { get; set; } = true;

	public void Update(Enemy enemy)
	{
		if (modifierManager.ActiveModifiers.ContainsKey("chain_spells"))
		{
			ChainSpell(enemy);
		}
		else if (!modifierManager.ActiveModifiers.ContainsKey("pierce_spells"))
		{
			despawnComponent.Despawn();
		}

		if (modifierManager.ActiveModifiers.ContainsKey("fire_spells"))
		{
			ApplyDot(enemy, modifierManager.ActiveModifiers["fire_spells"]);
		}
	}

	public override void _Ready()
	{
		//modifierManager = GetNode<ModifierManager>("../../Player/ModifierManager");
		//despawnComponent = GetNode<DespawnComponent>("../DespawnComponent");
		chainRadius = GetNode<Area2D>("../ChainRadius");
		projectileSpawner = GetNode<ProjectileSpawner>("../../ProjectileSpawner");
	}

	public void Initialize(ModifierManager modMan, DespawnComponent despawnComp, SpellMovementComponent moveComp)
	{
		modifierManager = modMan;
		despawnComponent = despawnComp;
		movementComponent = moveComp;
	}

	public void ChainSpell(Enemy currTarget)
	{
		if (chainCount <= 0)
		{
			return;
		}

		List<Node2D> chainTargets = new();

		// Acquire potential chain candidates
		foreach (Node2D body in chainRadius.GetOverlappingAreas())
		{
			// Potential pitfalls for future debugging: this grabs ANY area2D, including projectiles and player
			// TODO: clean up tree navigation to avoid future errors
			Node2D bodyRoot = (Node2D)body.GetParent().GetParent();
			if (body.IsInGroup("enemies") && bodyRoot != currTarget)
			{
				chainTargets.Add(body);
			}
		}

		// No enemies in chain radius
		if (chainTargets.Count == 0)
		{
			return;
		}

		// Obtain closest enemy
		Node2D closestEnemy = chainTargets[0];
		float currDistance = chainTargets[0].GlobalPosition.DistanceTo(currTarget.GlobalPosition);
		if (chainTargets.Count > 1)
		{
			for (int i = 1; i < chainTargets.Count; i++)
			{
				float distance = chainTargets[i].GlobalPosition.DistanceTo(currTarget.GlobalPosition);
				if (distance < currDistance)
				{
					closestEnemy = chainTargets[i];
					currDistance = distance;
				}
			}
		}
		
		ChainTo(currTarget, closestEnemy);
	}

	// TODO: change chain structure to recursive by passing in curr and nearest enemy,
	// decrementing chain limit until 0

	public void ChainTo(Enemy currTarget, Node2D enemy)
	{
		float dir = Mathf.Atan2(enemy.GlobalPosition.Y - currTarget.GlobalPosition.Y,
								enemy.GlobalPosition.X - currTarget.GlobalPosition.X);
		movementComponent.GetParent().GetNode<Area2D>("ProjArea2D").Rotation = dir;
		movementComponent.Direction = new Vector2(Mathf.Cos(dir), Mathf.Sin(dir));
		movementComponent.Speed = 1000f;
		chainCount -= 1;
	}

	public void ApplyDot(Enemy enemy, ModifierData modifier)
	{
		float dotDamage = modifier.dotDamage;
		DotComponent dot = new()
		{
			DamagePerTick = dotDamage,
			TickInterval = DOT_TICK_INTERVAL,
			Duration = DOT_DURATION
		};

		if (!enemy.DotApplied())
		{
			enemy.AddChild(dot);
		}
	}

	public override void _Draw()
	{
		if (!ShowDebugCircle) return;

		// Get the current radius dynamically from your CollisionShape2D child
		var collisionShape = chainRadius.GetChild<CollisionShape2D>(0);
		if (collisionShape?.Shape is CircleShape2D circle)
		{
			// Center of the circle relative to this node
			Vector2 center = Vector2.Zero; 
			
			// Color(Red, Green, Blue, Alpha/Translucency)
			Color circleColor = new Color(0.1f, 0.6f, 1.0f, 0.3f); // Translucent Blue

			// Draw the filled translucent circle
			DrawCircle(center, circle.Radius, circleColor);

			// Optional: Draw a solid crisp border outline so it stands out
			Color borderColor = new Color(0.1f, 0.6f, 1.0f, 0.8f);
			DrawArc(center, circle.Radius, 0, Mathf.Tau, 64, borderColor, 2.0f);
		}
	}
}
