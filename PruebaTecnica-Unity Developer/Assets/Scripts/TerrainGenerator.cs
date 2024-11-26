using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int seed = 0;
    public ChunkGenerator chunkGenerator;
    public PathGenerator pathGenerator;
    
    void Start()
    {
        pathGenerator.chunkGenerator = chunkGenerator;
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        Random.InitState(seed == 0 ? (int)System.DateTime.Now.Ticks : seed);
        
        if (chunkGenerator != null && pathGenerator != null)
        {
            StartCoroutine(GenerateChunksAndPaths());
        }
    }

    IEnumerator GenerateChunksAndPaths()
    {
        Vector3 startChunkPosition = new Vector3(0, 0, 0);
        Debug.Log("Generating chunks and paths..." + chunkGenerator.numberOfChunks);
        for (int i = 0; i < chunkGenerator.numberOfChunks; i++)
        {
            Debug.Log("Generating chunk " + i);
            // Crear el chunk en la posición actual
            yield return StartCoroutine(chunkGenerator.CreateChunk(startChunkPosition));

            Vector3 startPosition = new Vector3 (chunkGenerator.chunkWidth / 2, 0, chunkGenerator.chunkLength / 2); 
            // Generar el camino en el chunk actual
            bool pathCompleted = false;
            yield return StartCoroutine(pathGenerator.GeneratePath(startPosition, chunkGenerator.numberOfChunks, (lastPathPosition) =>
            {
                startPosition = lastPathPosition;
                pathCompleted = true;
            }));

            // Esperar hasta que el camino esté completo
            while (!pathCompleted)
            {
                yield return null;
            }
            // Solo actualizar la posición del siguiente chunk si hay más chunks por generar
        
        if (i < chunkGenerator.numberOfChunks - 1)
        {
            startChunkPosition = chunkGenerator.GetNextChunkPosition();
            // Crear el siguiente chunk
            yield return StartCoroutine(chunkGenerator.CreateChunk(startChunkPosition));
            // Obtener la posición de inicio del camino en el siguiente chunk
            //startPosition = pathGenerator.GetNextChunkStartPosition(startChunkPosition);
            //Debug.Log("StartPosition: " + startPosition);
        }
    }
        Debug.Log("Chunks and paths generated.");
    }
}