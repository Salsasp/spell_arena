using Godot;
using System.Collections.Generic;

public partial class IceWand : Wand
{
	public override ModifierAffinity Affinity => ModifierAffinity.Ice;
    public List<ModifierData> modifiers = [];

	public override void _Ready()
	{
		InnateMod = ModifierCatalog.Get("ice_spells");
		wandInventory.SetInnateMod(InnateMod);
	}
}
