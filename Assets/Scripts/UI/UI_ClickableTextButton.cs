using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ClickableTextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI textComponent;
    private Color originalColor;
    [SerializeField]public Color hoverColor = Color.HSVToRGB(215, 75, 100); // �趨��ͣʱ����ɫ

    private void Awake()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();//��ȡButton��Text���
        originalColor = textComponent.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        textComponent.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textComponent.color=originalColor;
    }
}
