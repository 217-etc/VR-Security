using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GogoGaga.OptimizedRopesAndCable
{
    public class ERopeCollision : MonoBehaviour
    {
        [SerializeField] private ERope rope;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int ropeDivisions = 10; // ������ ���� ��
        [SerializeField] private float ropeCollisionThreshold = 0.1f; // �浹 ���� �Ӱ谪
        [SerializeField] private LayerMask collisionLayer; // �浹 ���̾�

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
            colliders = new Collider[10]; // �ʿ��� ��ŭ�� ���� �Ҵ�
        }

        private void Update()
        {
            if (rope == null || lineRenderer == null)
            {
                return;
            }

            // ������ �� ���� ������Ʈ�մϴ�.
            ApplyAdjustedPoints();

            // �浹 ó�� ����
            HandleCollisions();
        }

        private void ApplyAdjustedPoints()
        {
            // Adjusted points�� ������ �ݿ�
            for (int i = 0; i <= ropeDivisions; i++)
            {
                float t = (float)i / ropeDivisions;

                // GetPointAt�� ����Ͽ� �� ���� ���
                ropePoints[i] = rope.GetPointAt(t);

                // LineRenderer�� �ش� ��ġ�� ������Ʈ
                lineRenderer.SetPosition(i, ropePoints[i]);
            }
        }

        private void HandleCollisions()
        {
            for (int i = 0; i < ropePoints.Length; i++)
            {
                Vector3 point = ropePoints[i];

                // �ش� �������� �浹 �˻� (Collider���� �浹 ����)
                int colliderCount = Physics.OverlapSphereNonAlloc(point, ropeCollisionThreshold, colliders, collisionLayer);

                // �浹�� �����Ǿ��� �� ó��
                if (colliderCount > 0)
                {
                    foreach (Collider collider in colliders)
                    {
                        if (collider != null)
                        {
                            // �浹�� ������ ��� ���� ���� �ݿ��ϰų� �߰� ó��
                            Debug.Log($"Collision detected at point: {point} with collider: {collider.gameObject.name}");
                            // ���⿡ �浹 ó�� ���� �߰� (��: ���� �ݿ��ϰų� ���� ���ϴ� ��)
                        }
                    }
                }
            }
        }
    }
}


