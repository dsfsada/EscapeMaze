using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCellObject : MonoBehaviour
{
    [SerializeField] GameObject topWall;      // 위쪽 벽 게임 오브젝트
    [SerializeField] GameObject bottomWall;   // 아래쪽 벽 게임 오브젝트
    [SerializeField] GameObject leftWall;     // 왼쪽 벽 게임 오브젝트
    [SerializeField] GameObject rightWall;    // 오른쪽 벽 게임 오브젝트

    // 벽의 활성 상태를 설정하는 초기화 함수
    public void Init(bool top, bool bottom, bool right, bool left)
    {
        topWall.SetActive(top);        // 위쪽 벽 활성화/비활성화
        bottomWall.SetActive(bottom);  // 아래쪽 벽 활성화/비활성화
        rightWall.SetActive(right);    // 오른쪽 벽 활성화/비활성화
        leftWall.SetActive(left);      // 왼쪽 벽 활성화/비활성화
    }
}
