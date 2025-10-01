using UnityEngine;

public class EditTerrainTool : MonoBehaviour
{
    [SerializeField] private Terrain terrain;
    private int heightMapWidth;
    private int heightMapHeight;
    private TerrainData terrainData;

    [SerializeField] private float strength = 0.01f;
    [SerializeField] private float sizeBrush = 1f;
    
    

    void Start()
    {
        terrainData = Instantiate(terrain.terrainData);
        terrain.terrainData = terrainData;
        heightMapWidth = terrainData.heightmapResolution;
        heightMapHeight = terrainData.heightmapResolution;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                RaiseTerrainTool(hit.point);
            }
        }

        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(ray, out hit))
            {
                LowerTerrainTool(hit.point);
            }
        }
    }

    void RaiseTerrainTool(Vector3 pos)
    {
        Vector3 terrainPos = pos - terrain.transform.position;

        int mouseX = Mathf.RoundToInt((terrainPos.x / terrainData.size.x) * heightMapWidth);
        int mouseZ = Mathf.RoundToInt((terrainPos.z / terrainData.size.z) * heightMapHeight);

        mouseX = Mathf.Clamp(mouseX, 0, heightMapWidth - 1);
        mouseZ = Mathf.Clamp(mouseZ, 0, heightMapHeight - 1);

        float[,] heights = terrainData.GetHeights(mouseX, mouseZ, 1, 1);
        heights[0, 0] = Mathf.Clamp01(heights[0, 0] + strength * Time.deltaTime);
        terrainData.SetHeights(mouseX, mouseZ, heights);
    }

    void LowerTerrainTool(Vector3 pos)
    {
        Vector3 terrainPos = pos - terrain.transform.position;

        int mouseX = Mathf.RoundToInt((terrainPos.x / terrainData.size.x) * heightMapWidth);
        int mouseZ = Mathf.RoundToInt((terrainPos.z / terrainData.size.z) * heightMapHeight);

        mouseX = Mathf.Clamp(mouseX, 0, heightMapWidth - 1);
        mouseZ = Mathf.Clamp(mouseZ, 0, heightMapHeight - 1);

        float[,] heights = terrainData.GetHeights(mouseX, mouseZ, 1, 1);
        heights[0, 0] = Mathf.Clamp01(heights[0, 0] - strength * Time.deltaTime);
        terrainData.SetHeights(mouseX, mouseZ, heights);
    }
}
