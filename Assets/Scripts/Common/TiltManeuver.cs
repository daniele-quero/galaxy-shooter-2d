using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltManeuver : MonoBehaviour
{
    [SerializeField]
    private float _angle = 35f;

    public void Tilt(float input)
    {
        input = Mathf.Clamp(input, -1f, 1f);
        transform.localRotation = Quaternion.Euler(0, input * _angle, 0);
    }
}
