public interface IHealthSystem 
{
    public void SetHealth(float newHealth);
    public void SetMaxHealigh(float newMaxHealth);
    public void ReceiveDamage(float damage);
    public bool IsDead();
}