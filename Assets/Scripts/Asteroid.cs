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

    void Start()
    {
        _animator = GetComponent<Animator>();
        Utilities.CheckNullGrabbed(_animator, "Animator");

        _rigidBody = GetComponent<Rigidbody2D>();
        Utilities.CheckNullGrabbed(_animator, "Rigid Body 2D");

        DefineAsteroidField();

        _rigidBody.angularVelocity = _angSpeed;
        if (isFreeToMove)
            InitAsteroid();

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
        _animator.SetTrigger("onAsteroidDestruction");
        _rigidBody.velocity *= 0.75f;
        _rigidBody.angularVelocity *= 0.75f;
        AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
        GetComponent<Collider2D>().enabled = false;
        GameObject.Destroy(this.gameObject, clips[0].length);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "laser":
                {
                    _lives--;
                    if (_lives < 0)
                    {
                        AsteroidDestruction();
                        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                        if (player != null)
                            player.AddScore(_score);
                    }

                    GameObject.Destroy(collision.gameObject);
                    _rigidBody.AddForce(Vector2.up * 0.1f);
                    break;
                }
            case "enemy":
                {
                    _lives--;
                    if (_lives < 0)
                        AsteroidDestruction();

                    Enemy enemy = collision.GetComponent<Enemy>();
                    if (enemy != null)
                        enemy.EnemyDeath();

                    break;
                }
            case "Player":
                {
                    _lives--;
                    if (_lives < 0)
                        AsteroidDestruction();
                    Player player = collision.GetComponent<Player>();
                    if (player != null)
                        player.Damage(1, transform.position.x);
                    break;
                }
            case "shields":
                AsteroidDestruction();
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
        Vector2 size = new Vector2(_spawnLimit.XMax - _spawnLimit.XMin + 15f, _spawnLimit.YMax - _spawnLimit.YMin + 15f);

        _asteroidField = new Rect(min, size);
    }
}
