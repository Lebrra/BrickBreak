using System.Collections;
using UnityEngine;
using BeauRoutine;

public class Paddle : MonoBehaviour
{
    [SerializeField]
    Transform paddle;

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


    float currentAngle = Mathf.PI * 3F / 2F;
    float currentDirection = 0F;
    float currentAngleVelocity = 0F;

    Routine movement;

    private void FixedUpdate()
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
    }

    /// <summary>
    /// Using the current speed of the paddle & the point of contact, generate a new normalized velocity for the ball
    /// </summary>
    public Vector2 GenerateBallVelocity(Vector2 contact)
    {
        // 50% will be impacted by the speed of the paddle:
        var horizontalImact = (currentAngleVelocity / defaultSpeed) * 0.5F;

        // 50% will be impacted by the position hit on the paddle: (converted to angles because math)
        var contactAngle = Mathf.Atan2(contact.normalized.y, contact.normalized.x);
        var paddleAngle = Mathf.Atan2(paddle.position.normalized.y, paddle.position.normalized.x);
        float difference = contactAngle - paddleAngle;
        // [-0.2, 0.2] == [left edge, right edge]

        horizontalImact += ((difference / edgeAngle) * 0.5F);

        // note: y (z) will always be positive => y = 1 - Mathf.Abs(x)
        return new Vector2(horizontalImact, 1F - Mathf.Abs(horizontalImact));
    }
}