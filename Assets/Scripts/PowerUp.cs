using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInterfaces;

public class PowerUp : MonoBehaviour, ISpawnable
{
    [SerializeField]
    private float _speed = 6f;
    public float duration = 5f;
    public float boost = 1.5f;
    public int shields = 2;
    public int scoreValue = 5;
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
        switch (collision.tag)
        {
            case "Player":
                {
                    Player player = collision.GetComponent<Player>();
                    Debug.Log(tag + " collected");
                    if (player != null)
                        player.ActivatePowerUp(this);
                    break;
                }
            case "laser":
                {
                    GameObject.Destroy(collision.gameObject);
                    Player player = GameObject.FindGameObjectWithTag("Player") != null
                        ? GameObject.FindGameObjectWithTag("Player").GetComponent<Player>()
                        : null;
                    if (player != null)
                    {
                        player.Score += scoreValue;
                        UIManager ui = GameObject.Find("Canvas").GetComponent<UIManager>();
                        if (ui != null)
                            ui.UpdateScoreText(player.Score);
                    }
                    break;
                }
            default:
                break;

        }

        SelfDestroy();
    }

    private void SelfDestroy()
    {
        GameObject.Destroy(this.gameObject);
    }

}
