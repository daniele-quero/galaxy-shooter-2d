using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathRay : MonoBehaviour
{
    [SerializeField]
    float _time = 2.52f;
    [SerializeField]
    float _nextTime = -1, _hitRate = 0.5f;

    void Start()
    {
        GameObject.Destroy(gameObject, _time);
        transform.parent = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "enemy":
                {
                    if (Time.time > _nextTime)
                    {
                        _nextTime = Time.time + _hitRate;
                        if (collision != null)
                            collision.GetComponent<Enemy>().EnemyKill(true);
                    }


                    break;
                }
            case "asteroid":
                {
                    if (Time.time > _nextTime)
                    {
                        _nextTime = Time.time + _hitRate;
                        if (collision != null)
                            collision.GetComponent<Asteroid>().AsteroidDamage(true);
                    }

                    break;
                }
            default:
                break;
        }
    }
}
