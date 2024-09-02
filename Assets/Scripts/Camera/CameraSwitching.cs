using UnityEngine;

public class CameraSwitching : MonoBehaviour
{
    public Camera povCamera;
    public Camera topDownCamera;
    public KeyCode switchKey = KeyCode.C;

    private bool isThirdPersonView = true;

    public ControllerCharacter characterController;

    void Start()
    {
        // Initialize cameras
        povCamera.enabled = false;
        topDownCamera.enabled = true;
        characterController.SetViewMode(isThirdPersonView);
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            isThirdPersonView = !isThirdPersonView;
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        povCamera.enabled = !isThirdPersonView;
        topDownCamera.enabled = isThirdPersonView;
        characterController.SetViewMode(isThirdPersonView);
    }
}
