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
        origin += Offset(direction) * 2f;
        return Engage(otherTags, direction, origin);
    }

    public bool Engage(string[] otherTags, Vector2 direction, Vector2 origin)
    {
        Vector2 offset;
        if (name.Contains("Boss"))
        {
            offset = GetComponent<BossMovement>().SpawnLimit.Off;
            offset.x *= 0.5f;
        }
        else
            offset = _enemy.movement.SpawnLimit.Off;

        RaycastHit2D hit = Physics2D.BoxCast(origin, offset * 2, 0f, direction, 100f);

        if (hit.collider != null)
        {
            foreach (var ot in otherTags)
                if (hit.collider.tag.Contains(ot))
                    return true;
        }

        return false;
    }

    public Vector2 Offset(Vector2 direction)
    {
        Vector2 offset;
        if (name.Contains("Boss"))
            offset = GetComponent<BossMovement>().SpawnLimit.Off;
        else
            offset = _enemy.movement.SpawnLimit.Off;
        return Mathf.Abs(Vector2.Dot(offset, direction)) * direction;
    }
}
