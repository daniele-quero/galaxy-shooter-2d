using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private float _enemyRate = 5f,
        _powerUpRateMin = 4f, _powerUpRateMax = 8f,
        _asteroidRateMin = 8f, _asteroidRateMax = 10f,
        _ammoRate = 5f;

    [SerializeField]
    private GameObject _enemyContainer, _powerUpContainer, _asteroidContainer;

    [SerializeField]
    private GameObject _enemyPrefab, _asteroidPrefab;

    [SerializeField]
    private List<GameObject> _powerUpPrefabs;

    [SerializeField]
    private GameObject _ammoPrefab;

    private Vector3 _enemyPosition = Vector3.zero;
    private Vector3 _powerUpPosition = Vector3.zero;
    private Vector3 _asteroidPosition = Vector3.zero;
    private Vector3 _ammoPosition = Vector3.zero;
    private Player _player = null;
    private Enemy _enemy = null;

    public bool isSpawningEnemiesOverride = true;
    public bool isSpawningPowerUpsOverride = true;
    public bool isSpawningAsteroidsOverride = true;
    public bool defaultLevelSettings = true;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Utilities.CheckNullGrabbed(_player, "Player Script");

        _enemy = _enemyPrefab.GetComponent<Enemy>();
        Utilities.CheckNullGrabbed(_enemy, "Enemy Script");

        if (defaultLevelSettings)
        {
            LvlManager lvlManager = GameObject.Find("LevelManager").GetComponent<LvlManager>();
            _enemyRate = lvlManager.enemySpawnRate;
            _ammoRate = lvlManager.ammoSpawnRate;
            _powerUpRateMin = lvlManager.powerUpSpawnRate[0];
            _powerUpRateMax = lvlManager.powerUpSpawnRate[1];
            _asteroidRateMin = lvlManager.asteroidSpawnRate[0];
            _asteroidRateMax = lvlManager.asteroidSpawnRate[1];
        }

        StartCoroutine(SpawnEnemy(_enemyRate));
        StartCoroutine(SpawnAmmo(_ammoRate));
        StartCoroutine(SpawnAsteroids(_asteroidRateMin, _asteroidRateMax, _asteroidPrefab));

        foreach (GameObject powerUp in _powerUpPrefabs)
            StartCoroutine(SpawnPowerUps(_powerUpRateMin, _powerUpRateMax, powerUp));

    }

    void Update()
    {

    }

    IEnumerator SpawnEnemy(float rate)
    {
        while (isSpawningEnemiesOverride)
        {
            if (ShouldSpawn(isSpawningEnemiesOverride))
            {
                _enemyPosition.y = _enemy.SpawnLimit.YMax;
                if (_enemyPosition.y > 50f)
                    yield return null;

                else
                {
                    _enemyPosition.x = Random.Range(_enemy.SpawnLimit.XMin, _enemy.SpawnLimit.XMax);
                    Instantiate(_enemyPrefab, _enemyPosition, Quaternion.identity).transform.SetParent(_enemyContainer.transform);
                    yield return new WaitForSeconds(rate);
                }
            }

            else
                yield return null;
        }
    }

    IEnumerator SpawnPowerUps(float minRate, float maxRate, GameObject powerUp)
    {
        while (isSpawningPowerUpsOverride)
        {
            if (ShouldSpawn(isSpawningPowerUpsOverride))
            {
                float rate = Random.Range(minRate, maxRate);
                PowerUp selected = powerUp.GetComponent<PowerUp>();

                _powerUpPosition.y = selected.SpawnLimit.YMax;

                yield return new WaitForSeconds(rate);
                if (_powerUpPosition.y > 50f)
                    yield return null;

                else
                {
                    _powerUpPosition.x = Random.Range(selected.SpawnLimit.XMin, selected.SpawnLimit.XMax);
                    Instantiate(powerUp, _powerUpPosition, Quaternion.identity).transform.SetParent(_powerUpContainer.transform);
                }
            }

            else
                yield return null;
        }
    }
    IEnumerator SpawnAsteroids(float minRate, float maxRate, GameObject asteroid)
    {
        while (isSpawningAsteroidsOverride)
        {

            if (ShouldSpawn(isSpawningAsteroidsOverride))
            {
                float rate = Random.Range(minRate, maxRate);
                Asteroid script = asteroid.GetComponent<Asteroid>();
                int state = Random.Range(0, 3);
                switch (state)
                {
                    case 0:
                        _asteroidPosition.y = script.SpawnLimit.YMax;
                        _asteroidPosition.x = script.SpawnLimit.XMax;
                        break;
                    case 1:
                        _asteroidPosition.y = script.SpawnLimit.YMax;
                        _asteroidPosition.x = script.SpawnLimit.XMin;
                        break;
                    case 2:
                        _asteroidPosition.y = script.SpawnLimit.YMin;
                        _asteroidPosition.x = script.SpawnLimit.XMax;
                        break;
                    case 3:
                        _asteroidPosition.y = script.SpawnLimit.YMin;
                        _asteroidPosition.x = script.SpawnLimit.XMin;
                        break;
                }

                yield return new WaitForSeconds(rate);
                
                Instantiate(asteroid, _asteroidPosition, Quaternion.identity).transform.SetParent(_asteroidContainer.transform);

            }

            else
                yield return null;
        }
    }

    IEnumerator SpawnAmmo(float rate)
    {
        while (true)
        {
            if (ShouldSpawn(isSpawningPowerUpsOverride))
            {
                PowerUp ammoUp = _ammoPrefab.GetComponent<PowerUp>();
                _ammoPosition.y = ammoUp.SpawnLimit.YMax;

                yield return new WaitForSeconds(rate);
                if (_ammoPosition.y > 50f)
                    yield return null;

                else
                {
                    _ammoPosition.x = Random.Range(ammoUp.SpawnLimit.XMin, ammoUp.SpawnLimit.XMax);
                    Instantiate(_ammoPrefab, _ammoPosition, Quaternion.identity).transform.SetParent(_powerUpContainer.transform);
                }
            }
            else
                yield return null;
        }
    }

    private bool ShouldSpawn(bool spawnOverride)
    {
        return spawnOverride
            && _player.Lives >= 0
            && IsWaveClear();
    }

    private bool IsWaveClear()
    {
        foreach (GameObject asteroid in GameObject.FindGameObjectsWithTag("asteroid"))
        {
            if (asteroid.GetComponent<Asteroid>().isWaveAsteroid)
                return false;
        }
        return true;
    }
}
