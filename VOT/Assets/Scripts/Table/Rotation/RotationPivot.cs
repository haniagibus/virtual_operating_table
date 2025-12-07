using UnityEngine;

public class RotationPivot : MonoBehaviour
{
    public string pivotName = "Default Pivot";

    [Header("Allowed Axes")]
    public bool allowX = false;
    public bool allowY = true;
    public bool allowZ = false;

    [Header("X Axis Rotation Limits")]
    public float minAngleX = -180f;
    public float maxAngleX = 180f;

    [Header("Y Axis Rotation Limits")]
    public float minAngleY = -180f;
    public float maxAngleY = 180f;

    [Header("Z Axis Rotation Limits")]
    public float minAngleZ = -180f;
    public float maxAngleZ = 180f;

    [Header("Current Angles (Read Only)")]
    [HideInInspector]
    public float currentAngleX = 0f;
    [HideInInspector]
    public float currentAngleY = 0f;
    [HideInInspector]
    public float currentAngleZ = 0f;

    void Start()
    {
        // Inicjalizuj kąty z aktualnej rotacji obiektu
        Vector3 euler = transform.localEulerAngles;
        
        currentAngleX = NormalizeAngle(euler.x);
        currentAngleY = NormalizeAngle(euler.y);
        currentAngleZ = NormalizeAngle(euler.z);
        
        Debug.Log(pivotName + " - Kąty początkowe: X=" + currentAngleX.ToString("F1") + 
                  ", Y=" + currentAngleY.ToString("F1") + 
                  ", Z=" + currentAngleZ.ToString("F1"));
    }

    // ============================================================
    // GŁÓWNA FUNKCJA - Obrót z Vector3 jako osią
    // ZWRACA: true jeśli obrót wykonany, false jeśli osiągnięto limit
    // ============================================================
    public bool RotateWithVector3(Vector3 axis, float delta)
    {
        // Określ którą oś używamy na podstawie Vector3
        char detectedAxis = DetectAxis(axis);
        
        if (detectedAxis == '?')
        {
            Debug.LogWarning("Nieznana oś: " + axis);
            return false;
        }

        // Pobierz limity i aktualny kąt dla tej osi
        float currentAngle = 0f;
        float minAngle = -180f;
        float maxAngle = 180f;
        bool allowed = false;

        switch (detectedAxis)
        {
            case 'X':
                currentAngle = currentAngleX;
                minAngle = minAngleX;
                maxAngle = maxAngleX;
                allowed = allowX;
                break;
            case 'Y':
                currentAngle = currentAngleY;
                minAngle = minAngleY;
                maxAngle = maxAngleY;
                allowed = allowY;
                break;
            case 'Z':
                currentAngle = currentAngleZ;
                minAngle = minAngleZ;
                maxAngle = maxAngleZ;
                allowed = allowZ;
                break;
        }

        // Sprawdź czy oś jest dozwolona
        if (!allowed)
        {
            Debug.LogWarning("Oś " + detectedAxis + " jest wyłączona dla " + pivotName);
            return false;
        }

        // Oblicz nowy kąt
        float newAngle = currentAngle + delta;
        bool hitLimit = false;

        // Sprawdź limity
        if (newAngle > maxAngle)
        {
            Debug.Log(pivotName + " - Osiągnięto maksymalny kąt na osi " + detectedAxis + ": " + maxAngle.ToString("F1") + "°");
            newAngle = maxAngle;
            delta = maxAngle - currentAngle;
            hitLimit = true;
        }
        else if (newAngle < minAngle)
        {
            Debug.Log(pivotName + " - Osiągnięto minimalny kąt na osi " + detectedAxis + ": " + minAngle.ToString("F1") + "°");
            newAngle = minAngle;
            delta = minAngle - currentAngle;
            hitLimit = true;
        }

        // Wykonaj obrót (tylko jeśli delta != 0)
        if (Mathf.Abs(delta) > 0.001f)
        {
            transform.Rotate(axis, delta, Space.Self);

            // Zaktualizuj zapisany kąt
            switch (detectedAxis)
            {
                case 'X':
                    currentAngleX = newAngle;
                    break;
                case 'Y':
                    currentAngleY = newAngle;
                    break;
                case 'Z':
                    currentAngleZ = newAngle;
                    break;
            }

            Debug.Log(pivotName + " - Oś " + detectedAxis + ": " + currentAngle.ToString("F1") + 
                     "° → " + newAngle.ToString("F1") + "° (delta: " + delta.ToString("F1") + "°)");
        }

        // Zwróć false jeśli osiągnięto limit (zatrzyma coroutine)
        return !hitLimit;
    }

    // ============================================================
    // HELPER - Wykrywa którą oś reprezentuje Vector3
    // ============================================================
    private char DetectAxis(Vector3 axis)
    {
        // Normalizuj wektor
        axis = axis.normalized;

        // Sprawdź który komponent jest dominujący
        float absX = Mathf.Abs(axis.x);
        float absY = Mathf.Abs(axis.y);
        float absZ = Mathf.Abs(axis.z);

        if (absX > absY && absX > absZ)
            return 'X';
        else if (absY > absX && absY > absZ)
            return 'Y';
        else if (absZ > absX && absZ > absY)
            return 'Z';
        
        return '?'; // Nieznana oś
    }

    // ============================================================
    // HELPER - Normalizacja kątów z [0, 360] do [-180, 180]
    // ============================================================
    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            return angle - 360f;
        return angle;
    }

    // ============================================================
    // FUNKCJE POMOCNICZE - sprawdzanie limitów
    // ============================================================
    public bool IsAtLimitX()
    {
        return currentAngleX <= minAngleX || currentAngleX >= maxAngleX;
    }

    public bool IsAtLimitY()
    {
        return currentAngleY <= minAngleY || currentAngleY >= maxAngleY;
    }

    public bool IsAtLimitZ()
    {
        return currentAngleZ <= minAngleZ || currentAngleZ >= maxAngleZ;
    }

    public bool IsAtLimit(char axis)
    {
        switch (axis)
        {
            case 'X':
            case 'x':
                return IsAtLimitX();
            case 'Y':
            case 'y':
                return IsAtLimitY();
            case 'Z':
            case 'z':
                return IsAtLimitZ();
            default:
                return false;
        }
    }
}