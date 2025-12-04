using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;
    private bool isCursorLocked = true;

    private bool menuActive = false;
    private bool HandPanelActive = false;

    void Start()
    {
        LockCursor();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            menuActive = !menuActive;
            UpdateCursorState();
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            HandPanelActive = !HandPanelActive;
            UpdateCursorState();
        }
        
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
    
    void UpdateCursorState()
    {
        if (menuActive || HandPanelActive)
        {
            isCursorLocked = false;
            UnlockCursorButKeepInvisible(); 
        }
        else
        {
            isCursorLocked = true;
            LockCursor();
        }
    }
    
    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void UnlockCursorButKeepInvisible()
    {
        Cursor.lockState = CursorLockMode.Confined;  
        Cursor.visible = false;                       
    }
}