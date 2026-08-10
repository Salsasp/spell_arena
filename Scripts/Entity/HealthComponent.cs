using Godot;

public partial class HealthComponent : Component
{
    [Export] public int MaxHealth = 100;
    private int currentHealth;

    [Signal] public delegate void HealthChangedEventHandler(int newHealth);

    [Signal] public delegate void DeadEventHandler();

    public override void _Ready()
    {
        base._Ready();
        currentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        EmitSignal(SignalName.HealthChanged, currentHealth);
        //GD.Print($"{owner.Name} took {damage} damage. HP: {currentHealth}/{MaxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
        EmitSignal(SignalName.HealthChanged, currentHealth);
    }

    public void Die()
    {
        EmitSignal(SignalName.Dead);
        owner.QueueFree();
    }

    public int GetCurrentHealth() => currentHealth;
}