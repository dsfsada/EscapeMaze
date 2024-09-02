using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyChase : MonoBehaviour
{
    public Transform player; // �Ʊ��� Transform
    public float detectionRange = 10f; // ������ �þ� ����
    public float fieldOfView = 120f; // �þ߰�
    public float gravity = -9.81f;

    private NavMeshAgent agent;
    private CharacterController controller;
    private bool isChasingPlayer = false;
    private List<Vector3> floorPositions; // �ٴ� Ÿ�� ��ġ ���
    private Vector3 currentTarget; // ���� ��ǥ ��ġ

    void Start()
    {
        // ������ �±װ� "Player"�� ��ü�� ã�� �Ҵ�
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player �±׸� ���� ��ü�� ã�� �� �����ϴ�.");
        }

        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = true;

        // NavMeshAgent ��ֹ� ȸ�� ����
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(30, 70); // ������Ʈ���� �ٸ� �켱������ ��
        agent.radius = 0.5f;
        agent.speed = Random.Range(2.0f, 4.0f);

        // MazeRender ��ũ��Ʈ���� �ٴ� Ÿ�� ��ġ ����� ������
        MazeRender mazeRender = FindObjectOfType<MazeRender>();
        if (mazeRender != null)
        {
            floorPositions = mazeRender.floorPositions;
        }
        else
        {
            Debug.LogError("MazeRender ��ũ��Ʈ�� ã�� �� �����ϴ�.");
        }

        SetRandomDestination(); // ó�� ������ ������ ����
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // �Ʊ��� �þ� ���� ���� �ִ��� Ȯ��
        if (distanceToPlayer <= detectionRange && angleToPlayer <= fieldOfView / 2f)
        {
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, detectionRange))
            {
                Debug.DrawRay(transform.position, directionToPlayer.normalized * detectionRange, Color.red);
                if (hit.transform.CompareTag("Player")) // �±� �˻�
                {
                    // �Ʊ��� �þ߿� �����Ǹ� �߰� ����
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
            // �÷��̾ ���� ���� ��
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
            // ������ ��ġ�� �̵� ���� ��
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

        // �ٴ� Ÿ�� ��ġ ��Ͽ��� ������ ��ġ ����
        Vector3 randomPosition = floorPositions[Random.Range(0, floorPositions.Count)];

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            currentTarget = hit.position; // ���� ��ǥ ��ġ ����
            agent.SetDestination(currentTarget);
        }
        else
        {
            Debug.LogError("Failed to find random NavMesh position");
        }
    }

    void OnDrawGizmos()
    {
        // ������ �þ� ������ ������ �ð�ȭ
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, detectionRange);

        // ������ �þ߰��� �ð�ȭ
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2f, 0) * transform.forward * detectionRange;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // ���� ��ǥ ��ġ �ð�ȭ
        if (currentTarget != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(currentTarget, 0.5f);
        }

        // NavMeshAgent ��� �ð�ȭ
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
