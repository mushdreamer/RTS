using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    Camera cam;
    NavMeshAgent agent;
    public LayerMask ground;

    // (新增)
    public float interactionDistance = 1.5f;

    // 用于存储当前正在运行的协程，方便管理
    private Coroutine resetCoroutine;

    public bool isCommandToMove;

    DirectionIndicator directionIndicator;
    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();

        // 初始化时，使用默认的交互距离
        agent.stoppingDistance = interactionDistance;

        directionIndicator = GetComponent<DirectionIndicator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && IsMovingPossible())
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // 修改核心：这里未来可以扩展，用于检测敌人、道具等
            // 目前我们只检测地面
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                // 1. 如果有正在运行的“恢复”协程，先停止它
                //    这可以防止上一次的移动指令干扰本次指令
                if (resetCoroutine != null)
                {
                    StopCoroutine(resetCoroutine);
                }
                // (修改) 因为是移动到地面，所以需要精确到达。
                // 在设置目标前，将stoppingDistance临时设为0。
                agent.stoppingDistance = 0f;

                isCommandToMove = true;
                StartCoroutine(NoCommand());
                agent.SetDestination(hit.point); // 将射线碰撞点设为目标

                SoundManager.Instance.PlayUnitCommandSound();

                directionIndicator.DrawLine(hit);

                // 3. 启动一个新的协程，让它在移动结束后负责恢复stoppingDistance
                resetCoroutine = StartCoroutine(ResetStoppingDistanceAfterArrival());
            }
        }

        /*if (agent.hasPath == false || agent.remainingDistance <= agent.stoppingDistance)
        {
            isCommandToMove = false;
        }*/
    }

    private bool IsMovingPossible()
    {
        return CursorManager.Instance.currentCursor != CursorManager.CursorType.UnAvailable;
    }

    IEnumerator NoCommand()
    {
        yield return new WaitForSeconds(1);
        isCommandToMove = false;
    }
    /// <summary>
    /// 这是一个协程，它会等待单位移动到目的地，然后恢复stoppingDistance
    /// </summary>
    private IEnumerator ResetStoppingDistanceAfterArrival()
    {
        // 等待，直到路径计算完成且单位已经非常接近目的地
        // 我们用一小个缓冲值（0.1f）来确保它能稳定触发
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= 0.1f);

        // 单位已经到达，现在恢复默认的交互距离
        agent.stoppingDistance = interactionDistance;
    }
}

//1. 右键点击地板控制单位移动
