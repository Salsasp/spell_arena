using System.Collections.Generic;

public class ModifierInventory
{
	public const int WandSlotCount = 12;
	public const int PlayerPoolSlotCount = 24;

	private readonly ModifierData[] slots;

	public ModifierInventory(int slotCount)
	{
		slots = new ModifierData[slotCount];
	}

	public int SlotCount => slots.Length;

	public ModifierData GetSlot(int index)
	{
		if (index < 0 || index >= slots.Length)
		{
			return null;
		}

		return slots[index];
	}

	public void SetSlot(int index, ModifierData modifier)
	{
		if (index < 0 || index >= slots.Length)
		{
			return;
		}

		slots[index] = modifier;
	}

	public bool TryAdd(ModifierData modifier)
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i] == null)
			{
				slots[i] = modifier;
				return true;
			}
		}

		return false;
	}

	public bool ContainsId(string id)
	{
		foreach (ModifierData modifier in slots)
		{
			if (modifier != null && modifier.id == id)
			{
				return true;
			}
		}

		return false;
	}

	public List<ModifierData> GetOccupied()
	{
		List<ModifierData> occupied = [];
		foreach (ModifierData modifier in slots)
		{
			if (modifier != null)
			{
				occupied.Add(modifier);
			}
		}

		return occupied;
	}
}
