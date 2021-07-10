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

    Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        Utilities.CheckNullGrabbed(_animator, "Animator");
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

        if (transform.position.y <= _spawnLimit.YMin && _animator.GetCurrentAnimatorStateInfo(0).IsName("enemy_ok"))
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

                    EnemyDeath();
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
                EnemyDeath();
                break;
            default:
                break;
        }
    }

    public void EnemyDeath()
    {
        _animator.SetTrigger("onEnemyDeath");
        AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
        GetComponent<Collider2D>().enabled = false;
        SelfDestroy(clips[0].length);
    }

    public SpawnLimit CalculateSpawnLimits()
    {
        return _spawnLimit.Calculate(gameObject, _spriteRenderer, _cameraBounds);
    }

    private void SelfDestroy()
    {
        GameObject.Destroy(this.gameObject);
    }

    private void SelfDestroy(float time)
    {
        GameObject.Destroy(this.gameObject, time);
    }
}
