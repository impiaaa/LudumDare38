using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCharacter : MonoBehaviour {
    public AnimationCurve bounce;
    public float bounceHeight;
    public float bounceSpeed;
    public float offset;
    public bool visible;

    GameObject character;

	void Start () {
        character = GameObject.FindGameObjectWithTag("Player");
        foreach (CanvasRenderer canvas in GetComponentsInChildren<CanvasRenderer>())
        {
            canvas.SetAlpha(0);
        }
        visible = false;
    }
	
	void Update () {
        Vector2 pos = Camera.main.WorldToScreenPoint(character.transform.position);
        pos += new Vector2(0, bounce.Evaluate(Time.time*bounceSpeed)*bounceHeight + offset);
        GetComponent<RectTransform>().position = pos;
	}

    public IEnumerator FadeOut(float fadeDuration)
    {
        float startTime = Time.time;
        while (Time.time - startTime <= fadeDuration)
        {
            yield return null;
            float x = (Time.time - startTime) / fadeDuration;
            x = 1 - x;
            foreach (CanvasRenderer canvas in GetComponentsInChildren<CanvasRenderer>())
            {
                canvas.SetAlpha(x);
            }
        }
        visible = false;
    }

    public IEnumerator FadeIn(float fadeDuration)
    {
        Debug.Log("FadeIn");
        while (character == null || !character.GetComponent<DiggerCharacter>().enableInput)
        {
            Debug.Log("Waiting for player to be ready");
            yield return new WaitForSeconds(1.0f);
        }
        Debug.Log("FadeIn start");
        float startTime = Time.time;
        while (Time.time - startTime <= fadeDuration)
        {
            yield return null;
            float x = (Time.time - startTime) / fadeDuration;
            foreach (CanvasRenderer canvas in GetComponentsInChildren<CanvasRenderer>())
            {
                canvas.SetAlpha(x);
            }
        }
        visible = true;
        Debug.Log("FadeIn end");
    }
}
