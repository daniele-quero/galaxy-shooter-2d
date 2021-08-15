using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    private LvlManager _lvl;
    private Player _player;
    private SpawnLimit _spawnLimit = new SpawnLimit();
    private SpriteRenderer _spriteRenderer;

    public CameraBounds cameraBounds = null;[SerializeField]
    private float _speed = 2f, _yPos = 5f;
    private int _killTemp;

    public SpawnLimit SpawnLimit { get => CalculateSpawnLimits(); }

    void Start()
    {
        transform.position = new Vector3(0, 25f, 0);

        _player = GameObject.Find("Player").GetComponent<Player>();
        _lvl = GameObject.Find("LevelManager").GetComponent<LvlManager>();
        _killTemp = _player.Kills;
        StartCoroutine(EnterMove());
    }

    public SpawnLimit CalculateSpawnLimits()
    {
        return _spawnLimit.Calculate(gameObject, _spriteRenderer, cameraBounds);
    }
   
    private IEnumerator EnterMove()
    {
        GetComponent<EnemyShooting>().IsPaused = true;
        Enemy enemy = GetComponent<Enemy>();
        Boss boss = GetComponent<Boss>();

        float timeStep = 0.02f;
        while (transform.position.y > _yPos)
        {
            if (Camera.main.orthographicSize < 20)
                Camera.main.orthographicSize += 0.04f;

            transform.Translate(Vector3.down * _speed * timeStep);
            _player.Kills = _killTemp;
            enemy.Lives = boss.BossLives;
            yield return new WaitForSeconds(timeStep);
        }

        GetComponent<EnemyShooting>().IsPaused = false;
    }
}
