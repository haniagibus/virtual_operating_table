using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartState
{
    public Transform part;
    public Vector3 initialLocalPosition;    // ZMIANA: local zamiast world
    public Quaternion initialLocalRotation; // ZMIANA: local zamiast world
    public Vector3 initialLocalScale;
}

public class TableStateManager : MonoBehaviour
{
    [Header("Elementy stołu")]
    public List<Transform> tableParts = new List<Transform>();

    [Header("Ustawienia animacji")]
    public float resetSpeed = 2f;
    public AnimationCurve resetCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private List<PartState> savedStates = new List<PartState>();
    [HideInInspector] public bool isResetting = false;

    void Start()
    {
        SaveInitialState();
    }

    void Update()
    {

        if (isResetting)
        {
            foreach (var state in savedStates)
            {
                if (state.part == null) continue;

                state.part.localPosition = Vector3.Lerp(
                    state.part.localPosition,
                    state.initialLocalPosition,
                    Time.deltaTime * resetSpeed
                );

                state.part.localRotation = Quaternion.Lerp(
                    state.part.localRotation,
                    state.initialLocalRotation,
                    Time.deltaTime * resetSpeed
                );

                state.part.localScale = Vector3.Lerp(
                    state.part.localScale,
                    state.initialLocalScale,
                    Time.deltaTime * resetSpeed
                );

                 // if (pivot.allowX && entry.sliderX != null) entry.sliderX.value = 0;
                 // if (pivot.allowY && entry.sliderY != null) entry.sliderY.value = 0;
                // if (pivot.allowZ && entry.sliderZ != null) entry.sliderZ.value = 0;
            }
        }
    }

    void SaveInitialState()
    {
        savedStates.Clear();

        foreach (var part in tableParts)
        {
            if (part == null) continue;
            SaveTransformRecursive(part);
        }

        Debug.Log("Zapisano " + savedStates.Count + " elementów (z dziećmi)");
    }

    void SaveTransformRecursive(Transform t)
    {
        PartState state = new PartState
        {
            part = t,
            initialLocalPosition = t.localPosition,    // ZMIANA: local
            initialLocalRotation = t.localRotation,    // ZMIANA: local
            initialLocalScale = t.localScale
        };

        savedStates.Add(state);
        Debug.Log("Zapisano: " + t.name + 
                  " (localPos: " + t.localPosition + 
                  ", localRot: " + t.localRotation.eulerAngles + ")");

        foreach (Transform child in t)
        {
            SaveTransformRecursive(child);
        }
    }

    public void SaveCurrentAsNormal()
    {
        SaveInitialState();
        Debug.Log("Zapisano pozycję normalną.");
    }
}