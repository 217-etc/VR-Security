using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowUI : MonoBehaviour
{
    [SerializeField] private Transform _playerCamera;  
    [SerializeField] private float _followSpeed = 5f;
    [SerializeField] private float _bufferTime = 0.5f;

    private float _initialDistance;
    private Vector3 _initialOffset;

    private Vector3 _targetPosition;
    private Vector3 _velocity = Vector3.zero;

    void Start()
    {
        if (_playerCamera == null)
        {
            Debug.LogError("Player Camera가 설정되지 않았습니다.");
            return;
        }

        
        _initialOffset = transform.position - _playerCamera.position;
        _initialOffset.y = 0; 
        _initialDistance = _initialOffset.magnitude;

        _targetPosition = transform.position;
    }

    void Update()
    {
        if (_playerCamera == null) return;

        Vector3 playerPosition = _playerCamera.position;
        playerPosition.y = transform.position.y; 

        float yawAngle = _playerCamera.eulerAngles.y;
        Quaternion rotation = Quaternion.AngleAxis(yawAngle, Vector3.up);

        Vector3 newTargetPosition = playerPosition + rotation * (_initialOffset.normalized * _initialDistance);

        _targetPosition = Vector3.SmoothDamp(_targetPosition, newTargetPosition, ref _velocity, _bufferTime);
        transform.position = _targetPosition;

        Vector3 lookDirection = _playerCamera.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 180, 0);
    }
}
