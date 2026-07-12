using Godot;
using System.Collections.Generic;

public partial class GameUi : Control
{
	[Signal] public delegate void ModifierSelectedEventHandler(string modifierId);

	private Label waveLabel;
	private Label statusLabel;
	private Panel modifierDraftPanel;
	private HBoxContainer modifierOptionsContainer;
	private Panel gameOverPanel;
	private Label gameOverLabel;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		waveLabel = GetNode<Label>("WaveLabel");
		statusLabel = GetNode<Label>("StatusLabel");
		modifierDraftPanel = GetNode<Panel>("ModifierDraftPanel");
		modifierOptionsContainer = GetNode<HBoxContainer>("ModifierDraftPanel/MarginContainer/VBoxContainer/ModifierOptions");
		gameOverPanel = GetNode<Panel>("GameOverPanel");
		gameOverLabel = GetNode<Label>("GameOverPanel/MarginContainer/GameOverLabel");

		HideModifierDraft();
		HideGameOver();
	}

	public void SetWave(int waveNumber)
	{
		waveLabel.Text = $"Wave {waveNumber}";
		statusLabel.Text = string.Empty;
	}

	public void ShowModifierDraft(List<ModifierData> options)
	{
		ClearModifierOptions();
		statusLabel.Text = "Choose a spell modifier";

		foreach (ModifierData modifier in options)
		{
			Button button = new()
			{
				CustomMinimumSize = new Vector2(220, 140),
				Text = $"{modifier.name}\n\n{modifier.description}\n\n[{modifier.rarity}]",
			};
			button.Pressed += () => EmitSignal(SignalName.ModifierSelected, modifier.id);
			modifierOptionsContainer.AddChild(button);
		}

		modifierDraftPanel.Visible = true;
	}

	public void HideModifierDraft()
	{
		modifierDraftPanel.Visible = false;
		statusLabel.Text = string.Empty;
		ClearModifierOptions();
	}

	public void ShowGameOver(int waveReached)
	{
		gameOverLabel.Text = $"Game Over\nYou reached wave {waveReached}";
		gameOverPanel.Visible = true;
	}

	public void HideGameOver()
	{
		gameOverPanel.Visible = false;
	}

	private void ClearModifierOptions()
	{
		foreach (Node child in modifierOptionsContainer.GetChildren())
		{
			child.QueueFree();
		}
	}
}
