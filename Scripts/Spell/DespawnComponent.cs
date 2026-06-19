using Godot;
using System;

public partial class DespawnComponent : Node
{
	public override void _Ready()
	{
	}

	public void Despawn()
	{
		GetParent().QueueFree();
	}
}
