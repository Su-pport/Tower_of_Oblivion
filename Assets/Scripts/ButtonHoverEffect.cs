using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Fill Panel")]
    [SerializeField] private GameObject fillPanel;

    [Header("Text")]
    [SerializeField] private TMP_Text buttonText;

    [Header("Colors")]
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color hoverTextColor = Color.black;

    [Header("Scale")]
    [SerializeField] private float hoverScale = 1.05f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;

        // 초기 상태
        if (fillPanel != null)
            fillPanel.SetActive(false);

        if (buttonText != null)
            buttonText.color = normalTextColor;
    }

    // 마우스 올라감
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (fillPanel != null)
            fillPanel.SetActive(true);

        if (buttonText != null)
            buttonText.color = hoverTextColor;

        //커지기
        transform.localScale = originalScale * hoverScale;
    }

    // 마우스 벗어남
    public void OnPointerExit(PointerEventData eventData)
    {
        if (fillPanel != null)
            fillPanel.SetActive(false);

        if (buttonText != null)
            buttonText.color = normalTextColor;
        
        //원래 크기로
        transform.localScale = originalScale;
    }

    private void OnDisable()
    {
        //Fill 끄기
        fillPanel.SetActive(false);

        //텍스트 색 원래대로
        buttonText.color = normalTextColor;

        //스케일 원래대로
        transform.localScale = originalScale;
    }
}


