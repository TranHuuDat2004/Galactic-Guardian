using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraMode { LookAt, FollowFixed, FollowOrbit, FirstPerson }

    [Header("Target")]
    public Transform player;

    [Header("Settings")]
    public CameraMode currentMode = CameraMode.FollowOrbit;
    public KeyCode switchKey = KeyCode.C;

    [Header("Mouse Settings (Shared)")]
    public float mouseSensitivity = 4.0f;
    public float yMinLimit = -60f;
    public float yMaxLimit = 80f;

    [Header("Orbit (Third Person) Settings")]
    public float distance = 5.0f;
    public float orbitDampening = 10.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;

    [Header("First Person Settings")]
    public Vector3 firstPersonOffset = new Vector3(0, 1.6f, 0.3f);
    
    public CameraMode CurrentCameraMode => currentMode;

    private Vector3 fixedOffset;
    private Vector3 localRot; // Biến xoay chính, dùng cho TẤT CẢ các chế độ
    private Vector3 initialPosition;
    private SkinnedMeshRenderer playerMeshRenderer;
    private CameraMode previousMode;

    void Start()
    {
        if (player != null)
        {
            fixedOffset = transform.position - player.position;
            playerMeshRenderer = player.GetComponentInChildren<SkinnedMeshRenderer>();
        }
        initialPosition = transform.position;
        previousMode = currentMode;
        UpdateCursorState();
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            if (currentMode == CameraMode.LookAt) currentMode = CameraMode.FollowFixed;
            else if (currentMode == CameraMode.FollowFixed) currentMode = CameraMode.FollowOrbit;
            else if (currentMode == CameraMode.FollowOrbit) currentMode = CameraMode.FirstPerson;
            else if (currentMode == CameraMode.FirstPerson) currentMode = CameraMode.LookAt;
        }

        if (previousMode != currentMode)
        {
            UpdateCursorState();
            previousMode = currentMode;
        }
        
        // Luôn cho phép xoay ở chế độ FPS, hoặc khi giữ chuột phải ở các chế độ khác
        if (currentMode == CameraMode.FirstPerson || Input.GetMouseButton(1))
        {
            HandleMouseRotation();
        }
        
        if (currentMode != CameraMode.FirstPerson)
        {
            if (Input.GetMouseButtonDown(1)) UpdateCursorState(true);
            if (Input.GetMouseButtonUp(1)) UpdateCursorState(false);
        }
    }

    void LateUpdate()
    {
        if (player == null) return;
        
        if (playerMeshRenderer != null)
        {
            playerMeshRenderer.enabled = (currentMode != CameraMode.FirstPerson);
        }

        switch (currentMode)
        {
            case CameraMode.LookAt:
                HandleStationaryRotationMode();
                break;
            case CameraMode.FollowFixed:
                HandleFollowFixedRotationMode();
                break;
            case CameraMode.FollowOrbit:
                HandleOrbitMode();
                break;
            case CameraMode.FirstPerson:
                HandleFirstPersonMode();
                break;
        }
    }

    // --- CÁC HÀM XỬ LÝ ---

    void HandleMouseRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        localRot.x += mouseX;
        localRot.y -= mouseY;
        localRot.y = Mathf.Clamp(localRot.y, yMinLimit, yMaxLimit);
    }
    
    // Chế độ camera đứng yên nhưng xoay được
    void HandleStationaryRotationMode()
    {
        transform.position = initialPosition;
        transform.rotation = Quaternion.Euler(localRot.y, localRot.x, 0);
    }

    // Chế độ camera đi theo nhưng giữ góc quay riêng
    void HandleFollowFixedRotationMode()
    {
        transform.position = player.position + fixedOffset;
        transform.rotation = Quaternion.Euler(localRot.y, localRot.x, 0);
    }

    // Chế độ camera xoay quanh người chơi
    void HandleOrbitMode()
    {
        Quaternion qt = Quaternion.Euler(localRot.y, localRot.x, 0);
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        distance = Mathf.Clamp(distance - scrollInput, minDistance, maxDistance);
        Vector3 desiredPosition = player.position - (qt * Vector3.forward * distance);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * orbitDampening);
        transform.LookAt(player.position);
    }

    // Chế độ góc nhìn thứ nhất
    void HandleFirstPersonMode()
    {
        // Lấy sự thay đổi của trục X từ input (không phải từ localRot)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        
        // Xoay người chơi sang trái/phải
        player.Rotate(Vector3.up * mouseX);

        // Xoay camera lên/xuống bằng giá trị đã tính trong HandleMouseRotation
        transform.localRotation = Quaternion.Euler(localRot.y, 0, 0);
        
        transform.position = player.position + player.TransformDirection(firstPersonOffset);
    }
    
    void UpdateCursorState(bool forceLock = false)
    {
        if (currentMode == CameraMode.FirstPerson || forceLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}