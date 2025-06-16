using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class PushLastSupporter : MonoBehaviour
{
    Vector3 startEuler = new Vector3(90f, 0f, 0f);
    Vector3 endEuler = new Vector3(0f, 180f, 180f);
    public bool isEnd = false;
    public ScrewGauge gauge;
    public StepManager stepManager;
    private void Update()
    {
        if (!isEnd)
        {
            Vector3 currentEuler = transform.localEulerAngles;

            float progressX = NormalizeProgress(startEuler.x, endEuler.x, currentEuler.x);
            float progressY = NormalizeProgress(startEuler.y, endEuler.y, currentEuler.y);
            float progressZ = NormalizeProgress(startEuler.z, endEuler.z, currentEuler.z);

            // 평균 기반 진행률
            float normalizedProgress = (progressX + progressY + progressZ) / 3f;
            gauge.UpdateScrewGauge(normalizedProgress);

            if(normalizedProgress >= 1f)
            {
                isEnd = true;
                stepManager.OnPlayerActionCompleted();
            }
        }

        float NormalizeProgress(float start, float end, float current)
        {
            // 각도는 360도 순환하므로 차이 계산시 주의
            float delta = Mathf.DeltaAngle(start, end);
            float progress = Mathf.DeltaAngle(start, current);

            return Mathf.Clamp01(progress / delta);
        }
    }
}
