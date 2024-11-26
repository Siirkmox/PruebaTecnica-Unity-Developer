using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCubeDetector : MonoBehaviour
{
    public int collisionCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PathTag"))
        {
            collisionCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PathTag"))
        {
            collisionCount--;
        }
    }
}