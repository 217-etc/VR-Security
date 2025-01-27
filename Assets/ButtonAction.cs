using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Color originalColor;
    public Color hoverColor = Color.yellow; // ���콺 ���� ����
    public Color clickColor = Color.red;     // Ŭ�� ����

    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color; // ���� ���� ����
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.color = hoverColor; // ���콺 ���� �� ���� ����
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.color = originalColor; // ���콺�� ������ ���� �������� ����
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonImage.color = clickColor; // Ŭ�� �� ���� ����
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonImage.color = hoverColor; // Ŭ�� �� ���콺 ���� �������� ����
    }
}
