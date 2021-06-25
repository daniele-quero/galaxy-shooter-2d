using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private float _rate = 5f;

    [SerializeField]
    private GameObject _enemyContainer, _enemyPrefab;

    private Vector3 _position = Vector3.zero;
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
        StartCoroutine(SpawnEnemy(_rate));
    }

    void Update()
    {

    }

    IEnumerator SpawnEnemy(float rate)
    {
        while (_player.Lives > 0)
        {
            _position.y = _enemy.SpawnLimit.YMax;
            if (_position.y > 50f)
                yield return null;

            else
            {
                _position.x = Random.Range(_enemy.SpawnLimit.XMin, _enemy.SpawnLimit.XMax);
                Instantiate(_enemyPrefab, _position, Quaternion.identity).transform.SetParent(_enemyContainer.transform);
                yield return new WaitForSeconds(rate);
            }            
        }
    }
}
