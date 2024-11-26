using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTerrain : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int seed = 0;
    public PruebaChunkGenerator chunkGenerator;
    public PruebaPathGenerator pathGenerator;
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        Random.InitState(seed == 0 ? (int)System.DateTime.Now.Ticks : seed);
        
        if (chunkGenerator != null && pathGenerator != null)
        {
            chunkGenerator.GenerateChunks();

            StartCoroutine(pathGenerator.GeneratePath(new Vector3(chunkGenerator.chunkWidth / 2, 0, chunkGenerator.chunkLength / 2), chunkGenerator.numberOfChunks));

        }
    }
}
