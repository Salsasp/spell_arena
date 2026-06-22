using Godot;

public abstract partial class Component : Node2D
{
    protected Node2D owner;

    public override void _Ready()
    {
        owner = GetParent<Node2D>();
    }

    public static T FindComponent<T>(Node2D node) where T : Component
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is T component)
            {
                return component;
            }
        }
        return null;
    }
}