using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6f;

    private CameraBounds _cameraBounds = null;
    private Vector3 _direction = Vector3.up;
    private GameObject _laserContainer = null;
    void Start()
    {
        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            _cameraBounds = camObj.GetComponent<CameraBounds>();
        else
            Utilities.LogNullGrabbed("Camera");

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
        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
    }

    private void RemoveFarLasers()
    {
        Vector2 min = new Vector2(-_cameraBounds.CameraVisual.x, -_cameraBounds.CameraVisual.y);
        Vector2 size = new Vector2(_cameraBounds.CameraVisual.x, _cameraBounds.CameraVisual.y) * 2.1f;
        Rect field = new Rect(min, size);

        if (!field.Contains(transform.position))
        {
            if (transform.parent.tag.Contains("enemy"))
                GameObject.Destroy(transform.parent.gameObject);
            else
                GameObject.Destroy(this.gameObject);
        }
    }

    public void SetEnemyLaser(Vector2 direction)
    {
        this.tag = "enemyLaser";
        _speed = GameObject.Find("LevelManager").GetComponent<LvlManager>().enemyLaserSpeed;
        _direction = direction;
    }
}
