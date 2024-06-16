using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class GhostLapData : ScriptableObject
{
    [SerializeField]
    List<Vector3> carPositions = new List<Vector3>(); // Inicializa la lista de posiciones
    [SerializeField]
    List<Quaternion> carRotations = new List<Quaternion>(); // Inicializa la lista de rotaciones
    [SerializeField]
    float duration;

    // Constructor para inicializar el ScriptableObject
    public GhostLapData()
    {
        carPositions = new List<Vector3>();
        carRotations = new List<Quaternion>();
        duration = 0f;
    }

    public void AddNewData(Transform transform)
    {
        carPositions.Add(transform.position);
        carRotations.Add(transform.rotation);
        Debug.Log("ADDED - " + carPositions.Count + ": Pos (" + transform.position + ") - Rot (" + transform.rotation.eulerAngles + ").");
    }

    public void AddDuration(float duration)
    {
        this.duration = duration;
    }

    public void GetDataAt(int sample, out Vector3 position, out Quaternion rotation)
    {
        position = carPositions[sample];
        rotation = carRotations[sample];
        Debug.Log("PLAYED - " + sample + ": Pos (" + position + ") - Rot (" + rotation.eulerAngles + ").");
    }

    public float GetDuration()
    {
        return duration;
    }

    public void Reset()
    {
        Debug.Log("RESET");
        carPositions.Clear();
        carRotations.Clear();
        duration = 0f;
    }
}
