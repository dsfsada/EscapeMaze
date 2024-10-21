using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MazeGenerator : MonoBehaviour
{
    [Range(5, 500)]
    public int mazeWidth = 5, mazeHeight = 5;       // 미로의 크기.
    public int startX, startY;                      // 알고리즘이 시작할 위치.
    MazeCell[,] maze;                               // 미로 그리드를 나타내는 미로 셀 배열.

    Vector2Int currentCell;                         // 현재 보고 있는 미로 셀.

    public MazeCell[,] GetMazes()
    {
        maze = new MazeCell[mazeWidth, mazeHeight];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y] = new MazeCell(x, y);
            }
        }

        // 미로 경로를 시작 위치에서부터 새기기 시작.
        CarvePath(startX, startY);

        return maze;
    }

    readonly List<Direction> directions = new()
    {
        Direction.Up, Direction.Down, Direction.Left, Direction.Right
    };

    List<Direction> GetRandomDirections()
    {
        // 조작할 수 있도록 방향 리스트의 복사본 만들기.
        List<Direction> dir = new(directions);

        // 랜덤화된 방향을 넣을 방향 리스트 만들기.
        List<Direction> rndDir = new();

        while (dir.Count > 0)                            // rndDir 리스트가 비어있을 때까지 반복.
        {
            int rnd = Random.Range(0, dir.Count);       // 리스트에서 랜덤 인덱스 선택.
            rndDir.Add(dir[rnd]);                       // 랜덤 방향을 리스트에 추가.
            dir.RemoveAt(rnd);                          // 해당 방향을 제거하여 다시 선택할 수 없게 함.
        }

        // 네 방향을 모두 랜덤한 순서로 얻었다면, 큐를 반환.
        return rndDir;
    }

    bool IsCellValid(int x, int y)
    {
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1 || maze[x, y].visited) return false;
        else return true;
    }

    void BreakWalls(Vector2Int primaryCell, Vector2Int secondaryCell)
    {
        // Primary Cell's Left Wall
        if (primaryCell.x > secondaryCell.x)
        {
            maze[primaryCell.x, primaryCell.y].leftWall = false;
        }
        // Secondary Cell's Left Wall
        else if (primaryCell.x < secondaryCell.x)
        {
            maze[secondaryCell.x, secondaryCell.y].leftWall = false;
        }
        // Primary Cell's Top Wall
        else if (primaryCell.y < secondaryCell.y)
        {
            maze[primaryCell.x, primaryCell.y].topWall = false;
        }
        // Secondary Cell's Top Wall
        else if (primaryCell.y > secondaryCell.y)
        {
            maze[secondaryCell.x, secondaryCell.y].topWall = false;
        }
    }

    Vector2Int CheckNeighbours()
    {
        List<Direction> rndDir = GetRandomDirections();

        for (int i = 0; i < rndDir.Count; i++)
        {
            // Set Neighbour coordinates to current cell for now.
            Vector2Int neighbour = currentCell;

            // Modify neighbour coordinates based on the random directions we're currently trying.
            switch (rndDir[i])
            {
                case Direction.Up:
                    neighbour.y++;
                    break;
                case Direction.Down:
                    neighbour.y--;
                    break;
                case Direction.Right:
                    neighbour.x++;
                    break;
                case Direction.Left:
                    neighbour.x--;
                    break;
            }

            // If the neighbour we just tried is valid, we can return that neighbour. If not, we go again.
            if (IsCellValid(neighbour.x, neighbour.y)) return neighbour;
        }

        // If we tried all directions and didn't find a valid neighbour, we return the currentCell values.
        return currentCell;
    }


    void CarvePath(int x, int y)
    {
        // 시작 위치가 맵 경계를 벗어나는지 빠르게 확인,
        // 그렇다면 기본값(여기서는 0)으로 설정하고 경고 메시지 표시.
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1)
        {
            x = y = 0;
            Debug.LogWarning("시작 위치가 경계를 벗어났습니다. 기본값 0, 0으로 설정합니다.");
        }

        // 현재 셀을 전달받은 시작 위치로 설정.
        currentCell = new Vector2Int(x, y);

        // 현재 경로를 추적할 리스트.
        List<Vector2Int> path = new();

        // 막다른 길을 만날 때까지 반복.
        bool deadEnd = false;
        while (!deadEnd)
        {
            // 시도할 다음 셀 얻기.
            Vector2Int nextCell = CheckNeighbours();

            // 유효한 이웃이 없는 경우, deadEnd를 true로 설정하여 루프 종료.
            if (nextCell == currentCell)
            {
                // 유효한 이웃이 없는 경우, deadEnd를 true로 설정하여 루프 종료.
                for (int i = path.Count - 1; i >= 0; i--)
                {
                    currentCell = path[i];
                    path.RemoveAt(i);
                    nextCell = CheckNeighbours();

                    // 유효한 이웃을 찾으면 루프 탈출.
                    if (nextCell != currentCell) break;
                }

                if (nextCell == currentCell)
                    deadEnd = true;
            }
            else
            {
                BreakWalls(currentCell, nextCell);                      // 두 셀의 벽 플래그 설정.
                maze[currentCell.x, currentCell.y].visited = true;      // 이동하기 전에 셀을 방문으로 설정.
                currentCell = nextCell;                                 // 현재 셀을 찾은 유효한 이웃으로 설정.
                path.Add(currentCell);                                  // 이 셀을 경로에 추가.
            }
        }
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class MazeCell
{
    public bool visited;
    public int x, y;

    public bool topWall;
    public bool leftWall;

    // 편의를 위해 x와 y를 Vector2Int로 반환.
    public Vector2Int Position
    {
        get
        {
            return new Vector2Int(x, y);
        }
    }

    public MazeCell(int x, int y)
    {
        // 미로 그리드에서 이 셀의 좌표
        this.x = x;
        this.y = y;

        // 알고리즘이 이 셀을 방문했는지 여부 - 처음에는 false
        visited = false;

        // 알고리즘이 벽을 제거할 때까지 모든 벽이 존재.
        topWall = leftWall = true;
    }
}
