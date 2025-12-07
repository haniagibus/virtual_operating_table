using UnityEngine;

public class MovementAxis : MonoBehaviour
{
    public string axisName = "Default Movement";

    [Header("Allowed Axes")]
    public bool allowX = false;
    public bool allowY = true;
    public bool allowZ = false;

    [Header("X Axis Movement Limits (in Unity units)")]
    public float minDistanceX = 0f;
    public float maxDistanceX = 0f;

    [Header("Y Axis Movement Limits")]
    public float minDistanceY = 0f;
    public float maxDistanceY = 0f;

    [Header("Z Axis Movement Limits")]
    public float minDistanceZ = 0f;
    public float maxDistanceZ = 0f;

    [Header("Current Position (Read Only)")]
    [HideInInspector]
    public float currentPositionX = 0f;
    [HideInInspector]
    public float currentPositionY = 0f;
    [HideInInspector]
    public float currentPositionZ = 0f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
        currentPositionX = 0f;
        currentPositionY = 0f;
        currentPositionZ = 0f;
    }

    /// <summary>
    /// Przesuwa element wzdłuż osi. Zwraca false jeśli osiągnięto limit.
    /// </summary>
    public bool MoveWithVector3(Vector3 axis, float delta)
    {
        char detectedAxis = DetectAxis(axis);
        
        if (detectedAxis == '?')
        {
            Debug.LogWarning("Nieznana oś: " + axis);
            return false;
        }

        float currentPos = 0f;
        float minDist = -1f;
        float maxDist = 1f;
        bool allowed = false;

        switch (detectedAxis)
        {
            case 'X':
                currentPos = currentPositionX;
                minDist = minDistanceX;
                maxDist = maxDistanceX;
                allowed = allowX;
                break;
            case 'Y':
                currentPos = currentPositionY;
                minDist = minDistanceY;
                maxDist = maxDistanceY;
                allowed = allowY;
                break;
            case 'Z':
                currentPos = currentPositionZ;
                minDist = minDistanceZ;
                maxDist = maxDistanceZ;
                allowed = allowZ;
                break;
        }

        if (!allowed)
        {
            Debug.LogWarning("Oś " + detectedAxis + " jest wyłączona dla " + axisName);
            return false;
        }

        float newPos = currentPos + delta;
        bool hitLimit = false;

        if (newPos > maxDist)
        {
            Debug.Log(axisName + " - Osiągnięto maksymalną pozycję na osi " + detectedAxis);
            newPos = maxDist;
            delta = maxDist - currentPos;
            hitLimit = true;
        }
        else if (newPos < minDist)
        {
            Debug.Log(axisName + " - Osiągnięto minimalną pozycję na osi " + detectedAxis);
            newPos = minDist;
            delta = minDist - currentPos;
            hitLimit = true;
        }

        if (Mathf.Abs(delta) > 0.001f)
        {
            Vector3 movement = axis.normalized * delta;
            transform.localPosition += movement;

            switch (detectedAxis)
            {
                case 'X':
                    currentPositionX = newPos;
                    break;
                case 'Y':
                    currentPositionY = newPos;
                    break;
                case 'Z':
                    currentPositionZ = newPos;
                    break;
            }
        }

        return !hitLimit;
    }

    private char DetectAxis(Vector3 axis)
    {
        axis = axis.normalized;
        float absX = Mathf.Abs(axis.x);
        float absY = Mathf.Abs(axis.y);
        float absZ = Mathf.Abs(axis.z);

        if (absX > absY && absX > absZ)
            return 'X';
        else if (absY > absX && absY > absZ)
            return 'Y';
        else if (absZ > absX && absZ > absY)
            return 'Z';
        
        return '?';
    }

    public void ResetPosition()
    {
        transform.localPosition = initialPosition;
        currentPositionX = 0f;
        currentPositionY = 0f;
        currentPositionZ = 0f;
    }
}