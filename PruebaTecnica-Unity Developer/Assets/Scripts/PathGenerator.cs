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

    private Vector3 entryEdge;

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

         // Aplicar el factor de expansión
        if (Random.Range(0f, 1f) < expansionFactor)
        {
            // Moverse hacia los bordes del chunk
            if (Random.Range(0f, 1f) < 0.5f)
            {
                currentPosition.x += Mathf.Sign(Random.Range(-1f, 1f));
            }
            else
            {
                currentPosition.z += Mathf.Sign(Random.Range(-1f, 1f));
            }
        }

        Debug.Log($"MoveTowards: currentPosition={currentPosition}, target={target}, irregularity={irregularity}, expansionFactor={expansionFactor}");

        // Asegurarse de que la posición no se mueva más allá del objetivo
        currentPosition.x = Mathf.Clamp(currentPosition.x, Mathf.Min(currentPosition.x, target.x), Mathf.Max(currentPosition.x, target.x));
        currentPosition.z = Mathf.Clamp(currentPosition.z, Mathf.Min(currentPosition.z, target.z), Mathf.Max(currentPosition.z, target.z));

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
            float halfChunkWidth = chunkGenerator.chunkWidth / 2f;
            float halfChunkLength = chunkGenerator.chunkLength / 2f;

            // Asegurarse de que el camino cubra al menos la mitad del chunk
            int steps = Mathf.FloorToInt(Mathf.Max(Mathf.CeilToInt(halfChunkWidth), Mathf.CeilToInt(halfChunkLength)));

            // Calcular la probabilidad de bifurcación basada en el número de chunks generados
            float dynamicBranchingProbability = 1 - (1/(float)(i + 1));

            // Establecer la irregularidad a 0 para el primer chunk
            float currentIrregularity = (i == 0) ? 0 : irregularity;
            float currentExpansionFactor = (i == 0) ? 0 : expansionFactor;

            if(i == 0)
            {
                currentPosition = chunkCenter;
                if(CanPlacePathCube(currentPosition))
                {
                    CreatePathCube(currentPosition);
                }
            }
            
            for (int step = 0; step < steps; step++)
            {

                currentPosition = MoveTowards(currentPosition, chunkCenter, currentIrregularity, currentExpansionFactor);
                if (CanPlacePathCube(currentPosition))
                {
                    CreatePathCube(currentPosition);
                }
            }

                float random = Random.Range(0f, 1f);
                // Intentar crear una bifurcación
                Debug.Log(random + " < " + dynamicBranchingProbability);
                if (random < dynamicBranchingProbability)
                {
                    Vector3 branchStartPosition = currentPosition;
                    GenerateBranch(branchStartPosition, _currentChunk, i);
                    Debug.Log("Branch created");
                }

            // Moverse hacia un borde para conectar con el siguiente chunk
            Vector3 targetEdge = GetTargetEdge(currentPosition, _currentChunk, i);

            while (currentPosition != targetEdge)
            {
                currentPosition = MoveTowards(currentPosition, targetEdge, currentIrregularity, currentExpansionFactor);
                
                if(CanPlacePathCube(currentPosition))
                {
                    CreatePathCube(currentPosition);
                }
            }

            // Almacenar el borde de entrada para el siguiente chunk
            entryEdge = currentPosition;   
        }
    }

    // Genera una bifurcación del camino
    private void GenerateBranch(Vector3 startPosition, Vector3 currentChunk, int chunkIndex)
    {
        Vector3 branchPosition = startPosition;
        Vector3 targetEdge = GetTargetEdgeBranch(branchPosition, currentChunk, chunkIndex);

        Debug.Log("Branch target edge: " + targetEdge);
        while (Vector3.Distance(branchPosition, targetEdge) > 0.1f)
        {
            branchPosition = MoveTowards(branchPosition, targetEdge, irregularity, expansionFactor);
            if (CanPlacePathCube(branchPosition))
            {
                CreatePathCube(branchPosition);
            }
        }
    }

private Vector3 GetTargetEdgeBranch(Vector3 currentPosition, Vector3 chunkPosition, int chunkIndex)
{
    // Determine the main path direction
    Vector3 nextChunkPosition = chunkGenerator.chunkPositions[(chunkIndex + 1) % chunkGenerator.chunkPositions.Count];
    Vector3 directionToNextChunk = nextChunkPosition - chunkPosition;

    List<Vector3> edgesForBranch = new List<Vector3>
    {
        new Vector3(chunkPosition.x + chunkGenerator.chunkWidth - 1, 0, currentPosition.z), // Borde derecho
        new Vector3(chunkPosition.x, 0, currentPosition.z), // Borde izquierdo
        new Vector3(currentPosition.x, 0, chunkPosition.z + chunkGenerator.chunkLength - 1), // Borde superior
        new Vector3(currentPosition.x, 0, chunkPosition.z) // Borde inferior
    };

    // Excluir la dirección del camino principal
    if (Mathf.Abs(directionToNextChunk.x) > Mathf.Abs(directionToNextChunk.z))
    {
        // Camino principal se mueve horizontalmente (derecha o izquierda)
        if (directionToNextChunk.x > 0)
        {
            edgesForBranch.Remove(new Vector3(chunkPosition.x + chunkGenerator.chunkWidth - 1, 0, currentPosition.z)); // Borde derecho
        }
        else
        {
            edgesForBranch.Remove(new Vector3(chunkPosition.x, 0, currentPosition.z)); // Borde izquierdo
        }
    }
    else
    {
        // Camino principal se mueve verticalmente (arriba o abajo)
        if (directionToNextChunk.z > 0)
        {
            edgesForBranch.Remove(new Vector3(currentPosition.x, 0, chunkPosition.z + chunkGenerator.chunkLength - 1)); // Borde superior
        }
        else
        {
            edgesForBranch.Remove(new Vector3(currentPosition.x, 0, chunkPosition.z)); // Borde inferior
        }
    }

    // Filtrar las posiciones que ya están ocupadas por cubos de camino
    edgesForBranch = edgesForBranch.FindAll(edge => !pathCubesList.Exists(cube => cube.transform.position == edge));

    if (edgesForBranch.Count == 0)
    {
        Debug.LogError("No hay bordes libres disponibles para la rama.");
        return currentPosition; // Retornar la posición actual si no hay bordes libres
    }

    // Seleccionar un borde aleatorio de los restantes
    Vector3 selectedEdge = edgesForBranch[Random.Range(0, edgesForBranch.Count)];

    return selectedEdge;
}

    // Obtiene el borde objetivo para conectar con el siguiente chunk
    private Vector3 GetTargetEdge(Vector3 currentPosition, Vector3 chunkPosition, int chunkIndex)
    {
        
        List<Vector3> edges = new List<Vector3>
        {
            new Vector3(chunkPosition.x + chunkGenerator.chunkWidth - 1, 0, currentPosition.z), // Borde derecho
            new Vector3(chunkPosition.x, 0, currentPosition.z), // Borde izquierdo
            new Vector3(currentPosition.x, 0, chunkPosition.z + chunkGenerator.chunkLength - 1), // Borde superior
            new Vector3(currentPosition.x, 0, chunkPosition.z) // Borde inferior
        };

        if (chunkGenerator.numberOfChunks == 1)
        {
            // Seleccionar un borde aleatorio
            return edges[Random.Range(0, edges.Count)];
        }
    
        if (chunkIndex == chunkGenerator.numberOfChunks - 1)
        {
            // Encontrar el borde que está más cerca del entryEdge
            Vector3 closestEdge = edges[0];
            float minDistance = Vector3.Distance(entryEdge, edges[0]);

            foreach (Vector3 edge in edges)
            {
                float distance = Vector3.Distance(entryEdge, edge);
                if (distance < minDistance)
                {
                    closestEdge = edge;
                    minDistance = distance;
                }
            }

            // Remover el borde más cercano al entryEdge
            edges.Remove(closestEdge);
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