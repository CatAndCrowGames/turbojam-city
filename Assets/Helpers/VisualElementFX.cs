
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class VisualElementFX : MonoBehaviour
{
    void Fade(VisualElement element, float time, float targetOpacity)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(element, time, targetOpacity));
    }

    private IEnumerator FadeRoutine(VisualElement element, float time, float targetOpacity)
    {
        throw new NotImplementedException();
    }

    void FadeOutAndHide()
    {

    }

    void SetHidden(VisualElement element)
    {
        element.style.visibility = Visibility.Hidden;
    }

    void SetVisible(VisualElement element)
    {
        element.style.visibility = Visibility.Visible;
    }
}
