using Godot;
using System.Collections.Generic;

public partial class LightningWand : Wand
{
	public override ModifierAffinity Affinity => ModifierAffinity.Lightning;
	public override void _Ready()
	{
		InnateMod = ModifierCatalog.Get("lightning_wand_innate");
		wandInventory.SetInnateMod(InnateMod);
	}
}
