using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyChase : MonoBehaviour
{
    public Transform player; // 아군의 Transform
    public float detectionRange = 10f; // 적군의 시야 범위
    public float fieldOfView = 120f; // 시야각
    public float gravity = -9.81f;

    private NavMeshAgent agent;
    private CharacterController controller;
    private bool isChasingPlayer = false;
    private List<Vector3> floorPositions; // 바닥 타일 위치 목록
    private Vector3 currentTarget; // 현재 목표 위치

    void Start()
    {
        // 씬에서 태그가 "Player"인 객체를 찾아 할당
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player 태그를 가진 객체를 찾을 수 없습니다.");
        }

        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = true;

        // NavMeshAgent 장애물 회피 설정
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(30, 70); // 에이전트마다 다른 우선순위를 줌
        agent.radius = 0.5f;
        agent.speed = Random.Range(2.0f, 4.0f);

        // MazeRender 스크립트에서 바닥 타일 위치 목록을 가져옴
        MazeRender mazeRender = FindObjectOfType<MazeRender>();
        if (mazeRender != null)
        {
            floorPositions = mazeRender.floorPositions;
        }
        else
        {
            Debug.LogError("MazeRender 스크립트를 찾을 수 없습니다.");
        }

        SetRandomDestination(); // 처음 무작위 목적지 설정
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player가 할당되지 않았습니다.");
            return;
        }

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // 아군이 시야 범위 내에 있는지 확인
        if (distanceToPlayer <= detectionRange && angleToPlayer <= fieldOfView / 2f)
        {
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, detectionRange))
            {
                Debug.DrawRay(transform.position, directionToPlayer.normalized * detectionRange, Color.red);
                if (hit.transform.CompareTag("Player")) // 태그 검사
                {
                    // 아군이 시야에 포착되면 추격 시작
                    isChasingPlayer = true;
                    agent.SetDestination(player.position);
                }
                //else
                //{
                //    Debug.Log("Raycast hit: " + hit.transform.name);
                //}
            }
            else
            {
                Debug.Log("Raycast did not hit anything.");
            }
        }

        if (isChasingPlayer)
        {
            // 플레이어를 추적 중일 때
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                controller.Move(agent.velocity * Time.deltaTime);
                // animator.SetBool(moveHash, true);
            }
            else
            {
                controller.Move(Vector3.zero);
                isChasingPlayer = false;
                // animator.SetBool(moveHash, false);
            }
        }
        else
        {
            // 무작위 위치로 이동 중일 때
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                SetRandomDestination();
            }

            controller.Move(agent.velocity * Time.deltaTime);
        }
    }

    private void SetRandomDestination()
    {
        if (floorPositions == null || floorPositions.Count == 0)
        {
            Debug.LogError("No floor positions available.");
            return;
        }

        // 바닥 타일 위치 목록에서 무작위 위치 선택
        Vector3 randomPosition = floorPositions[Random.Range(0, floorPositions.Count)];

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            currentTarget = hit.position; // 현재 목표 위치 저장
            agent.SetDestination(currentTarget);
        }
        else
        {
            Debug.LogError("Failed to find random NavMesh position");
        }
    }

    void OnDrawGizmos()
    {
        // 적군의 시야 범위를 원으로 시각화
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 적군의 시야각을 시각화
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2f, 0) * transform.forward * detectionRange;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // 현재 목표 위치 시각화
        if (currentTarget != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(currentTarget, 0.5f);
        }

        // NavMeshAgent 경로 시각화
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.green;
            Vector3[] pathCorners = agent.path.corners;
            for (int i = 0; i < pathCorners.Length - 1; i++)
            {
                Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
            }
        }
    }
}
