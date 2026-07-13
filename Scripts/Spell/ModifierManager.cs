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

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
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
                onDespawn = modData["on_despawn"].ToString(),
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

    public void PrintModifiers()
    {
        foreach (var (key, value) in modifiers)
        {
            GD.Print(key + ": " + value);
        }
    }
}