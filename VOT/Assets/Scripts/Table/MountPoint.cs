using UnityEngine;

public class MountPoint : MonoBehaviour
{
    [Header("Mount Point Info")]
    public string mountPointId = "mount_1";
    public string displayName = "Punkt montażowy";
    
    [Header("Compatibility")]
    public string[] compatibleAccessoryTypes = new string[] { "armSupport", "legHolder" };
    
    [Header("Status")]
    [Tooltip("Aktualne akcesoria podłączone")]
    public GameObject attachedAccessory = null;
    
    public bool IsOccupied 
    { 
        get { return attachedAccessory != null; } 
    }
    
    /// <summary>
    /// Sprawdza czy dane akcesorium może być podłączone
    /// </summary>
    public bool CanAttach(GameObject accessory)
    {
        if (IsOccupied) return false;
        
        TableElement element = accessory.GetComponent<TableElement>();
        if (element == null) return false;
        if (element.type != TableElement.ElementType.Accessory) return false;
        
        // Sprawdź kompatybilność typów (opcjonalne)
        if (compatibleAccessoryTypes.Length == 0) return true;
        
        foreach (string compatibleType in compatibleAccessoryTypes)
        {
            if (element.elementId.Contains(compatibleType))
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Podłącza akcesorium
    /// </summary>
    public bool Attach(GameObject accessory)
    {
        if (!CanAttach(accessory))
        {
            Debug.LogWarning("[MountPoint] Nie można podłączyć akcesoria");
            return false;
        }
        
        attachedAccessory = accessory;
        accessory.transform.SetParent(transform);
        accessory.transform.localPosition = Vector3.zero;
        accessory.transform.localRotation = Quaternion.identity;
        
        Debug.Log("[MountPoint] Podłączono: " + accessory.name);
        return true;
    }
    
    /// <summary>
    /// Odłącza akcesorium
    /// </summary>
    public void Detach()
    {
        if (attachedAccessory != null)
        {
            Debug.Log("[MountPoint] Odłączono: " + attachedAccessory.name);
            attachedAccessory.transform.SetParent(null);
            attachedAccessory = null;
        }
    }
}