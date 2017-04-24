using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuActions : MonoBehaviour {
    public CanvasRenderer[] canvases;
    public float fadeDuration;
    public new AudioSource audio;

    private AsyncOperation loadop;

    public void Begin()
    {
        loadop = SceneManager.LoadSceneAsync("Main");
        loadop.allowSceneActivation = false;
        Camera.main.GetComponent<FlyAwayAnimator>().StartCoroutine("BeginAnimation", new System.Action(LevelReady));
        StartCoroutine("FadeOutStuff");
    }

    public void LevelReady()
    {
        loadop.allowSceneActivation = true;
    }

    IEnumerator FadeOutStuff()
    {
        float startTime = Time.time;
        while (Time.time - startTime <= fadeDuration)
        {
            yield return null;
            float x = (Time.time - startTime) / fadeDuration;
            x = 1 - x;
            foreach (CanvasRenderer canvas in canvases)
                canvas.SetAlpha(x);
            audio.volume = x;
        }
    }
}
