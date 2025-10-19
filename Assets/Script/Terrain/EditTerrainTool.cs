using UnityEngine;
using System.Collections.Generic;

public class EditTerrainTool : MonoBehaviour
{
    [Header("Terrains à modifier")]
    [SerializeField] private List<Terrain> terrains = new List<Terrain>();

    [Header("Paramètres de modification")]
    [SerializeField] private float strength = 0.01f;
    [SerializeField] private float maxHeight = 10f;
    [SerializeField] private float sizeBrush = 10f;
    [SerializeField] private int circleSegments = 64;
    
    private LineRenderer _lineRenderer;
    private GameObject _brushCircleObject;

    void Awake()
    {
        _brushCircleObject = new GameObject("BrushCircle");
        _brushCircleObject.transform.parent = this.transform;

        _lineRenderer = _brushCircleObject.AddComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.loop = true;
        _lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        _lineRenderer.material.color = Color.red;
        _lineRenderer.widthMultiplier = 0.05f;
    }
    void Start()
    {
        ResetTerrains();
    }

    void OnEnable() => _brushCircleObject?.SetActive(true);
    void OnDisable() => _brushCircleObject?.SetActive(false);

    void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Terrain touchedTerrain = null;
            foreach (var terrain in terrains)
            {
                if (hit.collider.gameObject == terrain.gameObject)
                {
                    touchedTerrain = terrain;
                    break;
                }
            }

            if (touchedTerrain == null)
                return;

            Vector3 brushCenter = hit.point;
            float brushRadius = sizeBrush / 2f;

            DrawBrushCircle(brushCenter, brushRadius);

            bool isLeftClick = Input.GetMouseButton(0);
            bool isRightClick = Input.GetMouseButton(1);

            if (isLeftClick || isRightClick)
                ModifyTerrain(touchedTerrain, brushCenter, isLeftClick ? 1f : -1f);
        }
    }

    void ModifyTerrain(Terrain terrain, Vector3 worldPos, float direction)
    {
        TerrainData terrainData = terrain.terrainData;

        int heightMapWidth = terrainData.heightmapResolution;
        int heightMapHeight = terrainData.heightmapResolution;
        float radius = sizeBrush / 2f;
        float normalizedMaxHeight = maxHeight / terrainData.size.y;

        Vector3 terrainPos = worldPos - terrain.transform.position;

        int centerX = Mathf.RoundToInt((terrainPos.x / terrainData.size.x) * heightMapWidth);
        int centerZ = Mathf.RoundToInt((terrainPos.z / terrainData.size.z) * heightMapHeight);

        int radiusInSamples = Mathf.RoundToInt((radius / terrainData.size.x) * heightMapWidth);
        int diameter = radiusInSamples * 2;

        int startX = Mathf.Clamp(centerX - radiusInSamples, 0, heightMapWidth - 1);
        int startZ = Mathf.Clamp(centerZ - radiusInSamples, 0, heightMapHeight - 1);

        int validWidth = Mathf.Min(diameter, heightMapWidth - startX);
        int validHeight = Mathf.Min(diameter, heightMapHeight - startZ);

        float[,] heights = terrainData.GetHeights(startX, startZ, validWidth, validHeight);

        for (int x = 0; x < validWidth; x++)
        {
            for (int z = 0; z < validHeight; z++)
            {
                int globalX = startX + x;
                int globalZ = startZ + z;
                
                if (globalX <= 1 || globalX >= heightMapWidth - 2 ||
                    globalZ <= 1 || globalZ >= heightMapHeight - 2)
                    continue;

                float worldX = (globalX / (float)heightMapWidth) * terrainData.size.x;
                float worldZ = (globalZ / (float)heightMapHeight) * terrainData.size.z;

                float dx = worldX - terrainPos.x;
                float dz = worldZ - terrainPos.z;
                float dist = Mathf.Sqrt(dx * dx + dz * dz);

                if (dist > radius)
                    continue;

                float falloff = Mathf.Clamp01(1f - (dist / radius));
                float delta = strength * direction * falloff * Time.deltaTime;

                float newHeight = heights[z, x] + delta;
                heights[z, x] = Mathf.Clamp(newHeight, 0f, normalizedMaxHeight);
            }
        }

        terrainData.SetHeights(startX, startZ, heights);
    }

    void DrawBrushCircle(Vector3 center, float radius)
    {
        if (_lineRenderer == null) return;

        _lineRenderer.positionCount = circleSegments + 1;

        for (int i = 0; i <= circleSegments; i++)
        {
            float angle = i * 2 * Mathf.PI / circleSegments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            Vector3 point = new Vector3(center.x + x, center.y + 0.05f, center.z + z);
            _lineRenderer.SetPosition(i, point);
        }
    }
    void ResetTerrains()
    {
        foreach (var terrain in terrains)
        {
            TerrainData data = terrain.terrainData;
            int width = data.heightmapResolution;
            int height = data.heightmapResolution;
            float[,] flatHeights = new float[height, width];
            data.SetHeights(0, 0, flatHeights);
        }
    }
}
