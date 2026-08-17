using Godot;

public partial class SpellSpawnComponent : Component
{
	[Export] public bool OrbitAllowed = true;
	private ModifierManager modifierManager;
	private DamageComponent damageComponent;

	public void Update()
	{
		PlayerProjectile projectile = GetParent<PlayerProjectile>();
		ModifierContext ctx = ModifierContext.ForProjectile(projectile);
		AggregatedSpellStats stats = SpellStatBuilder.Build(
			projectile.CastModifiers ?? modifierManager.GetCombatModifiers(),
			projectile.WandStats,
			modifierManager.SpellEnlargeAmount,
			modifierManager.SpellHastenAmount
		);
		damageComponent.ApplyFromStats(stats);

		projectile.projArea2D.Scale = Vector2.One * stats.ScaleMultiplier;

		ctx.Movement.Speed *= stats.SpeedMultiplier;

		ModifierEffectRegistry.Apply(ModifierPhase.OnSpawn, ctx);
	}

	public void Initialize(ModifierManager modMan, DamageComponent damCom)
	{
		modifierManager = modMan;
		damageComponent = damCom;
	}
}
