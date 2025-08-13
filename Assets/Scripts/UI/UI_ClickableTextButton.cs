using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ClickableTextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI textComponent;
    private Color originalColor;
    [SerializeField]public Color hoverColor = Color.HSVToRGB(215, 75, 100); // 设定悬停时的颜色

    private void Awake()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();//获取Button的Text组件
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
