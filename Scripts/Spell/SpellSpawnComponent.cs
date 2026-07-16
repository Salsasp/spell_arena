using Godot;

public partial class SpellSpawnComponent : Component
{
	private ModifierManager modifierManager;
	private DamageComponent damageComponent;
	private SpellMovementComponent movementComponent;

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
	}

	public void Initialize(ModifierManager modMan, DamageComponent damCom, SpellMovementComponent moveComp)
	{
		modifierManager = modMan;
		damageComponent = damCom;
		movementComponent = moveComp;
	}
}
