using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Chunk Settings")]
    public GameObject chunkPrefab;
    public int chunkWidth = 13;
    public int chunkLength = 13;
    private int chunkHeight = 2;
    public int numberOfChunks = 1;
    private int chunkCounter = 1;
    private List<Vector3> chunkPositions = new List<Vector3>();
    private List<GameObject> chunksList = new List<GameObject>();

    [Header("Cube Settings")]
    public GameObject cubePrefab;
    private List<GameObject> terrainCubesList = new List<GameObject>();
    private int terrainCounter = 1;
    private Vector3 lastPathCubePosition = Vector3.zero;
    private PathGenerator pathGenerator;

    public void GenerateChunks()
    {
        chunkPositions.Clear();

        Vector3 currentPosition = new Vector3(0, 0, 0);
        chunkPositions.Add(currentPosition);
        CreateChunk(currentPosition);

        for (int i = 1; i < numberOfChunks; i++)
        {
            currentPosition = GetNextChunkPosition();
            Debug.Log("Current Position: " + currentPosition);
            if (currentPosition == Vector3.zero)
            {
                Debug.LogError("No se encontró una posición válida para el siguiente chunk.");
                break;
            }
            chunkPositions.Add(currentPosition);
            CreateChunk(currentPosition);
        }
    }

    public Vector3 GetNextChunkPosition()
    {
        pathGenerator = GetComponent<PathGenerator>();
        // Obtener la última posición del pathCube
        Vector3 lastPathCubePosition = pathGenerator.GetLastPathPosition();
        Debug.Log("Last Path Cube Position: " + lastPathCubePosition);

        // Determinar la dirección del siguiente chunk basado en la posición del último pathCube
        Vector3 nextPosition = Vector3.zero;

        if (lastPathCubePosition.x == 0)
        {
            nextPosition = new Vector3(-chunkWidth, 0, 0); // Izquierda
        }
        else if (lastPathCubePosition.x == chunkWidth - 1)
        {
            nextPosition = new Vector3(chunkWidth, 0, 0); // Derecha
        }
        else if (lastPathCubePosition.z == 0)
        {
            nextPosition = new Vector3(0, 0, -chunkLength); // Abajo
        }
        else if (lastPathCubePosition.z == chunkLength - 1)
        {
            nextPosition = new Vector3(0, 0, chunkLength); // Arriba
        }

        // Comprobar si la nueva posición está ocupada
        if (chunkPositions.Contains(nextPosition))
        {
            Debug.LogError("La posición del siguiente chunk ya está ocupada.");
            return Vector3.zero;
        }

        return nextPosition;
    }

    public IEnumerator CreateChunk(Vector3 position)
    {   
        // Instanciar un nuevo chunk en la posición especificada
        GameObject chunk = Instantiate(chunkPrefab, position, Quaternion.identity);
        // Hacer que el chunk sea hijo de este objeto
        chunk.transform.parent = this.transform;
        // Añadir el chunk a la lista de Chunks.
        chunksList.Add(chunk);
        // Asignar la etiqueta "ChunkTag" al nuevo chunk
        chunk.tag = "ChunkTag";
        // Asignar nombre al chunk
        chunk.name = "Chunk" + chunkCounter;
        // Incrementar el contador de chunks
        chunkCounter++;

        yield return StartCoroutine(CreateTerrainCubes(chunk, position));
    }

    private IEnumerator CreateTerrainCubes(GameObject chunk, Vector3 position)
    {
        // Crear una cuadrícula de cubos dentro del chunk
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkLength; z++)
                {
                    Vector3 cubePosition = new Vector3(x, y, z) + position;
                    // Instanciar un nuevo TerrainCube en la posición especificada
                    GameObject terrainCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
                    // Hacer que el cubo sea hijo del chunk
                    terrainCube.transform.parent = chunk.transform;
                    // Asignar la etiqueta "TerrainTag" al cubo
                    terrainCube.tag = "TerrainTag"; 
                    // Asignar nombre al cubo
                    terrainCube.name = "TerrainCube" + terrainCounter;
                    // Incrementar el contador de cubos
                    terrainCounter++;
                    // Añadir el cubo a la lista de TerrainCubes
                    terrainCubesList.Add(terrainCube);

                    if(y == 0)
                    {
                        // Asignar el material "Grass" al cubo
                        terrainCube.GetComponent<Renderer>().material = terrainCube.GetComponent<CubeScript>().pathMaterial;
                    }
                    else if(y == 1)
                    {
                        // Asignar el material "Dirt" al cubo
                        terrainCube.GetComponent<Renderer>().material = terrainCube.GetComponent<CubeScript>().terrainMaterial;
                    }
                    yield return null;
                }
            }
        }
    }

    public GameObject GetChunkAt(int chunkX, int chunkZ)
    {
        Vector3 chunkPosition = new Vector3(chunkX * chunkWidth, 0, chunkZ * chunkLength);
        foreach (GameObject chunk in chunksList)
        {
            if (chunk.transform.position == chunkPosition)
            {
                return chunk;
            }
        }
        return null;
    }
}

        /*Vector3 nextPosition = currentPosition;
        bool validPositionFound = false;

        // Intentar encontrar una posición válida
        while (!validPositionFound && possibleMoves.Count > 0)
        {   
            // Elegir un movimiento al azar
            int randomIndex = Random.Range(0, possibleMoves.Count);
            Vector3 move = possibleMoves[randomIndex];

            // Calcular la nueva posición
            nextPosition = currentPosition + move;
                
            // Comprobar si la nueva posición está ocupada
            if(!chunkPositions.Contains(nextPosition))
            {
                validPositionFound = true;
            }
            else
            {
                // Eliminar el movimiento de la lista de posibles movimientos
                possibleMoves.RemoveAt(randomIndex);
            }
        }

        // Si no se encontró una posición válida, devolver Vector3.zero
        if (!validPositionFound)
        {
            nextPosition = Vector3.zero;
        }*/