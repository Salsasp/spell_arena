using Godot;

public partial class InventoryModifierSlot : PanelContainer
{
    [Export] public Color NormalColor = Colors.White;
    [Export] public Color HoverColor = new Color(0.6f, 0.6f, 0.6f);
    [Export] public float TweenDuration = 0.1f;
	[Export] public TextureRect IconFrame;

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    private void OnMouseEntered()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate", HoverColor, TweenDuration);
    }

    private void OnMouseExited()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate", NormalColor, TweenDuration);
    }
}