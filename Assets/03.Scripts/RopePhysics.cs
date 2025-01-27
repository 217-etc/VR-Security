using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePhysics : MonoBehaviour
{
    [Header("Rope Settings")]
    [SerializeField] private int segmentCount = 10; // 밧줄 세그먼트 개수
    [SerializeField] private float segmentLength = 1f; // 각 세그먼트 길이
    [SerializeField] private float ropeWidth = 0.1f; // 밧줄의 너비
    [SerializeField] private float ropeStiffness = 100f; // 밧줄의 강성
    [SerializeField] private float ropeDamping = 5f; // 밧줄의 댐핑
    [SerializeField] private LayerMask collisionLayer; // 충돌 레이어

    [Header("End Points")]
    [SerializeField] private Transform startPoint; // 밧줄의 시작점
    [SerializeField] private Transform endPoint; // 밧줄의 끝점

    private GameObject[] segments; // 세그먼트 객체 배열
    private Rigidbody[] segmentRigidbodies; // 세그먼트의 Rigidbody 배열
    private Collider[] segmentColliders; // 세그먼트의 Collider 배열

    void Start()
    {
        // 세그먼트 배열 초기화
        segments = new GameObject[segmentCount];
        segmentRigidbodies = new Rigidbody[segmentCount];
        segmentColliders = new Collider[segmentCount];

        // 세그먼트 생성
        CreateRope();
    }

    void Update()
    {
        // 세그먼트들의 위치를 업데이트
        UpdateRope();
    }

    private void CreateRope()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            // 세그먼트 오브젝트 생성
            GameObject segment = new GameObject("RopeSegment_" + i);
            segment.transform.SetParent(transform);

            // 각 세그먼트의 위치 계산
            float t = i / (float)(segmentCount - 1);
            Vector3 segmentPosition = Vector3.Lerp(startPoint.position, endPoint.position, t);

            // 세그먼트의 위치 설정
            segment.transform.position = segmentPosition;

            // 세그먼트에 Rigidbody 추가
            Rigidbody rb = segment.AddComponent<Rigidbody>();
            rb.useGravity = false; // 중력 비활성화
            rb.isKinematic = true; // 물리 엔진의 영향을 받지 않도록 설정

            // 세그먼트에 SphereCollider 추가
            SphereCollider collider = segment.AddComponent<SphereCollider>();
            collider.radius = ropeWidth / 2; // 콜라이더 크기 설정
            collider.isTrigger = true; // 트리거로 설정하여 물리적 충돌을 처리

            // 세그먼트의 배열에 추가
            segments[i] = segment;
            segmentRigidbodies[i] = rb;
            segmentColliders[i] = collider;

            // 첫 번째 세그먼트는 시작점과 연결, 마지막 세그먼트는 끝점과 연결
            if (i == 0)
            {
                // 첫 번째 세그먼트에 Joint 추가 (시작점 연결)
                HingeJoint joint = segment.AddComponent<HingeJoint>();
                joint.connectedBody = startPoint.GetComponent<Rigidbody>();
                joint.anchor = Vector3.zero;
                joint.autoConfigureConnectedAnchor = false;
            }
            else if (i == segmentCount - 1)
            {
                // 마지막 세그먼트에 Joint 추가 (끝점 연결)
                HingeJoint joint = segment.AddComponent<HingeJoint>();
                joint.connectedBody = endPoint.GetComponent<Rigidbody>();
                joint.anchor = Vector3.zero;
                joint.autoConfigureConnectedAnchor = false;
            }
            else
            {
                // 중간 세그먼트들에 Joint 추가 (인접 세그먼트들과 연결)
                HingeJoint joint = segment.AddComponent<HingeJoint>();
                joint.connectedBody = segmentRigidbodies[i - 1];
                joint.anchor = Vector3.zero;
                joint.autoConfigureConnectedAnchor = false;
            }
        }
    }

    private void UpdateRope()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            // 각 세그먼트의 위치 업데이트
            Vector3 targetPosition = Vector3.Lerp(startPoint.position, endPoint.position, i / (float)(segmentCount - 1));
            segmentRigidbodies[i].MovePosition(targetPosition);
        }
    }

    // 충돌 처리 메서드 (각 세그먼트의 Collider가 다른 객체와 충돌할 때 호출됨)
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체가 지정된 레이어에 있는지 확인
        if (((1 << other.gameObject.layer) & collisionLayer) != 0)
        {
            // 충돌 처리 로직 (예: 물리적 반응을 추가할 수 있음)
            Debug.Log("Collision detected with: " + other.gameObject.name);
        }
    }
}
