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

    // (����)
    public float interactionDistance = 1.5f;

    // ���ڴ洢��ǰ�������е�Э�̣��������
    private Coroutine resetCoroutine;

    public bool isCommandToMove;

    DirectionIndicator directionIndicator;
    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();

        // ��ʼ��ʱ��ʹ��Ĭ�ϵĽ�������
        agent.stoppingDistance = interactionDistance;

        directionIndicator = GetComponent<DirectionIndicator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && IsMovingPossible())
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // �޸ĺ��ģ�����δ��������չ�����ڼ����ˡ����ߵ�
            // Ŀǰ����ֻ������
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                // 1. ������������еġ��ָ���Э�̣���ֹͣ��
                //    ����Է�ֹ��һ�ε��ƶ�ָ����ű���ָ��
                if (resetCoroutine != null)
                {
                    StopCoroutine(resetCoroutine);
                }
                // (�޸�) ��Ϊ���ƶ������棬������Ҫ��ȷ���
                // ������Ŀ��ǰ����stoppingDistance��ʱ��Ϊ0��
                agent.stoppingDistance = 0f;

                isCommandToMove = true;
                StartCoroutine(NoCommand());
                agent.SetDestination(hit.point); // ��������ײ����ΪĿ��

                SoundManager.Instance.PlayUnitCommandSound();

                directionIndicator.DrawLine(hit);

                // 3. ����һ���µ�Э�̣��������ƶ���������ָ�stoppingDistance
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
    /// ����һ��Э�̣�����ȴ���λ�ƶ���Ŀ�ĵأ�Ȼ��ָ�stoppingDistance
    /// </summary>
    private IEnumerator ResetStoppingDistanceAfterArrival()
    {
        // �ȴ���ֱ��·����������ҵ�λ�Ѿ��ǳ��ӽ�Ŀ�ĵ�
        // ������һС������ֵ��0.1f����ȷ�������ȶ�����
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= 0.1f);

        // ��λ�Ѿ�������ڻָ�Ĭ�ϵĽ�������
        agent.stoppingDistance = interactionDistance;
    }
}

//1. �Ҽ�����ذ���Ƶ�λ�ƶ�
