using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MazeGenerator : MonoBehaviour
{
    [Range(5, 500)]
    public int mazeWidth = 5, mazeHeight = 5;       // �̷��� ũ��.
    public int startX, startY;                      // �˰����� ������ ��ġ.
    MazeCell[,] maze;                               // �̷� �׸��带 ��Ÿ���� �̷� �� �迭.

    Vector2Int currentCell;                         // ���� ���� �ִ� �̷� ��.

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

        // �̷� ��θ� ���� ��ġ�������� ����� ����.
        CarvePath(startX, startY);

        return maze;
    }

    readonly List<Direction> directions = new()
    {
        Direction.Up, Direction.Down, Direction.Left, Direction.Right
    };

    List<Direction> GetRandomDirections()
    {
        // ������ �� �ֵ��� ���� ����Ʈ�� ���纻 �����.
        List<Direction> dir = new(directions);

        // ����ȭ�� ������ ���� ���� ����Ʈ �����.
        List<Direction> rndDir = new();

        while (dir.Count > 0)                            // rndDir ����Ʈ�� ������� ������ �ݺ�.
        {
            int rnd = Random.Range(0, dir.Count);       // ����Ʈ���� ���� �ε��� ����.
            rndDir.Add(dir[rnd]);                       // ���� ������ ����Ʈ�� �߰�.
            dir.RemoveAt(rnd);                          // �ش� ������ �����Ͽ� �ٽ� ������ �� ���� ��.
        }

        // �� ������ ��� ������ ������ ����ٸ�, ť�� ��ȯ.
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
        // ���� ��ġ�� �� ��踦 ������� ������ Ȯ��,
        // �׷��ٸ� �⺻��(���⼭�� 0)���� �����ϰ� ��� �޽��� ǥ��.
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1)
        {
            x = y = 0;
            Debug.LogWarning("���� ��ġ�� ��踦 ������ϴ�. �⺻�� 0, 0���� �����մϴ�.");
        }

        // ���� ���� ���޹��� ���� ��ġ�� ����.
        currentCell = new Vector2Int(x, y);

        // ���� ��θ� ������ ����Ʈ.
        List<Vector2Int> path = new();

        // ���ٸ� ���� ���� ������ �ݺ�.
        bool deadEnd = false;
        while (!deadEnd)
        {
            // �õ��� ���� �� ���.
            Vector2Int nextCell = CheckNeighbours();

            // ��ȿ�� �̿��� ���� ���, deadEnd�� true�� �����Ͽ� ���� ����.
            if (nextCell == currentCell)
            {
                // ��ȿ�� �̿��� ���� ���, deadEnd�� true�� �����Ͽ� ���� ����.
                for (int i = path.Count - 1; i >= 0; i--)
                {
                    currentCell = path[i];
                    path.RemoveAt(i);
                    nextCell = CheckNeighbours();

                    // ��ȿ�� �̿��� ã���� ���� Ż��.
                    if (nextCell != currentCell) break;
                }

                if (nextCell == currentCell)
                    deadEnd = true;
            }
            else
            {
                BreakWalls(currentCell, nextCell);                      // �� ���� �� �÷��� ����.
                maze[currentCell.x, currentCell.y].visited = true;      // �̵��ϱ� ���� ���� �湮���� ����.
                currentCell = nextCell;                                 // ���� ���� ã�� ��ȿ�� �̿����� ����.
                path.Add(currentCell);                                  // �� ���� ��ο� �߰�.
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

    // ���Ǹ� ���� x�� y�� Vector2Int�� ��ȯ.
    public Vector2Int Position
    {
        get
        {
            return new Vector2Int(x, y);
        }
    }

    public MazeCell(int x, int y)
    {
        // �̷� �׸��忡�� �� ���� ��ǥ
        this.x = x;
        this.y = y;

        // �˰����� �� ���� �湮�ߴ��� ���� - ó������ false
        visited = false;

        // �˰����� ���� ������ ������ ��� ���� ����.
        topWall = leftWall = true;
    }
}
