using Godot;
using System;

public partial class DespawnComponent : Component
{
	public override void _Ready()
	{
	}

	public void Despawn()
	{
		GetParent().QueueFree();
	}
}
