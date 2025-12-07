using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Teleskopowy system ruchu - sekcje wysuwają się po kolei
/// Wykorzystuje istniejące komponenty MovementAxis
/// </summary>
public class TelescopicMovement : MonoBehaviour
{
    [System.Serializable]
    public class TelescopicSection
    {
        [Tooltip("MovementAxis tej sekcji")]
        public MovementAxis movementAxis;
        
        [Tooltip("Nazwa sekcji (opcjonalne, do debugowania)")]
        public string sectionName = "";
    }
    
    [Header("Telescopic Sections")]
    [Tooltip("Lista sekcji w kolejności wysuwania (od pierwszej do ostatniej)")]
    public TelescopicSection[] sections;
    
    [Header("Movement Settings")]
    [Tooltip("Oś ruchu (domyślnie Y - do góry)")]
    public Vector3 movementAxis = Vector3.up;
    
    void Start()
    {
        ValidateSections();
    }
    
    /// <summary>
    /// Sprawdza czy wszystkie sekcje są poprawnie skonfigurowane
    /// </summary>
    private void ValidateSections()
    {
        int validCount = 0;
        foreach (TelescopicSection section in sections)
        {
            if (section.movementAxis != null)
            {
                validCount++;
                if (string.IsNullOrEmpty(section.sectionName))
                {
                    section.sectionName = section.movementAxis.gameObject.name;
                }
            }
            else
            {
                Debug.LogWarning("[TelescopicMovement] Sekcja bez przypisanego MovementAxis!");
            }
        }
        
        Debug.Log("[TelescopicMovement] Zainicjalizowano " + validCount + " sekcji teleskopowych");
    }
    
    /// <summary>
    /// Przesuwa teleskop o podaną deltę
    /// Automatycznie rozkłada ruch na sekcje według limitów MovementAxis
    /// </summary>
    public bool Move(float delta)
    {
        if (sections.Length == 0)
        {
            Debug.LogWarning("[TelescopicMovement] Brak zdefiniowanych sekcji!");
            return false;
        }
        
        float remainingDelta = delta;
        bool anyMoved = false;
        
        // Przesuń sekcje po kolei
        foreach (TelescopicSection section in sections)
        {
            if (section.movementAxis == null)
                continue;
            
            if (Mathf.Abs(remainingDelta) < 0.001f)
                break;
            
            // Użyj MovementAxis do przesunięcia
            bool moved = section.movementAxis.MoveWithVector3(movementAxis, remainingDelta);
            
            if (moved)
            {
                // Sekcja się poruszyła
                anyMoved = true;
                
                // Ta sekcja może dalej się ruszać - zostań przy niej
                // (nie przechodź do następnej, dopóki ta nie osiągnie limitu)
            }
            else
            {
                // Ta sekcja osiągnęła limit - sprawdź ile faktycznie się przesunęła
                // i odejmij to od remainingDelta, potem przejdź do następnej
                
                // Pobierz aktualną pozycję z MovementAxis
                float currentPos = GetCurrentPosition(section.movementAxis);
                float maxPos = GetMaxPosition(section.movementAxis);
                float minPos = GetMinPosition(section.movementAxis);
                
                // Oblicz ile zostało do limitu
                if (remainingDelta > 0)
                {
                    // Ruch do góry
                    float remaining = maxPos - currentPos;
                    remainingDelta -= remaining;
                }
                else
                {
                    // Ruch w dół
                    float remaining = currentPos - minPos;
                    remainingDelta += remaining;
                }
                
                // Przejdź do następnej sekcji
                Debug.Log("[TelescopicMovement] Sekcja " + section.sectionName + " osiągnęła limit, przechodzę do następnej");
            }
        }
        
        // Zwróć false jeśli wszystkie sekcje osiągnęły limity i nie udało się wykorzystać całej delty
        return anyMoved;
    }
    
    /// <summary>
    /// Pobiera aktualną pozycję z MovementAxis
    /// </summary>
    private float GetCurrentPosition(MovementAxis axis)
    {
        char detectedAxis = DetectAxisChar(movementAxis);
        switch (detectedAxis)
        {
            case 'X': return axis.currentPositionX;
            case 'Y': return axis.currentPositionY;
            case 'Z': return axis.currentPositionZ;
            default: return 0f;
        }
    }
    
    /// <summary>
    /// Pobiera maksymalną pozycję z MovementAxis
    /// </summary>
    private float GetMaxPosition(MovementAxis axis)
    {
        char detectedAxis = DetectAxisChar(movementAxis);
        switch (detectedAxis)
        {
            case 'X': return axis.maxDistanceX;
            case 'Y': return axis.maxDistanceY;
            case 'Z': return axis.maxDistanceZ;
            default: return 0f;
        }
    }
    
    /// <summary>
    /// Pobiera minimalną pozycję z MovementAxis
    /// </summary>
    private float GetMinPosition(MovementAxis axis)
    {
        char detectedAxis = DetectAxisChar(movementAxis);
        switch (detectedAxis)
        {
            case 'X': return axis.minDistanceX;
            case 'Y': return axis.minDistanceY;
            case 'Z': return axis.minDistanceZ;
            default: return 0f;
        }
    }
    
    /// <summary>
    /// Wykrywa którą oś reprezentuje Vector3
    /// </summary>
    private char DetectAxisChar(Vector3 axis)
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
    
    /// <summary>
    /// Sprawdza czy wszystkie sekcje osiągnęły maksymalną pozycję
    /// </summary>
    public bool IsFullyExtended()
    {
        foreach (TelescopicSection section in sections)
        {
            if (section.movementAxis == null)
                continue;
                
            float current = GetCurrentPosition(section.movementAxis);
            float max = GetMaxPosition(section.movementAxis);
            
            if (current < max - 0.001f)
            {
                return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// Sprawdza czy wszystkie sekcje są w minimalnej pozycji
    /// </summary>
    public bool IsFullyRetracted()
    {
        foreach (TelescopicSection section in sections)
        {
            if (section.movementAxis == null)
                continue;
                
            float current = GetCurrentPosition(section.movementAxis);
            float min = GetMinPosition(section.movementAxis);
            
            if (current > min + 0.001f)
            {
                return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// Resetuje wszystkie sekcje do pozycji minimalnej
    /// </summary>
    public void Reset()
    {
        foreach (TelescopicSection section in sections)
        {
            if (section.movementAxis != null)
            {
                section.movementAxis.ResetPosition();
            }
        }
        
        Debug.Log("[TelescopicMovement] Zresetowano teleskop");
    }
    
    /// <summary>
    /// Pobiera całkowitą wysokość teleskopu
    /// </summary>
    public float GetTotalHeight()
    {
        float total = 0f;
        foreach (TelescopicSection section in sections)
        {
            if (section.movementAxis != null)
            {
                total += GetCurrentPosition(section.movementAxis);
            }
        }
        return total;
    }
    
    /// <summary>
    /// Pobiera maksymalną możliwą wysokość
    /// </summary>
    public float GetMaxHeight()
    {
        float total = 0f;
        foreach (TelescopicSection section in sections)
        {
            if (section.movementAxis != null)
            {
                total += GetMaxPosition(section.movementAxis);
            }
        }
        return total;
    }
}