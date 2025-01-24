using Godot;

[Tool]
[GlobalClass] 
public partial class Health : Node2D
{
    [Export] public int maxHealth { get; private set; } = 100;

    public int currentHealth { get; private set; }
    public bool invincible;

    [Signal] public delegate void OnChangeHealthEventHandler(int currentHealth);

    [Signal] public delegate void OnTakeDamageEventHandler();

    [Signal] public delegate void OnHealEventHandler();

    [Signal] public delegate void OnDieEventHandler();

    bool died;

    public override void _Ready() {
        base._Ready();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage) {
        if(invincible)
            return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        EmitSignal(SignalName.OnTakeDamage);
        EmitSignal(SignalName.OnChangeHealth, currentHealth);

        if (currentHealth == 0)
            Die();
    }

    public void Heal(int amount) {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        EmitSignal(SignalName.OnHeal);
        EmitSignal(SignalName.OnChangeHealth, currentHealth);
    }

    public void Die() {
        if (died)
            return;
        died = true;
        EmitSignal(SignalName.OnDie);
    }
}