using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCharacter : MonoBehaviour
{
    #region Variables
    public float speed = 5f;
    public float jumpHeight = 2f;
    public float dashDistance = 5f;

    public float gravity = -9.81f;
    public Vector3 drags;

    private CharacterController characterController;

    public Vector3 inputDirection = Vector3.zero;

    public ControllerCharacter(Vector3 inputDirection)
    {
        this.inputDirection = inputDirection;
    }

    private Vector3 calcVelocity = Vector3.zero;

    [SerializeField]
    private bool isGrounded = false;
    public LayerMask groundLayerMask;
    public float groundCheckDistance = 0.3f;

    public Transform povCameraTransform;
    public Transform topDownCameraTransform;

    private bool isThirdPersonView = true;
    #endregion Variables

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && calcVelocity.y < 0)
        {
            calcVelocity.y = 0;
        }

        // 사용자 입력 처리
        Vector3 move = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputDirection = move.normalized;

        if (isThirdPersonView)
        {
            Vector3 moveDirection = (topDownCameraTransform.right * move.x + topDownCameraTransform.forward * move.z).normalized;
            moveDirection.y = 0; // y 방향은 무시합니다.

            characterController.Move(speed * Time.deltaTime * moveDirection);
            if (move != Vector3.zero)
            {
                transform.forward = moveDirection;
            }
        }
        else
        {
            Vector3 moveDirection = (povCameraTransform.right * move.x + povCameraTransform.forward * move.z).normalized;
            moveDirection.y = 0; // y 방향은 무시합니다.

            characterController.Move(speed * Time.deltaTime * moveDirection);
            // 1인칭 모드에서는 오브젝트 회전을 막음
        }

        // 점프 입력 처리
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            calcVelocity.y += Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }

        // 대시 입력 처리
        if (Input.GetButtonDown("Fire3"))
        {
            Vector3 dashVelocity = transform.forward * dashDistance;
            calcVelocity += dashVelocity;
        }

        // 중력 적용
        calcVelocity.y += gravity * Time.deltaTime;

        // 드래그 적용
        calcVelocity.x /= 1 + drags.x * Time.deltaTime;
        calcVelocity.y /= 1 + drags.y * Time.deltaTime;
        calcVelocity.z /= 1 + drags.z * Time.deltaTime;

        characterController.Move(calcVelocity * Time.deltaTime);
    }

    #region Helper Methods
    public void SetViewMode(bool thirdPersonView)
    {
        isThirdPersonView = thirdPersonView;
    }
    #endregion Helper Methods
}
