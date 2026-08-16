using Godot;
using System.Collections.Generic;

public partial class ModifyWandUI : Control
{
	[Signal] public delegate void ContinuePressedEventHandler();

	[Export] public GridContainer ArcaneGrid;
	[Export] public GridContainer LightningGrid;
	[Export] public GridContainer FireGrid;
	[Export] public GridContainer IceGrid;
	[Export] public GridContainer InventoryGrid;
	[Export] public Button ContinueButton;
	[Export] public Control TooltipPanel;
	[Export] public Label TooltipNameLabel;
	[Export] public Label TooltipDescriptionLabel;
	[Export] public Label TooltipRarityLabel;
	[Export] public Label TooltipAffinityLabel;

	private Player player;
	private InventoryModifierSlot hoveredSlot;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Visible = false;
		HideModifierTooltip();
		if (ContinueButton != null)
		{
			ContinueButton.Pressed += OnContinuePressed;
		}
	}

	public void ShowCustomize(Player targetPlayer)
	{
		player = targetPlayer;
		BindAll();
		Refresh();
		Visible = true;
		MoveToFront();
	}

	public void HideCustomize()
	{
		HideModifierTooltip();
		Visible = false;
	}

	public void Refresh()
	{
		if (ArcaneGrid == null)
		{
			return;
		}

		foreach (InventoryModifierSlot slot in GetAllSlots())
		{
			slot.Refresh();
		}

		if (hoveredSlot != null)
		{
			ShowModifierTooltip(hoveredSlot);
		}
	}

	public void ShowModifierTooltip(InventoryModifierSlot slot)
	{
		hoveredSlot = slot;
		if (TooltipPanel == null || slot?.AssignedModifier == null || GetViewport().GuiIsDragging())
		{
			if (TooltipPanel != null)
			{
				TooltipPanel.Visible = false;
			}
			return;
		}

		ModifierData modifier = slot.AssignedModifier;
		TooltipNameLabel.Text = modifier.name;
		TooltipDescriptionLabel.Text = modifier.description;
		TooltipRarityLabel.Text = $"Rarity: {FormatDisplay(modifier.rarity)}";
		TooltipAffinityLabel.Text = $"Affinity: {FormatDisplay(modifier.affinity.ToString())}";
		TooltipRarityLabel.Modulate = RarityColor(modifier.rarity);
		TooltipAffinityLabel.Modulate = AffinityColor(modifier.affinity);

		TooltipPanel.Visible = true;
		TooltipPanel.MoveToFront();
		CallDeferred(MethodName.PositionTooltip, slot);
	}

	public void HideModifierTooltip(InventoryModifierSlot slot = null)
	{
		if (slot != null && hoveredSlot != slot)
		{
			return;
		}

		hoveredSlot = null;
		if (TooltipPanel != null)
		{
			TooltipPanel.Visible = false;
		}
	}

	private void PositionTooltip(InventoryModifierSlot slot)
	{
		if (TooltipPanel == null || slot == null || !GodotObject.IsInstanceValid(slot) || !TooltipPanel.Visible)
		{
			return;
		}

		TooltipPanel.ResetSize();
		Vector2 size = TooltipPanel.Size;
		Rect2 slotRect = slot.GetGlobalRect();
		Rect2 viewport = GetViewport().GetVisibleRect();

		float x = slotRect.GetCenter().X - size.X * 0.5f;
		x = Mathf.Clamp(x, viewport.Position.X + 8f, viewport.End.X - size.X - 8f);

		float y = slotRect.Position.Y - size.Y - 8f;
		if (y < viewport.Position.Y + 8f)
		{
			y = slotRect.End.Y + 8f;
		}

		if (y + size.Y > viewport.End.Y - 8f)
		{
			y = viewport.End.Y - size.Y - 8f;
		}

		TooltipPanel.GlobalPosition = new Vector2(x, y);
	}

	private static string FormatDisplay(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return "—";
		}

		string trimmed = value.Trim();
		return char.ToUpperInvariant(trimmed[0]) + trimmed[1..].ToLowerInvariant();
	}

	private static Color RarityColor(string rarity)
	{
		return rarity?.Trim().ToLowerInvariant() switch
		{
			"uncommon" => new Color(0.24f, 0.8f, 0.42f),
			"rare" => new Color(0.29f, 0.64f, 1f),
			_ => new Color(0.82f, 0.82f, 0.82f),
		};
	}

	private static Color AffinityColor(ModifierAffinity affinity)
	{
		return affinity switch
		{
			ModifierAffinity.Lightning => new Color(0.94f, 0.83f, 0.29f),
			ModifierAffinity.Fire => new Color(1f, 0.48f, 0.24f),
			ModifierAffinity.Ice => new Color(0.49f, 0.78f, 1f),
			_ => new Color(0.82f, 0.82f, 0.82f),
		};
	}

	public bool CanMoveModifier(InventoryModifierSlot source, InventoryModifierSlot destination)
	{
		if (source == null || destination == null || source == destination)
		{
			return false;
		}

		if (source.AssignedModifier == null || source.Inventory == null || destination.Inventory == null)
		{
			return false;
		}

		if (!CanAccept(destination, source.AssignedModifier))
		{
			return false;
		}

		ModifierData destinationModifier = destination.AssignedModifier;
		if (destinationModifier != null && !CanAccept(source, destinationModifier))
		{
			return false;
		}

		return true;
	}

	public void MoveModifier(InventoryModifierSlot source, InventoryModifierSlot destination)
	{
		if (!CanMoveModifier(source, destination))
		{
			return;
		}

		ModifierData moving = source.Inventory.GetSlot(source.SlotIndex);
		ModifierData replaced = destination.Inventory.GetSlot(destination.SlotIndex);
		destination.Inventory.SetSlot(destination.SlotIndex, moving);
		source.Inventory.SetSlot(source.SlotIndex, replaced);
		source.Refresh();
		destination.Refresh();
		if (hoveredSlot == source || hoveredSlot == destination)
		{
			ShowModifierTooltip(hoveredSlot);
		}
	}

	private void BindAll()
	{
		if (player == null)
		{
			return;
		}

		BindGrid(ArcaneGrid, player.ArcaneWand.wandInventory.Slots, player.ArcaneWand);
		BindGrid(LightningGrid, player.LightningWand.wandInventory.Slots, player.LightningWand);
		BindGrid(FireGrid, player.FireWand.wandInventory.Slots, player.FireWand);
		BindGrid(IceGrid, player.IceWand.wandInventory.Slots, player.IceWand);
		BindGrid(InventoryGrid, player.ModifierPool, null);
	}

	private void BindGrid(GridContainer grid, ModifierInventory inventory, Wand hostWand)
	{
		int index = 0;
		foreach (Node child in grid.GetChildren())
		{
			if (child is not InventoryModifierSlot slot)
			{
				continue;
			}

			slot.Bind(this, inventory, index, hostWand);
			index++;
		}
	}

	private static bool CanAccept(InventoryModifierSlot slot, ModifierData modifier)
	{
		if (slot.HostWand == null)
		{
			return true;
		}

		return slot.HostWand.CanAccept(modifier);
	}

	private IEnumerable<InventoryModifierSlot> GetAllSlots()
	{
		foreach (GridContainer grid in new[] { ArcaneGrid, LightningGrid, FireGrid, IceGrid, InventoryGrid })
		{
			if (grid == null)
			{
				continue;
			}

			foreach (Node child in grid.GetChildren())
			{
				if (child is InventoryModifierSlot slot)
				{
					yield return slot;
				}
			}
		}
	}

	private void OnContinuePressed()
	{
		EmitSignal(SignalName.ContinuePressed);
	}
}
