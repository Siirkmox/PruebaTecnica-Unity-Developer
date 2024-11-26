using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaPathGenerator : MonoBehaviour
{
    public GameObject pathCubePrefab;
    public PruebaChunkGenerator chunkGenerator;
    private List<Vector3> pathPositions = new List<Vector3>();
    private List<GameObject> pathCubesList = new List<GameObject>();
    [SerializeField] private int pathCounter = 1;

    public Vector3 GetChunkCenter(int chunkX, int chunkZ)
    {
        return new Vector3(chunkX * chunkGenerator.chunkWidth + chunkGenerator.chunkWidth / 2, 0, chunkZ * chunkGenerator.chunkLength + chunkGenerator.chunkLength / 2);
    }

    public IEnumerator GeneratePath(Vector3 startPosition, int numberOfChunks)
    {
        
        if (chunkGenerator == null)
        {
            Debug.LogError("chunkGenerator es nulo. Asegúrate de que esté asignado correctamente.");
            yield break;
        }

        pathPositions.Clear();
        pathCubesList.Clear();

        Vector3 currentPosition = startPosition;
        pathPositions.Add(currentPosition);
        CreatePathCube(currentPosition);
        yield return null;

        int pathLength = chunkGenerator.chunkWidth / 2;

        for (int i = 0; i < numberOfChunks; i++)
        {
            int createdPathCubes = 0;

            while (createdPathCubes < pathLength || !IsAtChunkEdge(currentPosition))
            {
                Vector3 nextPosition = GetNextPosition(currentPosition);
                
                if (IsWithinChunkBounds(nextPosition) && !pathPositions.Contains(nextPosition))
                {
                    if (CreatePathCube(nextPosition))
                    {
                        pathPositions.Add(nextPosition);
                        currentPosition = nextPosition;
                        createdPathCubes++;   
                        yield return null; 
                    }
                }
                else
                {
                    nextPosition = GetNextPosition(currentPosition);
                }
            } 
        }
    }

    private Vector3 GetNextPosition(Vector3 currentPosition)
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
}
