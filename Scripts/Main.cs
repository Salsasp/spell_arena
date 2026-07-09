using Godot;
using System;

public partial class Main : Node2D
{
	DebugUi debugUi;
	HealthComponent healthComponent;
	public override void _Ready()
	{
		debugUi = GetNode<DebugUi>("DebugUi");
		healthComponent = GetNode<HealthComponent>("Player/HealthComponent");
		healthComponent.HealthChanged += debugUi.OnHealthChanged;
		debugUi.InitComponents(healthComponent);
	}
}
