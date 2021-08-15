using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [SerializeField]
    private bool _isShooting = false, _isPaused = false;

    private float _laserRateMin = 3f;
    private float _laserRateMax = 7f;

    [SerializeField]
    private GameObject _laser;

    private Enemy _enemy;
    private EnemyTargetingSystem _targeting;
    private Collider2D _collider;

    public bool IsShooting { get => _isShooting; set => _isShooting = value; }
    public float LaserRateMin { get => _laserRateMin; set => _laserRateMin = value; }
    public float LaserRateMax { get => _laserRateMax; set => _laserRateMax = value; }
    public bool IsPaused { get => _isPaused; set => _isPaused = value; }

    void Start()
    {
        _enemy = GetComponent<Enemy>();
        _targeting = GetComponent<EnemyTargetingSystem>();
        _collider = GetComponent<Collider2D>();

        _isShooting = _enemy.lvlManager.isEnemyShooting;
        _laserRateMin = _enemy.lvlManager.enemyLaserRate[0];
        _laserRateMax = _enemy.lvlManager.enemyLaserRate[1];

        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        while (_isShooting && _collider.enabled)
        {
            yield return new WaitForSeconds(Random.Range(_laserRateMin, _laserRateMax));
            if (!_isPaused)
            {
                Vector2 direction = Vector2.down;
                if (_targeting.Engage(new string[] { "Player", "shields", "PowerUp" }, direction))
                    Shooting(direction, _laser, _enemy.sounds["laser"]);

                direction = Vector2.up;
                if (_targeting.Engage(new string[] { "Player", "shields" }, direction))
                    Shooting(direction, _laser, _enemy.sounds["laser"]);
            }

        }
    }

    public GameObject Shooting(Vector2 direction, GameObject shot, AudioSource audio)
    {
        Vector2 laserSpawn = transform.position;
        laserSpawn += _targeting.Offset(direction);
        GameObject enemyLaser = Instantiate(shot, laserSpawn, Quaternion.identity);
        foreach (var las in enemyLaser.GetComponentsInChildren<Laser>())
            las.SetEnemyLaser(direction);

        enemyLaser.transform.localRotation = Quaternion.LookRotation(Vector3.forward, direction);
        audio.Play();
        return enemyLaser;
    }

}
