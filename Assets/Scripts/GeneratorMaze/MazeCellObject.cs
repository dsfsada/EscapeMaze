using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCellObject : MonoBehaviour
{
    [SerializeField] GameObject topWall;      // ���� �� ���� ������Ʈ
    [SerializeField] GameObject bottomWall;   // �Ʒ��� �� ���� ������Ʈ
    [SerializeField] GameObject leftWall;     // ���� �� ���� ������Ʈ
    [SerializeField] GameObject rightWall;    // ������ �� ���� ������Ʈ

    // ���� Ȱ�� ���¸� �����ϴ� �ʱ�ȭ �Լ�
    public void Init(bool top, bool bottom, bool right, bool left)
    {
        topWall.SetActive(top);        // ���� �� Ȱ��ȭ/��Ȱ��ȭ
        bottomWall.SetActive(bottom);  // �Ʒ��� �� Ȱ��ȭ/��Ȱ��ȭ
        rightWall.SetActive(right);    // ������ �� Ȱ��ȭ/��Ȱ��ȭ
        leftWall.SetActive(left);      // ���� �� Ȱ��ȭ/��Ȱ��ȭ
    }
}
