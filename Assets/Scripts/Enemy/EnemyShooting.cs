using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [SerializeField]
    private bool _isShooting = false;

    private float _laserRateMin = 3f;
    private float _laserRateMax = 7f;

    [SerializeField]
    private GameObject _laser;

    private Enemy _enemy;
    private EnemyTargetingSystem _targeting;

    void Start()
    {
        _enemy = GetComponent<Enemy>();
        _targeting = GetComponent<EnemyTargetingSystem>();

        _isShooting = _enemy.lvlManager.isEnemyShooting;
        _laserRateMin = _enemy.lvlManager.enemyLaserRate[0];
        _laserRateMax = _enemy.lvlManager.enemyLaserRate[1];

        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        while (_isShooting)
        {
            yield return new WaitForSeconds(Random.Range(_laserRateMin, _laserRateMax));

            Vector2 direction = Vector2.down;
            if (_targeting.Engage(new string[] { "Player", "shields", "PowerUp" }, direction))
                LaserShooting(direction);

            direction = Vector2.up;
            if (_targeting.Engage(new string[] { "Player", "shields" }, direction))
                LaserShooting(direction);

        }
    }

    private void LaserShooting(Vector2 direction)
    {
        Vector2 laserSpawn = transform.position;
        laserSpawn.y += _targeting.YOffset(direction);
        GameObject enemyLaser = Instantiate(_laser, laserSpawn, Quaternion.identity);
        foreach (var las in enemyLaser.GetComponentsInChildren<Laser>())
            las.SetEnemyLaser(direction);

        _enemy._sounds["laser"].Play();
    }
}
