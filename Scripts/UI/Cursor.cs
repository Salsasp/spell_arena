using Godot;
using System;

public partial class Cursor : Node2D
{
	[Export] public Texture2D CrosshairCursor;
	[Export] public Texture2D MenuCursor;

	public override void _Ready()
	{
		//change this after main menu has been implemented
		Input.SetCustomMouseCursor(CrosshairCursor, Input.CursorShape.Arrow, new Vector2(15, 15));
	}

	public override void _Process(double delta)
	{
	}
}
