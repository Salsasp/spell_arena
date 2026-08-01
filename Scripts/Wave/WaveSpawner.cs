using Godot;
using System.Collections.Generic;

public partial class WaveSpawner : Node2D
{
	[Export] public NodePath EnemyContainerPath { get; set; }
	[Export] public NodePath ArenaBoundsPath { get; set; }
	[Export] public NodePath PlayerPath { get; set; }
	[Export] public PackedScene StrafeShooterScene { get; set; }
	[Export] public PackedScene ChargerScene { get; set; }

	[Signal] public delegate void WaveCompleteEventHandler();
	[Signal] public delegate void EnemyCountChangedEventHandler(int aliveCount);

	private Node2D enemyContainer;
	private ArenaBounds arenaBounds;
	private Player player;
	private WaveDefinition activeWave;
	private int aliveCount;
	private int spawnedCount;
	private float spawnTimer;
	private bool isSpawning;

	public override void _Ready()
	{
		enemyContainer = GetNode<Node2D>(EnemyContainerPath);
		arenaBounds = GetNode<ArenaBounds>(ArenaBoundsPath);
		player = GetNode<Player>(PlayerPath);
	}

	public void BeginWave(WaveDefinition wave)
	{
		activeWave = wave;
		spawnedCount = 0;
		aliveCount = 0;
		spawnTimer = 0f;
		isSpawning = true;
	}

	public override void _Process(double delta)
	{
		if (!isSpawning || activeWave == null)
		{
			return;
		}

		spawnTimer -= (float)delta;
		if (spawnTimer <= 0f && spawnedCount < activeWave.EnemyCount)
		{
			SpawnEnemy();
			spawnedCount++;
			spawnTimer = activeWave.SpawnInterval;
		}

		if (spawnedCount >= activeWave.EnemyCount)
		{
			isSpawning = false;
			CheckWaveComplete();
		}
	}

	private void SpawnEnemy()
	{
		PackedScene scene = GD.Randf() < activeWave.ChargerWeight ? ChargerScene : StrafeShooterScene;
		Enemy enemy = scene.Instantiate<Enemy>();
		ApplyStatMultipliers(enemy);

		Vector2 playerPos = player != null && GodotObject.IsInstanceValid(player)
			? player.GlobalPosition
			: Vector2.Zero;
		Camera2D camera = player?.GetNodeOrNull<Camera2D>("Camera2D");
		Vector2 spawnPos = arenaBounds.GetSpawnPosition(playerPos, camera);
		enemy.Position = enemyContainer.ToLocal(spawnPos);
		enemyContainer.AddChild(enemy);

		HealthComponent health = enemy.GetNode<HealthComponent>("HealthComponent");
		health.Dead += OnEnemyDied;
		aliveCount++;
		EmitSignal(SignalName.EnemyCountChanged, aliveCount);
	}

	private void ApplyStatMultipliers(Enemy enemy)
	{
		HealthComponent health = enemy.GetNode<HealthComponent>("HealthComponent");
		health.MaxHealth = Mathf.Max(1, Mathf.RoundToInt(health.MaxHealth * activeWave.HealthMultiplier));

		MovementComponent movement = enemy.GetNode<MovementComponent>("MovementComponent");
		movement.speed *= activeWave.SpeedMultiplier;

		ProjectileAttackComponent projectileAttack = enemy.GetNodeOrNull<ProjectileAttackComponent>("ProjectileAttackComponent");
		if (projectileAttack != null)
		{
			projectileAttack.Damage = Mathf.Max(1, Mathf.RoundToInt(projectileAttack.Damage * activeWave.DamageMultiplier));
		}

		MeleeAttackComponent meleeAttack = enemy.GetNodeOrNull<MeleeAttackComponent>("MeleeAttackComponent");
		if (meleeAttack != null)
		{
			meleeAttack.Damage = Mathf.Max(1, Mathf.RoundToInt(meleeAttack.Damage * activeWave.DamageMultiplier));
		}
	}

	private void OnEnemyDied()
	{
		aliveCount = Mathf.Max(0, aliveCount - 1);
		EmitSignal(SignalName.EnemyCountChanged, aliveCount);
		CheckWaveComplete();
	}

	private void CheckWaveComplete()
	{
		if (!isSpawning && aliveCount <= 0)
		{
			EmitSignal(SignalName.WaveComplete);
		}
	}

	public void ClearAllEnemies()
	{
		isSpawning = false;
		activeWave = null;
		aliveCount = 0;
		spawnedCount = 0;

		foreach (Node child in enemyContainer.GetChildren())
		{
			child.QueueFree();
		}
	}
}
