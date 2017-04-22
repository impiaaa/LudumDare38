using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanAnimator : MonoBehaviour {
    public float speed;
    public float rotationMagnitude;
    public float heightMagnitude;
    public Vector3 rotationAxis;
    public float phaseOffset;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

	void Update () {
        transform.localPosition = initialPosition + new Vector3(0, Mathf.Sin(Time.time * speed * Mathf.PI) * heightMagnitude, 0);
        transform.localRotation = Quaternion.AngleAxis(Mathf.Sin((Time.time * speed + phaseOffset) * Mathf.PI) * rotationMagnitude * Mathf.PI, rotationAxis) * initialRotation;
	}
}
