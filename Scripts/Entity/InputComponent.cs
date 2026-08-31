using Godot;
using System;

public partial class InputComponent : Component
{
	public Vector2 MoveDir { get; private set; } = Vector2.Zero;
	public bool LeftClickPressed { get; private set; } = false;
	public bool LeftClickJustPressed { get; private set; } = false;
	public bool RightClickPressed { get; private set; } = false;
	public bool DodgePressed { get; private set; } = false;
	public bool Wand1Pressed { get; private set; } = false;
	public bool Wand2Pressed { get; private set; } = false;
	public bool Wand3Pressed { get; private set; } = false;
	public bool Wand4Pressed { get; private set; } = false;
	public bool WandCycleUpPressed { get; private set; } = false;
	public bool WandCycleDownPressed { get; private set; } = false;
	public Vector2 MousePos { get; private set; } = Vector2.Zero;

	public void Update()
	{
		MoveDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		LeftClickPressed = Input.IsMouseButtonPressed(MouseButton.Left);
		LeftClickJustPressed = Input.IsActionJustReleased("left_click");
		RightClickPressed = Input.IsMouseButtonPressed(MouseButton.Right);
		DodgePressed = Input.IsActionJustPressed("dodge");
		MousePos = GetGlobalMousePosition();

		Wand1Pressed = Input.IsActionJustReleased("wand1");
		Wand2Pressed = Input.IsActionJustReleased("wand2");
		Wand3Pressed = Input.IsActionJustReleased("wand3");
		Wand4Pressed = Input.IsActionJustReleased("wand4");
		WandCycleUpPressed = Input.IsActionJustPressed("wand_cycle_up");
		WandCycleDownPressed = Input.IsActionJustPressed("wand_cycle_down");
	}
	public override void _Ready()
	{
	}

	// public override void _Process(double delta)
	// {
	// 	QueueRedraw();
	// }

	// // For debugging
	// public override void _Draw()
    // {
	// 	if (LeftClickPressed)
	// 	{
	// 		DrawLine(Position, MousePos, Colors.Red);
	// 	}
    // }
}
