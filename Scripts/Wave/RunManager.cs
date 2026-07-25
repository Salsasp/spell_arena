using Godot;
using System.Collections.Generic;
using System.ComponentModel;

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
	[Signal] public delegate void ForceEndWaveEventHandler();

	private WaveSpawner waveSpawner;
	private GameUi gameUi;
	private Player player;
	private ModifierManager modifierManager;
	private HealthComponent playerHealth;
	private int currentWave = 1;
	private RunPhase phase = RunPhase.Combat;
	private const int DEFAULT_DRAFT_OPTIONS = 3;

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
		ForceEndWave += OnForceEndWave;

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

		int numDraftOptions = DEFAULT_DRAFT_OPTIONS;
		if (modifierManager.HasActiveModifier("extra_modifier"))
		{
			numDraftOptions += modifierManager.ActiveModifiers["extra_modifier"].copies;	
		}

		gameUi.ShowModifierDraft(ModifierDraftService.RollDraft(numDraftOptions));
	}

	private void OnModifierSelected(string modifierId)
	{
		if (phase != RunPhase.ModifierDraft)
		{
			return;
		}

		if (!modifierManager.TryAddActiveModifier(modifierId))
		{
			GD.Print($"Modifier '{modifierId}' was not added.");
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

	private void OnForceEndWave()
	{
		waveSpawner.ClearAllEnemies();
		waveSpawner.EmitSignal(WaveSpawner.SignalName.WaveComplete);
	}
}
