using UnityEngine;

public class TableElement : MonoBehaviour
{
    [Header("Identification")]
    public string elementId = "element_1";
    public string displayName = "Element stołu";
    
    [Header("Element Type")]
    public ElementType type = ElementType.Component;
    
    [Header("Optional")]
    [Tooltip("Grupowanie elementów (np. 'backSection', 'legSection')")]
    public string groupId = "";
    
    public enum ElementType
    {
        Component,      // Stały element stołu
        Accessory      // Akcesoria do podłączania]
    }
    
    // Helper - pobierz wszystkie pivoty na tym elemencie
    public RotationPivot[] GetRotationPivots()
    {
        return GetComponents<RotationPivot>();
    }
    
    // Helper - pobierz wszystkie osie ruchu
    public MovementAxis[] GetMovementAxes()
    {
        return GetComponents<MovementAxis>();
    }
    
    // Helper - pobierz punkty montażowe
    public MountPoint[] GetMountPoints()
    {
        return GetComponentsInChildren<MountPoint>();
    }
}