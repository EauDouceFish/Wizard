using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AttributeItem : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI attributeValueText;

    private void Start()
    {
        if (icon == null) icon = GetComponentInChildren<Image>();
        if (attributeValueText == null) attributeValueText = GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>
    /// ���ù�����
    /// </summary>
    public void SetAttack(int attack)
    {
        if (attributeValueText != null)
        {
            attributeValueText.text = attack.ToString();
        }
    }

    /// <summary>
    /// ��ǰѪ��/���Ѫ��
    /// </summary>
    public void SetHealth(float currentHealth, float maxHealth)
    {
        if (attributeValueText != null)
        {
            attributeValueText.text = $"{Mathf.FloorToInt(currentHealth)}/{Mathf.FloorToInt(maxHealth)}";
        }
    }
}