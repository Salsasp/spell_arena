using Godot;
using System.Collections.Generic;
using System.Linq;

public static class ModifierDraftService
{
	private static readonly Dictionary<string, float> RarityWeights = new()
	{
		{ "common", 70f },
		{ "uncommon", 25f },
		{ "rare", 5f },
	};

	public static List<ModifierData> RollDraft(int count = 3)
	{
		ModifierManager manager = ModifierManager.Instance;
		if (manager == null)
		{
			return new List<ModifierData>();
		}

		List<ModifierData> pool = manager.GetAllModifiers()
			.Where(modifier => modifier.id != "debug_modifier")
			.Where(modifier => !manager.HasActiveModifier(modifier.id) ||
			 (manager.HasActiveModifier(modifier.id) && modifier.isStackable))
			.ToList();

		if (pool.Count == 0)
		{
			GD.PrintErr("No viable modifiers found for modifier draft");
			pool = manager.GetAllModifiers()
				.Where(modifier => modifier.id != "debug_modifier")
				.ToList();
		}

		List<ModifierData> draft = new();
		List<ModifierData> remaining = new(pool);

		while (draft.Count < count && remaining.Count > 0)
		{
			ModifierData picked = PickWeighted(remaining);
			draft.Add(picked);
			remaining.Remove(picked);
		}

		return draft;
	}

	private static ModifierData PickWeighted(List<ModifierData> pool)
	{
		float totalWeight = 0f;
		foreach (ModifierData modifier in pool)
		{
			totalWeight += GetWeight(modifier.rarity);
		}

		float roll = GD.Randf() * totalWeight;
		float cumulative = 0f;

		foreach (ModifierData modifier in pool)
		{
			cumulative += GetWeight(modifier.rarity);
			if (roll <= cumulative)
			{
				return modifier;
			}
		}

		return pool[^1];
	}

	private static float GetWeight(string rarity)
	{
		if (rarity != null && RarityWeights.TryGetValue(rarity, out float weight))
		{
			return weight;
		}

		return RarityWeights["common"];
	}
}
