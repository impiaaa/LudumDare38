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
        initialRotation = transform.rotation;
        DoMove(new Vector3());
    }

    void Update () {
        if (Time.time - lastMoveTime > movementCooldown)
        {
            transform.position = targetPosition;
            lastPosition = transform.position;

            Vector2 joyPosition = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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
                    transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg*Mathf.Atan2(delta.z, delta.x), new Vector3(0, 1, 0)) * initialRotation;
                }

                DoMove(delta);
            }
        }
        else
        {
            float animTime = (Time.time - lastMoveTime) / movementCooldown;
            Vector3 newpos = lastPosition * (1.0f - animTime) + targetPosition * animTime;
            newpos = new Vector3(newpos.x, jumpAnim.Evaluate(animTime) * Mathf.Abs(lastPosition.y - targetPosition.y)/2 + newpos.y, newpos.z);
            transform.position = newpos;
        }
    }

    void DoMove(Vector3 delta)
    {
        lastMoveTime = Time.time;
        Vector3 newPos = transform.position + delta;
        int x = (int)newPos.x, y = (int)newPos.y, z = (int)newPos.z;
        // raise up
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
        while (!level.Collides(x, y-1, z) && y > 0)
        {
            Debug.Log("Fall down");
            y--;
        }
        targetPosition = new Vector3(x, y, z);
        if (targetPosition != transform.position)
        {
            GetComponent<AudioSource>().Play();
        }
    }
}
