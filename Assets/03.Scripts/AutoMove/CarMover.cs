using UnityEngine;

public class CarMover : MonoBehaviour
{
    public float moveSpeed = 10f;      // 이동 속도
    public float turnSpeed = 90f;      // 회전 속도 (도/초)

    private Vector3 targetPos1 = new Vector3(888f, 1.07f, -505f); // x축 888까지 가기
    private Vector3 targetPos2 = new Vector3(888f, 1.07f, -800f); // 우회전

    private enum MoveState { MoveX, TurnRight, MoveZ, Done }
    private MoveState moveState = MoveState.MoveX;

    void Start()
    {
        // transform.localPosition = new Vector3(110f, 1.07f, -505f);
        transform.localEulerAngles = new Vector3(0f, 90f, 0f); // 초기 방향: 90도
    }

    void Update()
    {
        switch (moveState)
        {
            case MoveState.MoveX:
                MoveTowards(targetPos1);
                if (Vector3.Distance(transform.localPosition, targetPos1) < 0.1f)
                {
                    moveState = MoveState.TurnRight;
                }
                break;

            case MoveState.TurnRight:
                float currentY = transform.localEulerAngles.y;
                float newY = Mathf.MoveTowardsAngle(currentY, 179.9f, turnSpeed * Time.deltaTime);
                transform.localEulerAngles = new Vector3(0f, newY, 0f);
                if (Mathf.Abs(Mathf.DeltaAngle(currentY, 179.9f)) < 0.5f)
                {
                    moveState = MoveState.MoveZ;
                }
                break;

            case MoveState.MoveZ:
                MoveTowards(targetPos2);
                if (Vector3.Distance(transform.localPosition, targetPos2) < 0.1f)
                {
                    moveState = MoveState.Done;
                }
                break;

            case MoveState.Done:
                // 이동 종료
                break;
        }
    }

    void MoveTowards(Vector3 target)
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, moveSpeed * Time.deltaTime);
    }
}

