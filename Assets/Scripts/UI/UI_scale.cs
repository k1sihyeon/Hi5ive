using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_scale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scaleFactor = 1.3f; // ���콺 ���� �� ũ�� ��ȭ ����
    public float shrinkFactor = 0.7f; // ���콺 ��� �� ũ�� ��ȭ ����
    public float smoothSpeed = 10f;   // ũ�� ��ȭ �ӵ�
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