using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoriManager : MonoBehaviour
{
    bool _isComplete = false;
    [SerializeField] GameObject _jojeolgi;

    [SerializeField] private Transform[] _waypoints; // 이동할 포인트
    [SerializeField] private float _moveSpeed = 1.5f; // 이동 속도

    private int _currentWaypointIndex = 0;
    private float _t = 0; // 베지어 곡선 진행 정도

    IEnumerator MoveAlongBezierCurve()
    {
        while (true)
        {
            if (_currentWaypointIndex + 2 >= _waypoints.Length)
                yield break; // 더 이상 이동할 곳이 없으면 종료

            Transform p0 = _waypoints[_currentWaypointIndex];     // 시작점
            Transform p1 = _waypoints[_currentWaypointIndex + 1]; // 컨트롤 포인트1
            Transform p2 = _waypoints[_currentWaypointIndex + 2]; // 컨트롤 포인트1
            Transform p3 = _waypoints[_currentWaypointIndex + 3]; // 목표점

            _t = 0;

            while (_t < 1)
            {
                _t += Time.deltaTime * _moveSpeed; // 속도 조절
                transform.position = CubicBezier(p0.position, p1.position, p2.position, p3.position, _t);
                yield return null;
            }

            _currentWaypointIndex += 2; // 다음 베지어 곡선 구간으로 이동

            if (_currentWaypointIndex + 2 >= _waypoints.Length)
            {
                Debug.Log("모든 웨이포인트를 완료했습니다.");
                yield break;
            }
        }
    }
    private Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        return (uuu * p0) + (3 * uu * t * p1) + (3 * u * tt * p2) + (ttt * p3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hole") && !_isComplete)
        {
            _isComplete = true;
            _jojeolgi.transform.parent = gameObject.transform;
            Debug.LogWarning("속도조절기에 닿았음");
            StartCoroutine(MoveAlongBezierCurve());
        }
    }
}
