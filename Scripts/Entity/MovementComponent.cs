using Godot;

public partial class MovementComponent : Component
{
	[Export] public CharacterBody2D body;
	[Export] public float Speed { get; set; } = 100f;
	public float SpeedMultiplier = 1.0f;

	public Vector2 direction = Vector2.Zero;
	public Vector2 mouseDir = Vector2.Zero;

	public void Update(double delta)
	{
		if (body == null)
		{
			return;
		}
		body.Velocity = direction * (Speed*SpeedMultiplier);

		body.MoveAndSlide();
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
}
