using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	[Export] public InputComponent inputComponent;
	[Export] public MovementComponent movementComponent;
	[Export] public HealthComponent healthComponent;
	[Export] public HitboxComponent hitboxComponent;
	private ProjectileSpawner projectileSpawner;
	[Export] public AnimationComponent animationComponent;
	[Export] public Wand wand;
	[Export] public Node2D wandSnapPoints;
	[Export] public ModifierManager modifierManager;

	public override void _Ready()
	{
		projectileSpawner = GetNode<ProjectileSpawner>("../ProjectileSpawner");

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
		wand.SnapPivotToDirection(movementComponent, wandSnapPoints);
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

		bool isCrit = damageComponent.IsCriticalHit();
		healthComponent.TakeDamage(damageComponent.CalculateDamage(isCrit));
		projectileRoot.QueueFree();
	}
}
