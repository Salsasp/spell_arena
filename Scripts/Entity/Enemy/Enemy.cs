using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Enemy : CharacterBody2D, IEntity
{
	[Export] public HealthComponent healthComponent { get; set; }
	[Export] public HitboxComponent hitboxComponent { get; set; }
	[Export] public MovementComponent movementComponent { get; set; }
	[Export] public AIMovementBehaviorComponent movementBehavior { get; set; }
	[Export] public AttackBehaviorComponent attackBehavior { get; set; }
	[Export] public AnimationComponent animationComponent { get; set; }
	[Export] public ArenaClampComponent arenaClamp { get; set; }
	public bool isFalling { get; set; }
	private EnemyContext context;
	private TextManagerComponent textManagerComponent;

	public override void _Ready()
	{
		isFalling = false;
		textManagerComponent = GetNode<TextManagerComponent>("../../TextManagerComponent");

		ProjectileSpawner projectileSpawner = GetNode<ProjectileSpawner>("../../ProjectileSpawner");
		Player player = GetNodeOrNull<Player>("../../Player");
		ArenaBounds arenaBounds = GetNodeOrNull<ArenaBounds>("../../ArenaBounds");
		context = new EnemyContext(this, player, movementComponent, projectileSpawner, arenaBounds);

		movementBehavior?.Initialize(context);
		attackBehavior?.Initialize(context);
		arenaClamp?.Initialize(context);

		healthComponent.HealthChanged += OnHealthChanged;
		healthComponent.Dead += OnDead;
		hitboxComponent.Hit += OnHit;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!context.IsPlayerValid)
		{
			movementComponent.direction = Vector2.Zero;
			movementComponent.Update(delta);
			return;
		}

		movementBehavior?.Update(delta);
		attackBehavior?.Update(delta);
		movementComponent.Update(delta);
		animationComponent.Update(movementComponent);
		arenaClamp?.Apply();
	}

	private T FindBehavior<T>() where T : Component
	{
		foreach (Node child in GetChildren())
		{
			if (child is T behavior)
			{
				return behavior;
			}
		}

		return null;
	}

	private void OnHealthChanged(int newHealth)
	{
		//GD.Print($"Health changed: {newHealth}");
	}

	public void OnHit(Area2D hitBy)
	{
		if ((hitBy.CollisionLayer & CollisionLayers.PlayerProjectile) == 0)
		{
			return;
		}

		Node2D projectileRoot = hitBy.GetParent<Node2D>();
		DamageComponent hitByDamageComponent = projectileRoot.GetNodeOrNull<DamageComponent>("DamageComponent");
		HitComponent hitByHitComponent = projectileRoot.GetNodeOrNull<HitComponent>("HitComponent");
		if (hitByDamageComponent == null)
		{
			GD.PrintErr("Hit by area without valid DamageComponent");
			return;
		}

		bool isCrit = hitByDamageComponent.IsCriticalHit();
		int damage = hitByDamageComponent.CalculateDamage(isCrit);

		healthComponent.TakeDamage(damage);

		if (hitByHitComponent != null)
		{
			hitByHitComponent.Update(this);
		}
		else
		{
			projectileRoot.QueueFree();
		}

		List<string> textModifiers = new List<string>() { "color=red", "center" };
		string damageString = damage.ToString();

		if (isCrit)
		{
			textModifiers.Add("shake");
			damageString += " - CRIT!";
		} 
		
		if (textManagerComponent != null && projectileRoot is not TextProjectile)
		{
			textManagerComponent.DrawAbove(this, -30, damageString, textModifiers, damage);
		}
	}

	private void OnDead()
	{
		//GD.Print("Enemy died!");
	}

	public bool DotApplied()
	{
		foreach (Node child in GetChildren())
		{
			if (child is DotComponent)
			{
				return true;
			}
		}

		return false;
	}

	public void Fall()
	{
		
	}
}
