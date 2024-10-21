using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POVCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Vector3 cameraOffset = new(0, .3f, 0); // 카메라 위치 오프셋

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

        // 카메라의 회전 설정
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // 플레이어의 회전 설정
        playerBody.localRotation = Quaternion.Euler(0f, yRotation, 0f);

        // 카메라 위치를 플레이어 위치에 동기화
        transform.position = playerBody.position + cameraOffset;
    }
}
