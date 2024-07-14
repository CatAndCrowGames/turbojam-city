using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class Tutorial : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    Label label;
    HoverDisplay hoverDisplay;
    WaitForSeconds pause = new WaitForSeconds(10f);

    void Awake()
    {
        label = uiDocument.rootVisualElement.Query<Label>();
        hoverDisplay = FindFirstObjectByType<HoverDisplay>();
    }

    void Start()
    {
        StartCoroutine(TutorialRoutine());
    }

    IEnumerator TutorialRoutine()
    {
        uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        yield return new WaitForSeconds(3);
        MusicPlayer.PlayRandomSongs();
        uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        label.text = "LEFT CLICK to Move";
        yield return pause;
        label.text = "Workers (white dots) power the barrier";
        yield return pause;
        label.text = "RIGHT CLICK to Disrupt - but the cops will hunt you!";
        yield return pause;
        label.text = "Disrupt the paths of workers to turn them into rebels";
        yield return pause;
        label.text = "Rebels (green dots) weaken the barrier";
        yield return pause;
        label.text = "Destroy the barrier to win - Good luck!";
        yield return pause;
        uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }
}

