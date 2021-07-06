using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInterfaces;

public class Enemy : MonoBehaviour, ISpawnable
{
    [SerializeField]
    private float _speed = 4f;

    private CameraBounds _cameraBounds = null;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _respawnPosition = new Vector3(0, 0, 0);
    private SpawnLimit _spawnLimit = new SpawnLimit();

    [SerializeField]
    private int _scoreValue = 10;

    public SpawnLimit SpawnLimit { get => CalculateSpawnLimits(); }

    void Start()
    {

    }

    void Update()
    {
        Move();
        RespawnAtTop();
    }

    private void Move()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    public void RespawnAtTop()
    {
        _spawnLimit = SpawnLimit;

        if (transform.position.y <= _spawnLimit.YMin)
        {
            _respawnPosition.y = _spawnLimit.YMax;
            _respawnPosition.x = Random.Range(_spawnLimit.XMin, _spawnLimit.XMax);
            transform.position = _respawnPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "laser":
                {
                    GameObject.Destroy(collision.gameObject);
                    Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                    if (player != null)
                    {
                        player.Score += _scoreValue;
                        UIManager ui = GameObject.Find("Canvas").GetComponent<UIManager>();
                        if (ui != null)
                            ui.UpdateScoreText(player.Score);
                    }
                    SelfDestroy();
                    break;
                }
            case "Player":
                {
                    Player player = collision.GetComponent<Player>();
                    if (player != null)
                        player.Damage(1);
                    break;
                }
            case "shields":
                SelfDestroy();
                break;
            default:
                break;
        }
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

    private void SelfDestroy()
    {
        GameObject.Destroy(this.gameObject);
    }
}
