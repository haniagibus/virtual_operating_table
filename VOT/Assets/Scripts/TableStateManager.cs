using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartState
{
    public Transform part;
    public Vector3 initialPosition;
    public Quaternion initialRotation;
    public Vector3 initialScale;
}

public class TableStateManager : MonoBehaviour
{
    [Header("Elementy stołu")]
    public List<Transform> tableParts = new List<Transform>();

    [Header("Ustawienia animacji")]
    public float resetDuration = 2f; // czas w sekundach na powrót do pozycji
    public AnimationCurve resetCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private List<PartState> savedStates = new List<PartState>();
    private bool isResetting = false;

    void Start()
    {
        SaveInitialState();
    }

   // Zapisz pozycje całej hierarchii
    void SaveInitialState()
    {
        savedStates.Clear();

        foreach (var part in tableParts)
        {
            if (part == null) continue;

            // Zapisz element
            SaveTransformRecursive(part);
        }

        Debug.Log("Zapisano " + savedStates.Count + " elementów (z dziećmi)");
    }

    // Rekurencyjnie zapisz element i wszystkie jego dzieci
    void SaveTransformRecursive(Transform t)
    {
        PartState state = new PartState
        {
            part = t,
            initialPosition = t.position,
            initialRotation = t.rotation,
            initialScale = t.localScale
        };

        savedStates.Add(state);
        Debug.Log("Zapisano: " + t.name + " (pos: " + t.position + ", rot: " + t.rotation.eulerAngles + ")");

        // Zapisz wszystkie dzieci
        foreach (Transform child in t)
        {
            SaveTransformRecursive(child);
        }
    }

    public void ResetToNormalPosition()
    {
        if (isResetting)
        {
            Debug.LogWarning("Resetowanie w toku");
            return;
        }

        Debug.Log("Rozpoczynam reset do pozycji normalnej");
        StartCoroutine(ResetCoroutine());
    }

    IEnumerator ResetCoroutine()
    {
        isResetting = true;
        float elapsed = 0f;

        // Zapisz aktualne pozycje i rotacje
        Dictionary<Transform, Vector3> startPositions = new Dictionary<Transform, Vector3>();
        Dictionary<Transform, Quaternion> startRotations = new Dictionary<Transform, Quaternion>();

        foreach (var state in savedStates)
        {
            if (state.part != null)
            {
                startPositions[state.part] = state.part.position;
                startRotations[state.part] = state.part.rotation;

            }
        }

        // Animuj powrót
        while (elapsed < resetDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / resetDuration;
            float curvedProgress = resetCurve.Evaluate(progress);

            foreach (var state in savedStates)
            {
                if (state.part == null) continue;

                state.part.position = Vector3.Lerp(
                    startPositions[state.part],
                    state.initialPosition,
                    curvedProgress
                );

                state.part.rotation = Quaternion.Lerp(
                    startRotations[state.part],
                    state.initialRotation,
                    curvedProgress
                );
            }

            yield return null;
        }

        // Ustaw dokładne pozycje końcowe
        foreach (var state in savedStates)
        {
            if (state.part != null)
            {
                state.part.position = state.initialPosition;
                state.part.rotation = state.initialRotation;
                Debug.Log("Zresetowano: " + state.part.name);
            }
        }

        isResetting = false;
        Debug.Log("Reset zakończony");
    }

    public void SaveCurrentAsNormal()
    {
        SaveInitialState();
        Debug.Log("Zapisano pozycję normalną.");
    }
}