using Godot;
using System;

public partial class LifetimeComponent : Node
{
	[Export] public int Lifetime; // Seconds
	public bool TimedOut = false;
	private Timer timer;

	
	public void Update()
	{
		
	}

	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		timer.WaitTime = Lifetime;
		timer.Timeout += OnTimerTimeout;
		timer.Start();
	}

	private void OnTimerTimeout()
	{
		TimedOut = true;
	}
}
