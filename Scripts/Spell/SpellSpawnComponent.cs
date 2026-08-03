using Godot;

public partial class SpellSpawnComponent : Component
{
	[Export] public bool OrbitAllowed = true;
	private ModifierManager modifierManager;
	private DamageComponent damageComponent;

	public void Update()
	{
		AggregatedSpellStats stats = SpellStatBuilder.Build(
			modifierManager.ActiveModifiers.Values,
			modifierManager.SpellEnlargeAmount);
		damageComponent.ApplyFromStats(stats);

		Area2D area = GetParent<Node2D>().GetNode<Area2D>("ProjArea2D");
		area.Scale = Vector2.One * stats.ScaleMultiplier;

		ModifierEffectRegistry.Apply(ModifierPhase.OnSpawn, ModifierContext.ForProjectile(GetParent<Projectile>()));
	}

	public void Initialize(ModifierManager modMan, DamageComponent damCom)
	{
		modifierManager = modMan;
		damageComponent = damCom;
	}
}
