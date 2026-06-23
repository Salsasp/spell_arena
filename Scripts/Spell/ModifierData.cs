
[System.Serializable]
public class ModifierData
{
    public string id;
    public string name;
    public string description;
    public string icon;
    public string rarity;
    public string damageType;
    public float critAdditive;
    public float critMultiplicative;
    public string onHit;
    public string orbital;
    public string onSpawn;
    public string onDespawn;

    public override string ToString()
    {
        return $"[{id}] {name} ({rarity}) - {description}\n" +
            $"Damage: {damageType} | Crit: +{critAdditive} x{critMultiplicative}\n" +
            $"Events: onHit={onHit}, onSpawn={onSpawn}, onDespawn={onDespawn}, orbital={orbital}";
    }
}

[System.Serializable]
public class ModifierDatabase
{
    public ModifierData[] modifiers;
}