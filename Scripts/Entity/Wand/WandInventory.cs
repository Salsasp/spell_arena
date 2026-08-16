using System.Collections.Generic;

public class WandInventory
{
	public ModifierInventory Slots { get; } = new(ModifierInventory.WandSlotCount);
	private ModifierData InnateModifier { get; set; }

	public List<ModifierData> GetMods()
	{
		return Slots.GetOccupied();
	}

	public void AddMod(ModifierData mod)
	{
		Slots.TryAdd(mod);
	}

	public void RemoveMod(string modId)
	{
		for (int i = 0; i < Slots.SlotCount; i++)
		{
			ModifierData modifier = Slots.GetSlot(i);
			if (modifier != null && modifier.id == modId)
			{
				Slots.SetSlot(i, null);
				return;
			}
		}
	}

	public void SetInnateMod(ModifierData innateMod)
	{
		InnateModifier = innateMod;
	}

	public ModifierData GetInnateMod()
	{
		return InnateModifier;
	}
}
