using Godot;

public partial class HitboxComponent : Component
{
    private Area2D area2D;

    [Signal]
    public delegate void HitEventHandler(Node2D hitBy);

    public override void _Ready()
    {
        base._Ready();
        area2D = GetNode<Area2D>("Area2D");
        area2D.AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area2D area)
    {
        EmitSignal(SignalName.Hit, area.GetParent<Node2D>());
    }

    public Area2D GetArea2D() => area2D;
}