using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaChunkGenerator : MonoBehaviour
{
    [Header("Chunk Settings")]
    public GameObject chunkPrefab; // Prefab del chunk
    public int chunkWidth = 13; // Ancho del chunk
    public int chunkLength = 13; // Largo del chunk
    private int chunkHeight = 2; // Altura del chunk
    public int numberOfChunks = 1; // Número de chunks a generar
    private int chunkCounter = 1; // Contador de chunks generados
    public List<Vector3> chunkPositions = new List<Vector3>(); // Lista de posiciones de los chunks
    private List<GameObject> chunksList = new List<GameObject>(); // Lista de chunks generados

    [Header("Cube Settings")]
    public GameObject cubePrefab; // Prefab del cubo de terreno
    public List<GameObject> terrainCubesList = new List<GameObject>(); // Lista de cubos de terreno generados
    private int terrainCounter = 1; // Contador de cubos de terreno generados

    public PruebaPathGenerator pathGenerator; // Referencia al generador de caminos
    private PruebaPathGenerator.Direction newDirection; // Dirección del camino

    // Método para generar los chunks
    public void GenerateChunks()
    {
        chunkPositions.Clear(); // Limpiar la lista de posiciones de chunks
        chunksList.Clear(); // Limpiar la lista de chunks generados
        chunkCounter = 1; // Reiniciar el contador de chunks

        GameObject pathParent = new GameObject("PathParent");
        pathParent.transform.parent = this.transform;
        
        Vector3 currentPosition = new Vector3(0, 0, 0);
        chunkPositions.Add(currentPosition);
        CreateChunk(currentPosition);

        // Generar el camino en el primer chunk
        Vector3 startPathPos = GetChunkCenter(currentPosition);
        
        if (pathGenerator != null && numberOfChunks == 1)
        {
            PruebaPathGenerator.Direction initialDirection = GetRandomDirection();
            pathGenerator.GeneratePath(startPathPos, initialDirection); // Generar el camino hacia adelante inicialmente
        }

        PruebaPathGenerator.Direction lastDirection = PruebaPathGenerator.Direction.Forward;

        for (int i = 1; i < numberOfChunks; i++)
        {
            Vector3 nextChunkPosition = GetNextChunkPosition(currentPosition); // Obtener la posición del siguiente chunk
            if (nextChunkPosition == Vector3.zero)
            {
                Debug.LogError("No se encontró una posición válida para el siguiente chunk.");
                break;
            }
            chunkPositions.Add(nextChunkPosition); // Añadir la posición del siguiente chunk a la lista
            CreateChunk(nextChunkPosition); // Crear el siguiente chunk

            // Determinar la dirección del camino en función de la posición del siguiente chunk
            PruebaPathGenerator.Direction direction = DetermineDirection(currentPosition, nextChunkPosition);

            // Generar el camino en el chunk actual
            if (pathGenerator != null)
            {
                Vector3 newPathPos = pathGenerator.GetLastPathPosition();
                pathGenerator.GeneratePath(newPathPos, direction); // Generar el camino hasta el borde más cercano
            }
            lastDirection = direction;
            currentPosition = nextChunkPosition; // Actualizar la posición actual al siguiente chunk
        }

        // Generar una dirección aleatoria hacia una de las paredes en el último chunk
        if (pathGenerator != null && numberOfChunks > 1)
        {
            Vector3 lastChunkPosition = chunkPositions[chunkPositions.Count - 1];
            Vector3 lastPathPos = pathGenerator.GetLastPathPosition();
            PruebaPathGenerator.Direction randomWallDirection = GetRandomDirectionExcludingOpposite(lastDirection);
            pathGenerator.GeneratePath(lastPathPos, randomWallDirection);
        }
    }

    // Método para crear un chunk en una posición especificada
    public void CreateChunk(Vector3 position)
    {
        // Instanciar un nuevo chunk en la posición especificada
        GameObject chunk = Instantiate(chunkPrefab, position, Quaternion.identity);
        // Hacer que el chunk sea hijo de este objeto
        chunk.transform.parent = this.transform;
        // Añadir el chunk a la lista de chunks generados
        chunksList.Add(chunk);
        // Asignar la etiqueta "ChunkTag" al nuevo chunk
        chunk.tag = "ChunkTag";
        // Asignar nombre al chunk
        chunk.name = "Chunk" + chunkCounter;
        // Incrementar el contador de chunks
        chunkCounter++;

        CreateTerrainCubes(chunk, position); // Crear los cubos de terreno dentro del chunk
    }

    // Método para crear una cuadrícula de cubos dentro del chunk
    public void CreateTerrainCubes(GameObject chunk, Vector3 position)
    {
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkLength; z++)
                {
                    Vector3 cubePosition = new Vector3(x, y, z) + position; // Calcular la posición del cubo
                    // Instanciar un nuevo cubo de terreno en la posición especificada
                    GameObject terrainCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
                    // Hacer que el cubo sea hijo del chunk
                    terrainCube.transform.parent = chunk.transform;
                    // Asignar la etiqueta "TerrainTag" al cubo
                    terrainCube.tag = "TerrainTag";
                    // Asignar nombre al cubo
                    terrainCube.name = "TerrainCube" + terrainCounter;
                    // Incrementar el contador de cubos
                    terrainCounter++;
                    // Añadir el cubo a la lista de cubos de terreno generados
                    terrainCubesList.Add(terrainCube);

                    if (y == 0)
                    {
                        // Asignar el material "Grass" al cubo
                        terrainCube.GetComponent<Renderer>().material = terrainCube.GetComponent<CubeScript>().pathMaterial;
                    }
                    else if (y == 1)
                    {
                        // Asignar el material "Dirt" al cubo
                        terrainCube.GetComponent<Renderer>().material = terrainCube.GetComponent<CubeScript>().terrainMaterial;
                    }
                }
            }
        }
    }

    public Vector3 GetChunkCenter(Vector3 chunkPosition)
    {
        return new Vector3(chunkPosition.x + chunkWidth / 2, 0, chunkPosition.z + chunkLength / 2);
    }

    // Método para obtener la posición del siguiente chunk
    public Vector3 GetNextChunkPosition(Vector3 _currentPosition)
    {
        List<Vector3> possibleMoves = new List<Vector3>
        {
            new Vector3(chunkWidth, 0, 0), // Movimiento hacia la derecha
            new Vector3(-chunkWidth, 0, 0), // Movimiento hacia la izquierda
            new Vector3(0, 0, chunkLength), // Movimiento hacia adelante
            new Vector3(0, 0, -chunkLength) // Movimiento hacia atrás
        };

        List<PruebaPathGenerator.Direction> possibleDirections = new List<PruebaPathGenerator.Direction>
        {
            PruebaPathGenerator.Direction.Right,
            PruebaPathGenerator.Direction.Left,
            PruebaPathGenerator.Direction.Forward,
            PruebaPathGenerator.Direction.Back
        };

        Vector3 _nextPosition = _currentPosition;
        bool validPositionFound = false;

        // Intentar encontrar una posición válida
        while (!validPositionFound && possibleMoves.Count > 0)
        {
            // Elegir un movimiento al azar
            int randomIndex = Random.Range(0, possibleMoves.Count);
            Vector3 move = possibleMoves[randomIndex];

            // Calcular la nueva posición
            _nextPosition = _currentPosition + move;

            // Comprobar si la nueva posición está ocupada
            if (!chunkPositions.Contains(_nextPosition))
            {
                validPositionFound = true;
                newDirection = possibleDirections[randomIndex]; // Guardar la dirección del movimiento
            }
            else
            {
                // Eliminar el movimiento de la lista de posibles movimientos
                possibleMoves.RemoveAt(randomIndex);
                possibleDirections.RemoveAt(randomIndex);
            }
        }

        // Si no se encontró una posición válida, devolver Vector3.zero
        if (!validPositionFound)
        {
            Debug.LogError("No se encontró una posición válida para el siguiente chunk.");
            _nextPosition = Vector3.zero;
        }
        return _nextPosition;
    }

    // Método para obtener el chunk en una posición específica
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

    private PruebaPathGenerator.Direction DetermineDirection(Vector3 currentPosition, Vector3 nextPosition)
    {
        if (nextPosition.z > currentPosition.z)
        {
            return PruebaPathGenerator.Direction.Forward;
        }
        else if (nextPosition.z < currentPosition.z)
        {
            return PruebaPathGenerator.Direction.Back;
        }
        else if (nextPosition.x > currentPosition.x)
        {
            return PruebaPathGenerator.Direction.Right;
        }
        else if (nextPosition.x < currentPosition.x)
        {
            return PruebaPathGenerator.Direction.Left;
        }
        else
        {
            return PruebaPathGenerator.Direction.Forward; // Valor por defecto
        }
    }

        private PruebaPathGenerator.Direction GetRandomDirection()
    {
        int randomValue = Random.Range(0, 4);
        switch (randomValue)
        {
            case 0:
                return PruebaPathGenerator.Direction.Forward;
            case 1:
                return PruebaPathGenerator.Direction.Back;
            case 2:
                return PruebaPathGenerator.Direction.Left;
            case 3:
                return PruebaPathGenerator.Direction.Right;
            default:
                return PruebaPathGenerator.Direction.Forward;
        }
    }

    private PruebaPathGenerator.Direction GetRandomDirectionExcludingOpposite(PruebaPathGenerator.Direction excludeDirection)
    {
        List<PruebaPathGenerator.Direction> possibleDirections = new List<PruebaPathGenerator.Direction>
        {
            PruebaPathGenerator.Direction.Forward,
            PruebaPathGenerator.Direction.Back,
            PruebaPathGenerator.Direction.Left,
            PruebaPathGenerator.Direction.Right
        };

        // Remove the opposite direction
        switch (excludeDirection)
        {
            case PruebaPathGenerator.Direction.Forward:
                possibleDirections.Remove(PruebaPathGenerator.Direction.Back);
                break;
            case PruebaPathGenerator.Direction.Back:
                possibleDirections.Remove(PruebaPathGenerator.Direction.Forward);
                break;
            case PruebaPathGenerator.Direction.Left:
                possibleDirections.Remove(PruebaPathGenerator.Direction.Right);
                break;
            case PruebaPathGenerator.Direction.Right:
                possibleDirections.Remove(PruebaPathGenerator.Direction.Left);
                break;
        }

        // Choose a random direction from the remaining possible directions
        int randomIndex = Random.Range(0, possibleDirections.Count);
        return possibleDirections[randomIndex];
    }


    // Método para obtener la lista de posiciones de los chunks
    public List<Vector3> GetChunkPositions()
    {
        return chunkPositions;
    }
}