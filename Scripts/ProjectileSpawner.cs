using Godot;

public partial class ProjectileSpawner : Node2D
{
	[Export] public PackedScene ProjectilePrefab { get; set; }

	public override void _Ready()
	{
		
	}

	public void SpawnProjectile(Vector2 pos, Vector2 dir)
	{
		Node2D projectile = ProjectilePrefab.Instantiate<Node2D>();
		GetParent().AddChild(projectile);
		projectile.Position = pos;
		
		var spellMovementComponent = projectile.GetNode<SpellMovementComponent>("SpellMovementComponent");
    	spellMovementComponent.Direction = dir;
    	var projectilePivot = projectile.GetNode<Node2D>("ProjectilePivot");
		projectilePivot.Rotation = dir.Angle();
	}

}