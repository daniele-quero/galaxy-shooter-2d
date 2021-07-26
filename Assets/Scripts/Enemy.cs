using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInterfaces;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour, ISpawnable
{
    [SerializeField]
    private float _speed = 4f;

    private CameraBounds _cameraBounds = null;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _respawnPosition = new Vector3(0, 0, 0);
    private SpawnLimit _spawnLimit = new SpawnLimit();

    [SerializeField]
    private int _scoreValue = 10,
        _lives = 0;

    public SpawnLimit SpawnLimit { get => CalculateSpawnLimits(); }

    Animator _animator;
    private AudioSource[] _sources;
    private Dictionary<string, AudioSource> _sounds;
    private LvlManager _lvlManager;

    public bool defaultLevelSettings = true;
    private bool _isShooting = false;
    private float _laserRateMin = 3f;
    private float _laserRateMax = 7f;

    [SerializeField]
    GameObject _laser;

    void Start()
    {
        _animator = GetComponent<Animator>();
        Utilities.CheckNullGrabbed(_animator, "Animator");
        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            _cameraBounds = camObj.GetComponent<CameraBounds>();
        else
            Utilities.LogNullGrabbed("Camera");

        _sources = GetComponents<AudioSource>();
        _sounds = new Dictionary<string, AudioSource>()
        {
            ["collision"] = _sources[0],
            ["explosion"] = _sources[1],
            ["laser"] = _sources[2],
        };

        _lvlManager = GameObject.Find("LevelManager").GetComponent<LvlManager>();

        if (defaultLevelSettings)
        {
            _speed = _lvlManager.enemySpeed;
            _lives = _lvlManager.enemyLives;
            _scoreValue = _lvlManager.enemyScore;
            _isShooting = _lvlManager.isEnemyShooting;
            _laserRateMin = _lvlManager.enemyLaserRate[0];
            _laserRateMax = _lvlManager.enemyLaserRate[1];
        }

        StartCoroutine(Shoot());
    }

    void Update()
    {
        Move();
        RespawnAtTop();
        Shoot();
    }

    private void Move()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    private IEnumerator Shoot()
    {
        while (_isShooting)
        {
            yield return new WaitForSeconds(Random.Range(_laserRateMin, _laserRateMax));

            if (Engage(new string[]{"Player", "shields"}))
            {
                Vector2 laserSpawn = transform.position;
                laserSpawn.y -= _spawnLimit.YOff;
                GameObject enemyLaser = Instantiate(_laser, laserSpawn, Quaternion.identity);
                foreach(var las in enemyLaser.GetComponentsInChildren<Laser>())
                    las.SetEnemyLaser();

                _sounds["laser"].Play();
            }
        }
    }

    private bool Engage(string[] otherTags)
    {
        Vector2 origin = transform.position;
        origin.y -= _spawnLimit.YOff;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down);

        if (hit.collider != null)
        {
            foreach (var ot in otherTags)
                if (hit.collider.tag == ot)
                    return true; 
        }

        return  false;
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

    private void EnemyDamageBase()
    {
        _lives--;
        _sounds["collision"].Play();
        if (_lives < 0)
            EnemyDeath();
    }

    public void EnemyKill(bool isRam)
    {
        EnemyDamageBase();
        if (_lives < 0)
        {
            Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            if (player != null)
            {
                player.AddScore(_scoreValue);
                player.addKill(1);
                if(isRam)
                    player.Damage(1, transform.position.x);
            }
        }
    }

    public void EnemyKill()
    {
        EnemyKill(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "laser":
                {
                    GameObject.Destroy(collision.gameObject);
                    EnemyKill();
                    break;
                }
            case "Player":
                {
                    EnemyKill(true);
                    break;
                }
            case "shields":
                {
                    collision.GetComponent<AudioSource>().Play();
                    Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                    if (player != null)
                    {
                        player.AddScore(_scoreValue);
                        player.addKill(1);
                    }

                    EnemyDeath();
                    break;
                }
            default:
                break;
        }
    }

    public void EnemyDeath()
    {
        AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
        GameObject.FindGameObjectWithTag("ppv").GetComponent<PostProcessingManager>().ExplosionBloom(clips[0].length);
        _animator.SetTrigger("onEnemyDeath");
        _cameraBounds.CameraShake();
        GetComponent<Collider2D>().enabled = false;
        _speed *= 0.75f;
        _sounds["explosion"].Play();
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
