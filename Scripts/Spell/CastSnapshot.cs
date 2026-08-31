using System.Collections.Generic;

public sealed class CastSnapshot
{
	public List<ModifierData> Modifiers { get; init; } = [];
	public AggregatedSpellStats WandStats { get; init; } = new();
}
