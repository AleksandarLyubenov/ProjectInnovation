using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class PanelAnimator : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private AnimationCurve movementCurve;

    private RectTransform rectTransform;
    private Vector2 onScreenPos;
    private Vector2 offScreenPos;
    private float canvasHeight;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasHeight = GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect.height;

        onScreenPos = rectTransform.anchoredPosition;
        offScreenPos = new Vector2(onScreenPos.x, -canvasHeight - rectTransform.rect.height);
        rectTransform.anchoredPosition = offScreenPos;
    }

    public void ShowPanel() => StartCoroutine(Animate(offScreenPos, onScreenPos));
    public void HidePanel() => StartCoroutine(Animate(onScreenPos, offScreenPos));

    IEnumerator Animate(Vector2 start, Vector2 end)
    {
        float elapsed = 0;
        rectTransform.anchoredPosition = start;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(
                start,
                end,
                movementCurve.Evaluate(elapsed / animationDuration)
            );
            yield return null;
        }

        rectTransform.anchoredPosition = end;
    }
}