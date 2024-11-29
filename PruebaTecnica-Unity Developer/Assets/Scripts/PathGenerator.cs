using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    public GameObject pathCubePrefab; // Prefab del cubo que representa el camino
    public ChunkGenerator chunkGenerator; // Referencia al generador de chunks
    public List<GameObject> pathCubesList = new List<GameObject>(); // Lista de cubos del camino
    public int pathCounter = 1; // Contador de cubos del camino

    [Range(0, 1)] public float irregularity = 0.5f; // Valor configurable entre 0 y 1 que modifica la irregularidad del camino
    [Range(0, 1)] public float expansionFactor = 0.5f; // Valor configurable entre 0 y 1 que modifica la expansión del camino

    // Obtiene el centro de un chunk dado sus coordenadas X y Z
    public Vector3 GetChunkCenter(int chunkX, int chunkZ)
    {
        float centerX = chunkX * chunkGenerator.chunkWidth + chunkGenerator.chunkWidth / 2;
        float centerZ = chunkZ * chunkGenerator.chunkLength + chunkGenerator.chunkLength / 2;

        return new Vector3(centerX, 0, centerZ);
    }

    // Elimina los cubos de terreno que se superponen con los cubos del camino
    public void RemoveOverlappingTerrainCubes()
    {
        List<GameObject> terrainCubesToRemove = new List<GameObject>();

        foreach (GameObject pathCube in pathCubesList)
        {
            foreach (GameObject terrainCube in chunkGenerator.terrainCubesList)
            {
                if (terrainCube.transform.position == pathCube.transform.position ||
                    terrainCube.transform.position == pathCube.transform.position + Vector3.up)
                {
                    terrainCubesToRemove.Add(terrainCube);
                }
            }
        }

        foreach (GameObject terrainCube in terrainCubesToRemove)
        {
            chunkGenerator.terrainCubesList.Remove(terrainCube);
            Destroy(terrainCube);
        }
    }

    // Mueve la posición actual hacia el objetivo con cierta irregularidad y expansión
    private Vector3 MoveTowards(Vector3 currentPosition, Vector3 target, float irregularity, float expansionFactor)
    {
        // Calcular la probabilidad de moverse en una dirección u otra
        float moveProbability = Random.Range(0f, 1f);

        // Si la irregularidad es 0, el camino será muy recto
        // Si la irregularidad es 1, el camino será muy irregular

            if (moveProbability < irregularity)
            {
                // Moverse en la dirección menos corta (más irregular)
                if (Mathf.Abs(target.x - currentPosition.x) <= Mathf.Abs(target.z - currentPosition.z))
                {
                    currentPosition.x += Mathf.Sign(target.x - currentPosition.x);
                }
                else
                {
                    currentPosition.z += Mathf.Sign(target.z - currentPosition.z);
                }
            }
            else
            {
                // Moverse en la dirección más corta (más regular)
                if (Mathf.Abs(target.x - currentPosition.x) > Mathf.Abs(target.z - currentPosition.z))
                {
                    currentPosition.x += Mathf.Sign(target.x - currentPosition.x);
                }
                else
                {
                    currentPosition.z += Mathf.Sign(target.z - currentPosition.z);
                }
            }

        return currentPosition;
    }

    // Crea un cubo de camino en una posición dada
    private bool CreatePathCube(Vector3 _position)
    {
        // Verificar si ya existe un pathCube en la posición dada
        foreach (GameObject existingPathCube in pathCubesList)
        {
            if (existingPathCube.transform.position == _position)
            {
                return false; // Ya existe un pathCube en esta posición
            }
        }

        // Verificar si se puede colocar el pathCube en la posición dada
        if (!CanPlacePathCube(_position))
        {
            return false; // No se puede colocar el pathCube debido a cubos adyacentes
        }

        int chunkX = Mathf.FloorToInt(_position.x / chunkGenerator.chunkWidth);
        int chunkZ = Mathf.FloorToInt(_position.z / chunkGenerator.chunkLength);

        GameObject chunk = chunkGenerator.GetChunkAt(chunkX, chunkZ);

        if (chunk == null)
        {
            Debug.LogError("No se encontró el chunk en la posición especificada.");
            return false;
        }

        GameObject pathCube = Instantiate(pathCubePrefab, _position, Quaternion.identity, chunk.transform);
        pathCubesList.Add(pathCube);
        pathCube.tag = "PathTag";
        pathCube.GetComponent<Renderer>().material = pathCube.GetComponent<CubeScript>().pathMaterial;
        pathCube.name = "Path" + pathCounter;
        pathCube.transform.parent = GameObject.Find("PathParent").transform;
        pathCounter++;

        return true;
    }

    // Genera el camino desde una posición inicial a través de los chunks
    public void GeneratePath(Vector3 _startPosition, Vector3 _currentChunk)
    {
        Vector3 currentPosition = _startPosition;

        for (int i = 0; i < chunkGenerator.chunkPositions.Count; i++)
        {
            _currentChunk = chunkGenerator.chunkPositions[i];
            Vector3 chunkCenter = GetChunkCenter(Mathf.FloorToInt(_currentChunk.x / chunkGenerator.chunkWidth), Mathf.FloorToInt(_currentChunk.z / chunkGenerator.chunkLength));
            float halfChunkWidth = chunkGenerator.chunkWidth / 2;
            float halfChunkLength = chunkGenerator.chunkLength / 2;

            // Asegurarse de que el camino cubra al menos la mitad del chunk
            int steps = Mathf.FloorToInt(Mathf.Min(halfChunkWidth, halfChunkLength));

            // Establecer la irregularidad a 0 para el primer chunk
            float currentIrregularity = (i == 0) ? 0 : irregularity;
            float currentExpansionFactor = (i == 0) ? 0 : expansionFactor;

            for (int j = 0; j < steps; j++)
            {
                Debug.Log("Current position: " + currentPosition + "chunkCenter: " + chunkCenter);
                currentPosition = MoveTowards(currentPosition, chunkCenter, currentIrregularity, currentExpansionFactor);
                CreatePathCube(currentPosition);
            }

            // Moverse hacia un borde para conectar con el siguiente chunk
            Vector3 targetEdge = GetTargetEdge(currentPosition, _currentChunk, i);
            while (currentPosition != targetEdge)
            {
                currentPosition = MoveTowards(currentPosition, targetEdge, currentIrregularity, currentExpansionFactor);
                CreatePathCube(currentPosition);
            }
        }
    }

    // Obtiene el borde objetivo para conectar con el siguiente chunk
    private Vector3 GetTargetEdge(Vector3 currentPosition, Vector3 chunkPosition, int chunkIndex)
    {
        if (chunkGenerator.numberOfChunks == 1)
        {
            List<Vector3> edges = new List<Vector3>
            {
                new Vector3(chunkPosition.x + chunkGenerator.chunkWidth - 1, 0, currentPosition.z), // Borde derecho
                new Vector3(chunkPosition.x, 0, currentPosition.z), // Borde izquierdo
                new Vector3(currentPosition.x, 0, chunkPosition.z + chunkGenerator.chunkLength - 1), // Borde superior
                new Vector3(currentPosition.x, 0, chunkPosition.z) // Borde inferior
            };

            // Remover el borde de entrada
            edges.RemoveAll(edge => edge == currentPosition);

            // Seleccionar un borde aleatorio
            return edges[Random.Range(0, edges.Count)];
        }
        else if (chunkIndex == chunkGenerator.numberOfChunks - 1)
        {
            // Último chunk, seleccionar un borde diferente al borde de entrada
            List<Vector3> edges = new List<Vector3>
            {
                new Vector3(chunkPosition.x + chunkGenerator.chunkWidth - 1, 0, currentPosition.z), // Borde derecho
                new Vector3(chunkPosition.x, 0, currentPosition.z), // Borde izquierdo
                new Vector3(currentPosition.x, 0, chunkPosition.z + chunkGenerator.chunkLength - 1), // Borde superior
                new Vector3(currentPosition.x, 0, chunkPosition.z) // Borde inferior
            };

            // Remover el borde de entrada
            edges.RemoveAll(edge => edge == currentPosition);

            // Seleccionar un borde aleatorio diferente al de entrada
            return edges[Random.Range(0, edges.Count)];
        }
        else
        {
            // Dirigir el camino hacia el siguiente chunk
            Vector3 nextChunkPosition = chunkGenerator.chunkPositions[(chunkIndex + 1) % chunkGenerator.chunkPositions.Count];
            Vector3 directionToNextChunk = nextChunkPosition - chunkPosition;

            if (Mathf.Abs(directionToNextChunk.x) > Mathf.Abs(directionToNextChunk.z))
            {
                // Moverse horizontalmente
                return directionToNextChunk.x > 0 ? new Vector3(chunkPosition.x + chunkGenerator.chunkWidth - 1, 0, currentPosition.z) : new Vector3(chunkPosition.x, 0, currentPosition.z);
            }
            else
            {
                // Moverse verticalmente
                return directionToNextChunk.z > 0 ? new Vector3(currentPosition.x, 0, chunkPosition.z + chunkGenerator.chunkLength - 1) : new Vector3(currentPosition.x, 0, chunkPosition.z);
            }
        }
    }

    // Verifica si se puede colocar un cubo de camino en una posición dada
    private bool CanPlacePathCube(Vector3 position)
    {
        // Definir las posiciones adyacentes a verificar
        Vector3[] adjacentPositions = new Vector3[]
        {
            position + Vector3.left,
            position + Vector3.right,
            position + Vector3.forward,
            position + Vector3.back,
        };

        int adjacentCount = 0;

        // Verificar si hay cubos de camino en las posiciones adyacentes
        foreach (Vector3 adjacentPosition in adjacentPositions)
        {
            foreach (GameObject existingPathCube in pathCubesList)
            {
                if (existingPathCube.transform.position == adjacentPosition)
                {
                    adjacentCount++;
                    if (adjacentCount >= 2)
                    {
                        return false; // Hay dos o más cubos de camino adyacentes
                    }
                }
            }
        }

        return true; // Menos de dos cubos de camino adyacentes
    }

    // Establece el factor de expansión del camino
    public void SetMovementFromCenter(float value)
    {
        expansionFactor = value;
    }

    // Establece la irregularidad del camino
    public void SetIrregularity(float value)
    {
        irregularity = value;
    }
}