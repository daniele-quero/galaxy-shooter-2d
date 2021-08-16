using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private Enemy _enemy;
    private int _bossLives = 49;
    private bool _isAngry = false;

    public int BossLives { get => _bossLives; set => _bossLives = value; }

    void Start()
    {
        _enemy = GetComponent<Enemy>();
        OverrideEnemyValues();
    }

    private void Update()
    {
        if (!_isAngry && _enemy.Lives <= _bossLives/2)
            AngryMode();
    }

    public void OverrideEnemyValues()
    {
        _enemy.Lives = _bossLives;
        _enemy.ScoreValue = 1000;
        _enemy.sounds.Add("torpedo", GetComponents<AudioSource>()[3]);
    }

    private void AngryMode()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        GetComponent<EnemyShooting>().LaserRateMin *= 0.75f;
        GetComponent<EnemyShooting>().LaserRateMax *= 0.75f;
        GetComponent<BossShootingSystem>().TorpedoSpeed += 0.6f;
        GetComponent<BossShootingSystem>().TorpedoRate -= 0.5f;
        _isAngry = true;
    }
}
