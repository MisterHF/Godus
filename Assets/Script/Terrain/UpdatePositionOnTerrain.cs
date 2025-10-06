using Unity.VisualScripting;
using UnityEngine;

public class UpdatePositionOnTerrain : MonoBehaviour
{
    [SerializeField] private Terrain terrain;
    [SerializeField] private float offsetY = 0f; 

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (terrain == null)
        {
            terrain = Terrain.activeTerrain; 
        }
    }

    void FixedUpdate()
    {
        if (terrain == null) return;

        Vector3 pos = transform.position;

        float terrainHeight = terrain.SampleHeight(pos) + terrain.transform.position.y;

        float newY = terrainHeight + offsetY;

        if (rb != null && !rb.isKinematic)
        {
            Vector3 targetPos = new Vector3(pos.x, newY, pos.z);
            rb.MovePosition(targetPos);
        }
        else
        {
            pos.y = newY;
            transform.position = pos;
        }
    }
}
