public readonly struct AggregatedSpellStats
{
	public float ScaleMultiplier { get; init; } = 1f;
	public float SpeedMultiplier { get; init; } = 1f;
	public float CritChanceBonus { get; init; }
	public string DamageType { get; init; } = "";
	public int ElementalDamage { get; init; }

	public AggregatedSpellStats() { }
}
