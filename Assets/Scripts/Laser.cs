using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6f,
        _boundOffset = 1f;

    private CameraBounds _cameraBounds = null;

    private GameObject _laserContainer = null;
    void Start()
    {
        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            _cameraBounds = camObj.GetComponent<CameraBounds>();
        else
            Debug.LogError("Camera not found");

        _laserContainer = GameObject.FindGameObjectWithTag("laserContainer");
        if (_laserContainer != null)
        {
            if (this.transform.parent != null)
                this.transform.parent.transform.SetParent(_laserContainer.transform);
            else
                this.transform.SetParent(_laserContainer.transform);
        }
            
    }

    
    void Update()
    {
        Move();
        RemoveFarLasers();
    }

    private void Move()
    {
        transform.Translate(Vector3.up* _speed * Time.deltaTime);
    }

    private void RemoveFarLasers()
    {
        if(transform.position.y >= _cameraBounds.CameraVisual.y + _boundOffset)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
