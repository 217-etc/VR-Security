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
            Debug.LogError("Center Eye가 설정되지 않았습니다!");
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
            float currentYaw = _centerEye.eulerAngles.y;
            float yawDifference = currentYaw - _initialYaw;

            float offsetX = Mathf.Clamp(-yawDifference * _rotationMultiplier, -_maxOffsetX, _maxOffsetX);

            Vector3 newTargetPosition = _centerEye.position + _centerEye.forward * 2f + _centerEye.right * offsetX;
            newTargetPosition.y = _initialPosition.y;
            _targetPosition = Vector3.Lerp(_targetPosition, newTargetPosition, Time.deltaTime / _bufferTime);

            transform.position = Vector3.Lerp(transform.position, _targetPosition, _followSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(_centerEye.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _followSpeed * Time.deltaTime);
        }
    }
}
