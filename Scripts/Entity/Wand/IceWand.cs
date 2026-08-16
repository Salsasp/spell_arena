using Godot;
using System.Collections.Generic;

public partial class IceWand : Wand
{
    public List<ModifierData> modifiers = [];

	public override void _Ready()
	{
		InnateMod = ModifierCatalog.Get("freeze");
		wandInventory.SetInnateMod(InnateMod);
	}
}
