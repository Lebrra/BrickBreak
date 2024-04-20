using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // the standard (x, y) movement here is mapped to (angle, z) where angle will be transformed to (x,y) based on unit circle * Track.TrackSize/2

    public static Action RecalculateBallVelocity;

    float defaultSpeed => GameProperties.BallSpeed;

    // in respect to (angle, z)
    Vector2 velocity = new Vector2(0F, 0F);

    Vector3 startPosition;
    public float StartZ => startPosition.z;

    private void Start()
    {
        startPosition = transform.position;
        GameManager.OnReset += OnReset;
        GameManager.OnGameOver += OnLose;
        RecalculateBallVelocity += RecalculateVelocity;
    }

    private void FixedUpdate()
    {
        var lastPos = transform.position;

        // need to update velocity every frame with respect to current velocity.x angle (or simulate all of this and don't use rb)
        var normal = lastPos.normalized;
        var currentAngle = Mathf.Atan2(normal.y, normal.x);
        var futureAngle = currentAngle + velocity.x;

        transform.position = new Vector3(velocity.x == 0 ? lastPos.x : GameProperties.TrackSize/2F * Mathf.Cos(futureAngle),
            velocity.x == 0 ? lastPos.y : GameProperties.TrackSize/2F * Mathf.Sin(futureAngle),
            lastPos.z + velocity.y);

        if (transform.position.z < -5F && GameManager.AllowInput)
            GameManager.OnBallLost?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!GameManager.AllowInput) return;

        var contact = collision.GetContact(0).point;
        if (collision.gameObject.CompareTag("Paddle"))
        {
            AudioManager.PlayClip?.Invoke("paddleBonk");
            var newVel = collision.gameObject.GetComponent<Paddle>()?.GenerateBallVelocity(contact) ?? Vector2.zero;
            if (newVel == Vector2.zero) Debug.LogError("Error finding Paddle script!");
            else if (newVel == Vector2.one)
            {
                // keyword to just flip y (z) - or we will just ensure y (z) is positive here:
                // (also see if x needs flipping)
                var normalAbs = (contact - transform.position).normalized;
                Debug.DrawRay(contact, normalAbs, Color.red, 10F);
                normalAbs = new Vector3(Mathf.Abs(normalAbs.x), Mathf.Abs(normalAbs.y), Mathf.Abs(normalAbs.z));

                velocity = new Vector2(normalAbs.z < 0.3F ? -velocity.x : velocity.x, Mathf.Abs(velocity.y));
            }
            else SetVelocity(newVel);
        }
        else
        {
            var normalAbs = (contact - transform.position).normalized;
            Debug.DrawRay(contact, normalAbs, Color.red, 10F);
            normalAbs = new Vector3(Mathf.Abs(normalAbs.x), Mathf.Abs(normalAbs.y), Mathf.Abs(normalAbs.z));

            // todo: possibly add threshold
            if (normalAbs.z >= 0.3F)
            {
                // flip 'y' => z 
                velocity = new Vector2(velocity.x, -velocity.y);

            }
            if ((normalAbs.x + normalAbs.y) >= 0.6F)
            {
                // flip 'x' => angle => unit values of x/y
                velocity = new Vector2(-velocity.x, velocity.y);
            }

            if (collision.gameObject.CompareTag("Brick"))
            {
                var brick = collision.gameObject.GetComponentsInParent<Brick>()[0] ?? null;
                if (brick) brick.HitBrick();
            }
            else AudioManager.PlayClip?.Invoke("backBonk");
        }

        StatsManager.AddBounce?.Invoke();
    }

    // normalized!!
    // (0, 1) == straight 'vertically'
    // (1, 0) == straight 'horizontally' (hopefully never)
    public void SetVelocity(float x, float y)
    {
        var multiplier = (GameProperties.TrackSize * 2F) / (Mathf.PI * 2F);
        velocity = new Vector2(x, y * multiplier) * defaultSpeed;
    }

    public void SetVelocity(Vector2 coord)
    {
        SetVelocity(coord.x, coord.y);
    }

    // ball speed was updated - recalculate
    public void RecalculateVelocity()
    {
        SetVelocity(velocity.normalized);
    }

    void OnReset()
    {
        if (GameManager.InGame)
        {
            SetVelocity(Vector2.zero);
            if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
        }
        else gameObject.SetActive(false);
    }

    void OnLose()
    {
        gameObject.SetActive(false);
    }
}
