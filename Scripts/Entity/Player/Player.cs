using Godot;
using System;

public partial class Player : CharacterBody2D, IEntity
{
	[Export] public InputComponent inputComponent { get; set; }
	[Export] public MovementComponent movementComponent { get; set; }
	[Export] public HealthComponent healthComponent { get; set; }
	[Export] public HitboxComponent hitboxComponent { get; set; }
	[Export] public AnimationComponent animationComponent { get; set; }
	[Export] public Wand wand { get; set; }
	[Export] public Node2D wandSnapPoints { get; set; }
	[Export] public ModifierManager modifierManager { get; set; }
    public bool IsFalling { get; set; }
	private ProjectileSpawner projectileSpawner { get; set; }
	private Vector2 respawnPos;
	private int pitfallDamage;

    public override void _Ready()
	{
		IsFalling = false;
		projectileSpawner = GetNode<ProjectileSpawner>("../ProjectileSpawner");

		animationComponent.FallAnimationFinished += OnFallFinished;
		hitboxComponent.Hit += OnHit;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsFalling) return;

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

		if (inputComponent.DodgePressed)
		{
			movementComponent.Dodge(inputComponent.MoveDir);
		}

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

	public void OnHit(Area2D hitBy)
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

    public void Fall(Pitfall pitfall)
    {
		this.pitfallDamage = pitfall.PitfallDamage; // ik this is sloppy but prevents having to pass it through signals
        if (IsFalling)
			return;
		IsFalling = true;
		respawnPos = pitfall.GetFallRespawnPosition(this);
		movementComponent.direction = Vector2.Zero;

		GlobalPosition = pitfall.FallStartPoint.GlobalPosition;

		if (!animationComponent.TryPlayFallAnimation(pitfall))
		{
			IsFalling = false; // no anim configured — fail safe
		}
    }

	private void OnFallFinished()
	{
		healthComponent.TakeDamage(pitfallDamage);
		GlobalPosition = respawnPos;
		IsFalling = false;
	}

}
