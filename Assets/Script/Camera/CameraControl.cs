using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{ 
    //Cam
    private Camera _camera;
    //Pan
    [SerializeField] private float panSpeed = 6f;

    //Zoom
    [SerializeField] private float zoomSpeed = 6f;
    [SerializeField] private float zoomSmoothness = 6f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 40f;
    private float _currentZoom;
    
    //Rotation
    [SerializeField] private float rotateSpeed = 20f;
    
    
  private void Awake()
  {
    _camera = GetComponentInChildren<Camera>();
  }

  private void Update()
  {
      Pan();
      Zoom();
      RotateLeft();
      RotateRight();
  }

  private void Pan()
  {
      Vector2 panPos = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
      transform.position += Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0) * new Vector3(panPos.x, 0, panPos.y) * (panSpeed *  Time.deltaTime);
  }

  private void Zoom()
  {
      _currentZoom = Mathf.Clamp(_currentZoom - Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
      _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _currentZoom, zoomSmoothness * Time.deltaTime);
  }

  float GetAxis()
  {
      float mouseDeltaX = Input.GetAxis("Mouse X");
      return mouseDeltaX;
  }
 

  private void RotateLeft()
  {
      transform.Rotate(Vector3.up, GetAxis() * rotateSpeed);
  }  
  
  private void RotateRight()
  {
      transform.Rotate(Vector3.up, -GetAxis() * rotateSpeed);
  }
}
