using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetingSystem : MonoBehaviour
{
    private Enemy _enemy;
    private void Start()
    {
        _enemy = GetComponent<Enemy>();
    }
    public bool Engage(string[] otherTags, Vector2 direction)
    {
        Vector2 origin = transform.position;
        origin.y += YOffset(direction) * 2;
        RaycastHit2D hit = Physics2D.BoxCast(origin, _enemy.movement.SpawnLimit.Off * 2, 0f, direction, 100f);

        if (hit.collider != null)
        {
            foreach (var ot in otherTags)
                if (hit.collider.tag.Contains(ot))
                    return true;
        }

        return false;
    }

    public float YOffset(Vector2 direction)
    {
        return direction.y * _enemy.movement.SpawnLimit.YOff;
    }
}
