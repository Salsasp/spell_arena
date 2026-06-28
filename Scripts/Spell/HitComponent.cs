using Godot;
using System;

public partial class HitComponent : Component
{
	private ModifierManager modifierManager;
	private DespawnComponent despawnComponent;
	private const int CHAIN_LIMIT = 3;
	private const int DOT_TICK_INTERVAL = 2;
	private const int DOT_DURATION = 10;

	public void Update(Enemy enemy)
	{
		if (modifierManager.ActiveModifiers.ContainsKey("chains_spells"))
		{
			ChainSpell();
		}
		else if (!modifierManager.ActiveModifiers.ContainsKey("pierce_spells"))
		{
			despawnComponent.Despawn();
		}

		if (modifierManager.ActiveModifiers.ContainsKey("fire_spells"))
		{
			ApplyDot(enemy, modifierManager.ActiveModifiers["fire_spells"]);
		}
	}

	public override void _Ready()
	{
		modifierManager = GetNode<ModifierManager>("../../Player/ModifierManager");
		despawnComponent = GetNode<DespawnComponent>("../DespawnComponent");
	}

	public void Initialize(ModifierManager modMan, DespawnComponent despawnComp)
	{
		modifierManager = modMan;
		despawnComponent = despawnComp;
	}

	public void ChainSpell()
	{
		// TODO: Implement
	}

	public void ApplyDot(Enemy enemy, ModifierData modifier)
	{
		float dotDamage = modifier.dotDamage;
		DotComponent dot = new()
        {
			DamagePerTick = dotDamage,
			TickInterval = DOT_TICK_INTERVAL,
			Duration = DOT_DURATION
		};

		if (!enemy.DotApplied())
		{
			enemy.AddChild(dot);
		}
	}
}
