using UnityEngine;

public class IslandDecorate : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public PolygonCollider2D area;
    public GameObject decorationPrefab;

    public void PlaceItemOnIsland(SpriteScript mapObject)
    {
        GameObject obj = Instantiate(decorationPrefab);

        MapDecoration decoration = obj.GetComponent<MapDecoration>();
        decoration.decorationArea = area;
        decoration.image.sprite = mapObject.image.sprite;
        decoration.transform.SetParent(transform);

        BoxCollider boxCollider = decoration.GetComponent<BoxCollider>();

        Bounds bounds = area.bounds;

        for (int i = 0; i < 100; i++)
        {
            Vector3 extents = boxCollider.bounds.extents;

            Vector3 position = new Vector3(
                Random.Range(bounds.min.x + extents.x, bounds.max.x - extents.x),
                Random.Range(bounds.min.y + extents.y, bounds.max.y - extents.y),
                0f
            );

            // Make sure the decoration is completely inside the island
            if (!IsDecorationInsideIsland(position, boxCollider))
                continue;

            // Check for obstacles
            Collider[] colliders = Physics.OverlapBox(
                position,
                boxCollider.size * 0.5f,
                decoration.transform.rotation,
                obstacleLayer
            );

            if (colliders.Length == 0)
            {
                decoration.transform.position = position;
                break;
            }
        }
    }

    private bool IsDecorationInsideIsland(
        Vector3 position,
        BoxCollider boxCollider)
    {
        Vector3 halfExtents = boxCollider.size * 0.5f;

        Vector3[] corners =
        {
            position + new Vector3(-halfExtents.x, -halfExtents.y, 0),
            position + new Vector3(-halfExtents.x,  halfExtents.y, 0),
            position + new Vector3( halfExtents.x, -halfExtents.y, 0),
            position + new Vector3( halfExtents.x,  halfExtents.y, 0)
        };

        foreach (Vector3 corner in corners)
        {
            Vector2 point = new Vector2(corner.x, corner.y);

            if (!area.OverlapPoint(point))
                return false;
        }

        return true;
    }
}
