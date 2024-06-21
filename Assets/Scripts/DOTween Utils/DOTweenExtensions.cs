using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public static class DOTweenExtensions
{
    public static Tweener DOFade(this VisualElement target, float endValue, float duration)
    {
        return DOTween.To(() => (float)target.style.opacity.value, x => target.style.opacity = new StyleFloat(x), endValue, duration);
    }

    public static Tweener DOShake(this VisualElement target, float duration, float strength = 10f, int vibrato = 10, float randomness = 90f)
    {
        Vector3 originalPosition = target.transform.position;

        return DOTween.Shake(() => target.transform.position, 
                             x => target.transform.position = x, 
                             duration, 
                             strength, 
                             vibrato, 
                             randomness)
                      .OnComplete(() => target.transform.position = originalPosition); // Ensure the element returns to its original position
    }
}