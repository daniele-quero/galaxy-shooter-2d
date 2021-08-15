using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShootingSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _torpedoLaunchers, _sideLaserTurrets;

    [SerializeField]
    private GameObject _torpedoPrefab, _enemyLaser;

    private Player _player;

    [SerializeField]
    private float _torpedoSpeed = 7f;

    private Enemy _enemy;
    private Collider2D _collider;
    private EnemyShooting _es;
    private EnemyTargetingSystem _ets;

    [SerializeField]
    private bool _isLaunching = true;

    void Start()
    {
        _enemy = GetComponent<Enemy>();
        _collider = GetComponent<Collider2D>();
        _es = GetComponent<EnemyShooting>();
        _ets = GetComponent<EnemyTargetingSystem>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        StartCoroutine(TorpedoShooting());
        StartCoroutine(SideLasersShooting());
    }
    

    private IEnumerator TorpedoShooting()
    {
        yield return new WaitForSeconds(2f);
        while (_player != null)
        {
            yield return new WaitForSeconds(3f);
            if(_isLaunching)
            {
                int l1 = Random.Range(0, 4);
                int l2 = l1;

                while (l2 == l1)
                    l2 = Random.Range(0, 4);

                Launch(_torpedoLaunchers[l1].transform.position);
                Launch(_torpedoLaunchers[l2].transform.position); 
            }
        }
    }

    private void Launch(Vector3 position)
    {
        GameObject shot = Instantiate(_torpedoPrefab, position, Quaternion.identity);
        shot.GetComponent<Torpedo>().SetEnemyTorpedo(Vector2.right * Mathf.Sign(position.x));
        shot.GetComponent<Torpedo>().Speed = _torpedoSpeed;
        _enemy.sounds["torpedo"].Play();
    }

    private void Shoot(Vector3 position)
    {
        Vector3 direction = Vector3.right * Mathf.Sign(position.x);
        Quaternion rot = Quaternion.Euler(Vector3.forward * 90 * Mathf.Sign(position.x));
        var laser = Instantiate(_enemyLaser, position, rot);
        foreach (var las in laser.GetComponentsInChildren<Laser>())
            las.SetEnemyLaser(direction);

    }

    private IEnumerator SideLasersShooting()
    {
        while (_es.IsShooting && _collider.enabled)
        {
            yield return new WaitForSeconds(Random.Range(_es.LaserRateMin, _es.LaserRateMax));
            if(!_es.IsPaused)
            {
                Vector2 direction = Vector2.right;
                if (_ets.Engage(new string[] { "Player", "shields", "PowerUp" }, direction))
                {
                    _es.Shooting(direction, _enemyLaser, _enemy.sounds["laser"]).transform.position = _sideLaserTurrets[1].transform.position; 
                    _es.Shooting(direction, _enemyLaser, _enemy.sounds["laser"]).transform.position = _torpedoLaunchers[3].transform.position; 
                }

                direction = Vector2.left;
                if (_ets.Engage(new string[] { "Player", "shields" }, direction))
                {
                    _es.Shooting(direction, _enemyLaser, _enemy.sounds["laser"]).transform.position = _sideLaserTurrets[0].transform.position;  
                    _es.Shooting(direction, _enemyLaser, _enemy.sounds["laser"]).transform.position = _torpedoLaunchers[2].transform.position;  
                }
            }

        }
    }
}
