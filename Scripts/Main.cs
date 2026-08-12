using Godot;
using System;

public partial class Main : Node2D
{
	DebugUI debugUi;
	HealthComponent healthComponent;
	public override void _Ready()
	{
		// Link game state data to UI components
		debugUi = GetNode<DebugUI>("UI/DebugUi");
		healthComponent = GetNode<HealthComponent>("Player/HealthComponent");
		healthComponent.HealthChanged += debugUi.OnHealthChanged;
		debugUi.InitComponents(healthComponent);
	}
}
