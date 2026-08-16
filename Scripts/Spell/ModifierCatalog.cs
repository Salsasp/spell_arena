using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Static catalog of every modifier definition from JSON.
/// Safe to call from any class at any time; loads on first access.
/// </summary>
public static class ModifierCatalog
{
	public const string DefaultPath = "res://Assets/Data/spell_modifiers.json";

	private static readonly Dictionary<string, ModifierData> modifiers = [];
	private static bool loaded;

	public static IEnumerable<ModifierData> GetAll()
	{
		EnsureLoaded();
		return modifiers.Values;
	}

	public static ModifierData Get(string id)
	{
		EnsureLoaded();
		if (modifiers.TryGetValue(id, out ModifierData modifier))
		{
			return modifier;
		}

		GD.PrintErr($"Modifier '{id}' not found");
		return null;
	}

	public static bool TryGet(string id, out ModifierData modifier)
	{
		EnsureLoaded();
		return modifiers.TryGetValue(id, out modifier);
	}

	public static void EnsureLoaded(string path = DefaultPath)
	{
		if (loaded)
		{
			return;
		}

		Load(path);
	}

	private static void Load(string path)
	{
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			GD.PrintErr($"Failed to load modifiers from {path}");
			return;
		}

		var json = new Json();
		Error error = json.Parse(file.GetAsText());
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

		loaded = true;
		GD.Print($"Loaded {modifiers.Count} modifiers");
	}
}
