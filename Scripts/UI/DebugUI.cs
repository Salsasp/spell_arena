using Godot;
using System;
using System.Collections.Generic;

public partial class DebugUI : Control
{
	[Export] public Label debugHealthLabel;
	[Export] public OptionButton modifierOptionButton;
	[Export] public PopupPanel modifierPickerPopup;
	[Export] public Label modifierApplyStatusLabel;
	private HealthComponent healthComponent;
	private RunManager runManager;
	private ModifierManager modifierManager;
	private bool pausedForModifierPicker;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		debugHealthLabel = GetNode<Label>("DebugHealthLabel");
		modifierOptionButton = GetNode<OptionButton>("ModifierPickerPopup/MarginContainer/VBoxContainer/ModifierOptionButton");
		modifierPickerPopup = GetNode<PopupPanel>("ModifierPickerPopup");
		modifierPickerPopup.ProcessMode = ProcessModeEnum.Always;
		modifierPickerPopup.PopupHide += OnModifierPickerHidden;
		modifierApplyStatusLabel = GetNode<Label>("ModifierPickerPopup/MarginContainer/VBoxContainer/ApplyStatusLabel");
		runManager = GetNode<RunManager>("../../RunManager");
		modifierManager = GetNode<ModifierManager>("../../Player/ModifierManager");

		GetNode<Button>("DebugButtons/EndRoundButton").Pressed += OnEndRoundButtonPressed;
		GetNode<Button>("DebugButtons/AddModifierButton").Pressed += OnAddModifierButtonPressed;
		GetNode<Button>("ModifierPickerPopup/MarginContainer/VBoxContainer/PickerActions/ApplyModifierButton").Pressed += OnApplyModifierPressed;
		GetNode<Button>("ModifierPickerPopup/MarginContainer/VBoxContainer/PickerActions/CloseModifierPickerButton").Pressed += HideModifierPicker;

		PopulateModifierOptions();
	}

	public void InitComponents(HealthComponent healthComp)
	{
		healthComponent = healthComp;
		OnHealthChanged(healthComponent.GetCurrentHealth());
	}

	public void OnHealthChanged(int newHealth)
	{
		debugHealthLabel.Text = $"HP: {newHealth}/{healthComponent.MaxHealth}";
	}

	private void OnEndRoundButtonPressed()
	{
		runManager.EmitSignal(RunManager.SignalName.ForceEndWave);
	}

	private void OnAddModifierButtonPressed()
	{
		PopulateModifierOptions();
		modifierApplyStatusLabel.Text = string.Empty;

		if (!GetTree().Paused)
		{
			pausedForModifierPicker = true;
			GetTree().Paused = true;
		}
		else
		{
			pausedForModifierPicker = false;
		}

		Vector2I popupPos = (Vector2I)GetGlobalMousePosition();
		modifierPickerPopup.PopupOnParent(new Rect2I(popupPos, Vector2I.Zero));
	}

	private void OnApplyModifierPressed()
	{
		int selectedIndex = modifierOptionButton.Selected;
		if (selectedIndex < 0)
		{
			modifierApplyStatusLabel.Text = "Select a modifier first.";
			return;
		}

		string modifierId = modifierOptionButton.GetItemMetadata(selectedIndex).AsString();
		bool added = modifierManager.TryCollectModifier(modifierId);
		ModifierData modifier = ModifierCatalog.Get(modifierId);
		modifierApplyStatusLabel.Text = added
			? $"Added {modifier.name}."
			: $"{modifier.name} could not be collected.";

		GameUI gameUi = GetNodeOrNull<GameUI>("../GameUI");
		if (gameUi?.ModifyWandUI != null && gameUi.ModifyWandUI.Visible)
		{
			gameUi.ModifyWandUI.Refresh();
		}
	}

	private void HideModifierPicker()
	{
		modifierPickerPopup.Hide();
	}

	private void OnModifierPickerHidden()
	{
		if (!pausedForModifierPicker)
		{
			return;
		}

		GetTree().Paused = false;
		pausedForModifierPicker = false;
	}

	private void PopulateModifierOptions()
	{
		modifierOptionButton.Clear();

		List<ModifierData> allModifiers = new(ModifierCatalog.GetAll());
		allModifiers.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase));

		foreach (ModifierData modifier in allModifiers)
		{
			int index = modifierOptionButton.ItemCount;
			modifierOptionButton.AddItem($"{modifier.name} [{modifier.rarity}]", index);
			modifierOptionButton.SetItemMetadata(index, modifier.id);
		}

		if (modifierOptionButton.ItemCount > 0)
		{
			modifierOptionButton.Select(0);
		}
	}
}
