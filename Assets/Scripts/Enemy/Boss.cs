using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private Enemy _enemy;
    private int _bossLives = 49;

    public int BossLives { get => _bossLives; set => _bossLives = value; }

    void Start()
    {
        _enemy = GetComponent<Enemy>();
        OverrideEnemyValues();
    }

    public void OverrideEnemyValues()
    {
        _enemy.Lives = _bossLives;
        _enemy.ScoreValue = 1000;
        _enemy.sounds.Add("torpedo", GetComponents<AudioSource>()[3]);
    }

}
