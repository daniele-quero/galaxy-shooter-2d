using CustomInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour, ISpawnable
{
    [SerializeField]
    private bool _isOscillating = false;
    private bool _recycle = true;


    private float _phase, _speed, _dodgeProbability;

    private Enemy _enemy;
    private EnemyTargetingSystem _target;
    private SpawnLimit _spawnLimit = new SpawnLimit();
    private Vector3 _respawnPosition = new Vector3(0, 0, 0);
    private SpriteRenderer _spriteRenderer;
    private Transform _playerTransform;
    //private TiltManeuver _tilt;

    public CameraBounds cameraBounds = null;

    public SpawnLimit SpawnLimit { get => CalculateSpawnLimits(); }
    public float Speed { get => _speed; set => _speed = value; }
    public bool Recycle { get => _recycle; set => _recycle = value; }

    void Start()
    {
        _enemy = GetComponent<Enemy>();
        _target = GetComponent<EnemyTargetingSystem>();
        //_tilt = GetComponent<TiltManeuver>();
        _dodgeProbability = _enemy.lvlManager.enemyDodgeProbability;
        _speed = _enemy.lvlManager.enemySpeed;
        _phase = Random.value;
        if (_enemy.lvlManager.enemyOscillationProbability > 0
            && _phase <= _enemy.lvlManager.enemyOscillationProbability)
            _isOscillating = true;

        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            cameraBounds = camObj.GetComponent<CameraBounds>();
        else
            Utilities.LogNullGrabbed("Camera");

        _playerTransform = GameObject.Find("Player").transform;
    }

    void Update()
    {
        Move();
        RespawnAtTop();
        Dodge();
        RamIntoPlayer();
    }

    public SpawnLimit CalculateSpawnLimits()
    {
        return _spawnLimit.Calculate(gameObject, _spriteRenderer, cameraBounds);
    }

    private void Move()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime, Space.World);
        if (_isOscillating)
            Oscillate();
    }

    private void Oscillate()
    {
        float a = _enemy.lvlManager.enemyOscillationAmplitude;
        float w = _enemy.lvlManager.enemyOscillationFrequency;
        float sin = Mathf.Sin(w * Time.time + _phase * Mathf.PI);
        //_tilt.Tilt(sin);
        transform.Translate(Vector3.right * a * sin);
    }

    public void RespawnAtTop()
    {
        _spawnLimit = SpawnLimit;
        
        if (_recycle)
        {
            if (transform.position.y <= _spawnLimit.YMin && _enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("enemy_ok"))
            {
                transform.localRotation = Quaternion.identity;
                _respawnPosition.y = _spawnLimit.YMax;
                _respawnPosition.x = Random.Range(_spawnLimit.XMin, _spawnLimit.XMax);
                transform.position = _respawnPosition;
            }
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    private void Dodge()
    {
        if (_target.Engage(new string[] { "laser", "deathRay", "torpedo" }, Vector2.down))
            if (Random.Range(0f, 1f) <= _dodgeProbability)
            {
                int r = Random.Range(0, 2);
                Vector3 direction = r == 0 ? Vector3.left : Vector3.right;
                StartCoroutine(Drift(direction, _speed * 1.2f, 0.5f));
            }

    }

    private void RamIntoPlayer()
    {

        if (_playerTransform != null && transform.position.y > _playerTransform.position.y
            && Vector3.Distance(transform.position, _playerTransform.position) < 6)
        {
            Vector3 dir = _playerTransform.position - transform.position;
            StartCoroutine(Drift(dir, _speed * 0.03f, 0.3f));
        }

    }

    private IEnumerator Drift(Vector3 dir, float speed, float duration)
    {
        float timeStep = 0.02f;
        float timer = 0f;
        while (timer <= duration)
        {
            transform.Translate(dir * speed * timeStep);
            timer += timeStep;
            yield return new WaitForSeconds(timeStep);
        }
    }
}
