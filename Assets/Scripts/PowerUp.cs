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
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
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
                    Player player = collision.GetComponent<Player>();
                    if (player != null)
                        player.ActivatePowerUp(this);

                    _sounds["collect"].Play();
                    SelfDestroy();
                    break;
                }
            case "laser":
                {
                    GameObject.Destroy(collision.gameObject);
                    Player player = GameObject.FindGameObjectWithTag("Player") != null
                        ? GameObject.FindGameObjectWithTag("Player").GetComponent<Player>()
                        : null;
                    if (player != null)
                        player.AddScore(scoreValue);

                    _sounds["destroy"].Play();
                    SelfDestroy();
                    break;
                }
            default:
                break;
        }

        
    }

    private void SelfDestroy()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Destroy(this.gameObject,0.5f);
    }

}
