public enum ModifierAffinity
{
	Neutral,
	Lightning,
	Fire,
	Ice
}

public static class ModifierAffinityParser
{
	public static ModifierAffinity Parse(string value)
	{
		return value?.Trim().ToLowerInvariant() switch
		{
			"lightning" => ModifierAffinity.Lightning,
			"fire" => ModifierAffinity.Fire,
			"ice" => ModifierAffinity.Ice,
			_ => ModifierAffinity.Neutral
		};
	}
}
