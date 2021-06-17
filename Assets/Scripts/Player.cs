using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;

    [SerializeField]
    private Vector2 _direction = Vector2.zero;

    private CameraBounds _cameraBounds = null;
    private Vector3 _playerPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            _cameraBounds = camObj.GetComponent<CameraBounds>();
        else
            Debug.LogError("Camera not found");
    }

    // Update is called once per frame
    void Update()
    {
        CheckBounds();
        Move();
    }

    private void Move()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        _direction.x = xInput;
        _direction.y = yInput;
        transform.Translate(_direction * Time.deltaTime * _speed);
    }

    private void CheckBounds()
    {
        if (_cameraBounds != null)
            _playerPosition.Set(
                Mathf.Clamp(transform.position.x, -_cameraBounds.CameraVisual.x, _cameraBounds.CameraVisual.x),
                Mathf.Clamp(transform.position.y, -_cameraBounds.CameraVisual.y, _cameraBounds.CameraVisual.y),
                transform.position.z);

        transform.position = _playerPosition;
    }
}
