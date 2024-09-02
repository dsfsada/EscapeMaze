using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraMove : MonoBehaviour
{
    public Camera minimapCamera;
    public Transform PlayerPos;

    private void Start()
    {
        minimapCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        minimapCamera.transform.position = new Vector3(PlayerPos.position.x, 50, PlayerPos.position.z);
    }
}
