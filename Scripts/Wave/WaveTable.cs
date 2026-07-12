using Godot;

public static class WaveTable
{
	public static WaveDefinition GetWave(int waveNumber)
	{
		int wave = Mathf.Max(1, waveNumber);

		return new WaveDefinition
		{
			WaveNumber = wave,
			EnemyCount = 3 + wave * 2,
			SpawnInterval = Mathf.Max(0.35f, 1.1f - wave * 0.07f),
			ChargerWeight = wave >= 3 ? Mathf.Clamp(0.25f + (wave - 3) * 0.06f, 0f, 0.75f) : 0f,
			HealthMultiplier = 1f + (wave - 1) * 0.12f,
			SpeedMultiplier = 1f + (wave - 1) * 0.04f,
			DamageMultiplier = 1f + (wave - 1) * 0.08f,
		};
	}
}
