using UnityEngine;

[ExecuteAlways]
public class TerraonCheckCollider : MonoBehaviour
{
    [SerializeField] private Terrain terrain;
    [SerializeField] private Color lineColor = Color.green;
    [SerializeField] private int step = 10; // Espacement entre les points du maillage

    private void OnDrawGizmos()
    {
        if (terrain == null) return;

        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        int width = data.heightmapResolution;
        int height = data.heightmapResolution;

        float[,] heights = data.GetHeights(0, 0, width, height);

        Gizmos.color = lineColor;

        for (int x = 0; x < width - step; x += step)
        {
            for (int z = 0; z < height - step; z += step)
            {
                Vector3 p1 = new Vector3(
                    x / (float)width * data.size.x,
                    heights[z, x] * data.size.y,
                    z / (float)height * data.size.z
                ) + terrainPos;

                Vector3 p2 = new Vector3(
                    (x + step) / (float)width * data.size.x,
                    heights[z, x + step] * data.size.y,
                    z / (float)height * data.size.z
                ) + terrainPos;

                Vector3 p3 = new Vector3(
                    x / (float)width * data.size.x,
                    heights[z + step, x] * data.size.y,
                    (z + step) / (float)height * data.size.z
                ) + terrainPos;

                // Lignes horizontales et verticales
                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p1, p3);
            }
        }
    }
}