using Godot;

public partial class MovementComponent : Node
{
	[Export] public CharacterBody2D body;
	[Export] public Sprite2D sprite; // replace this with animated sprite later
	[Export] public float speed { get; set; } = 100f;

	public Vector2 direction = Vector2.Zero;

	public void Update(double delta)
	{
		if (body == null)
		{
			return;
		}
		body.Velocity = direction * speed;

		body.MoveAndSlide();
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
}
