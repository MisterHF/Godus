using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraControl : MonoBehaviour
{ 
    private Camera _camera;

    [SerializeField] private float panSpeed = 6f;
    [SerializeField] private float baseHeight = 10f;
    [SerializeField] private float elevatedHeight = 30f;
    [SerializeField] private float heightTransitionDuration = 0.3f;

    private bool isElevated = false;
    private bool isTransitioning = false;

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
        Vector3 pos = _camera.transform.localPosition;
        pos.y = baseHeight;
        _camera.transform.localPosition = pos;
    }

    private void Update()
    {
        Pan();
        HandleScrollInput();
    }

    private void Pan()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 direction = Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0) * new Vector3(input.x, 0, input.y);
        transform.position += direction * panSpeed * Time.deltaTime;
    }

    private void HandleScrollInput()
    {
        if (isTransitioning)
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f && !isElevated)
        {
            ElevateCamera();
        }
        else if (scroll < 0f && isElevated)
        {
            LowerCamera();
        }
    }

    public void ElevateCamera()
    {
        if (isTransitioning || isElevated)
            return;

        StartCoroutine(SmoothHeightChange(elevatedHeight, true));
    }

    public void LowerCamera()
    {
        if (isTransitioning || !isElevated)
            return;

        StartCoroutine(SmoothHeightChange(baseHeight, false));
    }

    private IEnumerator SmoothHeightChange(float targetHeight, bool elevate)
    {
        isTransitioning = true;

        Vector3 start = _camera.transform.localPosition;
        Vector3 end = new Vector3(start.x, targetHeight, start.z);
        float elapsed = 0f;

        while (elapsed < heightTransitionDuration)
        {
            _camera.transform.localPosition = Vector3.Lerp(start, end, elapsed / heightTransitionDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _camera.transform.localPosition = end;
        isElevated = elevate;
        isTransitioning = false;
    }
}
