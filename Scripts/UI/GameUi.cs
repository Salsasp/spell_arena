using Godot;
using System.Collections.Generic;

public partial class GameUi : Control
{
	private const float DetailHideDelay = 0.12f;

	[Signal] public delegate void ModifierSelectedEventHandler(string modifierId);

	private Label waveLabel;
	private Label statusLabel;
	private Panel modifierDraftPanel;
	private HBoxContainer modifierOptionsContainer;
	private Control modifierDetailModal;
	private Panel detailPanel;
	private Label detailNameLabel;
	private Label detailDescriptionLabel;
	private Label detailRarityLabel;
	private Panel gameOverPanel;
	private Label gameOverLabel;
	private Timer hideDetailTimer;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		waveLabel = GetNode<Label>("WaveLabel");
		statusLabel = GetNode<Label>("StatusLabel");
		modifierDraftPanel = GetNode<Panel>("ModifierDraftPanel");
		modifierOptionsContainer = GetNode<HBoxContainer>("ModifierDraftPanel/MarginContainer/VBoxContainer/ModifierOptions");
		modifierDetailModal = GetNode<Control>("ModifierDetailModal");
		detailPanel = GetNode<Panel>("ModifierDetailModal/DetailPanel");
		detailNameLabel = GetNode<Label>("ModifierDetailModal/DetailPanel/MarginContainer/VBoxContainer/DetailNameLabel");
		detailDescriptionLabel = GetNode<Label>("ModifierDetailModal/DetailPanel/MarginContainer/VBoxContainer/DetailDescriptionLabel");
		detailRarityLabel = GetNode<Label>("ModifierDetailModal/DetailPanel/MarginContainer/VBoxContainer/DetailRarityLabel");
		gameOverPanel = GetNode<Panel>("GameOverPanel");
		gameOverLabel = GetNode<Label>("GameOverPanel/MarginContainer/GameOverLabel");

		hideDetailTimer = new Timer
		{
			OneShot = true,
			WaitTime = DetailHideDelay,
			ProcessMode = ProcessModeEnum.Always,
		};
		hideDetailTimer.Timeout += HideModifierDetail;
		AddChild(hideDetailTimer);

		detailPanel.MouseEntered += CancelHideModifierDetail;
		detailPanel.MouseExited += ScheduleHideModifierDetail;

		HideModifierDetail();
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
		HideModifierDetail();
		statusLabel.Text = "Choose a spell modifier";

		foreach (ModifierData modifier in options)
		{
			VBoxContainer card = new()
			{
				CustomMinimumSize = new Vector2(180, 0),
				SizeFlagsHorizontal = SizeFlags.ShrinkCenter,
			};

			Button selectButton = new()
			{
				CustomMinimumSize = new Vector2(180, 96),
				Text = $"{modifier.name}\n[{modifier.rarity}]",
				Icon = GD.Load<Texture2D>(modifier.icon),
				ExpandIcon = true,
			};
			selectButton.Pressed += () => EmitSignal(SignalName.ModifierSelected, modifier.id);
			selectButton.MouseEntered += () => OnModifierCardHovered(modifier, card);
			selectButton.MouseExited += ScheduleHideModifierDetail;

			card.AddChild(selectButton);
			modifierOptionsContainer.AddChild(card);
		}

		modifierDraftPanel.Visible = true;
	}

	public void HideModifierDraft()
	{
		modifierDraftPanel.Visible = false;
		statusLabel.Text = string.Empty;
		HideModifierDetail();
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

	private void OnModifierCardHovered(ModifierData modifier, Control anchor)
	{
		CancelHideModifierDetail();
		ShowModifierDetail(modifier, anchor);
	}

	private void ShowModifierDetail(ModifierData modifier, Control anchor)
	{
		detailNameLabel.Text = modifier.name;
		detailRarityLabel.Text = modifier.rarity;
		detailDescriptionLabel.Text = modifier.description;
		modifierDetailModal.Visible = true;
		modifierDetailModal.MoveToFront();
		CallDeferred(MethodName.PositionDetailPanel, anchor);
	}

	private void PositionDetailPanel(Control anchor)
	{
		if (!modifierDetailModal.Visible || anchor == null || !GodotObject.IsInstanceValid(anchor))
		{
			return;
		}

		Rect2 cardRect = anchor.GetGlobalRect();
		Vector2 panelSize = detailPanel.Size;
		float x = cardRect.GetCenter().X - panelSize.X * 0.5f;
		float y = cardRect.Position.Y - panelSize.Y - 12f;
		detailPanel.GlobalPosition = new Vector2(x, y);
	}

	private void ScheduleHideModifierDetail()
	{
		hideDetailTimer.Start();
	}

	private void CancelHideModifierDetail()
	{
		hideDetailTimer.Stop();
	}

	private void HideModifierDetail()
	{
		hideDetailTimer.Stop();
		modifierDetailModal.Visible = false;
	}

	private void ClearModifierOptions()
	{
		foreach (Node child in modifierOptionsContainer.GetChildren())
		{
			child.QueueFree();
		}
	}
}
