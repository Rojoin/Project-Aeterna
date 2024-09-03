public interface IHealthSystem 
{
    public void SetHealth(float newHealth);
    public float GetHealth();
    public void SetMaxHealth(float newMaxHealth);
    public void ReceiveDamage(float damage);
    public bool IsDead();
}