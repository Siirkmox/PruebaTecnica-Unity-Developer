using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaPathGenerator : MonoBehaviour
{
    public GameObject pathCubePrefab; // Prefab del cubo que representa el camino
    public PruebaChunkGenerator chunkGenerator; // Referencia al generador de chunks
    private List<Vector3> pathPositions = new List<Vector3>(); // Lista de posiciones del camino
    private List<GameObject> pathCubesList = new List<GameObject>(); // Lista de cubos del camino
    [SerializeField] private int pathCounter = 1; // Contador de cubos del camino

    [Range(0, 1)] public float irregularity = 0.5f; // Valor configurable entre 0 y 1 que modifica la irregularidad del camino
    [Range(0, 1)] public float expansionFactor = 0.5f; // Valor configurable entre 0 y 1 que modifica la expansión del camino/
    public enum Direction
    {
        Forward,
        Back,
        Left,
        Right
    }

    // Obtiene el centro de un chunk dado sus coordenadas X y Z
    public Vector3 GetChunkCenter(int chunkX, int chunkZ)
    {
        float centerX = chunkX * chunkGenerator.chunkWidth + chunkGenerator.chunkWidth / 2;
        float centerZ = chunkZ * chunkGenerator.chunkLength + chunkGenerator.chunkLength / 2;

        Debug.Log("Centro del chunk: " + new Vector3(centerX, 0, centerZ));
        return new Vector3(centerX, 0, centerZ);
    }

    public Vector3 GetChunkHalf(int chunkX, int chunkZ)
    {
        float halfX = chunkX * chunkGenerator.chunkWidth + chunkGenerator.chunkWidth / 2;
        float halfZ = chunkZ * chunkGenerator.chunkLength + chunkGenerator.chunkLength / 2;
        
        return new Vector3(chunkX, 0, chunkZ);
    }

    private Vector3 GetChunkCenterInDirection(Vector3 _currentChunkCenter, Direction _direction)
    {
        switch (_direction)
        {
            case Direction.Forward:
                return new Vector3(_currentChunkCenter.x, _currentChunkCenter.y, _currentChunkCenter.z + chunkGenerator.chunkLength);
            case Direction.Back:
                return new Vector3(_currentChunkCenter.x, _currentChunkCenter.y, _currentChunkCenter.z - chunkGenerator.chunkLength);
            case Direction.Right:
                return new Vector3(_currentChunkCenter.x + chunkGenerator.chunkWidth, _currentChunkCenter.y, _currentChunkCenter.z);
            case Direction.Left:
                return new Vector3(_currentChunkCenter.x - chunkGenerator.chunkWidth, _currentChunkCenter.y, _currentChunkCenter.z);
            default:
                return _currentChunkCenter;
        }
    }

    private Vector3 GetChunkHalfInDirection(Vector3 _currentChunkHalf, Direction direction)
    {
        switch (direction)
        {
            case Direction.Forward:
                return new Vector3(_currentChunkHalf.x, _currentChunkHalf.y, _currentChunkHalf.z + chunkGenerator.chunkLength);
            case Direction.Back:
                return new Vector3(_currentChunkHalf.x, _currentChunkHalf.y, _currentChunkHalf.z - chunkGenerator.chunkLength);
            case Direction.Right:
                return new Vector3(_currentChunkHalf.x + chunkGenerator.chunkWidth, _currentChunkHalf.y, _currentChunkHalf.z);
            case Direction.Left:
                return new Vector3(_currentChunkHalf.x - chunkGenerator.chunkWidth, _currentChunkHalf.y, _currentChunkHalf.z);
            default:
                Debug.LogError("Dirección no válida: " + direction);
                return _currentChunkHalf;
        }
    }

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
    
    private Vector3 MoveTowards(Vector3 _currentPosition, Vector3 _target, float _irregularity)
    {

        if (_currentPosition.x < _target.x)
        {
            _currentPosition.x++;
        }
        else if (_currentPosition.x > _target.x)
        {
            _currentPosition.x--;
        }

        if (_currentPosition.z < _target.z)
        {
            _currentPosition.z++;
        }
        else if (_currentPosition.z > _target.z)
        {
                _currentPosition.z--;
        }

        return _currentPosition;
    }
    
        // Obtiene el borde del chunk en la dirección especificada
    private Vector3 GetBorderInDirection(Vector3 _startPosition, Direction _direction)
    {
        
    int chunkX = Mathf.FloorToInt(_startPosition.x / chunkGenerator.chunkWidth);
    int chunkZ = Mathf.FloorToInt(_startPosition.z / chunkGenerator.chunkLength);
    Vector3 chunkCenter = GetChunkCenter(chunkX, chunkZ);

        switch (_direction)
        {
            case Direction.Forward:
                return new Vector3(chunkCenter.x, chunkCenter.y, chunkCenter.z + chunkGenerator.chunkLength / 2);
            case Direction.Back:
                return new Vector3(chunkCenter.x, chunkCenter.y, chunkCenter.z - chunkGenerator.chunkLength / 2);
            case Direction.Right:
                return new Vector3(chunkCenter.x + chunkGenerator.chunkWidth / 2, chunkCenter.y, chunkCenter.z);
            case Direction.Left:
                return new Vector3(chunkCenter.x - chunkGenerator.chunkWidth / 2, chunkCenter.y, chunkCenter.z);
            default:
                Debug.LogError("Dirección no válida: " + _direction);
                return _startPosition;
        }
    }
    
    private void GeneratePathToBorder(Vector3 _currentPosition, Vector3 _border, Direction _startDirection, float _irregularity)
    {
        // Obtener el centro del chunk actual
        Vector3 _currentChunkCenter = GetChunkCenter(Mathf.FloorToInt(_currentPosition.x / chunkGenerator.chunkWidth), Mathf.FloorToInt(_currentPosition.z / chunkGenerator.chunkLength));
        
        // Generar el camino hacia el centro del chunk actual
        while (_currentPosition != _currentChunkCenter)
        {
            _currentPosition = MoveTowards(_currentPosition, _currentChunkCenter, _irregularity);
            if (pathPositions.Contains(_currentPosition))
            {
                return;
            }

            pathPositions.Add(_currentPosition);
            CreatePathCube(_currentPosition);
        }

        // Obtener el centro del siguiente chunk en la dirección especificada
        Vector3 _nextChunkHalf = GetChunkCenterInDirection(_currentChunkCenter, _startDirection);

        // Generar el camino hacia el centro del siguiente chunk
        while (_currentPosition != _nextChunkHalf)
        {
            _currentPosition = MoveTowards(_currentPosition, _nextChunkHalf, _irregularity);
            if (pathPositions.Contains(_currentPosition))
            {
                return;
            }

            pathPositions.Add(_currentPosition);
            CreatePathCube(_currentPosition);
        }

        while (_currentPosition != _border)
        {
            _currentPosition = MoveTowards(_currentPosition, _border, _irregularity);

            if (pathPositions.Contains(_currentPosition))
            {
                return;
            }

            pathPositions.Add(_currentPosition);
            CreatePathCube(_currentPosition);
        }
    }

    // Crea un cubo de camino en una posición dada
    private bool CreatePathCube(Vector3 _position)
    {
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
    
    // Genera el camino desde una posición inicial hasta la posición del siguiente chunk
    public void GeneratePath(Vector3 _startPosition, Direction _direction)
    {
        if (chunkGenerator == null)
        {
            Debug.LogError("chunkGenerator es nulo. Asegúrate de que esté asignado correctamente.");
            return;
        }

        // Asegurarse de que la posición inicial sea el centro del chunk
        int chunkX = Mathf.FloorToInt(_startPosition.x / chunkGenerator.chunkWidth);
        int chunkZ = Mathf.FloorToInt(_startPosition.z / chunkGenerator.chunkLength);
        Vector3 currentPosition = GetChunkCenter(chunkX, chunkZ);
        
            // Si hay posiciones de camino existentes, comenzar desde la última posición
            if (pathPositions.Count > 0)
            {
                currentPosition = pathPositions[pathPositions.Count - 1];
            }
            else
            {
                // Crear el cubo de camino en la posición actual
                CreatePathCube(currentPosition);
                pathPositions.Add(currentPosition);
            }

        Vector3 border = GetBorderInDirection(currentPosition, _direction);
        GeneratePathToBorder(currentPosition, border, _direction, irregularity);
    }

    // Retorna la última posición de la lista de posiciones del camino
    public Vector3 GetLastPathPosition()
    {
        if (pathPositions.Count > 0)
        {
            return pathPositions[pathPositions.Count - 1];
        }
        return new Vector3(chunkGenerator.chunkWidth / 2, 0, chunkGenerator.chunkLength / 2);
    }

    //Genera la expansion del camino
    public Vector3 ExpansionFactor(Vector3 _currentChunkCenter)
    {
        // Asegurarse de que la posición inicial sea el centro del chunk
        int chunkX = chunkGenerator.chunkWidth - 1 / 2;
        int chunkZ = chunkGenerator.chunkLength - 1 / 2;

        if(expansionFactor == 0)
        {
            _currentChunkCenter = GetChunkCenter(chunkX, chunkZ);
        }
        else if (expansionFactor == 1f)
        {
            List<Vector3> borderPositions = new List<Vector3>();

            // Añadir posiciones del borde del chunk
            for (int x = 0; x < chunkGenerator.chunkWidth - 1; x++)
            {
                borderPositions.Add(new Vector3(x, 0, 0)); // Borde inferior
                borderPositions.Add(new Vector3(x, 0, chunkGenerator.chunkLength - 1)); // Borde superior
            }

            for (int z = 0; z < chunkGenerator.chunkLength - 1; z++)
            {
                borderPositions.Add(new Vector3(0, 0, z)); // Borde izquierdo
                borderPositions.Add(new Vector3(chunkGenerator.chunkWidth - 1, 0, z)); // Borde derecho
            }

            // Elegir una posición aleatoria del borde
            int randomIndex = Random.Range(0, borderPositions.Count);
            _currentChunkCenter = borderPositions[randomIndex];
        }
        return _currentChunkCenter;
    }
    void Irregularity(Vector3 _currentPosition, Direction _startDirection, float _irregularity)
    {
        // Generar un camino que recorra al menos la mitad del chunk en la dirección especificada
        int halfChunkWidth = chunkGenerator.chunkWidth / 2;
        int halfChunkLength = chunkGenerator.chunkLength / 2;
        int steps = Mathf.Max(halfChunkWidth, halfChunkLength);

        //Irregularidad del camino
        for (int i = 0; i < steps; i++)
        {
            if (Random.value < _irregularity)
            {
                switch (_startDirection)
                {
                    case Direction.Forward:
                        _currentPosition.z++;
                        break;
                    case Direction.Back:
                        _currentPosition.z--;
                        break;
                    case Direction.Right:
                        _currentPosition.x++;
                        break;
                    case Direction.Left:
                        _currentPosition.x--;
                        break;
                }
            }
            else
            {
                if (Random.value > 0.5f)
                {
                    if (_startDirection == Direction.Forward || _startDirection == Direction.Back)
                    {
                        _currentPosition.x += Random.value > 0.5f ? 1 : -1;
                    }
                    else
                    {
                        _currentPosition.z += Random.value > 0.5f ? 1 : -1;
                    }
                }
                else
                {
                    switch (_startDirection)
                    {
                        case Direction.Forward:
                            _currentPosition.z++;
                            break;
                        case Direction.Back:
                            _currentPosition.z--;
                            break;
                        case Direction.Right:
                            _currentPosition.x++;
                            break;
                        case Direction.Left:
                            _currentPosition.x--;
                            break;
                    }
                }
            }
        }
    }
}