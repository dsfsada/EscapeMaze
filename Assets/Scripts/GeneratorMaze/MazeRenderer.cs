using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MazeRender : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;   // 미로 생성기
    [SerializeField] GameObject MazeCellPrefab;     // 미로 셀 프리팹
    [SerializeField] GameObject agentPrefab;        // 에이전트 프리팹
    [SerializeField] int RespawnEnemy;              // 생성할 적의 수

    public NavMeshSurface navMeshSurface;           // 내비게이션 메시 서피스

    // 이것은 미로 셀의 실제 크기입니다. 이를 잘못 설정하면 셀 사이에 겹침이나 틈이 생길 수 있습니다.
    public float CellSize = 1f;

    public List<Vector3> floorPositions = new();

    private void Start()
    {
        MazeCell[,] maze = mazeGenerator.GetMazes();  // 미로 생성기에서 미로 데이터를 가져옴

        CellSize = MazeCellPrefab.transform.localScale.x;

        // 미로를 (0, 0, 0)에 중심에 맞추기 위한 오프셋 계산
        float xOffset = (mazeGenerator.mazeWidth - 1) * CellSize / 2.0f;
        float zOffset = (mazeGenerator.mazeHeight - 1) * CellSize / 2.0f;
        Vector3 mazeOrigin = new(-xOffset, 0f, -zOffset);


        for (int x = 0; x < mazeGenerator.mazeWidth; x++)
        {
            for (int y = 0; y < mazeGenerator.mazeHeight; y++)
            {
                // 계산된 mazeOrigin을 기준으로 새로운 셀의 위치를 계산
                Vector3 cellPosition = new Vector3((float)x * CellSize, 0f, (float)y * CellSize) + mazeOrigin;

                // MazeRenderer 오브젝트의 자식으로 새로운 미로 셀 프리팹 인스턴스화
                GameObject newCell = Instantiate(MazeCellPrefab, cellPosition, Quaternion.identity, transform);

                // 바닥 타일 위치 목록에 추가
                floorPositions.Add(cellPosition);

                // 셀의 MazeCellPrefab 스크립트에 대한 참조를 가져옴
                MazeCellObject mazeCell = newCell.GetComponent<MazeCellObject>();

                // 어떤 벽이 활성화되어야 하는지 결정
                bool top = maze[x, y].topWall;
                bool left = maze[x, y].leftWall;

                // 기본적으로 아래쪽과 오른쪽 벽은 비활성화되지만, 우리가 미로의 아래쪽 또는 오른쪽 가장자리에 있을 경우 활성화
                bool right = false;
                bool bottom = false;
                if (x == mazeGenerator.mazeWidth - 1) right = true;
                if (y == 0) bottom = true;

                // 미로 셀을 초기화
                mazeCell.Init(top, bottom, right, left);
            }
        }

        // 미로가 생성된 후 NavMesh 베이크
        BakeNavMesh();
    }

    public void BakeNavMesh()
    {
        // NavMeshSurface 객체가 할당되었는지 확인
        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface가 할당되지 않았습니다!");
            return;
        }

        // NavMesh 생성
        navMeshSurface.BuildNavMesh();

        // NavMesh가 빌드된 후 에이전트를 NavMesh 위에 생성
        StartCoroutine(CreateAgentOnNavMesh());
    }

    private IEnumerator CreateAgentOnNavMesh()
    {
        // NavMesh가 빌드될 때까지 잠시 대기
        yield return new WaitForEndOfFrame();

        // 무작위 위치에서 에이전트를 생성하는 루프
        for (int i = 0; i < RespawnEnemy; i++)  // 10개의 에이전트를 무작위 위치에 생성
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

        // 오프셋을 적용하여 미로의 중앙을 원점으로 설정
        float xOffset = (mazeGenerator.mazeWidth - 1) * CellSize / 2.0f;
        float zOffset = (mazeGenerator.mazeHeight - 1) * CellSize / 2.0f;
        Vector3 mazeOrigin = new(-xOffset, 0f, -zOffset);

        return new Vector3(x, 0f, z) + mazeOrigin;
    }
}
