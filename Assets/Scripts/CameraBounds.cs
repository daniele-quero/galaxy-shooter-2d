using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    [SerializeField]
    private Vector2 _cameraVisual = new Vector2(666, 666);

    private Camera _camera;

    public Vector2 CameraVisual { get => _cameraVisual; set => _cameraVisual = value; }

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraVisual();
    }

    private void UpdateCameraVisual()
    {
        float aspect = (float)Screen.width / (float)Screen.height;
        _cameraVisual.y = _camera.orthographicSize;
        _cameraVisual.x = CameraVisual.y * aspect;
    }

}
