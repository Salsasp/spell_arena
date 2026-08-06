using Godot;

public partial class SpellSpawnComponent : Component
{
	[Export] public bool OrbitAllowed = true;
	private ModifierManager modifierManager;
	private DamageComponent damageComponent;

	public void Update()
	{
		ModifierContext ctx = ModifierContext.ForProjectile(GetParent<Projectile>());

		AggregatedSpellStats stats = SpellStatBuilder.Build(
			modifierManager.ActiveModifiers.Values,
			modifierManager.SpellEnlargeAmount,
			modifierManager.SpellHastenAmount
		);
		damageComponent.ApplyFromStats(stats);

		Area2D area = GetParent<Projectile>().projArea2D;
		area.Scale = Vector2.One * stats.ScaleMultiplier;

		ctx.Movement.Speed *= stats.SpeedMultiplier;

		ModifierEffectRegistry.Apply(ModifierPhase.OnSpawn, ctx);
	}

	public void Initialize(ModifierManager modMan, DamageComponent damCom)
	{
		modifierManager = modMan;
		damageComponent = damCom;
	}
}
