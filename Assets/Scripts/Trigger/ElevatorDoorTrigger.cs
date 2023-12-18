using System;
using UnityEngine;

/// <summary>
/// 외나무다리씬 - 복도씬을 잇는 엘리베이터 트리거입니다.
/// </summary>
public class ElevatorDoorTrigger : MonoBehaviour
{
    // 외나무다리씬 - 복도씬을 잇는 엘리베이터 트리거입니다.
    // 이후 스테이지 2에서도 엘리베이터가 나오지만, 아직 씬이 확정되지 않았으므로 고려하지 않고 작성하였습니다.

    // 추후 문 여는 애니메이션을 추가하려면 코드 수정보다는 메시와 콜라이더를 나눠 애니메이터로 처리하는 것이 바람직합니다.

    public GameObject doorColliderObject;
    public MoveMachine elevator;
    private bool m_isTriggered = false;

    public void OpenDoor()
    {
        doorColliderObject.SetActive(false);
    }

    public void CloseDoor()
    {
        doorColliderObject.SetActive(true);
    }

    private void Start()
    {
        OpenDoor();
    }

    private void OnEnable()
    {
        if (elevator != null)
        {
            elevator.ElevatorMoveEnd += OnElevatorMoveEnd;
        }
        else
        {
            m_isTriggered = true;
        }
    }

    private void OnDisable()
    {
        if (elevator != null)
        {
            elevator.ElevatorMoveEnd -= OnElevatorMoveEnd;
        }
    }

    private void OnElevatorMoveEnd(object sender, EventArgs e)
    {
        OpenDoor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_isTriggered) return;
        if (other.TryGetComponent<Monster>(out var m))
        {
            if (m.IsPossessed)
            {
                CloseDoor();
                m_isTriggered = true;
            }
        }
    }
}
