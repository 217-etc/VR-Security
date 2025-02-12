using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateUI : MonoBehaviour
{
    [SerializeField] private Transform centerCamera;
    [SerializeField] private float rotationSpeed = 5f; // 회전 속도
    private float _initialX;

    private void Start()
    {
        _initialX = transform.rotation.eulerAngles.x;
    }
    void Update()
    {
        if (centerCamera != null)
        {
            Vector3 direction = centerCamera.position - transform.position;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                targetRotation *= Quaternion.Euler(0, 180, 0);
                Vector3 eulerAngles = targetRotation.eulerAngles;
                eulerAngles.x = _initialX;
                targetRotation = Quaternion.Euler(eulerAngles);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
