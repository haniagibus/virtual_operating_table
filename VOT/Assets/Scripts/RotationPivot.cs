using UnityEngine;

public class RotationPivot : MonoBehaviour
{
    public string pivotName = "Default Pivot";

    public bool allowX = false;
    public bool allowY = true;
    public bool allowZ = false;

    [Header("Rotation Limits")]
    public float minAngle = -180f;    // Minimalny kąt obrotu
    public float maxAngle = 180f;  // Maksymalny kąt obrotu

    [HideInInspector]
    public float currentAngle = 0f; // Aktualny kąt
}
