using UnityEngine;

public class EditTerrainTool : MonoBehaviour
{
    // Terrain
    private TerrainData _terrainData;
    [SerializeField] private Terrain terrain;

    // Modification du terrain
    private int _heightMapWidth;
    private int _heightMapHeight;
    [SerializeField] private float strength = 0.01f;

    // Brush
    [SerializeField] private float sizeBrush = 10f;
    [SerializeField] private int brushResolution = 32;

    // Line Renderer
    private LineRenderer _lineRenderer;
    [SerializeField] private int circleSegments = 64;

    UpdatePositionOnTerrain _updatePositionOnTerrain;
    void Awake()
    {
        if (_lineRenderer == null)
        {
            GameObject lrObj = new GameObject("BrushCircle");
            lrObj.transform.parent = this.transform;
            _lineRenderer = lrObj.AddComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.loop = true;
            _lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            _lineRenderer.material.color = Color.red;
            _lineRenderer.widthMultiplier = 0.05f;
        }
    }

    void Start()
    {
        _terrainData = Instantiate(terrain.terrainData);
        terrain.terrainData = _terrainData;
        TerrainCollider terrainCollider = terrain.GetComponent<TerrainCollider>();
        
        if (terrainCollider != null)
        {
            terrainCollider.terrainData = _terrainData;
        }

        _heightMapWidth = _terrainData.heightmapResolution;
        _heightMapHeight = _terrainData.heightmapResolution;
    }


    void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 brushCenter = hit.point;
            float brushRadius = sizeBrush / 2f;

            DrawBrushCircle(brushCenter, brushRadius);
            
            bool isLeftClick = Input.GetMouseButton(0);
            bool isRightClick = Input.GetMouseButton(1);

            if (isLeftClick)
                ModifyTerrain(brushCenter, 1f);
            else if (isRightClick)
                ModifyTerrain(brushCenter, -1f);
            
            UpdatePositionOnTerrain[] allTargets = FindObjectsOfType<UpdatePositionOnTerrain>(true);

            foreach (UpdatePositionOnTerrain target in allTargets)
            {
                Vector3 targetPos = target.transform.position;
                Vector2 brushXZ = new Vector2(brushCenter.x, brushCenter.z);
                Vector2 targetXZ = new Vector2(targetPos.x, targetPos.z);

                float distance = Vector2.Distance(brushXZ, targetXZ);
                bool isInBrush = distance <= brushRadius;

                if (isInBrush && (isLeftClick || isRightClick))
                {
                    if (!target.enabled)
                        target.enabled = true;
                }
                else
                {
                    if (target.enabled)
                        target.enabled = false;
                }
            }
        }
    }



    void ModifyTerrain(Vector3 pos, float direction)
    {
        Vector3 terrainPos = pos - terrain.transform.position;

        float radius = sizeBrush / 2f;

        int brushSize = brushResolution;
        int centerX = Mathf.RoundToInt((terrainPos.x / _terrainData.size.x) * _heightMapWidth);
        int centerZ = Mathf.RoundToInt((terrainPos.z / _terrainData.size.z) * _heightMapHeight);

        int halfBrushSize = brushSize / 2;
        int startX = Mathf.Clamp(centerX - halfBrushSize, 0, _heightMapWidth - brushSize);
        int startZ = Mathf.Clamp(centerZ - halfBrushSize, 0, _heightMapHeight - brushSize);

        float[,] heights = _terrainData.GetHeights(startX, startZ, brushSize, brushSize); //still don't quite truly understand this guy

        for (int x = 0; x < brushSize; x++)
        {
            for (int z = 0; z < brushSize; z++)
            {
                float offsetX = (float)(x - halfBrushSize) / brushSize * sizeBrush;
                float offsetZ = (float)(z - halfBrushSize) / brushSize * sizeBrush;

                float distance = Mathf.Sqrt(offsetX * offsetX + offsetZ * offsetZ);

                if (distance <= radius)
                {
                    float delta = strength * direction * Time.deltaTime;
                    heights[z, x] = Mathf.Clamp01(heights[z, x] + delta);
                }
            }
        }

        _terrainData.SetHeights(startX, startZ, heights);
        
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
}
