using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhenGrabTongGo : MonoBehaviour
{
    public GameObject newActiveObject;
    public GameObject prevBelt;
    public GameObject prevReel;
    public GameObject prevGo;
    public ToolUIManager toolUIManager1;
    public ToolUIManager toolUIManager2;
    public ToolUIManager toolUIManager3;
    public void WhenGrab()
    {
        // 잡은 속도조절기 사라지고
        // 새로운 속도 조절기 해당 위치에 나타내기
        newActiveObject.transform.localPosition = new Vector3(-0.0590000004f, -1.59500003f, -0.493000001f);
        newActiveObject.SetActive(true);
        prevBelt.SetActive(false);
        prevReel.SetActive(false);
        prevGo.SetActive(false);

        // 툴 이름 사라지게 하기
        toolUIManager1.DisappearUI();
        toolUIManager2.DisappearUI();
        toolUIManager3.DisappearUI();
    }
}
