using Godot;

public class ModifierContext
{
	public ModifierManager ModifierManager { get; init; }
	public PlayerProjectile Projectile { get; init; }
	public DamageComponent Damage { get; init; }
	public SpellMovementComponent Movement { get; init; }
	public HitComponent Hit { get; init; }
	public DespawnComponent Despawn { get; init; }
	public SpellSpawnComponent Spawn { get; init; }
	public PackedScene OrbitScene { get; init; }
	public Node2D HitTarget { get; init; }
	public HitResolution HitResult { get; init; } = new();

	public static ModifierContext ForProjectile(PlayerProjectile projectile, Node2D hitTarget = null)
	{
		return new ModifierContext
		{
			ModifierManager = projectile.modifierManager,
			Projectile = projectile,
			Damage = projectile.damageComponent,
			Movement = projectile.spellMovementComponent,
			Hit = projectile.hitComponent,
			Despawn = projectile.despawnComponent,
			Spawn = projectile.spawnComponent,
			OrbitScene = projectile.OrbitScene,
			HitTarget = hitTarget,
		};
	}

	/// <summary>
	/// Context for run-scoped events (e.g. OnAcquire) that happen outside of any
	/// projectile's lifetime. Only ModifierManager is populated.
	/// </summary>
	public static ModifierContext ForRun(ModifierManager modifierManager)
	{
		return new ModifierContext
		{
			ModifierManager = modifierManager,
		};
	}
}
