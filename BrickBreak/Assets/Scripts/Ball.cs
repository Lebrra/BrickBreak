using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class Ball : MonoBehaviour
{
    // the standard (x, y) movement here is mapped to (angle, z) where angle will be transformed to (x,y) based on unit circle * Track.TrackSize/2

    [SerializeField]
    float defaultSpeed = 1.2F;

    [SerializeField]
    bool horizontal = false;

    // in respect to (angle, z)
    Vector2 velocity = new Vector2(0F, 0F);


    private void Start()
    {
        if (!horizontal) SetVelocity(0, 1);
        else SetVelocity(1, 0);
    }

    private void FixedUpdate()
    {
        var lastPos = transform.position;

        // need to update velocity every frame with respect to current velocity.x angle (or simulate all of this and don't use rb)
        var normal = lastPos.normalized;
        var currentAngle = Mathf.Atan2(normal.y, normal.x);
        var futureAngle = currentAngle + velocity.x;

        transform.position = new Vector3(velocity.x == 0 ? lastPos.x : Track.TrackSize/2F * Mathf.Cos(futureAngle),
            velocity.x == 0 ? lastPos.y : Track.TrackSize/2F * Mathf.Sin(futureAngle),
            lastPos.z + velocity.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var contact = collision.GetContact(0).point;
        var normalAbs = (contact - transform.position).normalized;
        normalAbs = new Vector3(Mathf.Abs(normalAbs.x), Mathf.Abs(normalAbs.y), Mathf.Abs(normalAbs.z));

        // todo: possibly add threshold
        if (normalAbs.z >= (normalAbs.x + normalAbs.y))
        {
            // flip 'y' => z 
            velocity = new Vector2(velocity.x, -velocity.y);

        }
        if (normalAbs.z <= (normalAbs.x + normalAbs.y))
        {
            // flip 'x' => angle => unit values of x/y
            velocity = new Vector2(-velocity.x, velocity.y);
        }
    }

    // normalized!!
    // (0, 1) == straight 'vertically'
    // (1, 0) == straight 'horizontally' (hopefully never)
    public void SetVelocity(float x, float y)
    {
        var multiplier = (Track.TrackSize * 2F) / (Mathf.PI * 2F);
        velocity = new Vector2(x, y * multiplier) * defaultSpeed;
    }
}
