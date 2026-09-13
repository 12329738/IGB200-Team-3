using UnityEngine;

public class IslandDecorate : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public BoxCollider area;
    public void PlaceItemOnIsland(SpriteScript mapObject)
    {
        mapObject.gameObject.transform.SetParent(transform);
        mapObject.transform.localScale = mapObject.transform.localScale / 2;

        Bounds bounds = area.bounds;

        for (int i = 0; i < 100; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            Collider[] colliders = Physics.OverlapSphere(
                position,
                0.01f,
                obstacleLayer
            );

            if (colliders.Length == 0)
            {
                mapObject.transform.position = position;
            }
        }
    }
}

