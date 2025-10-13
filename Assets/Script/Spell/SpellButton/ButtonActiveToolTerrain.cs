using UnityEngine;

public class ButtonActiveToolTerrain : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _script;
    
    public void Toggle()
    {
        if (_script != null && _script.enabled)
        {
            _script.enabled = false;
        }
        else
        {
            _script.enabled = true;
        }
    }
}
