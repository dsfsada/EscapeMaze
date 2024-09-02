using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POVCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Vector3 cameraOffset = new(0, .3f, 0); // ī�޶� ��ġ ������

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        // ī�޶��� ȸ�� ����
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // �÷��̾��� ȸ�� ����
        playerBody.localRotation = Quaternion.Euler(0f, yRotation, 0f);

        // ī�޶� ��ġ�� �÷��̾� ��ġ�� ����ȭ
        transform.position = playerBody.position + cameraOffset;
    }
}
