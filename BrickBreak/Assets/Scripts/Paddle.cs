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


    float currentAngle = Mathf.PI * 3F / 2F;
    float currentDirection = 0F;

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

            if (forward) currentAngle += curveTime;
            else currentAngle -= curveTime;
            SetPaddleFromAngle(currentAngle);

            yield return new WaitForFixedUpdate();
            time += Time.deltaTime;
        }

        movement.Replace(MovePaddle(forward));
    }

    IEnumerator MovePaddle(bool forward)
    {
        while (true)
        {
            if (forward) currentAngle += defaultSpeed;
            else currentAngle -= defaultSpeed;
            SetPaddleFromAngle(currentAngle);

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator DeceleratePaddle(bool forward)
    {
        float time = 0F;
        while (time < accelTime)
        {
            float curveTime = decelCurve.Evaluate(time / accelTime) * defaultSpeed;

            if (forward) currentAngle += curveTime;
            else currentAngle -= curveTime;
            SetPaddleFromAngle(currentAngle);

            yield return new WaitForFixedUpdate();
            time += Time.deltaTime;
        }
    }

    void SetPaddleFromAngle(float angle)
    {
        float multiplier = Track.TrackSize / 2F;
        paddle.localPosition = multiplier * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0F);
        paddle.localEulerAngles = Vector3.forward * Mathf.Rad2Deg * (angle - Mathf.PI / 2F);
    }
}