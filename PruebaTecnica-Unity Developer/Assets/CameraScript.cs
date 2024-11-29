using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public ChunkGenerator chunkGenerator; // Referencia al generador de chunks
    public float padding = 10f; // Espacio adicional alrededor de los chunks

    void Start()
    {
        // Centrar y ajustar el zoom de la cámara al inicio
        CenterAndZoomCamera();
    }

    void Update()
    {
        CenterAndZoomCamera();
    }

    // Método para centrar y ajustar el zoom de la cámara
    public void CenterAndZoomCamera()
    {
        // Verificar que el generador de chunks y las posiciones de los chunks no sean nulos
        if (chunkGenerator == null || chunkGenerator.chunkPositions.Count == 0)
        {
            Debug.LogError("Chunk generator or chunk positions not set.");
            return;
        }

        // Calcular el centro de todos los chunks
        Vector3 centerPosition = Vector3.zero;
        foreach (Vector3 chunkPosition in chunkGenerator.chunkPositions)
        {
            centerPosition += chunkPosition;
        }
        centerPosition /= chunkGenerator.chunkPositions.Count;

        // Calcular el tamaño total de los chunks
        float totalWidth = chunkGenerator.chunkWidth * chunkGenerator.numberOfChunks;
        float totalLength = chunkGenerator.chunkLength * chunkGenerator.numberOfChunks;

        // Ajustar la posición de la cámara
        Camera.main.transform.position = new Vector3(centerPosition.x, Mathf.Max(totalWidth, totalLength) / 2 + padding, centerPosition.z);
        Camera.main.transform.LookAt(centerPosition);
    }
}