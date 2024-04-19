using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRotator : MonoBehaviour
{
    public static Action<float> SetLightRotation;

    [SerializeField]
    float radius = 3F;

    private void Awake()
    {
        SetLightRotation += SetRotation;
    }

    void SetRotation(float angle)
    {
        Vector3 norms = Utility.GetWorldPosition(new Vector2(angle, 0F)).normalized;
        transform.localEulerAngles = norms * radius;
    }
}
