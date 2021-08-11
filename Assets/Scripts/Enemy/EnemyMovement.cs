using CustomInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour, ISpawnable
{
    [SerializeField]
    private bool _isOscillating = false;

    [SerializeField]
    private float _phase, _speed, _dodgeProbability;

    private Enemy _enemy;
    private EnemyTargetingSystem _target;
    private SpawnLimit _spawnLimit = new SpawnLimit();
    private Vector3 _respawnPosition = new Vector3(0, 0, 0);
    private SpriteRenderer _spriteRenderer;

    public CameraBounds cameraBounds = null;

    public SpawnLimit SpawnLimit { get => CalculateSpawnLimits(); }

    void Start()
    {
        _enemy = GetComponent<Enemy>();
        _target = GetComponent<EnemyTargetingSystem>();

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

    }

    void Update()
    {
        Move();
        RespawnAtTop();
        Dodge();
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
        transform.Translate(Vector3.right * a * Mathf.Sin(w * Time.time + _phase * Mathf.PI));
    }

    public void RespawnAtTop()
    {
        _spawnLimit = SpawnLimit;

        if (transform.position.y <= _spawnLimit.YMin && _enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("enemy_ok"))
        {
            _respawnPosition.y = _spawnLimit.YMax;
            _respawnPosition.x = Random.Range(_spawnLimit.XMin, _spawnLimit.XMax);
            transform.position = _respawnPosition;
        }
    }

    public void OverrideSpeed(float newSpeed)
    {
        _speed = newSpeed;
    }

    private void Dodge()
    {
        if (_target.Engage(new string[] { "laser", "deathRay", "torpedo" }, Vector2.down))
            if (Random.Range(0f, 1f) <= _dodgeProbability)
                StartCoroutine(DodgeMovement()); 
    }

    private IEnumerator DodgeMovement()
    {
        int r = Random.Range(0, 2);
        Vector3 direction = r == 0 ? Vector3.left : Vector3.right;
        float duration = 0.5f;
        float timeStep = 0.02f;
        float timer = 0f;
        while (timer <= duration)
        {
            transform.Translate(direction * _speed * 1.2f * timeStep);
            timer += timeStep;
            yield return new WaitForSeconds(timeStep);
        }
    }
}