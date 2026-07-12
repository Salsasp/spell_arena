using Godot;
using System.Collections.Generic;

public enum RunPhase
{
	Combat,
	ModifierDraft,
	GameOver,
}

public partial class RunManager : Node
{
	[Export] public NodePath WaveSpawnerPath { get; set; }
	[Export] public NodePath GameUiPath { get; set; }
	[Export] public NodePath PlayerPath { get; set; }

	[Signal] public delegate void PhaseChangedEventHandler(RunPhase phase);
	[Signal] public delegate void WaveStartedEventHandler(int waveNumber);

	private WaveSpawner waveSpawner;
	private GameUi gameUi;
	private Player player;
	private ModifierManager modifierManager;
	private HealthComponent playerHealth;
	private int currentWave = 1;
	private RunPhase phase = RunPhase.Combat;

	public override void _Ready()
	{
		waveSpawner = GetNode<WaveSpawner>(WaveSpawnerPath);
		gameUi = GetNode<GameUi>(GameUiPath);
		player = GetNode<Player>(PlayerPath);
		modifierManager = player.GetNode<ModifierManager>("ModifierManager");
		playerHealth = player.GetNode<HealthComponent>("HealthComponent");

		waveSpawner.WaveComplete += OnWaveComplete;
		playerHealth.Dead += OnPlayerDead;
		gameUi.ModifierSelected += OnModifierSelected;

		StartWave();
	}

	private void StartWave()
	{
		SetPhase(RunPhase.Combat);
		WaveDefinition wave = WaveTable.GetWave(currentWave);
		gameUi.SetWave(currentWave);
		gameUi.HideModifierDraft();
		gameUi.HideGameOver();
		EmitSignal(SignalName.WaveStarted, currentWave);
		waveSpawner.BeginWave(wave);
	}

	private void OnWaveComplete()
	{
		if (phase == RunPhase.GameOver)
		{
			return;
		}

		SetPhase(RunPhase.ModifierDraft);
		GetTree().Paused = true;
		gameUi.ShowModifierDraft(ModifierDraftService.RollDraft());
	}

	private void OnModifierSelected(string modifierId)
	{
		if (phase != RunPhase.ModifierDraft)
		{
			return;
		}

		ModifierData modifier = modifierManager.GetModifier(modifierId);
		if (modifier != null && !modifierManager.HasActiveModifier(modifierId))
		{
			modifierManager.ActiveModifiers.Add(modifierId, modifier);
		}

		GetTree().Paused = false;
		currentWave++;
		StartWave();
	}

	private void OnPlayerDead()
	{
		SetPhase(RunPhase.GameOver);
		GetTree().Paused = true;
		waveSpawner.ClearAllEnemies();
		gameUi.HideModifierDraft();
		gameUi.ShowGameOver(currentWave);
	}

	private void SetPhase(RunPhase newPhase)
	{
		phase = newPhase;
		EmitSignal(SignalName.PhaseChanged, (int)newPhase);
	}
}
