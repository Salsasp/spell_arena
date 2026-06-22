using Godot;
using System;
using System.ComponentModel;

public partial class SpellMovementComponent : Component
{
	[Export] public Sprite2D Sprite;
	[Export] public float Speed = 50f;
	public Vector2 Velocity = Vector2.Zero;
	public Vector2 Direction = Vector2.Zero;

	public void Update()
	{
		if (Direction != Vector2.Zero)
		{
			Velocity = Direction * Speed;
		}
	}

	public override void _Ready()
	{
	}
}
