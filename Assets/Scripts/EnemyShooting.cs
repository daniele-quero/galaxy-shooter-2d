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

            if (Engage(new string[] { "Player", "shields" }))
            {
                Vector2 laserSpawn = transform.position;
                laserSpawn.y -= _enemy.movement.SpawnLimit.YOff;
                GameObject enemyLaser = Instantiate(_laser, laserSpawn, Quaternion.identity);
                foreach (var las in enemyLaser.GetComponentsInChildren<Laser>())
                    las.SetEnemyLaser();

                _enemy._sounds["laser"].Play();
            }
        }
    }

    private bool Engage(string[] otherTags)
    {
        Vector2 origin = transform.position;
        origin.y -= _enemy.movement.SpawnLimit.YOff;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down);
        Debug.DrawRay(origin, Vector2.down);
        if (hit.collider != null)
        {
            foreach (var ot in otherTags)
                if (hit.collider.tag == ot)
                    return true;
        }

        return false;
    }
}
