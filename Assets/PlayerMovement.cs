using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private AnimatorIDs animIDs;
    private CameraController cameraController; // Tham chiếu đến camera controller
    private Transform cameraTransform;

    [Header("Movement Settings")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 5.0f;
    public float thirdPersonRotationSpeed = 10.0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        // Tìm camera controller và transform của camera chính
        cameraController = Camera.main.GetComponent<CameraController>();
        cameraTransform = Camera.main.transform;
    }

    void Start()
    {
        // ... (phần lấy AnimatorIDs giữ nguyên) ...
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        if (gameControllerObject != null) animIDs = gameControllerObject.GetComponent<AnimatorIDs>();
        else Debug.LogError("Không tìm thấy Game Controller!");
    }

    void Update()
    {
        if (cameraController == null) return; // An toàn nếu không tìm thấy camera

        // --- LẤY INPUT ---
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentMoveSpeed = isRunning ? runSpeed : walkSpeed;

        // --- PHÂN LOẠI HÀNH VI DỰA TRÊN CHẾ ĐỘ CAMERA ---
        if (cameraController.CurrentCameraMode == CameraController.CameraMode.FirstPerson)
        {
            HandleFirstPersonMovement(verticalInput, horizontalInput, currentMoveSpeed);
        }
        else // Áp dụng cho Orbit, Fixed, và LookAt
        {
            HandleThirdPersonMovement(verticalInput, horizontalInput, currentMoveSpeed);
        }
    }

    // --- CÁC HÀM XỬ LÝ RIÊNG BIỆT ---

    void HandleThirdPersonMovement(float v, float h, float speed)
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * v + cameraRight * h).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            transform.position += moveDirection * speed * Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, thirdPersonRotationSpeed * Time.deltaTime);
        }

        float animationSpeed = speed * moveDirection.magnitude;
        animator.SetFloat(animIDs.speedFloat, animationSpeed);
    }

    void HandleFirstPersonMovement(float v, float h, float speed)
    {
        Vector3 moveDirection = (transform.forward * v + transform.right * h).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            transform.position += moveDirection * speed * Time.deltaTime;
        }

        // Với FPS, animation phức tạp hơn (cần blend đi ngang, đi lùi)
        // Tạm thời, chúng ta dùng logic đơn giản để nó chạy được
        float totalInputMagnitude = new Vector2(h, v).normalized.magnitude;
        float animationSpeed = speed * totalInputMagnitude;
        animator.SetFloat(animIDs.speedFloat, animationSpeed);
    }
}