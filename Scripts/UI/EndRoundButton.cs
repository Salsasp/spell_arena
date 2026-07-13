using Godot;
using System;

public partial class EndRoundButton : Button
{
	private RunManager runManager;

	public override void _Ready()
	{
		runManager = GetNode<RunManager>("../../RunManager");
		Pressed += () => {
			runManager.EmitSignal(RunManager.SignalName.ForceEndWave);
		};
	}
}
