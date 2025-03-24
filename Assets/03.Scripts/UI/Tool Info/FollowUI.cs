using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUI : MonoBehaviour
{
    [SerializeField] private Transform _centerEye;
    [SerializeField] private float _followSpeed = 5f;
    [SerializeField] private float _maxOffsetX = 0.5f;
    [SerializeField] private float _bufferTime = 0.5f;
    [SerializeField] private float _rotationMultiplier = 0.01f;

    private Vector3 _initialPosition;
    private float _initialYaw;
    private Vector3 _targetPosition;

    void Start()
    {
        if (_centerEye == null)
        {
            Debug.LogError("Center Eye가 설정되지 않았습니다.");
            return;
        }

        _initialPosition = transform.position;
        _initialYaw = _centerEye.eulerAngles.y;
        _targetPosition = _initialPosition;
    }

    void Update()
    {
        if (_centerEye != null)
        {
            Vector3 eulerRotation = _centerEye.eulerAngles;
            float currentYaw = eulerRotation.y;
            float yawDifference = currentYaw - _initialYaw;

            float offsetX = Mathf.Clamp(-yawDifference * _rotationMultiplier, -_maxOffsetX, _maxOffsetX);

            Vector3 forwardDirection = _centerEye.forward;
            forwardDirection.y = 0;
            forwardDirection.Normalize();

            Vector3 newTargetPosition = _centerEye.position + forwardDirection * 2f + _centerEye.right * offsetX;
            newTargetPosition.y = _initialPosition.y;

            _targetPosition = Vector3.Lerp(_targetPosition, newTargetPosition, Time.deltaTime / _bufferTime);
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _followSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.Euler(0, _centerEye.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _followSpeed * Time.deltaTime);
        }
    }
}
