using Godot;
using System.Collections.Generic;

public partial class GameUI : Control
{
	[Export] public Label waveLabel;
	[Export] public Label statusLabel;
	[Export] public ModifierDraftUI ModifierDraftUI;
	[Export] public ModifyWandUI ModifyWandUI;
	[Export] public Panel gameOverPanel;
	[Export] public Label gameOverLabel;

	public override void _Ready()
	{
		HideGameOver();
	}

	public void SetWave(int waveNumber)
	{
		waveLabel.Text = $"Wave {waveNumber}";
		statusLabel.Text = string.Empty;
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
}
