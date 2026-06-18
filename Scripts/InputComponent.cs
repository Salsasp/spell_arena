using Godot;
using System;

public partial class InputComponent : Node2D
{
	public Vector2 MoveDir { get; private set; } = Vector2.Zero;
	public bool LeftClickPressed { get; private set; } = false;
	public bool LeftClickJustPressed { get; private set; } = false;
	public bool RightClickPressed { get; private set; } = false;
	public Vector2 MousePos { get; private set; } = Vector2.Zero;

	public void Update()
	{
		MoveDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		LeftClickPressed = Input.IsMouseButtonPressed(MouseButton.Left);
		RightClickPressed = Input.IsMouseButtonPressed(MouseButton.Right);
		MousePos = GetGlobalMousePosition();
	}
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	// For debugging
	public override void _Draw()
    {
		if (LeftClickPressed)
		{
			DrawLine(Position, MousePos, Colors.Red);
		}
    }
}
