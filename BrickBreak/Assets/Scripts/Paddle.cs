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

    private void Start()
    {
        startPosition = paddle.transform.position;
        GameManager.OnReset += OnReset;
        GameManager.OnReady += OnReady;
    }

    private void Update()
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
    }

    private void FixedUpdate()
    {
        if (GameManager.AllowInput)
        {
            float direction = Input.GetAxisRaw("Horizontal");
            if (currentDirection != direction)
            {
                if (direction == 0F)
                {
                    // decelerate
                    movement.Replace(DeceleratePaddle(currentDirection > 0F));
                }
                else
                {
                    // accelerate
                    movement.Replace(AcceleratePaddle(direction > 0F));
                }

                currentDirection = direction;
            }
        }
    }

    // todo: for click movement - debug angle before and after while loop generated from \/
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
        currentAngleVelocity = 0F;
    }

    void SetPaddleFromAngle(float angle)
    {
        float multiplier = Track.TrackSize / 2F;
        paddle.localPosition = multiplier * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0F);
        paddle.localEulerAngles = Vector3.forward * Mathf.Rad2Deg * (angle - Mathf.PI / 2F);
        currentAngleVelocity = angle - currentAngle;
        currentAngle = angle;

        if (isReady)
        {
            // move ball too:
            ball.transform.position = new Vector3(multiplier * Mathf.Cos(angle), multiplier * Mathf.Sin(angle), ball.transform.position.z);
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
        currentDirection = 0F;
    }

    void OnReady()
    {
        ball.transform.position = new Vector3(paddle.position.x, paddle.position.y, ball.StartZ);
        isReady = true;
    }
}