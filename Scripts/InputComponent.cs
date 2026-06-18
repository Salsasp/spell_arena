using Godot;
using System;

public partial class InputComponent : Node
{
	public Vector2 moveDir = Vector2.Zero;

	public void Update()
	{
		moveDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
	}
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
}
