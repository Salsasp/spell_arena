using Godot;

public partial class InventoryModifierSlot : PanelContainer
{
	[Export] public Color NormalColor = Colors.White;
	[Export] public Color HoverColor = new Color(0.6f, 0.6f, 0.6f);
	[Export] public float TweenDuration = 0.1f;
	[Export] public TextureRect IconFrame;
	[Export] public TextureRect ModifierIcon;

	public ModifierInventory Inventory { get; private set; }
	public int SlotIndex { get; private set; }
	public Wand HostWand { get; private set; }
	public ModifierData AssignedModifier { get; private set; }

	private ModifyWandUI ownerUi;

	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	public void Bind(ModifyWandUI owner, ModifierInventory inventory, int index, Wand hostWand)
	{
		ownerUi = owner;
		Inventory = inventory;
		SlotIndex = index;
		HostWand = hostWand;
		Refresh();
	}

	public void Refresh()
	{
		AssignedModifier = Inventory?.GetSlot(SlotIndex);
		if (ModifierIcon == null)
		{
			return;
		}

		if (AssignedModifier == null || string.IsNullOrEmpty(AssignedModifier.icon))
		{
			ModifierIcon.Texture = null;
			return;
		}

		ModifierIcon.Texture = GD.Load<Texture2D>(AssignedModifier.icon);
	}

	public override Variant _GetDragData(Vector2 atPosition)
	{
		if (AssignedModifier == null)
		{
			return default;
		}

		TextureRect preview = new()
		{
			Texture = ModifierIcon.Texture,
			CustomMinimumSize = new Vector2(32, 32),
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
		};
		SetDragPreview(preview);
		ownerUi?.HideModifierTooltip(this);
		return this;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		if (data.AsGodotObject() is not InventoryModifierSlot source)
		{
			return false;
		}

		return ownerUi != null && ownerUi.CanMoveModifier(source, this);
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		if (data.AsGodotObject() is not InventoryModifierSlot source)
		{
			return;
		}

		ownerUi?.MoveModifier(source, this);
	}

	private void OnMouseEntered()
	{
		Tween tween = CreateTween();
		tween.SetPauseMode(Tween.TweenPauseMode.Process);
		tween.TweenProperty(this, "modulate", HoverColor, TweenDuration);
		ownerUi?.ShowModifierTooltip(this);
	}

	private void OnMouseExited()
	{
		Tween tween = CreateTween();
		tween.SetPauseMode(Tween.TweenPauseMode.Process);
		tween.TweenProperty(this, "modulate", NormalColor, TweenDuration);
		ownerUi?.HideModifierTooltip(this);
	}
}
