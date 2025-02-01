using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public OVRHand ovrHand; // OVRHand 참조
    public OVRSkeleton ovrSkeleton; // OVRSkeleton 참조
    public GameObject debugSpherePrefab;

    private Dictionary<string, Transform> boneTransforms = new Dictionary<string, Transform>();
    private Dictionary<string, GameObject> debugSpheres = new Dictionary<string, GameObject>();

    private bool detectStart = false; // 손 감지 시작
    private bool isFist = false; // 현재 주먹 상태
    private bool isFirstPalm = false; // 처음 손바닥을 펼쳤는지
    private const float angleThreshold = 120f; // 손가락이 구부러졌다고 판단하는 각도
    private const float thumbAngleThreshold = 25f; // 엄지 손가락이 구부러졌다고 판단하는 각도
    private const float palmThreshold = 40f; // 손가락 펼쳐졌다고 판단하는 각도
    private int fistCnt = 0;

    private Transform wristTransform; // 손목 Transform

    void Start()
    {
        StartCoroutine(InitializeBones());
    }

    void DetectStart()
    {
        detectStart = true;
        Debug.LogWarning("감지 시작");
    }

    private IEnumerator InitializeBones()
    {
        while (ovrSkeleton.Bones == null || ovrSkeleton.Bones.Count == 0)
        {
            yield return null;
        }

        // 손가락 이름 리스트 (Pinky 대신 Little 사용)
        string[] fingerNames = { "Thumb", "Index", "Middle", "Ring", "Little" };

        foreach (var bone in ovrSkeleton.Bones)
        {
            string boneName = bone.Transform.name; // Transform의 실제 이름 가져오기

            foreach (var finger in fingerNames)
            {
                if (boneName == $"XRHand_{finger}Proximal") // Base 관절
                {
                    boneTransforms[$"{finger}_Base"] = bone.Transform;
                }
                else if (boneName == $"XRHand_{finger}Tip") // Tip 손끝
                {
                    boneTransforms[$"{finger}_Tip"] = bone.Transform;
                }
                else if (boneName == "XRHand_Wrist") // 손목
                {
                    wristTransform = bone.Transform;
                }
            }
        }

        // 디버그 Sphere 생성 및 매핑
        foreach (var key in boneTransforms.Keys)
        {
            Transform boneTransform = boneTransforms[key];

            if (boneTransform != null)
            {
                GameObject debugSphere = Instantiate(debugSpherePrefab, boneTransform.position, Quaternion.identity);
                debugSphere.transform.localScale = Vector3.one * 0.02f; // Sphere 크기 조정
                debugSpheres[key] = debugSphere;
            }
        }

        Debug.Log("손가락 관절 및 디버그 Sphere 초기화 완료!");

        Invoke("DetectStart", 3f);
    }

    void Update()
    {
        if (!detectStart || boneTransforms.Count == 0 || wristTransform == null) return;

        // 디버그 Sphere 위치 업데이트 (손가락 관절을 따라감)
        foreach (var key in boneTransforms.Keys)
        {
            if (debugSpheres.ContainsKey(key))
            {
                debugSpheres[key].transform.position = boneTransforms[key].position;
            }
        }

        // 손가락 구부러짐 감지 (각도 기반)
        bool isThumbCurled = IsFingerCurled("Thumb");
        bool isIndexCurled = IsFingerCurled("Index");
        bool isMiddleCurled = IsFingerCurled("Middle");
        bool isRingCurled = IsFingerCurled("Ring");
        bool isLittleCurled = IsFingerCurled("Little");

        // 손가락 펼쳐짐 감지
        bool isThumbOpened = IsFingerOpened("Thumb");
        bool isIndexOpened = IsFingerOpened("Index");
        bool isMiddleOpened = IsFingerOpened("Middle");
        bool isRingOpened = IsFingerOpened("Ring");
        bool isLittleOpened = IsFingerOpened("Little");

        // 손을 처음 폈을 때 감지
        if (isThumbOpened && isIndexOpened && isMiddleOpened && isRingOpened && isLittleOpened)
        {
            if (!isFirstPalm)
            {
                isFirstPalm = true;
                Debug.LogWarning("처음 손바닥을 펼쳤습니다.");
                ChangeJointColorGreen();
                OnHandOpen();
                return;
            }
            else
            {
                if (isFist)
                {
                    Debug.LogWarning("다시 손바닥을 펼쳤습니다.");
                    ChangeJointColorGreen();
                    isFist = false;
                    OnHandOpen();
                    return;
                }
            }
        }

        // 모든 손가락이 구부러져 있으면 주먹으로 판단
        if (isFirstPalm && isIndexCurled && isMiddleCurled && isRingCurled && isLittleCurled)
        {
            if (!isFist) // 처음 주먹 상태로 변경될 때
            {
                isFist = true;
                fistCnt++;
                Debug.LogWarning($"손이 주먹을 쥐었습니다! 주먹 쥔 횟수: {fistCnt}");
                ChangeJointColorRed();
                OnFist();
                return;
            }
        }
    }

    // 손가락이 구부러졌는지 판단하는 함수 (각도 기반)
    private bool IsFingerCurled(string finger)
    {
        string baseKey = $"{finger}_Base";
        string tipKey = $"{finger}_Tip";

        if (!boneTransforms.ContainsKey(baseKey) || !boneTransforms.ContainsKey(tipKey) || wristTransform == null)
            return false;

        Transform baseJoint = boneTransforms[baseKey];
        Transform tipJoint = boneTransforms[tipKey];

        // 손가락 벡터
        Vector3 fingerVector = (tipJoint.position - baseJoint.position).normalized;

        // 손바닥 벡터 (손목 → 중지 Base 방향)
        Transform middleBase = boneTransforms.ContainsKey("Middle_Base") ? boneTransforms["Middle_Base"] : null;
        if (middleBase == null) return false;
        Vector3 palmVector = (middleBase.position - wristTransform.position).normalized;

        // 손가락과 손바닥 벡터 간의 각도
        float angle = Vector3.Angle(fingerVector, palmVector);

        if(finger == "Thumb")
        {
            //Debug.Log($"{finger} :  {angle}");
            return angle > thumbAngleThreshold;
        }
        else
        {
            //Debug.Log($"{finger} :  {angle}");
            return angle > angleThreshold;
        }
    }

    private bool IsFingerOpened(string finger)
    {
        string baseKey = $"{finger}_Base";
        string tipKey = $"{finger}_Tip";

        if (!boneTransforms.ContainsKey(baseKey) || !boneTransforms.ContainsKey(tipKey) || wristTransform == null)
            return false;

        Transform baseJoint = boneTransforms[baseKey];
        Transform tipJoint = boneTransforms[tipKey];

        // 손가락 벡터
        Vector3 fingerVector = (tipJoint.position - baseJoint.position).normalized;

        // 손바닥 벡터 (손목 → 중지 Base 방향)
        Transform middleBase = boneTransforms.ContainsKey("Middle_Base") ? boneTransforms["Middle_Base"] : null;
        if (middleBase == null) return false;
        Vector3 palmVector = (middleBase.position - wristTransform.position).normalized;

        // 손가락과 손바닥 벡터 간의 각도
        float angle = Vector3.Angle(fingerVector, palmVector);

        return angle < palmThreshold;
    }

    private void OnFist()
    {
        Debug.LogWarning("주먹 동작 실행 중...");
    }

    private void OnHandOpen()
    {
        Debug.LogWarning("손 펼침 동작 실행 중...");
    }

    private void ChangeJointColorGreen()
    {
        foreach(GameObject sphere in debugSpheres.Values)
        {
            Renderer sphereRenderer = sphere.GetComponent<Renderer>();
            if (sphereRenderer != null)
            {
                sphereRenderer.material.color = Color.green;
            }
        }  
    }

    private void ChangeJointColorRed()
    {
        foreach (GameObject sphere in debugSpheres.Values)
        {
            Renderer sphereRenderer = sphere.GetComponent<Renderer>();
            if (sphereRenderer != null)
            {
                sphereRenderer.material.color = Color.red;
            }
        }
    }
}

