using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	private InputComponent inputComponent;
	private MovementComponent movementComponent;
	private HealthComponent healthComponent;
	private HitboxComponent hitboxComponent;
	private ProjectileSpawner projectileSpawner;
	private AnimationComponent animationComponent;
	private Wand wand;
	private ModifierManager modifierManager;

	public override void _Ready()
	{
		projectileSpawner = GetNode<ProjectileSpawner>("../ProjectileSpawner");
		inputComponent = GetNode<InputComponent>("InputComponent");
		movementComponent = GetNode<MovementComponent>("MovementComponent");
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		hitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
		animationComponent = GetNode<AnimationComponent>("AnimationComponent");
		wand = GetNode<Wand>("Wand");
		modifierManager = GetNode<ModifierManager>("ModifierManager");
		hitboxComponent.Hit += OnHit;
	}

	public override void _PhysicsProcess(double delta)
	{
		inputComponent.Update();
		Vector2 EndOfWandPos = wand.GetEndOfWandPos();
		Vector2 projDir = EndOfWandPos.DirectionTo(inputComponent.MousePos).Normalized();

		if (inputComponent.LeftClickJustPressed)
		{
			projectileSpawner.SpawnProjectile(EndOfWandPos, projDir);
		}

		wand.RotateWand(inputComponent);
		movementComponent.direction = inputComponent.MoveDir;
		movementComponent.mouseDir = GlobalPosition.DirectionTo(inputComponent.MousePos).Normalized();
		movementComponent.Update(delta);
		animationComponent.Update(movementComponent);
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		if (inputComponent.LeftClickPressed)
		{
		}
	}

	private void OnHit(Area2D hitBy)
	{
		if ((hitBy.CollisionLayer & CollisionLayers.EnemyProjectile) == 0)
		{
			return;
		}

		Node2D projectileRoot = hitBy.GetParent<Node2D>();
		DamageComponent damageComponent = projectileRoot.GetNodeOrNull<DamageComponent>("DamageComponent");
		if (damageComponent == null)
		{
			GD.PrintErr("Hit by enemy projectile without DamageComponent");
			return;
		}

		healthComponent.TakeDamage(damageComponent.CalculateDamage());
		projectileRoot.QueueFree();
	}
}
