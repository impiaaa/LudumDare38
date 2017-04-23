using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiggerCharacter : MonoBehaviour {
    public float movementThreshold;
    public float movementCooldown;
    public LevelManager level;
    public AnimationCurve jumpAnim;
    public AudioClip jump;
    public AudioClip dig;
    public bool enableInput = true;

    enum Direction { POSX, POSZ, NEGX, NEGZ, NEUTRAL };
    Direction lastDirection = Direction.NEUTRAL;
    Direction facing = Direction.POSX;
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
        if (enableInput)
        {
            if (Input.GetAxis("Fire1") > 0 && transform.localPosition.y > 1)
            {
                DoDig((int)transform.localPosition.x, (int)transform.localPosition.y - 1, (int)transform.localPosition.z);
                DoMove(new Vector3());
            }
            else if (Input.GetAxis("Fire2") > 0)
            {
                int x = (int)transform.localPosition.x, y = (int)transform.localPosition.y, z = (int)transform.localPosition.z;
                switch (facing)
                {
                    case Direction.POSX:
                        x++;
                        break;
                    case Direction.POSZ:
                        z++;
                        break;
                    case Direction.NEGX:
                        x--;
                        break;
                    case Direction.NEGZ:
                        z--;
                        break;
                }
                DoDig(x, y, z);
            }
        }

        if (Time.time - lastMoveTime > movementCooldown && enableInput)
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
                lastDirection = newDirection;
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
                if (newDirection != Direction.NEUTRAL)
                {
                    transform.localRotation = Quaternion.AngleAxis(Mathf.Rad2Deg*Mathf.Atan2(delta.z, delta.x), new Vector3(0, -1, 0)) * initialRotation;
                    facing = newDirection;
                    DoMove(delta);
                }
            }
        }
        else if (transform.localPosition != targetPosition)
        {
            float animTime = (Time.time - lastMoveTime) / movementCooldown;
            Vector3 newpos = lastPosition * (1.0f - animTime) + targetPosition * animTime;
            float jumpHeight = 0.25f+Mathf.Abs(lastPosition.y - targetPosition.y) / 2;
            newpos = new Vector3(newpos.x, jumpAnim.Evaluate(animTime) * jumpHeight + newpos.y, newpos.z);
            transform.localPosition = newpos;
        }
    }

    void DoMove(Vector3 delta)
    {
        Debug.Log("DoMove");
        lastMoveTime = Time.time;
        Vector3 newPos = transform.localPosition + delta;
        int x = (int)newPos.x, y = (int)newPos.y, z = (int)newPos.z;
        // Jump up
        if (level.Collides(x, y, z))
        {
            if (!level.Collides(x, y+1, z) && !level.Collides((int)transform.localPosition.x, y+1, (int)transform.localPosition.z))
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
        if (!level.CanStandOn(x, y - 1, z))
        {
            Debug.Log("Can't fall here");
            return;
        }
        targetPosition = new Vector3(x, y, z);
        if (level.SafeGet(x,y,z) == 'a')
        {
            enableInput = false;
            level.DoLevelEnd();
        }
        else if (targetPosition != transform.localPosition && delta.sqrMagnitude > 0)
        {
            GetComponent<AudioSource>().clip = jump;
            GetComponent<AudioSource>().Play();
        }
    }

    void DoDig(int x, int y, int z)
    {
        if (level.CanDig(x, y, z))
        {
            level.level[x, y, z] = ' ';
            level.UpdateLevel();
            GetComponent<AudioSource>().clip = dig;
            GetComponent<AudioSource>().Play();
        }
    }
}
