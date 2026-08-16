using Godot;
using System.Collections.Generic;

public class WandInventory
{
    private List<ModifierData> Mods = [];
    private ModifierData InnateModifier { get; set; }


    public List<ModifierData> GetMods()
    {
        return Mods;
    }

    public void AddMod(ModifierData mod)
    {
        Mods.Add(mod);
    }

    public void RemoveMod(string modId)
    {
        Mods.Remove(Mods.Find(x => x.id == modId));
    }

    public void SetInnateMod(ModifierData innateMod)
    {
        InnateModifier = innateMod;
    }
}