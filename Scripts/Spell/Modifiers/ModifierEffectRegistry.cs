using System.Collections.Generic;

public static class ModifierEffectRegistry
{
	private static readonly Dictionary<(ModifierPhase Phase, string EffectId), IModifierEffect> Effects = new();
	private static bool defaultsRegistered;

	public static void Register(IModifierEffect effect)
	{
		Effects[(effect.Phase, effect.EffectId)] = effect;
	}

	public static void RegisterDefaults()
	{
		if (defaultsRegistered)
		{
			return;
		}

		Register(new HomingSpawnEffect());
		Register(new OrbitSpawnEffect());
		Register(new ChainHitEffect());
		Register(new LightningWandInnateEffect());
		Register(new PierceHitEffect());
		Register(new BurnHitEffect());
		Register(new FireWandInnateEffect());
		Register(new ExtraDraftOptionAcquireEffect());
		Register(new SwiftStepAcquireEffect());

		defaultsRegistered = true;
	}

	/// <summary>
	/// Applies effects for every active modifier matching the given phase. Used for
	/// per-projectile phases (OnSpawn, OnHit) where the full active set is re-evaluated
	/// every time. Effects are deduped by tag so multiple modifiers sharing a tag only
	/// trigger the effect once per call.
	/// </summary>
	public static void Apply(ModifierPhase phase, ModifierContext context)
	{
		if (context == null)
		{
			return;
		}

		HashSet<string> appliedEffectIds = [];
		IEnumerable<ModifierData> modifiers = context.CombatModifiers
			?? context.ModifierManager?.GetCombatModifiers();
		if (modifiers == null)
		{
			return;
		}

		foreach (ModifierData modifier in modifiers)
		{
			string effectId = GetEffectId(modifier, phase);
			if (string.IsNullOrEmpty(effectId) || appliedEffectIds.Contains(effectId))
			{
				continue;
			}

			if (!Effects.TryGetValue((phase, effectId), out IModifierEffect effect))
			{
				continue;
			}

			effect.Apply(context, modifier);
			appliedEffectIds.Add(effectId);
		}
	}

	/// <summary>
	/// Applies the effect for a single modifier instance, if one is registered for the
	/// given phase. Used for one-time events (e.g. OnAcquire) where only the modifier
	/// that triggered the event should fire, not the entire active set.
	/// </summary>
	public static void ApplyToModifier(ModifierPhase phase, ModifierContext context, ModifierData modifier)
	{
		if (context == null || modifier == null)
		{
			return;
		}

		string effectId = GetEffectId(modifier, phase);
		if (string.IsNullOrEmpty(effectId))
		{
			return;
		}

		if (!Effects.TryGetValue((phase, effectId), out IModifierEffect effect))
		{
			return;
		}

		effect.Apply(context, modifier);
	}

	private static string GetEffectId(ModifierData modifier, ModifierPhase phase)
	{
		return phase switch
		{
			ModifierPhase.OnAcquire => modifier.onAcquire,
			ModifierPhase.OnSpawn => modifier.onSpawn,
			ModifierPhase.OnHit => modifier.onHit,
			_ => "",
		};
	}
}
