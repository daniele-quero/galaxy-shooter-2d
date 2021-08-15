using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    private LvlManager _lvlman;
    private Player _player;
    [SerializeField]
    private GameObject _bossPrefab;
    // Start is called before the first frame update
    void Start()
    {
        _lvlman = GetComponent<LvlManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();

        StartCoroutine(BossLevelCoroutine());
    }

    private IEnumerator BossLevelCoroutine()
    {
        while (!IsLastKill())
        {
            yield return new WaitForSeconds(0.5f);
        }
        if (IsLastKill())
        {
            _lvlman.asteroidSpawnRate[0] *= 1.5f;
            _lvlman.asteroidSpawnRate[1] *= 1.5f;

            GameObject spm = GameObject.Find("SpawnManager");
            if (spm != null)
                spm.GetComponent<SpawnManager>().isSpawningEnemiesOverride = false;

            ClearEnemies();
            Instantiate(_bossPrefab).transform.SetParent(GameObject.Find("EnemyContainer").transform);
        }
    }

    private bool IsLastKill()
    {
        return _player.Kills == _lvlman.killTarget - 1;
    }

    private void ClearEnemies()
    {
        foreach (var em in GameObject.Find("EnemyContainer").GetComponentsInChildren<EnemyMovement>())
        {
            em.Recycle = false;
            em.GetComponent<Collider2D>().enabled = false;
            em.Speed *= 1.2f;
            em.GetComponent<EnemyShooting>().IsShooting = false;
        }
    }
}
