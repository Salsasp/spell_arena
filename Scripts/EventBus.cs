using System;
using Godot;

public partial class EventBus : Node
{
    public static EventBus Instance { get; private set; }

    //public event Action<Node2D> Fall;

    public override void _Ready()
    {
        Instance = this;
    }

    //public void EmitFall(Node2D body) => Fall?.Invoke(body);
}