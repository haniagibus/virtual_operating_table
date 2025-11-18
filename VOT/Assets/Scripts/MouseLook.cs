using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;
    private bool isCursorLocked = true;

      void Start()
    {
        LockCursor();
    }
    
    void Update()
    {
        // Przełączanie trybu kursora klawiszem I
        if (Input.GetKeyDown(KeyCode.I))
        {
            isCursorLocked = !isCursorLocked;
            
            if (isCursorLocked)
                LockCursor();
            else
                UnlockCursor();
        }
        
        // Obracanie kamery tylko gdy kursor jest zablokowany
        if (isCursorLocked)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
    
    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}