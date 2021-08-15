using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    [SerializeField]
    private float _speed, _torpedoRate;

    [SerializeField]
    private GameObject _torpedoPrefab;

    private EnemyShooting _es;
    private Enemy _enemy;
    private Collider2D _collider;

    void Start()
    {
        _enemy = GetComponent<Enemy>();
        
        _es = GetComponent<EnemyShooting>();
        _collider = GetComponent<Collider2D>();

        OverrideEnemyValues();
        StartCoroutine(TorpedoLaunchingRoutine());
    }


    void Update()
    {
        FighterSpecialMove();
    }

    private void FighterSpecialMove()
    {
        transform.Translate(Vector3.down * 0.1f * Mathf.Sin(Time.time));
    }

    private void OverrideEnemyValues()
    {
        _speed = _enemy.lvlManager.fighterSpeed;
        _torpedoRate = _enemy.lvlManager.fighterTorpedoRate;

        GetComponent<EnemyMovement>().Speed = _speed;
        GetComponent<EnemyShooting>().IsShooting = true;
        _enemy.Lives = _enemy.lvlManager.fighterLives;
        _enemy.ScoreValue *= 2;
        _enemy.sounds.Add("torpedo", GetComponents<AudioSource>()[3]);

    }

    private IEnumerator TorpedoLaunchingRoutine()
    {
        while (_collider.enabled)
        {
            var torpedo = _es.Shooting(Vector3.down,
             _torpedoPrefab,
             _enemy.sounds["torpedo"]);
            torpedo.GetComponent<Torpedo>().SetEnemyTorpedo();
            yield return new WaitForSeconds(Random.Range(_torpedoRate, _torpedoRate*1.5f));
        }
    }

}
