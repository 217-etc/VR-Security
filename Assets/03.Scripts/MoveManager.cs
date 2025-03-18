using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLimiter : MonoBehaviour
{
    private Vector3 initialPosition;

    [Header("축별 이동 제한 활성화")]
    public bool limitX = true;
    public bool limitY = false;
    public bool limitZ = false;

    [Header("X축 이동 제한")]
    public float xMin = -2f;
    public float xMax = 0f;

    [Header("Y축 이동 제한")]
    public float yMin = -2f;
    public float yMax = 2f;

    [Header("Z축 이동 제한")]
    public float zMin = -2f;
    public float zMax = 2f;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector3 clampedPosition = currentPosition;

        if (limitX)
            clampedPosition.x = Mathf.Clamp(currentPosition.x, initialPosition.x + xMin, initialPosition.x + xMax);

        if (limitY)
            clampedPosition.y = Mathf.Clamp(currentPosition.y, initialPosition.y + yMin, initialPosition.y + yMax);

        if (limitZ)
            clampedPosition.z = Mathf.Clamp(currentPosition.z, initialPosition.z + zMin, initialPosition.z + zMax);

        transform.position = clampedPosition;
    }
}
