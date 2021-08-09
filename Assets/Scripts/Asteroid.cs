using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInterfaces;

public class Asteroid : MonoBehaviour, ISpawnable
{
    [SerializeField]
    private float _angSpeed = 10f,
        _speed = 0;

    [SerializeField]
    private Vector3 _direction = Vector3.zero;

    [SerializeField]
    private int _lives = 1,
        _score = 5;

    public bool isWaveAsteroid = false;
    public bool isFreeToMove = false;

    private Animator _animator;
    private SpawnLimit _spawnLimit = new SpawnLimit();

    public SpawnLimit SpawnLimit { get => CalculateSpawnLimits(); }
    private CameraBounds _cameraBounds = null;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidBody;
    private Rect _asteroidField;
    private AudioSource[] _sources;
    private Dictionary<string, AudioSource> _sounds;

    void Start()
    {
        _animator = GetComponent<Animator>();
        Utilities.CheckNullGrabbed(_animator, "Animator");

        _rigidBody = GetComponent<Rigidbody2D>();
        Utilities.CheckNullGrabbed(_animator, "Rigid Body 2D");

        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            _cameraBounds = camObj.GetComponent<CameraBounds>();
        else
            Utilities.LogNullGrabbed("Camera");

        DefineAsteroidField();

        _rigidBody.angularVelocity = _angSpeed;
        if (isFreeToMove)
            InitAsteroid();

        _sources = GetComponents<AudioSource>();
        _sounds = new Dictionary<string, AudioSource>()
        {
            ["laserDamage"] = _sources[0],
            ["explosion"] = _sources[1],
            ["collision"] = _sources[2]
        };

        Move();
    }

    // Update is called once per frame
    void Update()
    {
        RespawnAtTop();
    }

    private void Move()
    {
        _speed = Random.Range(8f, 16f);
        _angSpeed = Random.Range(0f, 150f);
        _rigidBody.velocity = _direction * _speed;
        _rigidBody.angularVelocity = _angSpeed;
    }

    private void InitAsteroid()
    {
        transform.localScale *= Random.Range(0.5f, 2f);
        _rigidBody.mass *= Mathf.Pow(transform.localScale.x, 2);

        float sin = Random.Range(-1f, 1f);
        float cos = Random.Range(-1f, 1f);
        _direction.z = 0;
        _direction.x = cos;
        _direction.y = sin;
    }

    public void AsteroidDestruction()
    {
        AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
        GameObject.FindGameObjectWithTag("ppv").GetComponent<PostProcessingManager>().ExplosionBloom(clips[0].length);
        _animator.SetTrigger("onAsteroidDestruction");
        _cameraBounds.CameraShake();
        _rigidBody.velocity *= 0.75f;
        _rigidBody.angularVelocity *= 0.75f;  
        GetComponent<Collider2D>().enabled = false;
        _sounds["explosion"].Play();
        GameObject.Destroy(this.gameObject, clips[0].length);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "torpedo":
                {
                    _lives--;
                    goto case "laser";
                }
            case "laser":
                {
                    AsteroidDamage();
                    GameObject.Destroy(collision.gameObject);
                    break;
                }
            case "enemy":
                {
                    AsteroidDamageBase();
                    Enemy enemy = collision.GetComponent<Enemy>();
                    if (enemy != null)
                        enemy.EnemyDeath();

                    break;
                }
            case "Player":
                {
                    AsteroidDamageBase();
                    Player player = collision.GetComponent<Player>();
                    if (player != null)
                        player.Damage(1, transform.position.x);
                    break;
                }
            default:
                break;
        }
    }

    private void AsteroidDamageBase()
    {
        _lives--;
        _sounds["collision"].Play();
        if (_lives < 0)
            AsteroidDestruction();
    }

    public void AsteroidDamage()
    {
        AsteroidDamage(true);
    }

    public void AsteroidDamage(bool isScore)
    {
        AsteroidDamageBase();
        if (_lives < 0)
        {
            Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            if (player != null && isScore)
                player.AddScore(_score);
        }
        _rigidBody.AddForce(Vector2.up * 0.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            case "asteroid":
                _sounds["collision"].Play();
                break;
            case "shields":
                AsteroidDamageBase();

                Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                if (player != null)
                    player.Damage(1, transform.position.x);
                break;
            default:
                break;
        }
    }

    public SpawnLimit CalculateSpawnLimits()
    {
        return _spawnLimit.Calculate(gameObject, _spriteRenderer, _cameraBounds);
    }

    public void RespawnAtTop()
    {
        DefineAsteroidField();
        if (!_asteroidField.Contains(transform.position) && _animator.GetCurrentAnimatorStateInfo(0).IsName("asteroid_ok"))
        {
            Vector3 newPos = transform.position;
            if (transform.position.y > _asteroidField.yMax)
                newPos.y = _asteroidField.yMin;
            else if (transform.position.y < _asteroidField.yMin)
                newPos.y = _asteroidField.yMax;

            if (transform.position.x > _asteroidField.xMax)
                newPos.x = _asteroidField.xMin;
            else if (transform.position.x < _asteroidField.xMin)
                newPos.x = _asteroidField.xMax;

            transform.position = newPos;
        }

    }

    private void DefineAsteroidField()
    {
        _spawnLimit = SpawnLimit;
        Vector2 min = new Vector2(_spawnLimit.XMin, _spawnLimit.YMin);
        Vector2 size = new Vector2(_spawnLimit.XMax - _spawnLimit.XMin + 30f, _spawnLimit.YMax - _spawnLimit.YMin + 30f);

        _asteroidField = new Rect(min, size);
    }
}
