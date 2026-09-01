using System.Collections.Generic;

/// <summary>
/// Collapses repeated modifier entries into one entry per id whose <see cref="ModifierData.copies"/>
/// reflects how many were equipped.
///
/// This has to happen before combat aggregation because the two consumers disagree about how
/// duplicates arrive: SpellStatBuilder sees one list entry per equipped copy, but
/// ModifierEffectRegistry fires at most one effect per tag per phase and expects the magnitude to
/// come from copies. Without collapsing, equipping three Fire Magic would triple the elemental
/// damage while still only applying a single-stack burn.
/// </summary>
public static class ModifierStacker
{
	public static List<ModifierData> Collapse(IEnumerable<ModifierData> modifiers)
	{
		List<ModifierData> stacked = [];
		Dictionary<string, ModifierData> byId = [];

		foreach (ModifierData modifier in modifiers ?? [])
		{
			if (modifier == null)
			{
				continue;
			}

			if (byId.TryGetValue(modifier.id, out ModifierData existing))
			{
				if (modifier.isStackable)
				{
					existing.copies += modifier.copies;
				}

				continue;
			}

			// Cloned so accumulated stack counts never leak back into the shared catalog instance
			ModifierData entry = modifier.Clone();
			byId[modifier.id] = entry;
			stacked.Add(entry);
		}

		return stacked;
	}
}
