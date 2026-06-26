using Godot;
using System;

public partial class SpellSpawnComponent : Component
{
	private ModifierManager modifierManager;
	private DamageComponent damageComponent;
	public void Update()
	{
		// Add orbitals, damage types, etc
		if (modifierManager.ActiveModifiers.ContainsKey("fire_spells"))
		{
			damageComponent.DamageType = "fire";
			damageComponent.ElementalDamage = 10;
		}
		// TODO: fill this in for more damage types
	}

	public override void _Ready()
	{
	}

	public void Initialize(ModifierManager modMan, DamageComponent damCom)
	{
		modifierManager = modMan;
		damageComponent = damCom;
	}
}
