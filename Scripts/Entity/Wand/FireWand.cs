using Godot;
using System.Collections.Generic;

public partial class FireWand : Wand
{
    public List<ModifierData> modifiers = [];

	public override void _Ready()
	{
		InnateMod = ModifierCatalog.Get("fire_spells");
		wandInventory.SetInnateMod(InnateMod);
	}
}
