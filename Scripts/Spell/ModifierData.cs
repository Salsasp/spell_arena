
[System.Serializable]
public class ModifierData
{
    public string id;
    public string name;
    public string description;
    public string icon;
    public string rarity;
    public string damageType;
    public float dotDamage;
    public float critAdditive;
    public float critMultiplicative;
    public string onHit;
    public string orbital;
    public string onSpawn;
    public string onAcquire;
    public string uiOnHit;
    public bool isStackable;
    public int copies = 1;
    public ModifierAffinity affinity = ModifierAffinity.Neutral;

    /// <summary>
    /// Shallow copy. Every field is a value type or an immutable string, so this is safe.
    /// Used wherever <see cref="copies"/> has to be mutated without writing back to the
    /// single shared instance handed out by ModifierCatalog.
    /// </summary>
    public ModifierData Clone()
    {
        return (ModifierData)MemberwiseClone();
    }

    public bool IsRunScoped =>
        !string.IsNullOrEmpty(onAcquire) ||
        (!string.IsNullOrEmpty(uiOnHit) &&
         string.IsNullOrEmpty(onHit) &&
         string.IsNullOrEmpty(onSpawn) &&
         string.IsNullOrEmpty(orbital));
    public override string ToString()
    {
        return $"[{id}] {name} ({rarity}) - {description}\n" +
            $"Damage: {damageType} | Crit: +{critAdditive} x{critMultiplicative}\n" +
            $"Events: onHit={onHit}, onSpawn={onSpawn}, onAcquire={onAcquire}, uiOnHit={uiOnHit}, orbital={orbital}";
    }
}