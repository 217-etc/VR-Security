using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using UnityEngine.Assertions;

public class HandJointDebugger : MonoBehaviour
{
    public Hand hand; // Hand ��ũ��Ʈ�� ����
    public Color lineColor = Color.green; // ���� ����
    public float lineWidth = 0.005f; // ���� �β�
    public float sphereRadius = 0.01f; // ���� ���� ũ��
    public Color sphereColor = Color.blue; // ���� ���� ����

    private LineRenderer lineRenderer;
    private Dictionary<HandJointId, GameObject> jointSpheres = new Dictionary<HandJointId, GameObject>();

    void Start()
    {
        // LineRenderer �ʱ�ȭ
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;


        // ���� �� ����
        foreach (HandJointId jointId in System.Enum.GetValues(typeof(HandJointId)))
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = Vector3.one * sphereRadius;
            sphere.GetComponent<Renderer>().material.color = sphereColor;
            sphere.name = jointId.ToString();
            jointSpheres[jointId] = sphere;
        }
    }

    void Update()
    {
        if (!hand.IsTrackedDataValid) return; // �����Ͱ� ��ȿ���� ������ ��ȯ

        List<Vector3> jointPositions = new List<Vector3>();

        foreach (HandJointId jointId in System.Enum.GetValues(typeof(HandJointId)))
        {
            if (hand.GetJointPose(jointId, out Pose jointPose))
            {
                // ���� �� ��ġ ������Ʈ
                jointSpheres[jointId].transform.position = jointPose.position;

                // ���� ���� �� ������Ʈ (�θ� ������ �ִ� ���)
                if (GetParentJoint(jointId, out HandJointId parentId) &&
                    hand.GetJointPose(parentId, out Pose parentPose))
                {
                    jointPositions.Add(jointPose.position);
                    jointPositions.Add(parentPose.position);
                }
            }
        }

        // LineRenderer ������Ʈ
        lineRenderer.positionCount = jointPositions.Count;
        lineRenderer.SetPositions(jointPositions.ToArray());
    }

    private bool GetParentJoint(HandJointId jointId, out HandJointId parentId)
    {
        // ������ �θ� ������ ���� (�ո� -> �հ��� ����)
        parentId = HandJointId.Invalid;

        // ������ ��: �ո��� �θ�� ������
        if (jointId == HandJointId.HandWristRoot) return false; // �ո��� �θ�� ����
        if (jointId.ToString().Contains("Tip")) return false; // �հ��� ���� �������� ����

        parentId = jointId - 1; // �θ�� ���� ���� ID�� ���� ��ȣ
        return true;
    }
}
