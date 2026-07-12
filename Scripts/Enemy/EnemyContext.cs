using Godot;

public class EnemyContext
{
	public Enemy Owner { get; }
	public Player Player { get; }
	public MovementComponent Movement { get; }
	public ProjectileSpawner ProjectileSpawner { get; }

	public Vector2 GlobalPosition => Owner.GlobalPosition;

	public bool IsPlayerValid => Player != null && GodotObject.IsInstanceValid(Player);

	public EnemyContext(Enemy owner, Player player, MovementComponent movement, ProjectileSpawner projectileSpawner)
	{
		Owner = owner;
		Player = player;
		Movement = movement;
		ProjectileSpawner = projectileSpawner;
	}
}
