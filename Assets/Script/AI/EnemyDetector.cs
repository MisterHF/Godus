using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Raycast hit: " + hit.collider.gameObject.name + " | Tag: " + hit.collider.tag);

                if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log("ENEMY DETECTED -> Destroying");
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }
}