using System;
using Godot;

public partial class SpellSpawnComponent : Component
{
	private ModifierManager modifierManager;
	private DamageComponent damageComponent;
	private SpellMovementComponent movementComponent;
	private PackedScene orbitScene;

	public void Update()
	{
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

		if (modifierManager.ActiveModifiers.ContainsKey("fireball_orbit") || modifierManager.ActiveModifiers.ContainsKey("wizard_orbit"))
		{
			try
			{
				OrbitManager orbitManager = orbitScene.Instantiate<OrbitManager>();
				if (modifierManager.ActiveModifiers.ContainsKey("fireball_orbit"))
				{
					orbitManager.hasFireballs = true;
				}
				if (modifierManager.ActiveModifiers.ContainsKey("wizard_orbit"))
				{
					orbitManager.hasWizards = true;
				}

				GetParent<Projectile>().AddChild(orbitManager);
			}
			catch(Exception e)
			{
				GD.PrintErr(e);
			}
		}
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
