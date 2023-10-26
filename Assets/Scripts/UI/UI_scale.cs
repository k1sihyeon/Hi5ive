using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_scale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scaleFactor = 1.3f; // 마우스 오버 시 크기 변화 비율
    public float shrinkFactor = 0.7f; // 마우스 벗어날 시 크기 변화 비율
    public float smoothSpeed = 10f;   // 크기 변화 속도
    public Image check;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private bool isMouseOver;

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * scaleFactor;
    }

    private void Update()
    {
        Vector3 target = isMouseOver ? targetScale : originalScale;
        Vector3 newScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * smoothSpeed);
        transform.localScale = newScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        check.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        check.enabled = false;
        Shrink();
    }

    private void Shrink()
    {
        Vector3 newScale = Vector3.Lerp(transform.localScale, originalScale * shrinkFactor, Time.deltaTime * smoothSpeed);
        transform.localScale = newScale;
    }
}