using Godot;
using System;
using System.Collections.Generic;

public partial class ModifierDraftUI : Control
{
	private const float DetailHideDelay = 0.12f;

	[Signal] public delegate void ModifierSelectedEventHandler(string modifierId);
	[Export] public Panel modifierDraftPanel;
	[Export] public Panel draftTitlePanel;
	[Export] public HBoxContainer modifierOptionsContainer;
	[Export] public HBoxContainer modifierIconsContainer;
	[Export] public Control modifierDetailModal;
	[Export] public Panel detailPanel;
	[Export] public Label detailNameLabel;
	[Export] public Label detailDescriptionLabel;
	[Export] public Label detailRarityLabel;
	private Timer hideDetailTimer;
	private GameUI gameUI;

	public override void _Ready()
	{
		gameUI = GetParent<GameUI>();

		detailPanel.MouseEntered += CancelHideModifierDetail;
		detailPanel.MouseExited += ScheduleHideModifierDetail;

		hideDetailTimer = new Timer
		{
			OneShot = true,
			WaitTime = DetailHideDelay,
			ProcessMode = ProcessModeEnum.Always,
		};
		hideDetailTimer.Timeout += HideModifierDetail;
		AddChild(hideDetailTimer);

		HideModifierDetail();
		HideModifierDraft();
	}

	public override void _Process(double delta)
	{
	}

	public void HideModifierDraft()
	{
		modifierDraftPanel.Visible = false;
		draftTitlePanel.Visible = false;
		gameUI.statusLabel.Text = string.Empty;
		HideModifierDetail();
		ClearModifierOptions();
		ClearModifierIcons();
	}

	public void ShowModifierDraft(List<ModifierData> options)
	{
		ClearModifierOptions();
		ClearModifierIcons();
		HideModifierDetail();
		gameUI.statusLabel.Text = "Choose a spell modifier";

		foreach (ModifierData modifier in options)
		{
			VBoxContainer card = new()
			{
				CustomMinimumSize = new Vector2(180, 0),
				SizeFlagsHorizontal = SizeFlags.ShrinkCenter,
			};

			CenterContainer cardCenterContainer = new()
			{
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				SizeFlagsStretchRatio = 1.0f,
			};

			cardCenterContainer.AddChild(card);

			CenterContainer iconCenterContainer = new()
			{
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				SizeFlagsStretchRatio = 1.0f,
			};

			TextureRect modifierIcon = new()
			{
				Texture = GD.Load<Texture2D>(modifier.icon),
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
				CustomMinimumSize = new Vector2(50,50),
			};

			iconCenterContainer.AddChild(modifierIcon);

			modifierIconsContainer.AddChild(iconCenterContainer);

			Button selectButton = new()
			{
				CustomMinimumSize = new Vector2(180, 96),
				Text = $"{modifier.name}\n[{modifier.rarity}]",
				ExpandIcon = true,
				AutowrapMode = TextServer.AutowrapMode.Off,
				SizeFlagsHorizontal = SizeFlags.ShrinkCenter,
			};
			selectButton.Pressed += () => EmitSignal(SignalName.ModifierSelected, modifier.id);
			selectButton.MouseEntered += () => OnModifierCardHovered(modifier, card);
			selectButton.MouseExited += ScheduleHideModifierDetail;

			card.AddChild(selectButton);
			modifierOptionsContainer.AddChild(cardCenterContainer);
		}

		modifierDraftPanel.Visible = true;
		draftTitlePanel.Visible = true;
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

	private void ClearModifierIcons()
	{
		foreach (Node child in modifierIconsContainer.GetChildren())
		{
			child.QueueFree();
		}
	}

	private void CancelHideModifierDetail()
	{
		hideDetailTimer.Stop();
	}

	private void ScheduleHideModifierDetail()
	{
		hideDetailTimer.Start();
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
}
