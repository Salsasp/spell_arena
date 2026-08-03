using Godot;
using System;
using System.Collections.Generic;

public partial class ModifierManager : Node
{
    public static ModifierManager Instance { get; private set; }
    private Dictionary<string, ModifierData> modifiers = [];
    public Dictionary<string, ModifierData> ActiveModifiers = [];

    public IEnumerable<ModifierData> GetAllModifiers() => modifiers.Values;

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
        LoadModifiers("res://Assets/Data/spell_modifiers.json");
    }

    private void LoadModifiers(string path)
    {
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr($"Failed to load modifiers from {path}");
            return;
        }

        var json = new Json();
        var error = json.Parse(file.GetAsText());
        
        if (error != Error.Ok)
        {
            GD.PrintErr("Failed to parse modifiers JSON");
            return;
        }

        var root = json.GetData().AsGodotDictionary();
        var modifierArray = root["modifiers"].AsGodotArray();

        foreach (var modDict in modifierArray)
        {
            var modData = modDict.AsGodotDictionary();
            var modifier = new ModifierData
            {
                id = modData["id"].ToString(),
                name = modData["name"].ToString(),
                description = modData["description"].ToString(),
                icon = modData["icon"].ToString(),
                rarity = modData["rarity"].ToString(),
                damageType = modData["damage_type"].ToString(),
                dotDamage = modData["dot_damage"].ToString().ToFloat(),
                critAdditive = modData["crit_additive"].ToString().ToFloat(),
                critMultiplicative = modData["crit_multiplicative"].ToString().ToFloat(),
                onHit = modData["on_hit"].ToString(),
                orbital = modData["orbital"].ToString(),
                onSpawn = modData["on_spawn"].ToString(),
                onAcquire = modData.ContainsKey("on_acquire") ? modData["on_acquire"].ToString() : "",
                uiOnHit = modData.ContainsKey("ui_on_hit") ? modData["ui_on_hit"].ToString() : "",
                isStackable = Convert.ToBoolean(modData["stackable"].ToString())
            };

            modifiers[modifier.id] = modifier;
        }

        GD.Print($"Loaded {modifiers.Count} modifiers");
    }

    public ModifierData GetModifier(string id)
    {
        if (modifiers.TryGetValue(id, out var modifier))
            return modifier;
        
        GD.PrintErr($"Modifier '{id}' not found");
        return null;
    }

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
        foreach (var (key, value) in modifiers)
        {
            GD.Print(key + ": " + value);
        }
    }
}