using Godot;
using System;

public partial class AIMovementComponent : Node2D
{
	[Export] private float minDistance = 300f;
	[Export] private float falloffRange = 50f;
	[Export] private float maxSwingDegrees = 90f;
	[Export] private float oscSpeed = 2f;
	[Export] private float oscSpeedVariance = 0.5f;
	[Export] private float strafeSpeed = 100f;

	private float timeAccum = 0f;
	private float phaseOffset = 0f;
	private float personalOscSpeed = 0f;
	private CharacterBody2D player;
	private MovementComponent movementComponent;

	public override void _Ready()
	{
		// Randomize both the starting phase and the individual speed
		phaseOffset = (float)GD.RandRange(0.0, Mathf.Tau); // 0 to 2π
		personalOscSpeed = oscSpeed + (float)GD.RandRange(-oscSpeedVariance, oscSpeedVariance);

		// Stagger timeAccum's start too, so even frame-perfect spawns desync immediately
		timeAccum = (float)GD.RandRange(0.0, 10.0);
	}

	public void Initialize(MovementComponent movementComponent, Player player)
	{
		this.movementComponent = movementComponent;
		this.player = player;
	}

	public override void _PhysicsProcess(double delta)
	{
		Pathfind(player, (float)delta);
	}

	public void Pathfind(Node2D target, float delta)
    {
        if (target == null)
        {
            return;
        }

        timeAccum += delta;

        Vector2 toTarget = target.GlobalPosition - GlobalPosition;
        float distance = toTarget.Length();
        Vector2 radialDir = toTarget.Normalized();
        Vector2 tangentialDir = new Vector2(-radialDir.Y, radialDir.X);

        float osc = Mathf.Sin((timeAccum + phaseOffset) * oscSpeed);

        float radialFactor = Mathf.Clamp((distance - minDistance) / falloffRange, -1f, 1f);

        Vector2 desiredVelocity =
            radialDir * radialFactor +
            tangentialDir * osc * (strafeSpeed / Mathf.Max(movementComponent.speed, 0.01f));

        movementComponent.direction = desiredVelocity.Normalized();
    }
}
