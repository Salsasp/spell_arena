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
	[Export] public Node2D ElementalWands { get; set; }
	public Wand ActiveWand
	{
		get => activeWand;
		set
		{
			activeWand = value;
			if (modifierManager != null)
			{
				modifierManager.ActiveWand = value;
			}
		}
	}
	public ModifierInventory ModifierPool { get; } = new(ModifierInventory.PlayerPoolSlotCount);
	public Wand ArcaneWand { get; private set; }
	public Wand LightningWand { get; private set; }
	public Wand FireWand { get; private set; }
	public Wand IceWand { get; private set; }
	private Wand activeWand;
    public bool IsFalling { get; set; }
	private ProjectileSpawner projectileSpawner { get; set; }
	private Vector2 respawnPos;
	private int pitfallDamage;

	public override void _Ready()
	{
		IsFalling = false;
		projectileSpawner = GetNode<ProjectileSpawner>("../ProjectileSpawner");
		InitElementalWands();
		ActiveWand = ArcaneWand;
		SortLayers.Apply(this, SortLayers.Entities);

		animationComponent.FallAnimationFinished += OnFallFinished;
		hitboxComponent.Hit += OnHit;
	}

	public bool HasModifier(string id)
	{
		if (ModifierPool.ContainsId(id) || modifierManager.HasActiveModifier(id))
		{
			return true;
		}

		foreach (Wand loadoutWand in GetLoadoutWands())
		{
			if (loadoutWand.wandInventory.Slots.ContainsId(id))
			{
				return true;
			}
		}

		return false;
	}

	public Wand[] GetLoadoutWands()
	{
		return [ArcaneWand, LightningWand, FireWand, IceWand];
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsFalling) return;

		inputComponent.Update();
		Vector2 EndOfWandPos = ActiveWand.GetEndOfWandPos();
		Vector2 projDir = EndOfWandPos.DirectionTo(inputComponent.MousePos).Normalized();

		if (inputComponent.LeftClickJustPressed)
		{
			projectileSpawner.SpawnProjectile(EndOfWandPos, projDir, castModifiers: modifierManager.CreateCastSnapshot());
		}

		HandleWandSwitchInput();

		ActiveWand.RotateWand(inputComponent);
		ActiveWand.SnapPivotToDirection(movementComponent, wandSnapPoints);
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

	public void HandleWandSwitchInput()
	{
		ActiveWand.Visible = false;
		if (inputComponent.Wand1Pressed) {
			ActiveWand = ArcaneWand;
		}
		if (inputComponent.Wand2Pressed) {
			ActiveWand = LightningWand;
		}
		if (inputComponent.Wand3Pressed) {
			ActiveWand = FireWand;
		}
		if (inputComponent.Wand4Pressed) {
			ActiveWand = IceWand;
		}
		if (inputComponent.WandCycleUpPressed)
		{
			int idx = GetActiveWandIndex();
			ActiveWand = GetLoadoutWands()[(idx + 1) % 3];
		}
		if (inputComponent.WandCycleDownPressed)
		{
			int idx = GetActiveWandIndex();
			ActiveWand = GetLoadoutWands()[Math.Abs((idx - 1) % 3)];
		}
		ActiveWand.Visible =  true;
	}

	private void OnFallFinished()
	{
		healthComponent.TakeDamage(pitfallDamage);
		GlobalPosition = respawnPos;
		IsFalling = false;
	}

	private void InitElementalWands()
	{
		ArcaneWand = ElementalWands.GetNode<Wand>("ArcaneWand");
		IceWand = ElementalWands.GetNode<Wand>("IceWand");
		FireWand = ElementalWands.GetNode<Wand>("FireWand");
		LightningWand = ElementalWands.GetNode<Wand>("LightningWand");
	}

	private int GetActiveWandIndex()
	{
		Wand[] wandArr = GetLoadoutWands();
		for (int i = 0; i < wandArr.Length; i++)
		{
			if (wandArr[i] == ActiveWand)
			{
				return i;
			}
		}
		
		return 0;
	}
}
