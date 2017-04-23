using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAwayAnimator : MonoBehaviour {
    public float transitionHeight;
    public float transitionTime;
    public AnimationCurve transitionCurve;
    public bool reverse = false;

    IEnumerator BeginAnimation (System.Action callback) {
        float startTime = Time.time;
        float initialHeight = transform.position.y;

        for (; (Time.time - startTime) <= transitionTime;)
        {
            float time = (Time.time - startTime) / transitionTime;
            if (reverse)
            {
                time = 1.0f - time;
            }
            transform.localPosition = new Vector3(transform.localPosition.x, transitionCurve.Evaluate(time) * transitionHeight + initialHeight, transform.localPosition.z);
            yield return null;
        }

        callback();
	}
}
