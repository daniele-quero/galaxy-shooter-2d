using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInterfaces;

public class PowerUp : MonoBehaviour, ISpawnable
{
    [SerializeField]
    private float _speed = 6f;
    public float duration = 5f;
    public float magnitude = 3f;
    public int scoreValue = 5;
    private SpawnLimit _spawnLimit = new SpawnLimit();

    public SpawnLimit SpawnLimit { get => CalculateSpawnLimits(); }
    public float Speed { get => _speed; set => _speed = value; }

    private CameraBounds _cameraBounds = null;
    private SpriteRenderer _spriteRenderer;
    private AudioSource[] _sources;
    private Dictionary<string, AudioSource> _sounds;

    void Start()
    {
        _sources = GetComponents<AudioSource>();
        _sounds = new Dictionary<string, AudioSource>()
        {
            ["collect"] = _sources[0],
            ["destroy"] = _sources[1]
        };
    }

    void Update()
    {
        Move();
        RespawnAtTop();
    }

    private void Move()
    {
        transform.Translate(Vector3.down * Speed * Time.deltaTime);
    }

    public SpawnLimit CalculateSpawnLimits()
    {
        return _spawnLimit.Calculate(gameObject, _spriteRenderer, _cameraBounds);
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
            case "shields":
            case "Player":
                {
                    PlayerCollectionManager pcm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCollectionManager>();
                    if (pcm != null)
                        pcm.ActivatePowerUp(this);

                    _sounds["collect"].Play();
                    SelfDestroy();
                    break;
                }
            case "laser":
                {
                    GameObject.Destroy(collision.gameObject);
                    DestroyPowerUp();
                    break;
                }
            case "deathRay":
                DestroyPowerUp();
                break;
            case "enemyLaser":
                {
                    scoreValue = 0;
                    DestroyPowerUp();
                    break;
                }
            default:
                break;
        }        
    }

    private void DestroyPowerUp()
    {
        Player player = GameObject.FindGameObjectWithTag("Player") != null
                        ? GameObject.FindGameObjectWithTag("Player").GetComponent<Player>()
                        : null;
        if (player != null)
            player.AddScore(scoreValue);

        _sounds["destroy"].Play();
        SelfDestroy();
    }

    private void SelfDestroy()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Destroy(this.gameObject,0.6f);
    }
}
