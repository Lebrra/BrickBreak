using System.Collections;
using UnityEngine;
using BeauRoutine;

public class Paddle : MonoBehaviour
{
    [SerializeField]
    Transform paddle;
    [SerializeField]
    Ball ball;

    [SerializeField]
    float accelTime = 0.2F;
    [SerializeField]
    AnimationCurve accelCurve;
    [SerializeField]
    AnimationCurve decelCurve;

    [SerializeField]
    float defaultSpeed = 0.02F;
    [SerializeField]
    float edgeAngle = 0.2F;

    bool isReady = true;
    Vector3 startPosition;

    float currentAngle = Mathf.PI * 3F / 2F;
    float currentDirection = 0F;
    float currentAngleVelocity = 0F;

    Routine movement;
    Routine mouseMovement;

    private void Start()
    {
        startPosition = paddle.transform.position;
        GameManager.OnReset += OnReset;
        GameManager.OnReady += OnReady;
    }

    private void Update()
    {
        if (GameManager.AllowInput)
        {
            if (isReady)
            {
                if (Input.GetButtonDown("Fire"))
                {
                    isReady = false;
                    ball.transform.SetParent(transform.parent);
                    var ballVel = GenerateBallVelocity(paddle.transform.position);
                    ball.SetVelocity(ballVel == Vector2.one ? Vector2.up : ballVel);
                }
            }

            // mouse movement:
            if (Input.GetMouseButtonDown(0))
            {
                var norm = (new Vector2(Input.mousePosition.x - (Screen.width / 2F), Input.mousePosition.y - (Screen.height / 2F))).normalized;
                var angle = Mathf.Atan2(norm.y, norm.x);

                if (movement.Exists()) movement.Stop(); 
                mouseMovement.Replace(MovePaddleMouse(angle));
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.AllowInput)
        {
            float direction = Input.GetAxisRaw("Horizontal");
            if (currentDirection != direction)
            {
                if (direction == 0F && !mouseMovement.Exists())
                {
                    // decelerate
                    movement.Replace(DeceleratePaddle(currentDirection > 0F));
                }
                else if (direction != 0F)
                {
                    // accelerate
                    if (mouseMovement.Exists()) mouseMovement.Stop();
                    movement.Replace(AcceleratePaddle(direction > 0F));
                }

                currentDirection = direction;
            }
        }
    }

    IEnumerator AcceleratePaddle(bool forward)
    {
        float time = 0F;
        while (time < accelTime)
        {
            float curveTime = accelCurve.Evaluate(time / accelTime) * defaultSpeed;

            if (forward) SetPaddleFromAngle(currentAngle + curveTime);
            else SetPaddleFromAngle(currentAngle - curveTime);

            yield return new WaitForFixedUpdate();
            time += Time.deltaTime;
        }

        movement.Replace(MovePaddle(forward));
    }

    IEnumerator MovePaddle(bool forward)
    {
        while (true)
        {
            if (forward) SetPaddleFromAngle(currentAngle + defaultSpeed);
            else SetPaddleFromAngle(currentAngle - defaultSpeed);

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator DeceleratePaddle(bool forward)
    {
        float time = 0F;
        while (time < accelTime)
        {
            float curveTime = decelCurve.Evaluate(time / accelTime) * defaultSpeed;

            if (forward) SetPaddleFromAngle(currentAngle + curveTime);
            else SetPaddleFromAngle(currentAngle - curveTime);


            yield return new WaitForFixedUpdate();
            time += Time.deltaTime;
        }
        currentAngleVelocity = currentDirection = 0F;
    }

    IEnumerator MovePaddleMouse(float endAngle)
    {
        float distance = Mathf.DeltaAngle(currentAngle * Mathf.Rad2Deg, endAngle * Mathf.Rad2Deg) * Mathf.Deg2Rad;
        if (Mathf.Abs(distance) <= 0.001F)
        {
            // don't move
            currentDirection = 0F;
            yield break;
        }
        else if (distance > 0F)
            currentDirection = 1F;
        else 
            currentDirection = -1F;

        //endAngle = currentAngle + distance;
        var direction = currentDirection > 0F;
        var accelThreshold = (accelTime * 2.4F);

        // if disposition is less than accel + decel, just move the whole way slow: (with a small buffer in there
        if (Mathf.Abs(distance) <= accelThreshold)
        {
            var diff = defaultSpeed / 2F;
            if (direction)
            {
                while (distance > 0F)
                {
                    if (distance < diff) 
                        diff = distance;
                    distance -= diff;
                    SetPaddleFromAngle(currentAngle + diff);
                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                while (distance < 0F)
                {
                    if (Mathf.Abs(distance) < diff) 
                        diff = Mathf.Abs(distance);
                    distance += diff;
                    SetPaddleFromAngle(currentAngle - diff);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        // else do the whole thing:
        else
        {
            float time = 0F;

            // accel (if we are already going this speed then skip):
            if (currentAngleVelocity == 0F || (currentAngleVelocity != 0 && Mathf.Sign(currentAngleVelocity) != Mathf.Sign(distance)))
            {
                while (time < accelTime)
                {
                    float curveTime = accelCurve.Evaluate(time / accelTime) * defaultSpeed;

                    if (direction)
                    {
                        distance -= curveTime;
                        SetPaddleFromAngle(currentAngle + curveTime);
                    }
                    else
                    {
                        distance += curveTime;
                        SetPaddleFromAngle(currentAngle - curveTime);
                    }

                    yield return new WaitForFixedUpdate();
                    time += Time.deltaTime;
                }
            }

            // constant:
            var diff = defaultSpeed;
            if (direction)
            {
                while (distance > accelThreshold)
                {
                    distance -= diff;
                    SetPaddleFromAngle(currentAngle + diff);
                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                while (distance < -accelThreshold)
                {
                    distance += diff;
                    SetPaddleFromAngle(currentAngle - diff);
                    yield return new WaitForFixedUpdate();
                }
            }

            // decel:
            time = 0F;
            while (time < accelTime)
            {
                float curveTime = decelCurve.Evaluate(time / accelTime) * defaultSpeed;

                if (direction) SetPaddleFromAngle(currentAngle + curveTime);
                else SetPaddleFromAngle(currentAngle - curveTime);


                yield return new WaitForFixedUpdate();
                time += Time.deltaTime;
            }
        }

        currentAngleVelocity = currentDirection = 0F;
    }

    void SetPaddleFromAngle(float angle)
    {
        paddle.localPosition = Utility.GetWorldPosition(new Vector2(angle, 0F));
        paddle.localEulerAngles = Utility.GetWorldRotation(angle);
        currentAngleVelocity = angle - currentAngle;
        currentAngle = angle;

        if (isReady)
        {
            // move ball too:
            ball.transform.position = Utility.GetWorldPosition(new Vector2(angle, ball.transform.position.z));
        }
    }

    /// <summary>
    /// Using the current speed of the paddle & the point of contact, generate a new normalized velocity for the ball
    /// </summary>
    public Vector2 GenerateBallVelocity(Vector2 contact)
    {
        // 40% will be impacted by the speed of the paddle:
        var horizontalImact = (currentAngleVelocity / defaultSpeed) * 0.4F;

        if (horizontalImact == 0F)
        {
            // if we aren't moving, let's just bounce off instead because that makes more sense:
            return Vector2.one;     // one will be the keyword for this - its not possible with my math
        }
        else
        {
            // 40% will be impacted by the position hit on the paddle: (converted to angles because math)
            var contactAngle = Mathf.Atan2(contact.normalized.y, contact.normalized.x);
            var paddleAngle = Mathf.Atan2(paddle.position.normalized.y, paddle.position.normalized.x);
            float difference = contactAngle - paddleAngle;
            // [-0.2, 0.2] == [left edge, right edge]

            horizontalImact += ((difference / edgeAngle) * 0.4F);

            // note: y (z) will always be positive => y = 1 - Mathf.Abs(x)
            return new Vector2(horizontalImact, 1F - Mathf.Abs(horizontalImact));
        }
    }

    void OnReset()
    {
        if (movement.Exists()) movement.Stop();
        if (mouseMovement.Exists()) mouseMovement.Stop();
        currentDirection = 0F;
    }

    void OnReady()
    {
        ball.transform.position = new Vector3(paddle.position.x, paddle.position.y, ball.StartZ);
        isReady = true;
    }
}