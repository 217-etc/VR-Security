using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GogoGaga.OptimizedRopesAndCable
{
    public class ERopeCollision : MonoBehaviour
    {
        [SerializeField] private ERope rope;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int ropeDivisions = 10; // 로프의 분할 수
        [SerializeField] private float ropeCollisionThreshold = 0.1f; // 충돌 감지 임계값
        [SerializeField] private LayerMask collisionLayer; // 충돌 레이어

        private Vector3[] ropePoints;
        private Collider[] colliders;

        private void Start()
        {
            if (rope == null || lineRenderer == null)
            {
                Debug.LogError("Rope or LineRenderer is not assigned.");
                return;
            }

            ropePoints = new Vector3[ropeDivisions + 1];
            colliders = new Collider[10]; // 필요한 만큼의 공간 할당
        }

        private void Update()
        {
            if (rope == null || lineRenderer == null)
            {
                return;
            }

            // 로프의 각 점을 업데이트합니다.
            ApplyAdjustedPoints();

            // 충돌 처리 로직
            HandleCollisions();
        }

        private void ApplyAdjustedPoints()
        {
            // Adjusted points를 로프에 반영
            for (int i = 0; i <= ropeDivisions; i++)
            {
                float t = (float)i / ropeDivisions;

                // GetPointAt을 사용하여 각 점을 계산
                ropePoints[i] = rope.GetPointAt(t);

                // LineRenderer의 해당 위치를 업데이트
                lineRenderer.SetPosition(i, ropePoints[i]);
            }
        }

        private void HandleCollisions()
        {
            for (int i = 0; i < ropePoints.Length; i++)
            {
                Vector3 point = ropePoints[i];

                // 해당 점에서의 충돌 검사 (Collider와의 충돌 감지)
                int colliderCount = Physics.OverlapSphereNonAlloc(point, ropeCollisionThreshold, colliders, collisionLayer);

                // 충돌이 감지되었을 때 처리
                if (colliderCount > 0)
                {
                    foreach (Collider collider in colliders)
                    {
                        if (collider != null)
                        {
                            // 충돌이 감지된 경우 로프 점을 반영하거나 추가 처리
                            Debug.Log($"Collision detected at point: {point} with collider: {collider.gameObject.name}");
                            // 여기에 충돌 처리 로직 추가 (예: 점을 반영하거나 힘을 가하는 등)
                        }
                    }
                }
            }
        }
    }
}


