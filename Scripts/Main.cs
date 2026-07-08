using Godot;
using System;

public partial class Main : Node2D
{
	DebugHealthLabel debugHealthLabel;
	HealthComponent healthComponent;
	public override void _Ready()
	{
		debugHealthLabel = GetNode<DebugHealthLabel>("DebugUi/DebugHealthLabel");
		healthComponent = GetNode<HealthComponent>("Player/HealthComponent");
		healthComponent.HealthChanged += OnHealthChanged;

		// Initialize the debug label with the current health
		OnHealthChanged(healthComponent.GetCurrentHealth());
	}

	public void OnHealthChanged(int newHealth)
	{
		debugHealthLabel.Text = $"HP: {newHealth}/{healthComponent.MaxHealth}";
	}
}
