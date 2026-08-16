using Godot;
using System.Collections.Generic;

public partial class LightningWand : Wand
{
	public override void _Ready()
	{
		InnateMod = ModifierCatalog.Get("chain_lightning");
		wandInventory.SetInnateMod(InnateMod);
	}
}
