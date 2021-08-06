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

    void Start()
    {
        _enemy = GetComponent<Enemy>();
        _isShooting = _enemy.lvlManager.isEnemyShooting;
        _laserRateMin = _enemy.lvlManager.enemyLaserRate[0];
        _laserRateMax = _enemy.lvlManager.enemyLaserRate[1];
        StartCoroutine(Shoot());
    }

    void Update()
    {
        
    }
    private IEnumerator Shoot()
    {
        while (_isShooting)
        {
            yield return new WaitForSeconds(Random.Range(_laserRateMin, _laserRateMax));

            Vector2 direction = Vector2.down;
            if (Engage(new string[] { "Player", "shields" }, direction))
                LaserShooting(direction);

            direction = Vector2.up;
            if (Engage(new string[] { "Player", "shields" }, direction))
                LaserShooting(direction);

        }
    }

    private bool Engage(string[] otherTags, Vector2 direction)
    {
        Vector2 origin = transform.position;
        origin.y += LaserOffset(direction);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction);
        Debug.DrawRay(origin, Vector2.down);
        if (hit.collider != null)
        {
            foreach (var ot in otherTags)
                if (hit.collider.tag == ot)
                    return true;
        }

        return false;
    }

    private void LaserShooting(Vector2 direction)
    {
        Vector2 laserSpawn = transform.position;
        laserSpawn.y += LaserOffset(direction);
        GameObject enemyLaser = Instantiate(_laser, laserSpawn, Quaternion.identity);
        foreach (var las in enemyLaser.GetComponentsInChildren<Laser>())
            las.SetEnemyLaser(direction);

        _enemy._sounds["laser"].Play();
    }

    private float LaserOffset(Vector2 direction)
    {
        return direction.y * _enemy.movement.SpawnLimit.YOff;
    }
}
