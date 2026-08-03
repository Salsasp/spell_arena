using System.Collections.Generic;

/// <summary>
/// Pull-based queries for modifiers that affect UI/run systems outside the
/// per-projectile OnSpawn/OnHit pipeline. These use the dedicated `uiOnHit`
/// tag namespace so they never collide with registry-dispatched `onHit` tags.
/// </summary>
public static class RunStatBuilder
{
	public static bool HasDamagingText(IEnumerable<ModifierData> activeModifiers)
	{
		foreach (ModifierData modifier in activeModifiers)
		{
			if (modifier.uiOnHit == "damaging_text")
			{
				return true;
			}
		}

		return false;
	}
}
