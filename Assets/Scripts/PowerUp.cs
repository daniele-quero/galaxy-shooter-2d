using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInterfaces;

public class PowerUp : MonoBehaviour, ISpawnable
{
    [SerializeField]
    private float _speed = 6f;
    public float duration = 5f;

    private SpawnLimit _spawnLimit = new SpawnLimit();

    public SpawnLimit SpawnLimit { get => CalculateSpawnLimits(); }
    private CameraBounds _cameraBounds = null;
    private SpriteRenderer _spriteRenderer;

    void Update()
    {
        Move();
        RespawnAtTop();
    }

    private void Move()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    public SpawnLimit CalculateSpawnLimits()
    {
        _spriteRenderer = _spriteRenderer == null ? GetComponent<SpriteRenderer>() : _spriteRenderer;
        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            _cameraBounds = _cameraBounds == null ? camObj.GetComponent<CameraBounds>() : _cameraBounds;
        else
            Debug.LogError("Camera not found");
        float yOff = _spriteRenderer.sprite.rect.size.y / 2 / _spriteRenderer.sprite.pixelsPerUnit * transform.lossyScale.y;
        float xOff = _spriteRenderer.sprite.rect.size.x / 2 / _spriteRenderer.sprite.pixelsPerUnit * transform.lossyScale.x;
        _spawnLimit.YMax = _cameraBounds.CameraVisual.y + yOff;
        _spawnLimit.YMin = -_cameraBounds.CameraVisual.y - yOff;
        _spawnLimit.XMin = -_cameraBounds.CameraVisual.x + xOff;
        _spawnLimit.XMax = _cameraBounds.CameraVisual.x - xOff;
        return _spawnLimit;
    }

    public void RespawnAtTop()
    {
        _spawnLimit = SpawnLimit;

        if (transform.position.y <= _spawnLimit.YMin)
        {
            SelfDestroy();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player player = collision.GetComponent<Player>();
            if (tag == "tripleShotPowerUp")
            {
                Debug.Log("collected");
                if (player != null)
                    player.ActivatePowerUp(this);
            }
        }
        if (collision.tag == "laser")
            GameObject.Destroy(collision.gameObject);

        SelfDestroy();
    }

    private void SelfDestroy()
    {
        GameObject.Destroy(this.gameObject);
    }

}
