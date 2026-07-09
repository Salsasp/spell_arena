using Godot;
using System;

public partial class DebugUi : Control
{
	private Label debugHealthLabel;
	private HealthComponent healthComponent;
	public override void _Ready()
	{
		debugHealthLabel = GetNode<Label>("DebugHealthLabel");

	}

	public void InitComponents(HealthComponent healthComp)
	{
		healthComponent = healthComp;

		// Initialize the debug label with the current health
		OnHealthChanged(healthComponent.GetCurrentHealth());
	}

	public void OnHealthChanged(int newHealth)
	{
		debugHealthLabel.Text = $"HP: {newHealth}/{healthComponent.MaxHealth}";
	}
}
