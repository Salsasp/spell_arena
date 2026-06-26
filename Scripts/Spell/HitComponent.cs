using Godot;
using System;

public partial class HitComponent : Component
{
	private ModifierManager modifierManager;
	private DespawnComponent despawnComponent;
	private const int CHAIN_LIMIT = 3;

	public void Update()
	{
		if (modifierManager.ActiveModifiers.ContainsKey("chains_spells"))
		{
			ChainSpell();
		}
		else if (!modifierManager.ActiveModifiers.ContainsKey("pierce_spells"))
		{
			despawnComponent.Despawn();
		}
	}

	public override void _Ready()
	{
		modifierManager = GetNode<ModifierManager>("../../Player/ModifierManager");
		despawnComponent = GetNode<DespawnComponent>("../DespawnComponent");
	}

	public void ChainSpell()
	{
		// TODO: Implement
	}

	public static void ApplyDot(Projectile projectile, Enemy enemy)
	{
		// TODO: Implement
	}
}
