/// <summary>
/// ʵ����԰�HealthUI
/// </summary>
public interface IHasHealthUI
{
    void BindHealthUI(HealthUI healthUI);
    void UpdateHealthUI(float current, float max);
}