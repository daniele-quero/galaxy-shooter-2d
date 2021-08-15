using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6f;

    private bool _isInitialSprint = true;
    private bool _isEnemy = false;
    private bool _outOfTime = false;

    private Vector3 _lastDirection = Vector3.up;
    private CameraBounds _cameraBounds = null;
    private GameObject _enemyContainer;

    public float Speed { get => _speed; set => _speed = value; }

    void Start()
    {
        PutInContainer();

        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            _cameraBounds = camObj.GetComponent<CameraBounds>();
        else
            Utilities.LogNullGrabbed("Camera");

        _enemyContainer = GameObject.FindGameObjectWithTag("enemyContainer");
        StartCoroutine(InitialSprint(Vector2.up));
        StartCoroutine(SelfDestruct());
    }

    void Update()
    {
        RemoveFarTorpedo();

        if (!_isInitialSprint)
        {
            if (_isEnemy)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null)
                    SeekTarget(player.transform);
                else
                    SeekTarget(null);
            }
            else
                SeekTarget(IdentifyCloserEnemy());
        }
    }

    public void PutInContainer()
    {
        GameObject laserContainer = GameObject.FindGameObjectWithTag("laserContainer");
        if (laserContainer != null)
        {
            transform.SetParent(laserContainer.transform);
        }
    }

    public void SetEnemyTorpedo()
    {
        SetEnemyTorpedo(Vector2.down);
    }

    public void SetEnemyTorpedo(Vector2 dir)
    {
        tag = "enemyTorpedo";
        _isEnemy = true;
        StopCoroutine(InitialSprint(Vector2.up));
        StartCoroutine(InitialSprint(dir));
    }

    private Transform IdentifyCloserEnemy()
    {
        float minDistance = float.MaxValue;
        Transform closer = null;
        List<Transform> enemies = _enemyContainer.GetComponentsInChildren<Transform>().ToList();
        enemies.RemoveAll(t => t.tag == "enemyContainer");

        foreach (var t in enemies)
        {
            float d = Vector3.Distance(t.position, transform.position);
            if (d < minDistance)
            {
                minDistance = d;
                closer = t;
            }
        }

        return closer;
    }

    private void SeekTarget(Transform closer)
    {
        if (closer != null && !_outOfTime)
        {
            Vector3 direction = closer.position - transform.position;
            _lastDirection = direction;
        }
        transform.rotation = Quaternion.LookRotation(Vector3.forward, _lastDirection);
        transform.Translate(_lastDirection.normalized * _speed * Time.deltaTime, Space.World);
    }

    private void RemoveFarTorpedo()
    {
        Vector2 min = new Vector2(-_cameraBounds.CameraVisual.x, -_cameraBounds.CameraVisual.y);
        Vector2 size = new Vector2(_cameraBounds.CameraVisual.x, _cameraBounds.CameraVisual.y) * 2;
        Rect field = new Rect(min, size);

        if (!field.Contains(transform.position))
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    private IEnumerator InitialSprint(Vector2 dir)
    {
        float timer = 0f;
        float duration = 0.50f;
        float timeStep = 0.02f;
        while (timer <= duration)
        {
            transform.Translate(dir * timeStep * _speed);
            timer += timeStep;
            yield return new WaitForSeconds(timeStep);
        }

        _isInitialSprint = false;
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(5f);
        Destruct();
    }

    public void Destruct()
    {
        _outOfTime = true;
        GetComponent<Explosion>().Explode("onTimeOut", GetComponent<AudioSource>());
    }

}
