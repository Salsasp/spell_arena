using Godot;
using System.Collections.Generic;

public partial class ModifierManager : Node
{
    public static ModifierManager Instance { get; private set; }
    public Dictionary<string, ModifierData> ActiveModifiers = [];

    public IEnumerable<ModifierData> GetAllModifiers() => ModifierCatalog.GetAll();

    public bool HasActiveModifier(string id) => ActiveModifiers.ContainsKey(id);
    [Export] public float SpellEnlargeAmount = 0.1f; // additive transform scale modifier
    [Export] public float SpellHastenAmount = 0.1f; // additive speed modifier

    /// <summary>Run-scoped counter mutated by OnAcquire effects (e.g. ExtraDraftOptionAcquireEffect).</summary>
    public int ExtraDraftOptions { get; set; }

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        ModifierEffectRegistry.RegisterDefaults();
        ModifierCatalog.EnsureLoaded();
    }

    public ModifierData GetModifier(string id) => ModifierCatalog.Get(id);

    public bool TryAddActiveModifier(string id)
    {
        ModifierData modifier = GetModifier(id);
        if (modifier == null)
        {
            return false;
        }

        bool added;
        if (!HasActiveModifier(id))
        {
            ActiveModifiers.Add(id, modifier);
            added = true;
        }
        else if (modifier.isStackable)
        {
            ActiveModifiers[id].copies += 1;
            added = true;
        }
        else
        {
            added = false;
        }

        if (added)
        {
            // Only the modifier that was just picked fires its OnAcquire effect,
            // unlike OnSpawn/OnHit which re-evaluate the whole active set.
            ModifierEffectRegistry.ApplyToModifier(ModifierPhase.OnAcquire, ModifierContext.ForRun(this), modifier);
        }

        return added;
    }

    public void PrintModifiers()
    {
        foreach (ModifierData modifier in ModifierCatalog.GetAll())
        {
            GD.Print(modifier.id + ": " + modifier);
        }
    }
}