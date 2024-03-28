using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleCollision : MonoBehaviour
{
    [SerializeField]
    float forceAmount = 10F;

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Ball"))
        //{
        //    collision.rigidbody.AddForce(Vector3.forward * forceAmount, ForceMode.Impulse);
        //}
    }
}
