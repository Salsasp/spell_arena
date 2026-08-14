using Godot;

public partial class MovementComponent : Component
{
	[Export] public CharacterBody2D body;
	[Export] public float Speed { get; set; } = 100f;
	[Export] public float DodgeSpeed { get; set; } = 500f;
	[Export] public double DodgeDuration { get; set; } = 0.15;
	[Export] public double DodgeCooldown { get; set; } = 1.0;
	[Export] public Timer DodgeTimer;
	public float SpeedMultiplier = 1.0f;

	public Vector2 direction = Vector2.Zero;
	public Vector2 mouseDir = Vector2.Zero;
	private bool dodgeOnCooldown;
	private bool isDodging;
	private Vector2 dodgeDirection;
	private double dodgeTimeRemaining;

	public void Update(double delta)
	{
		if (body == null)
		{
			return;
		}

		if (isDodging)
		{
			dodgeTimeRemaining -= delta;
			if (dodgeTimeRemaining <= 0)
			{
				isDodging = false;
			}
			else
			{
				body.Velocity = dodgeDirection * DodgeSpeed;
				body.MoveAndSlide();
				return;
			}
		}

		body.Velocity = direction * (Speed * SpeedMultiplier);
		body.MoveAndSlide();
	}

	public override void _Ready()
	{
		if (DodgeTimer != null)
		{
			DodgeTimer.Timeout += OnDodgeTimerTimeout;
		}
	}

	public void Dodge(Vector2 moveDir)
	{
		if (dodgeOnCooldown || isDodging)
		{
			return;
		}

		if (moveDir != Vector2.Zero)
		{
			dodgeDirection = moveDir.Normalized();
		}
		else if (mouseDir != Vector2.Zero)
		{
			dodgeDirection = mouseDir.Normalized();
		}
		else
		{
			dodgeDirection = Vector2.Down;
		}

		isDodging = true;
		dodgeTimeRemaining = DodgeDuration;
		dodgeOnCooldown = true;
		DodgeTimer?.Start(DodgeCooldown);
	}

	private void OnDodgeTimerTimeout()
	{
		dodgeOnCooldown = false;
	}
}
