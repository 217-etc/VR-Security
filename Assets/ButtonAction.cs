using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Color originalColor;
    public Color hoverColor = Color.yellow; // 마우스 오버 색상
    public Color clickColor = Color.red;     // 클릭 색상

    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color; // 원래 색상 저장
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.color = hoverColor; // 마우스 오버 시 색상 변경
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.color = originalColor; // 마우스가 나가면 원래 색상으로 복원
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonImage.color = clickColor; // 클릭 시 색상 변경
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonImage.color = hoverColor; // 클릭 후 마우스 오버 색상으로 변경
    }
}
