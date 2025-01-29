using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePhysics : MonoBehaviour
{
    [Header("Rope Settings")]
    [SerializeField] private int segmentCount = 10; // ���� ���׸�Ʈ ����
    [SerializeField] private float segmentLength = 1f; // �� ���׸�Ʈ ����
    [SerializeField] private float ropeWidth = 0.1f; // ������ �ʺ�
    [SerializeField] private float ropeStiffness = 100f; // ������ ����
    [SerializeField] private float ropeDamping = 5f; // ������ ����
    [SerializeField] private LayerMask collisionLayer; // �浹 ���̾�

    [Header("End Points")]
    [SerializeField] private Transform startPoint; // ������ ������
    [SerializeField] private Transform endPoint; // ������ ����

    private GameObject[] segments; // ���׸�Ʈ ��ü �迭
    private Rigidbody[] segmentRigidbodies; // ���׸�Ʈ�� Rigidbody �迭
    private Collider[] segmentColliders; // ���׸�Ʈ�� Collider �迭

    void Start()
    {
        // ���׸�Ʈ �迭 �ʱ�ȭ
        segments = new GameObject[segmentCount];
        segmentRigidbodies = new Rigidbody[segmentCount];
        segmentColliders = new Collider[segmentCount];

        // ���׸�Ʈ ����
        CreateRope();
    }

    void Update()
    {
        // ���׸�Ʈ���� ��ġ�� ������Ʈ
        UpdateRope();
    }

    private void CreateRope()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            // ���׸�Ʈ ������Ʈ ����
            GameObject segment = new GameObject("RopeSegment_" + i);
            segment.transform.SetParent(transform);

            // �� ���׸�Ʈ�� ��ġ ���
            float t = i / (float)(segmentCount - 1);
            Vector3 segmentPosition = Vector3.Lerp(startPoint.position, endPoint.position, t);

            // ���׸�Ʈ�� ��ġ ����
            segment.transform.position = segmentPosition;

            // ���׸�Ʈ�� Rigidbody �߰�
            Rigidbody rb = segment.AddComponent<Rigidbody>();
            rb.useGravity = false; // �߷� ��Ȱ��ȭ
            rb.isKinematic = true; // ���� ������ ������ ���� �ʵ��� ����

            // ���׸�Ʈ�� SphereCollider �߰�
            SphereCollider collider = segment.AddComponent<SphereCollider>();
            collider.radius = ropeWidth / 2; // �ݶ��̴� ũ�� ����
            collider.isTrigger = true; // Ʈ���ŷ� �����Ͽ� ������ �浹�� ó��

            // ���׸�Ʈ�� �迭�� �߰�
            segments[i] = segment;
            segmentRigidbodies[i] = rb;
            segmentColliders[i] = collider;

            // ù ��° ���׸�Ʈ�� �������� ����, ������ ���׸�Ʈ�� ������ ����
            if (i == 0)
            {
                // ù ��° ���׸�Ʈ�� Joint �߰� (������ ����)
                HingeJoint joint = segment.AddComponent<HingeJoint>();
                joint.connectedBody = startPoint.GetComponent<Rigidbody>();
                joint.anchor = Vector3.zero;
                joint.autoConfigureConnectedAnchor = false;
            }
            else if (i == segmentCount - 1)
            {
                // ������ ���׸�Ʈ�� Joint �߰� (���� ����)
                HingeJoint joint = segment.AddComponent<HingeJoint>();
                joint.connectedBody = endPoint.GetComponent<Rigidbody>();
                joint.anchor = Vector3.zero;
                joint.autoConfigureConnectedAnchor = false;
            }
            else
            {
                // �߰� ���׸�Ʈ�鿡 Joint �߰� (���� ���׸�Ʈ��� ����)
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
            // �� ���׸�Ʈ�� ��ġ ������Ʈ
            Vector3 targetPosition = Vector3.Lerp(startPoint.position, endPoint.position, i / (float)(segmentCount - 1));
            segmentRigidbodies[i].MovePosition(targetPosition);
        }
    }

    // �浹 ó�� �޼��� (�� ���׸�Ʈ�� Collider�� �ٸ� ��ü�� �浹�� �� ȣ���)
    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ��ü�� ������ ���̾ �ִ��� Ȯ��
        if (((1 << other.gameObject.layer) & collisionLayer) != 0)
        {
            // �浹 ó�� ���� (��: ������ ������ �߰��� �� ����)
            Debug.Log("Collision detected with: " + other.gameObject.name);
        }
    }
}
