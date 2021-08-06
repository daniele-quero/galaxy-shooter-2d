using CustomInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour, ISpawnable
{
    [SerializeField]
    private bool _isOscillating = false;

    [SerializeField]
    private float _phase, _speed;

    private Enemy _enemy;
    private SpawnLimit _spawnLimit = new SpawnLimit();
    private Vector3 _respawnPosition = new Vector3(0, 0, 0);
    private SpriteRenderer _spriteRenderer;

    public CameraBounds cameraBounds = null;

    public SpawnLimit SpawnLimit { get => CalculateSpawnLimits(); }
    // Start is called before the first frame update
    void Start()
    {
        _enemy = GetComponent<Enemy>();

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

    // Update is called once per frame
    void Update()
    {
        Move();
        RespawnAtTop();
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

    public void ScaleSpeed(float factor)
    {
        _speed *= factor;
    }
}
