using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class Wand : Node2D
{
	[Export] public Marker2D EndOfWand;
	[Export] public Sprite2D WandSprite;
	[Export] public Node2D WandPivot;
	[Export] public Timer FireCooldownTimer;

	[ExportGroup("Base Stats")]
	[Export] public float BaseScaleMultiplier { get; set; } = 1f;
	[Export] public float BaseSpeedMultiplier { get; set; } = 1f;
	[Export] public float BaseCritChanceBonus { get; set; }
	[Export] public string BaseDamageType { get; set; } = "";
	[Export] public int BaseElementalDamage { get; set; }
	[Export] public int BaseDamage { get; set; } = 20;
	[Export] public float BaseFireRate { get; set; } = 3f;

	public WandInventory wandInventory = new();
	public ModifierData InnateMod;
	public virtual ModifierAffinity Affinity => ModifierAffinity.Neutral;

	public AggregatedSpellStats GetBaseStats()
	{
		return new AggregatedSpellStats
		{
			ScaleMultiplier = BaseScaleMultiplier,
			SpeedMultiplier = BaseSpeedMultiplier,
			CritChanceBonus = BaseCritChanceBonus,
			DamageType = BaseDamageType ?? "",
			ElementalDamage = BaseElementalDamage,
			BaseDamage = BaseDamage,
			FireRate = BaseFireRate,
		};
	}

	public bool CanFire()
	{
		EnsureFireCooldownTimer();
		return FireCooldownTimer.IsStopped();
	}

	public bool TryBeginShot()
	{
		if (!CanFire())
		{
			return false;
		}

		float fireRate = GetCombatFireRate();
		if (fireRate <= 0f)
		{
			return false;
		}

		FireCooldownTimer.Start(1f / fireRate);
		return true;
	}

	public float GetCombatFireRate()
	{
		return SpellStatBuilder.Build(GetCombatModifiers(), GetBaseStats()).FireRate;
	}

	public bool CanAccept(ModifierData modifier)
	{
		if (modifier == null)
		{
			return true;
		}

		if (modifier.IsRunScoped)
		{
			return false;
		}

		if (modifier.affinity == ModifierAffinity.Neutral)
		{
			return true;
		}

		return modifier.affinity == Affinity;
	}

	public override void _EnterTree()
	{
		SortLayers.Apply(this, SortLayers.Wands);
		EnsureFireCooldownTimer();
	}

	private void EnsureFireCooldownTimer()
	{
		if (FireCooldownTimer != null && GodotObject.IsInstanceValid(FireCooldownTimer))
		{
			ConfigureFireCooldownTimer();
			return;
		}

		FireCooldownTimer = GetNodeOrNull<Timer>("FireCooldownTimer");
		if (FireCooldownTimer == null)
		{
			FireCooldownTimer = new Timer
			{
				Name = "FireCooldownTimer",
			};
			AddChild(FireCooldownTimer);
		}

		ConfigureFireCooldownTimer();
	}

	private void ConfigureFireCooldownTimer()
	{
		FireCooldownTimer.OneShot = true;
		FireCooldownTimer.Autostart = false;
		FireCooldownTimer.ProcessCallback = Timer.TimerProcessCallback.Physics;
	}

	public override void _Ready()
	{
	}

	public void SnapPivotToDirection(MovementComponent movementComponent, Node2D wandSnapPoints)
	{
		int directionIndex = Util.GetDirectionIndex8D(movementComponent.mouseDir.Round());
		switch (directionIndex)
		{
			case 0: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("Right").GlobalPosition; break;
			case 1: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("DownRight").GlobalPosition; break;
			case 2: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("Down").GlobalPosition; break;
			case 3: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("DownLeft").GlobalPosition; break;
			case 4: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("Left").GlobalPosition; break;
			case 5: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("UpLeft").GlobalPosition; break;
			case 6: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("Up").GlobalPosition; break;
			case 7: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("UpRight").GlobalPosition; break;
		}

		bool behindEntities = directionIndex is 6 or 7;
		SortLayers.Apply(this, behindEntities ? SortLayers.Projectiles : SortLayers.Wands);
	}

	public void RotateWand(InputComponent inputComponent)
	{
		Vector2 dir = EndOfWand.GlobalPosition.DirectionTo(inputComponent.MousePos).Normalized();
		WandPivot.Rotation = dir.Angle();
	}

	public Vector2 GetEndOfWandPos()
	{
		return EndOfWand.GlobalPosition;
	}

	/// <summary>
	/// Retrieve all modifiers within a wand's inventory, with duplicates collapsed into a single
	/// stacked entry each. The innate is included first, so equipping more copies of a modifier the
	/// wand already has innately stacks on top of it.
	/// </summary>
	public List<ModifierData> GetCombatModifiers()
	{
		List<ModifierData> modifiers = [];
		ModifierData innate = InnateMod ?? wandInventory.GetInnateMod();
		if (innate != null)
		{
			modifiers.Add(innate);
		}

		modifiers.AddRange(wandInventory.GetMods());
		return ModifierStacker.Collapse(modifiers);
	}
}
