using System.Collections.Generic;

public static class SpellStatBuilder
{
	public static AggregatedSpellStats Build(
		IEnumerable<ModifierData> activeModifiers,
		AggregatedSpellStats baseStats,
		float enlargePerStack = 0.1f,
		float hastenPerStack = 0.1f)
	{
		float scaleMultiplier = baseStats.ScaleMultiplier;
		float speedMultiplier = baseStats.SpeedMultiplier;
		float critChanceBonus = baseStats.CritChanceBonus;
		int elementalDamage = baseStats.ElementalDamage;

		string damageType = baseStats.DamageType ?? "";
		string bestDamageTypeModifierId = null;
		float bestElementalContribution = elementalDamage;

		foreach (ModifierData modifier in activeModifiers ?? [])
		{
			critChanceBonus += modifier.critAdditive * modifier.copies;

			if (modifier.onSpawn == "enlarge")
			{
				scaleMultiplier += enlargePerStack * modifier.copies;
			}

			if (modifier.onSpawn == "hasten")
			{
				speedMultiplier += hastenPerStack * modifier.copies;
			}

			// Only modifiers that actually deal elemental damage get a say in the
			// projectile's damage type; a modifier with dotDamage == 0 (e.g.
			// fireball_orbit's cosmetic "fire" tag) must never override it.
			float elementalContribution = modifier.dotDamage * modifier.copies;
			if (elementalContribution <= 0f || string.IsNullOrEmpty(modifier.damageType))
			{
				continue;
			}

			elementalDamage += (int)elementalContribution;

			// Highest contribution wins; ties broken by modifier id so the result
			// never depends on ActiveModifiers' dictionary enumeration order.
			bool isBetterContribution = elementalContribution > bestElementalContribution;
			bool isTieBreakWinner = elementalContribution == bestElementalContribution
				&& (bestDamageTypeModifierId == null || string.CompareOrdinal(modifier.id, bestDamageTypeModifierId) < 0);

			if (isBetterContribution || isTieBreakWinner)
			{
				bestElementalContribution = elementalContribution;
				bestDamageTypeModifierId = modifier.id;
				damageType = modifier.damageType;
			}
		}

		return new AggregatedSpellStats
		{
			ScaleMultiplier = scaleMultiplier,
			SpeedMultiplier = speedMultiplier,
			CritChanceBonus = critChanceBonus,
			DamageType = damageType,
			ElementalDamage = elementalDamage,
			BaseDamage = baseStats.BaseDamage,
			FireRate = baseStats.FireRate,
		};
	}
}
