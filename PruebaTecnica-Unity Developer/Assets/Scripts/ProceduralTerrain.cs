using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTerrain : MonoBehaviour
{
    [Header("Terrain Settings")]
    private int seed = 0; // Semilla para la generación procedural
    public ChunkGenerator chunkGenerator; // Referencia al generador de chunks
    public PathGenerator pathGenerator; // Referencia al generador de caminos
    
    // Start is called before the first frame update
    void Start()
    {
        // Generar el terreno al inicio
        GenerateTerrain();
    }

    // Método para generar el terreno
    public void GenerateTerrain()
    {
        // Inicializar la semilla para la generación procedural
        Random.InitState(seed == 0 ? (int)System.DateTime.Now.Ticks : seed);
        
        // Verificar que los generadores de chunks y caminos no sean nulos
        if (chunkGenerator != null && pathGenerator != null)
        {
            // Generar los chunks del terreno
            chunkGenerator.GenerateChunks();

            // Obtener la posición inicial del primer chunk
            Vector3 firstChunkPosition = chunkGenerator.chunkPositions[0];
            Vector3 startPosition = chunkGenerator.GetChunkCenter(firstChunkPosition);

            // Generar el camino desde la posición inicial
            pathGenerator.GeneratePath(startPosition, firstChunkPosition);
            pathGenerator.RemoveOverlappingTerrainCubes();
        }
    }

    // Método para limpiar todos los objetos generados previamente
    public void ClearAllGeneratedObjects()
    {
        // Limpiar los cubos del camino
        foreach (GameObject pathCube in pathGenerator.pathCubesList)
        {
            Destroy(pathCube);
        }
        pathGenerator.pathCubesList.Clear();
        pathGenerator.pathCounter = 1;

        // Limpiar los chunks del terreno
        foreach (GameObject chunk in chunkGenerator.chunksList)
        {
            Destroy(chunk);
        }
        chunkGenerator.chunksList.Clear();
        chunkGenerator.chunkCounter = 1;

        // Limpiar los cubos del terreno
        foreach (GameObject terrainCube in chunkGenerator.terrainCubesList)
        {
            Destroy(terrainCube);
        }
        chunkGenerator.terrainCubesList.Clear();
        chunkGenerator.terrainCounter = 1;
    }
}