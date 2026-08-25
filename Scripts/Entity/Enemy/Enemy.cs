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
	public bool IsFalling { get; set; }
	public bool MarkedForBurnPoolSpawn = false;
	private EnemyContext context;
	private TextManagerComponent textManagerComponent;
	private Vector2 fallStartPosition;
	private int pitfallDamage;
	private Pitfall activePitfall;

	public override void _Ready()
	{
		IsFalling = false;
		SortLayers.Apply(this, SortLayers.Entities);

		Node sceneRoot = GetTree().CurrentScene;
		textManagerComponent = sceneRoot.GetNode<TextManagerComponent>("TextManagerComponent");

		ProjectileSpawner projectileSpawner = sceneRoot.GetNode<ProjectileSpawner>("ProjectileSpawner");
		Player player = sceneRoot.GetNodeOrNull<Player>("Player");
		ArenaBounds arenaBounds = sceneRoot.GetNodeOrNull<ArenaBounds>("ArenaBounds");
		context = new EnemyContext(this, player, movementComponent, projectileSpawner, arenaBounds);

		movementBehavior?.Initialize(context);
		attackBehavior?.Initialize(context);
		arenaClamp?.Initialize(context);

		healthComponent.HealthChanged += OnHealthChanged;
		healthComponent.Dead += OnDead;
		hitboxComponent.Hit += OnHit;
		animationComponent.FallAnimationFinished += OnFallFinished;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsFalling) return;

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

		var hitByRoot = hitBy.GetParent<Node2D>();
		DamageComponent hitByDamageComponent = hitByRoot.GetNodeOrNull<DamageComponent>("DamageComponent");
		HitComponent hitByHitComponent = hitByRoot.GetNodeOrNull<HitComponent>("HitComponent");
		if (hitByDamageComponent == null)
		{
			GD.PrintErr("Hit by area without valid DamageComponent");
			return;
		}

		bool isCrit = hitByDamageComponent.IsCriticalHit();
		int damage = hitByDamageComponent.CalculateDamage(isCrit);
		bool showText = hitByRoot is not TextProjectile;

		ApplyDamage(damage, isCrit, showText);

		if (hitByHitComponent != null)
		{
			hitByHitComponent.Update(this);
		}
		else
		{
			hitByRoot.QueueFree();
		}
	}

	public void ApplyDamage(int damage, bool isCrit, bool showText = true, bool showHitAnimation = true)
	{
		healthComponent.TakeDamage(damage);

		if (showText && textManagerComponent != null)
		{
			List<string> textModifiers = new List<string>() { "color=red", "center" };
			string damageString = damage.ToString();

			if (isCrit)
			{
				textModifiers.Add("shake");
				damageString += " - CRIT!";
			}

			textManagerComponent.DrawAbove(this, -30, damageString, textModifiers, damage);
		}

		if (showHitAnimation)
		{
			animationComponent.PlayProjectileHitAnimation(GlobalPosition);
		}
	}

	private void OnDead()
	{
		if (MarkedForBurnPoolSpawn)
		{
			Callable.From(() => PoolManager.Instance.SpawnBurnPool(GlobalPosition)).CallDeferred();
		}
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

    public void Fall(Pitfall pitfall)
    {
		this.pitfallDamage = pitfall.PitfallDamage; // ik this is sloppy but prevents having to pass it through signals
        if (IsFalling)
			return;
		IsFalling = true;
		movementComponent.direction = Vector2.Zero;

		GlobalPosition = pitfall.FallStartPoint.GlobalPosition;

		if (!animationComponent.TryPlayFallAnimation(pitfall))
		{
			IsFalling = false; // no anim configured — fail safe
		}
    }

	private void OnFallFinished()
	{
		QueueFree();
		healthComponent.Die();
	}
}
