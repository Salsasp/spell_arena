using System;
using Godot;

public partial class SpellSpawnComponent : Component
{
	[Export] public bool OrbitAllowed = true;
	private ModifierManager modifierManager;
	private DamageComponent damageComponent;
	private SpellMovementComponent movementComponent;
	private PackedScene orbitScene;

	public void Update()
	{
		if (modifierManager.ActiveModifiers.ContainsKey("larger_spells"))
		{
			ModifierData enlargeModifier = modifierManager.ActiveModifiers["larger_spells"];
			Vector2 modifiedScale = new(1.0f + (0.1f * enlargeModifier.copies), 1.0f + (0.1f * enlargeModifier.copies));
			Area2D area = GetParent<Node2D>().GetNode<Area2D>("ProjArea2D");
			area.Scale = modifiedScale;
		}
		
		if (modifierManager.ActiveModifiers.ContainsKey("fire_spells"))
		{
			ModifierData fireModifier = modifierManager.ActiveModifiers["fire_spells"];
			damageComponent.DamageType = "fire";
			damageComponent.ElementalDamage = 10 * fireModifier.copies;
		}

		if (modifierManager.ActiveModifiers.ContainsKey("homing_spells"))
		{
			ModifierData homingModifier = modifierManager.ActiveModifiers["homing_spells"];
			movementComponent.SetHomingEnabled(true, movementComponent.HomingTurnSpeed);
		}

		if (modifierManager.ActiveModifiers.ContainsKey("crit_chance_up"))
		{
			ModifierData modifier = modifierManager.ActiveModifiers["crit_chance_up"];
			damageComponent.CritChance += modifier.critAdditive * modifier.copies;
		}

		if (HasActiveOrbitModifier() && OrbitAllowed)
		{
			try
			{
				OrbitManager orbitManager = orbitScene.Instantiate<OrbitManager>();
				orbitManager.ConfigureFromModifiers(modifierManager);
				GetParent<Projectile>().AddChild(orbitManager);
			}
			catch (Exception e)
			{
				GD.PrintErr(e);
			}
		}
	}

	private bool HasActiveOrbitModifier()
	{
		return modifierManager.ActiveModifiers.ContainsKey("fireball_orbit")
			|| modifierManager.ActiveModifiers.ContainsKey("wizard_orbit");
	}

	public void Initialize(ModifierManager modMan, DamageComponent damCom, SpellMovementComponent moveComp, PackedScene orbitScene = null)
	{
		this.modifierManager = modMan;
		this.damageComponent = damCom;
		this.movementComponent = moveComp;
		if (orbitScene != null)
		{
			this.orbitScene = orbitScene;
		}
	}
}
