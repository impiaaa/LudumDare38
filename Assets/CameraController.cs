using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float mouseSensitivity;
    public float joystickSensitivity;
    public float angle;

    private float distance;
    private float height;
    private float pitch;

	void Start () {
        Vector2 overhead = new Vector2(transform.position.x, transform.position.z);
        distance = overhead.magnitude;
        angle = Mathf.Atan2(overhead.y, overhead.x);
        height = transform.position.y;
        pitch = transform.localEulerAngles.x;
        SetPosition();
	}
	
	void Update () {
        if (Input.GetAxis("Fire3") > 0)
        {
            angle += Input.GetAxis("Mouse X") * mouseSensitivity * Mathf.PI;
        }
        angle += Input.GetAxis("Horizontal2") * joystickSensitivity * Mathf.PI;
        angle %= Mathf.PI * 2;
        SetPosition();
	}

    void SetPosition()
    {
        transform.position = new Vector3(Mathf.Cos(angle)*distance, height, Mathf.Sin(angle)*distance);
        transform.rotation = Quaternion.Euler(pitch, -angle* Mathf.Rad2Deg - 90, 0);
    }
}
