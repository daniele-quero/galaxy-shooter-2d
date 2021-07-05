using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private float _enemyRate = 5f,
        _powerUpRateMin = 4f, _powerUpRateMax = 8f;

    [SerializeField]
    private GameObject _enemyContainer, _powerUpContainer;

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private List<GameObject> _powerUpPrefabs;

    private Vector3 _enemyPosition = Vector3.zero;
    private Vector3 _powerUpPosition = Vector3.zero;
    private Player _player = null;
    private Enemy _enemy = null;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (_player == null)
            Debug.LogError("No player found");


        _enemy = _enemyPrefab.GetComponent<Enemy>();
        if (_enemy == null)
            Debug.LogError("No Enemy Script found");

        StartCoroutine(SpawnEnemy(_enemyRate));

        foreach(GameObject powerUp in _powerUpPrefabs)
            StartCoroutine(SpawnPowerUps(_powerUpRateMin, _powerUpRateMax, powerUp));
    }

    void Update()
    {

    }

    IEnumerator SpawnEnemy(float rate)
    {
        while (_player.Lives >= 0)
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
    }

    IEnumerator SpawnPowerUps(float minRate, float maxRate, GameObject powerUp)
    {
        while (_player.Lives >= 0)
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
    }
}
