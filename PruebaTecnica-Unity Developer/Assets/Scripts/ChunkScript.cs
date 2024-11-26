using UnityEngine;

public class ChunkScript : MonoBehaviour
{
    [Header("Chunk Settings")]
    private int totalSideCollisions = 0;

    [Header("Collision Settings")]
    public SphereCollider leftCollider;
    public SphereCollider rightCollider;
    public SphereCollider frontCollider;
    public SphereCollider backCollider;

    void Awake()
    {
        leftCollider.center = new Vector3(-0.5f, 0.5f, 6f);
        rightCollider.center = new Vector3(12.5f, 0.5f, 6f);
        frontCollider.center = new Vector3(6f, 0.5f, 12.5f);
        backCollider.center = new Vector3(6f, 0.5f, -0.5f);

        leftCollider.radius = 0.75f;
        rightCollider.radius = 0.75f;
        frontCollider.radius = 0.75f;
        backCollider.radius = 0.75f;
    }

    public void CheckFrontSphereCollision()
    {
        if (frontCollider != null)
        {
            Collider[] colliders = Physics.OverlapSphere(frontCollider.bounds.center, frontCollider.radius);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("ChunkTag"))
                {
                    totalSideCollisions++;
                }
            }
        }
    }

    public void CheckBackSphereCollision()
    {
        if (backCollider != null)
        {
            Collider[] colliders = Physics.OverlapSphere(backCollider.bounds.center, backCollider.radius);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("ChunkTag"))
                {
                    totalSideCollisions++;
                }
            }
        }
    }

    public void CheckRightSphereCollision()
    {
        if (rightCollider != null)
        {
            Collider[] colliders = Physics.OverlapSphere(rightCollider.bounds.center, rightCollider.radius);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("ChunkTag"))
                {
                    totalSideCollisions++;
                }
            }
        }
    }

    public void CheckLeftSphereCollision()
    {
        if (leftCollider != null)
        {
            Collider[] colliders = Physics.OverlapSphere(leftCollider.bounds.center, leftCollider.radius);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("ChunkTag"))
                {
                    totalSideCollisions++;
                }
            }
        }
    }

    public bool CanPlaceChunk()
    {
        CheckFrontSphereCollision();
        CheckBackSphereCollision();
        CheckRightSphereCollision();
        CheckLeftSphereCollision();

    // Verificar si el total de colisiones laterales es menor que 2
    return totalSideCollisions < 2;
    }
}