using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    void Start()
    {
        _speed = GetComponent<Enemy>().lvlManager.fighterSpeed;
        GetComponent<EnemyMovement>().OverrideSpeed(_speed);
    }


    void Update()
    {
        FighterSpecialMove();
    }

    private void FighterSpecialMove()
    {
        transform.Translate(Vector3.down * 0.2f * Mathf.Sin(Time.time));
    }

}
