using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    public GameObject pathCubePrefab;
    public ChunkGenerator chunkGenerator;
    private List<Vector3> pathPositions = new List<Vector3>();
    private List<GameObject> pathCubesList = new List<GameObject>();
    [SerializeField] private int pathCounter = 1;

    void Awake()
    {
        chunkGenerator = GetComponent<ChunkGenerator>();
    }
    public Vector3 GetChunkCenter(int chunkX, int chunkZ)
    {
        return new Vector3(chunkX * chunkGenerator.chunkWidth + chunkGenerator.chunkWidth / 2, 0, chunkZ * chunkGenerator.chunkLength + chunkGenerator.chunkLength / 2);
    }

    public IEnumerator GeneratePath(Vector3 startPosition, int numberOfChunks, System.Action<Vector3> onPathComplete)
    {
        if (chunkGenerator == null)
        {
            Debug.LogError("chunkGenerator es nulo. Asegúrate de que esté asignado correctamente.");
            yield break;
        }

        //pathPositions.Clear();
        pathCubesList.Clear();

        Vector3 currentPosition = startPosition;
        pathPositions.Add(currentPosition);
        CreatePathCube(currentPosition);
        yield return null; // Espera un frame

        int pathLength = chunkGenerator.chunkWidth / 2;

        for (int i = 0; i < numberOfChunks; i++)
        {
            int createdPathCubes = 0;

            while (createdPathCubes < pathLength || !IsAtChunkEdge(currentPosition))
            {
                Vector3 nextPosition = GetNextPosition(currentPosition, out bool pathEnded);
                
                if (IsWithinChunkBounds(nextPosition) && !pathPositions.Contains(nextPosition))
                {
                    if (CreatePathCube(nextPosition))
                    {
                        pathPositions.Add(nextPosition);
                        currentPosition = nextPosition;
                        createdPathCubes++;
                        yield return null; // Espera un frame
                    
                        if(pathEnded)
                        {
                            Debug.Log("El camino ha llegado al borde del chunk y debe detenerse");
                            onPathComplete?.Invoke(currentPosition);
                            break;
                        }
                    }
                }
                else
                {
                    nextPosition = GetNextPosition(currentPosition, out pathEnded);
                }
            } 
        }
        
        onPathComplete?.Invoke(currentPosition);
    }

    public Vector3 GetNextChunkStartPosition(Vector3 chunkPosition)
    {
        Vector3 lastPathCubePosition = GetLastPathPosition();
        Vector3 nextStartPosition = Vector3.zero;

        if (lastPathCubePosition.x == chunkGenerator.chunkWidth - 1)
        {
            nextStartPosition = new Vector3(chunkPosition.x + 1, 0, lastPathCubePosition.z);
        }
        else if (lastPathCubePosition.x == 0)
        {
            nextStartPosition = new Vector3(chunkPosition.x + chunkGenerator.chunkWidth - 2, 0, lastPathCubePosition.z);
        }
        else if (lastPathCubePosition.z == chunkGenerator.chunkLength - 1)
        {
            nextStartPosition = new Vector3(lastPathCubePosition.x, 0, chunkPosition.z + 1);
        }
        else if (lastPathCubePosition.z == 0)
        {
            nextStartPosition = new Vector3(lastPathCubePosition.x, 0, chunkPosition.z + chunkGenerator.chunkLength - 2);
        }

        return nextStartPosition;
    }

    private Vector3 GetNextPosition(Vector3 currentPosition, out bool pathEnded)
    {
        List<Vector3> possibleMoves = new List<Vector3>
        {
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1)
        };

        Vector3 nextPosition = currentPosition;
        bool validPositionFound = false;
        pathEnded = false;

        while (!validPositionFound && possibleMoves.Count > 0)
        {
            // Elegir un movimiento al azar
            int randomIndex = Random.Range(0, possibleMoves.Count);
            Vector3 move = possibleMoves[randomIndex];

            // Calcular la nueva posición
            nextPosition = currentPosition + move;

            if (IsWithinChunkBounds(nextPosition) && !pathPositions.Contains(nextPosition))
            {
                GameObject tempCube = Instantiate(pathCubePrefab, nextPosition, Quaternion.identity);
                CubeScript cubeScript = tempCube.GetComponent<CubeScript>();

                if (cubeScript.CanPlacePathCube())
                {
                    validPositionFound = true;
                }

                Destroy(tempCube);
            }

            if (!validPositionFound)
            {
                possibleMoves.RemoveAt(randomIndex);
            }
        }

        if (!validPositionFound)
        {
            nextPosition = currentPosition;
        }
        
        if (IsAtChunkEdge(nextPosition))
        {
            pathEnded = true;
        }

        return nextPosition;
    }

    private bool CreatePathCube(Vector3 position)
    {
        int chunkX = Mathf.FloorToInt(position.x / chunkGenerator.chunkWidth);
        int chunkZ = Mathf.FloorToInt(position.z / chunkGenerator.chunkLength);

        GameObject chunk = chunkGenerator.GetChunkAt(chunkX, chunkZ);
        if (chunk == null)
        {
            Debug.LogError("No se encontró el chunk en la posición especificada.");
            return false;
        }

        GameObject pathCube = Instantiate(pathCubePrefab, position, Quaternion.identity, chunk.transform);
        pathCubesList.Add(pathCube);
        pathCube.tag = "PathTag";
        pathCube.GetComponent<Renderer>().material = pathCube.GetComponent<CubeScript>().pathMaterial;
        pathCube.name = "Path" + pathCounter;
        pathCounter++;

        CubeScript cubeScript = pathCube.GetComponent<CubeScript>();
        cubeScript.CheckBoxCollision();
        cubeScript.CheckTopSphereCollision();

        return true;
    }

    private bool IsWithinChunkBounds(Vector3 position)
    {
        int chunkX = Mathf.FloorToInt(position.x / chunkGenerator.chunkWidth);
        int chunkZ = Mathf.FloorToInt(position.z / chunkGenerator.chunkLength);

        return position.x >= chunkX * chunkGenerator.chunkWidth &&
               position.x < (chunkX + 1) * chunkGenerator.chunkWidth &&
               position.z >= chunkZ * chunkGenerator.chunkLength &&
               position.z < (chunkZ + 1) * chunkGenerator.chunkLength;
    }

    private bool IsAtChunkEdge(Vector3 position)
    {
        int chunkX = Mathf.FloorToInt(position.x / chunkGenerator.chunkWidth);
        int chunkZ = Mathf.FloorToInt(position.z / chunkGenerator.chunkLength);

        return position.x == chunkX * chunkGenerator.chunkWidth ||
               position.x == (chunkX + 1) * chunkGenerator.chunkWidth - 1||
               position.z == chunkZ * chunkGenerator.chunkLength ||
               position.z == (chunkZ + 1) * chunkGenerator.chunkLength - 1;
    }

    public Vector3 GetLastPathPosition()
    {
        if (pathPositions.Count > 0)
        {
            Debug.Log("DENTRO");
            return pathPositions[pathPositions.Count - 1];
        }
        Debug.Log("FUERA");
        return Vector3.zero;
    }
}

/*
1. Inicio del Camino: 
1.1. El camino del primer chunk debe empezar desde el centro del mismo.

2. Expansion del Camino Dentro del Chunk: 
2.1. El camino dentro de cada chunk debe recorrer al menos la mitad del chunk antes de dirigirse hacia la pared para finalizar en ese chunk.

2.2 Esto evita caminos extremadamente cortos y promueve una exploracion mas significativa.

2.3 El camino no puede pasar por los bordes del chunk, excepto en el punto final donde se conecta con otro chunk.

2.4 El camino no puede terminar en el mismo lateral por donde comenzó en el chunk. Esto asegura que el camino avance y no retroceda sobre sí mismo.

3. Terminacion del Camino en un Chunk: 
3.1 Al finalizar en un chunk, el camino debe dirigirse hacia una de las paredes y salir por allí para continuar en el chunk adyacente correspondiente.

4. Otro tipo de reglas:
4.1 No se puede elegir la misma pared por la que se salió en el chunk anterior para continuar el camino en el siguiente chunk.

4.2 Esto fomenta variedad en la dirección del camino y evita bucles innecesarios.

4.3. A medida que se generan más chunks, aumenta la probabilidad de que aparezcan caminos adicionales que se bifurcan del camino principal.

4.4 Estos caminos deben seguir las mismas reglas y no pueden retroceder hacia el chunk anterior por la misma pared.
*/

/*

*/