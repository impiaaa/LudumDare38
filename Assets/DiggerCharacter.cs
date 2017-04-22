using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiggerCharacter : MonoBehaviour {
    public float movementThreshold;
    public float movementCooldown;
    public LevelManager level;
    public AnimationCurve jumpAnim;

    enum Direction { POSX, POSZ, NEGX, NEGZ, NEUTRAL };
    Direction lastDirection = Direction.NEUTRAL;
    float lastMoveTime;
    Vector3 targetPosition;
    Vector3 lastPosition;
    Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
        DoMove(new Vector3());
    }

    void Update () {
        if (Time.time - lastMoveTime > movementCooldown)
        {
            transform.localPosition = targetPosition;
            lastPosition = transform.localPosition;

            Vector2 joyPosition = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            float cameraAngle = ((Camera.main.GetComponent<CameraController>().angle * Mathf.Rad2Deg) - 45 + 180)%360;
            joyPosition = (Vector2)(Quaternion.Euler(0, 0, cameraAngle) * (Vector3)(joyPosition));
            Direction newDirection;
            if (joyPosition.sqrMagnitude < movementThreshold * movementThreshold)
            {
                newDirection = Direction.NEUTRAL;
            }
            else
            {
                if (joyPosition.x > 0)
                {
                    if (joyPosition.y > 0)
                    {
                        newDirection = Direction.POSX;
                    }
                    else if (joyPosition.y < 0)
                    {
                        newDirection = Direction.NEGZ;
                    }
                    else
                    {
                        newDirection = Direction.NEGZ;
                    }
                }
                else if (joyPosition.x < 0)
                {
                    if (joyPosition.y > 0)
                    {
                        newDirection = Direction.POSZ;
                    }
                    else if (joyPosition.y < 0)
                    {
                        newDirection = Direction.NEGX;
                    }
                    else
                    {
                        newDirection = Direction.POSZ;
                    }
                }
                else
                {
                    if (joyPosition.y > 0)
                    {
                        newDirection = Direction.POSX;
                    }
                    else if (joyPosition.y < 0)
                    {
                        newDirection = Direction.NEGX;
                    }
                    else
                    {
                        newDirection = Direction.NEUTRAL;
                    }
                }
            }

            if (newDirection != lastDirection)
            {
                Vector3 delta;
                switch (newDirection)
                {
                    case Direction.NEGX:
                        delta = new Vector3(-1, 0, 0);
                        break;
                    case Direction.POSX:
                        delta = new Vector3(1, 0, 0);
                        break;
                    case Direction.NEGZ:
                        delta = new Vector3(0, 0, -1);
                        break;
                    case Direction.POSZ:
                        delta = new Vector3(0, 0, 1);
                        break;
                    default:
                        delta = new Vector3();
                        break;
                }
                lastDirection = newDirection;
                if (delta.sqrMagnitude > 0)
                {
                    transform.localRotation = Quaternion.AngleAxis(Mathf.Rad2Deg*Mathf.Atan2(delta.z, delta.x), new Vector3(0, -1, 0)) * initialRotation;
                }

                DoMove(delta);
            }
        }
        else
        {
            float animTime = (Time.time - lastMoveTime) / movementCooldown;
            Vector3 newpos = lastPosition * (1.0f - animTime) + targetPosition * animTime;
            newpos = new Vector3(newpos.x, jumpAnim.Evaluate(animTime) * Mathf.Abs(lastPosition.y - targetPosition.y)/2 + newpos.y, newpos.z);
            transform.localPosition = newpos;
        }
    }

    void DoMove(Vector3 delta)
    {
        lastMoveTime = Time.time;
        Vector3 newPos = transform.localPosition + delta;
        int x = (int)newPos.x, y = (int)newPos.y, z = (int)newPos.z;
        // Jump up
        if (level.Collides(x, y, z))
        {
            if (!level.Collides(x, y+1, z))
            {
                Debug.Log("Jump up");
                y++;
            }
            else
            {
                Debug.Log("Can't jump up");
                return;
            }
        }
        // Fall down
        while (!level.Collides(x, y-1, z))
        {
            Debug.Log("Fall down");
            y--;
            if (y <= 0)
            {
                Debug.Log("Fell off level");
                return;
            }
        }
        targetPosition = new Vector3(x, y, z);
        if (targetPosition != transform.localPosition && delta.sqrMagnitude > 0)
        {
            GetComponent<AudioSource>().Play();
        }
    }
}
