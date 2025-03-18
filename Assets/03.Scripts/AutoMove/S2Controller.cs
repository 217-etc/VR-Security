using UnityEngine;

public class S2Controller : MonoBehaviour
{
    public MonoBehaviour translateScript; // 이동 스크립트
    public MonoBehaviour rotateScript; // 회전 스크립트
    private bool halfMoved = false;
    private bool firstMove = false;

    void Start()
    {
        translateScript.enabled = true;
    }

    void Update()
    {
        Debug.Log("S2 y축 위치: " + transform.localPosition.y);
        // S2의 y축 위치가 -7보다 작아지면
        if (transform.localPosition.y <= -7f)
        {
            if (firstMove){ return; }
            translateScript.enabled = false; // 이동 스크립트 비활성화
            rotateScript.enabled = true; // 회전 스크립트 활성화
            if (rotateScript.enabled)
            {
                Debug.Log("회전 스크립트 활성화됨---------------------------------------------------------");
                // 로그만 뜨고 활성화 안됨
            }
            Debug.Log("firstMove --------------------------------------------------------- ");
            firstMove = true;
        }

        if (transform.localEulerAngles.x >= 269.0f && transform.localEulerAngles.x <= 271.0f)
        {
            halfMoved = true;
            Debug.Log("S2 절반 넘어감");
        }

        // S2의 x축 회전값이 특정 값이 되면
        if (halfMoved && transform.localEulerAngles.x >= 350.0f) // 예시: 85도 이상일 경우
        {
            rotateScript.enabled = false; // 회전 스크립트 비활성화
            translateScript.enabled = true; // 이동 스크립트 활성화
        }
    }
}
