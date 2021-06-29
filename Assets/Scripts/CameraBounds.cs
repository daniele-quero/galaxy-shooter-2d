using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    [SerializeField]
    private Vector2 _cameraVisual = new Vector2(666, 666);

    private Camera _camera;
    private SpriteRenderer _bg;
    private Vector3 _scale = new Vector3(1, 1, 1);
    private Vector3 _position = Vector3.zero;
    public Vector2 CameraVisual { get => _cameraVisual; set => _cameraVisual = value; }

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        _bg = GetComponentInChildren<SpriteRenderer>();
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

        _scale.y = _scale.x = 2 * _cameraVisual.x * _bg.sprite.pixelsPerUnit / _bg.sprite.rect.size.x;
        _bg.transform.localScale = _scale;
        _position = _camera.transform.position;
        _position.z = -_position.z;
        _bg.transform.localPosition = _position;
    }

}
