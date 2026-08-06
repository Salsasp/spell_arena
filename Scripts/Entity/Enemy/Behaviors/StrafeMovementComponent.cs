using Godot;

public partial class StrafeMovementComponent : AIMovementBehaviorComponent
{
	[Export] private float minDistance = 300f;
	[Export] private float falloffRange = 50f;
	[Export] private float oscSpeed = 2f;
	[Export] private float oscSpeedVariance = 0.5f;
	[Export] private float strafeSpeed = 100f;

	private float timeAccum;
	private float phaseOffset;
	private float personalOscSpeed;

	public override void _Ready()
	{
		base._Ready();
		phaseOffset = (float)GD.RandRange(0.0, Mathf.Tau);
		personalOscSpeed = oscSpeed + (float)GD.RandRange(-oscSpeedVariance, oscSpeedVariance);
		timeAccum = (float)GD.RandRange(0.0, 10.0);
	}

	public override void Update(double delta)
	{
		if (context == null || !context.IsPlayerValid)
		{
			movement.direction = Vector2.Zero;
			return;
		}

		Pathfind(context.Player, (float)delta);
	}

	private void Pathfind(Node2D target, float delta)
	{
		if (target == null || owner == null || !GodotObject.IsInstanceValid(target))
		{
			return;
		}

		timeAccum += delta;

		Vector2 toTarget = target.GlobalPosition - owner.GlobalPosition;
		float distance = toTarget.Length();
		Vector2 radialDir = toTarget.Normalized();
		Vector2 tangentialDir = new Vector2(-radialDir.Y, radialDir.X);

		float osc = Mathf.Sin((timeAccum + phaseOffset) * personalOscSpeed);
		float radialFactor = Mathf.Clamp((distance - minDistance) / falloffRange, -1f, 1f);

		Vector2 desiredVelocity =
			radialDir * radialFactor +
			tangentialDir * osc * (strafeSpeed / Mathf.Max(movement.Speed, 0.01f));

		movement.direction = desiredVelocity.Normalized();
	}
}
