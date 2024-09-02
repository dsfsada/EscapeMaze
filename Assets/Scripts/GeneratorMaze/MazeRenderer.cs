using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MazeRender : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;   // �̷� ������
    [SerializeField] GameObject MazeCellPrefab;     // �̷� �� ������
    [SerializeField] GameObject agentPrefab;        // ������Ʈ ������
    [SerializeField] int RespawnEnemy;              // ������ ���� ��

    public NavMeshSurface navMeshSurface;           // ������̼� �޽� ���ǽ�

    // �̰��� �̷� ���� ���� ũ���Դϴ�. �̸� �߸� �����ϸ� �� ���̿� ��ħ�̳� ƴ�� ���� �� �ֽ��ϴ�.
    public float CellSize = 1f;

    public List<Vector3> floorPositions = new();

    private void Start()
    {
        MazeCell[,] maze = mazeGenerator.GetMazes();  // �̷� �����⿡�� �̷� �����͸� ������

        CellSize = MazeCellPrefab.transform.localScale.x;

        // �̷θ� (0, 0, 0)�� �߽ɿ� ���߱� ���� ������ ���
        float xOffset = (mazeGenerator.mazeWidth - 1) * CellSize / 2.0f;
        float zOffset = (mazeGenerator.mazeHeight - 1) * CellSize / 2.0f;
        Vector3 mazeOrigin = new(-xOffset, 0f, -zOffset);


        for (int x = 0; x < mazeGenerator.mazeWidth; x++)
        {
            for (int y = 0; y < mazeGenerator.mazeHeight; y++)
            {
                // ���� mazeOrigin�� �������� ���ο� ���� ��ġ�� ���
                Vector3 cellPosition = new Vector3((float)x * CellSize, 0f, (float)y * CellSize) + mazeOrigin;

                // MazeRenderer ������Ʈ�� �ڽ����� ���ο� �̷� �� ������ �ν��Ͻ�ȭ
                GameObject newCell = Instantiate(MazeCellPrefab, cellPosition, Quaternion.identity, transform);

                // �ٴ� Ÿ�� ��ġ ��Ͽ� �߰�
                floorPositions.Add(cellPosition);

                // ���� MazeCellPrefab ��ũ��Ʈ�� ���� ������ ������
                MazeCellObject mazeCell = newCell.GetComponent<MazeCellObject>();

                // � ���� Ȱ��ȭ�Ǿ�� �ϴ��� ����
                bool top = maze[x, y].topWall;
                bool left = maze[x, y].leftWall;

                // �⺻������ �Ʒ��ʰ� ������ ���� ��Ȱ��ȭ������, �츮�� �̷��� �Ʒ��� �Ǵ� ������ �����ڸ��� ���� ��� Ȱ��ȭ
                bool right = false;
                bool bottom = false;
                if (x == mazeGenerator.mazeWidth - 1) right = true;
                if (y == 0) bottom = true;

                // �̷� ���� �ʱ�ȭ
                mazeCell.Init(top, bottom, right, left);
            }
        }

        // �̷ΰ� ������ �� NavMesh ����ũ
        BakeNavMesh();
    }

    public void BakeNavMesh()
    {
        // NavMeshSurface ��ü�� �Ҵ�Ǿ����� Ȯ��
        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        // NavMesh ����
        navMeshSurface.BuildNavMesh();

        // NavMesh�� ����� �� ������Ʈ�� NavMesh ���� ����
        StartCoroutine(CreateAgentOnNavMesh());
    }

    private IEnumerator CreateAgentOnNavMesh()
    {
        // NavMesh�� ����� ������ ��� ���
        yield return new WaitForEndOfFrame();

        // ������ ��ġ���� ������Ʈ�� �����ϴ� ����
        for (int i = 0; i < RespawnEnemy; i++)  // 10���� ������Ʈ�� ������ ��ġ�� ����
        {
            Vector3 randomPosition = GetRandomPositionInMaze();

            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                Instantiate(agentPrefab, hit.position, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Failed to place agent on NavMesh at position: " + randomPosition);
                i--;
            }
        }
    }

    private Vector3 GetRandomPositionInMaze()
    {
        float x = Random.Range(0, mazeGenerator.mazeWidth) * CellSize;
        float z = Random.Range(0, mazeGenerator.mazeHeight) * CellSize;

        // �������� �����Ͽ� �̷��� �߾��� �������� ����
        float xOffset = (mazeGenerator.mazeWidth - 1) * CellSize / 2.0f;
        float zOffset = (mazeGenerator.mazeHeight - 1) * CellSize / 2.0f;
        Vector3 mazeOrigin = new(-xOffset, 0f, -zOffset);

        return new Vector3(x, 0f, z) + mazeOrigin;
    }
}
