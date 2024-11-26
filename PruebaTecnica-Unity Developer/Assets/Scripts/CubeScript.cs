using UnityEngine;

public class CubeScript : MonoBehaviour
{
    [Header("Cube Settings")]
    public Material pathMaterial;
    public Material terrainMaterial;
    private int totalSideCollisions = 0;


    [Header("Collision Settings")]
    public BoxCollider boxCollider;
    public SphereCollider topCollider;
    public SphereCollider bottomCollider;
    public SphereCollider leftCollider;
    public SphereCollider rightCollider;
    public SphereCollider frontCollider;
    public SphereCollider backCollider;

    public void CheckBoxCollision()
    {
        if (boxCollider != null)
        {
            Collider[] colliders = Physics.OverlapBox(boxCollider.bounds.center, boxCollider.size / 2, Quaternion.identity);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("TerrainTag"))
                {
                    Destroy(collider.gameObject);
                }
            }
        }
    }

    public void CheckTopSphereCollision()
    {
        if (topCollider != null)
        {
            Collider[] colliders = Physics.OverlapSphere(topCollider.bounds.center, topCollider.radius);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("TerrainTag"))
                {
                    Destroy(collider.gameObject);
                }
            }
        }
    }
    
    public void CheckBottomSphereCollision()
    {
        if (bottomCollider != null)
        {
            Collider[] colliders = Physics.OverlapSphere(bottomCollider.bounds.center, bottomCollider.radius);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("TerrainTag"))
                {
                    Destroy(collider.gameObject);
                }
            }
        }
    }

    public void CheckFrontSphereCollision()
    {
        if (frontCollider != null)
        {
            Collider[] colliders = Physics.OverlapSphere(frontCollider.bounds.center, frontCollider.radius);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("PathTag"))
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
                if (collider.CompareTag("PathTag"))
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
                if (collider.CompareTag("PathTag"))
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
                if (collider.CompareTag("PathTag"))
                {
                    totalSideCollisions++;
                }
            }
        }
    }

    public bool CanPlacePathCube()
    {
        CheckFrontSphereCollision();
        CheckBackSphereCollision();
        CheckRightSphereCollision();
        CheckLeftSphereCollision();

    // Verificar si el total de colisiones laterales es menor que 2
    return totalSideCollisions < 2;
    }
}