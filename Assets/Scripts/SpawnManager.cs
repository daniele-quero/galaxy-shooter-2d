using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private float _enemyRate = 5f,
        _powerUpRateMin = 4f, _powerUpRateMax = 8f,
        _asteroidRateMin = 8f, _asteroidRateMax = 10f,
        _ammoRate = 5f,
        _1upRateMin = 15f, _1upRateMax = 20f,
        _deathRayRateMin = 20f, _deathRayRateMax = 25f;

    [SerializeField]
    private GameObject _enemyContainer, _powerUpContainer, _asteroidContainer;

    [SerializeField]
    private GameObject _enemyPrefab, _asteroidPrefab;

    [SerializeField]
    private List<GameObject> _powerUpPrefabs;

    [SerializeField]
    private GameObject _ammoPrefab, _1upPrefab, _deathRayPowerUpPrefab, _torpedoPowerUp;

    private Vector3 _position = Vector3.zero;
    private Player _player = null;
    private EnemyMovement _enemy = null;
    private LvlManager _lvl;

    public bool isSpawningEnemiesOverride = true;
    public bool isSpawningPowerUpsOverride = true;
    public bool isSpawningAsteroidsOverride = true;
    public bool defaultLevelSettings = true;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Utilities.CheckNullGrabbed(_player, "Player Script");

        _enemy = _enemyPrefab.GetComponent<EnemyMovement>();
        Utilities.CheckNullGrabbed(_enemy, "Enemy Movement Script");


        _lvl = GameObject.Find("LevelManager").GetComponent<LvlManager>();
        //_enemyRate = _lvl.enemySpawnRate;
        //_ammoRate = _lvl.ammoSpawnRate;
        //_powerUpRateMin = _lvl.powerUpSpawnRate[0];
        //_powerUpRateMax = _lvl.powerUpSpawnRate[1];
        //_1upRateMin = _lvl.oneUpSpawnRate[0];
        //_1upRateMax = _lvl.oneUpSpawnRate[1];
        //_asteroidRateMin = _lvl.asteroidSpawnRate[0];
        //_asteroidRateMax = _lvl.asteroidSpawnRate[1];
        //_deathRayRateMin = _lvl.torpedoSpawnRate[0];
        //_deathRayRateMax = _lvl.torpedoSpawnRate[1];


        StartCoroutine(SpawnEnemy(_lvl.enemySpawnRate, _enemyPrefab));

        StartCoroutine(SpawnAsteroids(_lvl.asteroidSpawnRate[0], _lvl.asteroidSpawnRate[1], _asteroidPrefab));

        StartCoroutine(SpawnSingleRatePowerUp(_lvl.ammoSpawnRate, _ammoPrefab));

        StartCoroutine(SpawnPowerUps(_lvl.oneUpSpawnRate[0], _lvl.oneUpSpawnRate[1], _1upPrefab));
        StartCoroutine(SpawnPowerUps(_lvl.deathRaySpawnRate[0], _lvl.deathRaySpawnRate[1], _deathRayPowerUpPrefab));
        StartCoroutine(SpawnPowerUps(_lvl.torpedoSpawnRate[0], _lvl.torpedoSpawnRate[1], _torpedoPowerUp));

        foreach (GameObject powerUp in _powerUpPrefabs)
            StartCoroutine(SpawnPowerUps(_lvl.powerUpSpawnRate[0], _lvl.powerUpSpawnRate[1], powerUp));

    }

    IEnumerator SpawnEnemy(float rate, GameObject enemyPrefab)
    {
        while (isSpawningEnemiesOverride)
        {
            if (ShouldSpawn(isSpawningEnemiesOverride))
            {
                _position.y = _enemy.SpawnLimit.YMax;
                if (_position.y > 50f)
                    yield return null;

                else
                {
                    _position.x = Random.Range(_enemy.SpawnLimit.XMin, _enemy.SpawnLimit.XMax);
                    Instantiate(enemyPrefab, _position, Quaternion.identity).transform.SetParent(_enemyContainer.transform);
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

                _position.y = selected.SpawnLimit.YMax;

                yield return new WaitForSeconds(rate);
                if (_position.y > 50f)
                    yield return null;

                else
                {
                    _position.x = Random.Range(selected.SpawnLimit.XMin, selected.SpawnLimit.XMax);
                    Instantiate(powerUp, _position, Quaternion.identity).transform.SetParent(_powerUpContainer.transform);
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
                        _position.y = script.SpawnLimit.YMax;
                        _position.x = script.SpawnLimit.XMax;
                        break;
                    case 1:
                        _position.y = script.SpawnLimit.YMax;
                        _position.x = script.SpawnLimit.XMin;
                        break;
                    case 2:
                        _position.y = script.SpawnLimit.YMin;
                        _position.x = script.SpawnLimit.XMax;
                        break;
                    case 3:
                        _position.y = script.SpawnLimit.YMin;
                        _position.x = script.SpawnLimit.XMin;
                        break;
                }

                yield return new WaitForSeconds(rate);

                Instantiate(asteroid, _position, Quaternion.identity).transform.SetParent(_asteroidContainer.transform);

            }

            else
                yield return null;
        }
    }

    IEnumerator SpawnSingleRatePowerUp(float rate, GameObject srpu)
    {
        while (isSpawningPowerUpsOverride)
        {
            if (ShouldSpawn(isSpawningPowerUpsOverride))
            {
                PowerUp pu = srpu.GetComponent<PowerUp>();
                _position.y = pu.SpawnLimit.YMax;

                yield return new WaitForSeconds(rate);
                if (_position.y > 50f)
                    yield return null;

                else
                {
                    _position.x = Random.Range(pu.SpawnLimit.XMin, pu.SpawnLimit.XMax);
                    Instantiate(srpu, _position, Quaternion.identity).transform.SetParent(_powerUpContainer.transform);
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
