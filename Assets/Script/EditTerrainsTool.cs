using UnityEngine;

public class EditTerrainsTool : MonoBehaviour
{
    [SerializeField] private Terrain terrain;
    private int heightMapWidth;
    private int heightMapHeight;
    private float[,] heights;
    private TerrainData terrainData;
    
    [SerializeField] private float strength = 0.01f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        terrainData = terrain.terrainData;
        heightMapWidth = terrainData.heightmapResolution;
        heightMapHeight = terrainData.heightmapResolution;
        heights = terrainData.GetHeights(0, 0, heightMapWidth, heightMapHeight);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if(Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                RaiseTerrainTool(hit.point);
            }
        }
        if(Input.GetMouseButton(1)){
            if (Physics.Raycast(ray, out hit))
            {
                LowerTerrainTool(hit.point);
            }
        }
    }

    Vector2Int GetmousePos(Vector3 worldPos)
    {
        int mouseX = (int)(worldPos.x / terrainData.size.x) * heightMapWidth;
        int mouseZ = (int)(worldPos.z / terrainData.size.z) * heightMapHeight;
        return new Vector2Int(mouseX, mouseZ);
    }

    void RaiseTerrainTool(Vector3 pos)
    {
        int mouseX = (int)(pos.x / terrainData.size.x) * heightMapWidth;
        int mouseZ = (int)(pos.z / terrainData.size.z) * heightMapHeight;

        float[,] modifiedHeights = new float[1, 1];
        float y = heights[mouseX, mouseZ];
        y += strength * Time.deltaTime;

        if (y > terrainData.size.y)
        {
            y = terrainData.size.y;
        }
        modifiedHeights[0, 0] = y;
        heights[mouseX, mouseZ] = y;
        terrainData.SetHeights(mouseX, mouseZ, heights);
    }

    void LowerTerrainTool(Vector3 pos)
    {
        int mouseX = (int)(pos.x / terrainData.size.x) * heightMapWidth;
        int mouseZ = (int)(pos.z / terrainData.size.z) * heightMapHeight;

        float[,] modifiedHeights = new float[1, 1];
        float y = heights[mouseX, mouseZ];
        y -= strength * Time.deltaTime;

        if (y < 0)
        {
            y = 0;
        }
        modifiedHeights[0, 0] = y;
        heights[mouseX, mouseZ] = y;
        terrainData.SetHeights(mouseX, mouseZ, heights);
    }
}
