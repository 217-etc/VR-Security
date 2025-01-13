using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using UnityEngine.Assertions;

public class HandJointDebugger : MonoBehaviour
{
    public Hand hand; // Hand 스크립트를 참조
    public Color lineColor = Color.green; // 선의 색상
    public float lineWidth = 0.005f; // 선의 두께
    public float sphereRadius = 0.01f; // 관절 구의 크기
    public Color sphereColor = Color.blue; // 관절 구의 색상

    private LineRenderer lineRenderer;
    private Dictionary<HandJointId, GameObject> jointSpheres = new Dictionary<HandJointId, GameObject>();

    void Start()
    {
        // LineRenderer 초기화
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;


        // 관절 구 생성
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
        if (!hand.IsTrackedDataValid) return; // 데이터가 유효하지 않으면 반환

        List<Vector3> jointPositions = new List<Vector3>();

        foreach (HandJointId jointId in System.Enum.GetValues(typeof(HandJointId)))
        {
            if (hand.GetJointPose(jointId, out Pose jointPose))
            {
                // 관절 구 위치 업데이트
                jointSpheres[jointId].transform.position = jointPose.position;

                // 관절 연결 선 업데이트 (부모 관절이 있는 경우)
                if (GetParentJoint(jointId, out HandJointId parentId) &&
                    hand.GetJointPose(parentId, out Pose parentPose))
                {
                    jointPositions.Add(jointPose.position);
                    jointPositions.Add(parentPose.position);
                }
            }
        }

        // LineRenderer 업데이트
        lineRenderer.positionCount = jointPositions.Count;
        lineRenderer.SetPositions(jointPositions.ToArray());
    }

    private bool GetParentJoint(HandJointId jointId, out HandJointId parentId)
    {
        // 관절의 부모 정보를 설정 (손목 -> 손가락 순서)
        parentId = HandJointId.Invalid;

        // 간단한 예: 손목이 부모로 설정됨
        if (jointId == HandJointId.HandWristRoot) return false; // 손목의 부모는 없음
        if (jointId.ToString().Contains("Tip")) return false; // 손가락 끝은 연결하지 않음

        parentId = jointId - 1; // 부모는 현재 관절 ID의 이전 번호
        return true;
    }
}
