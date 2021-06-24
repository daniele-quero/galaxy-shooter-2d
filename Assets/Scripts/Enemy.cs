using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;

    private CameraBounds _cameraBounds = null;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _respawnPosition = new Vector3(0, 0, 0);

    void Start()
    {
        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            _cameraBounds = camObj.GetComponent<CameraBounds>();
        else
            Debug.LogError("Camera not found");

        _spriteRenderer = GetComponent<SpriteRenderer>();  
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        RespawnAtTop();
    }

    private void Move()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    private void RespawnAtTop()
    {
        float yOff = _spriteRenderer.sprite.rect.size.y / 2 / _spriteRenderer.sprite.pixelsPerUnit * transform.lossyScale.y;
        float xOff = _spriteRenderer.sprite.rect.size.x / 2 / _spriteRenderer.sprite.pixelsPerUnit * transform.lossyScale.x;

        if (transform.position.y <= -_cameraBounds.CameraVisual.y - yOff)
        {
            _respawnPosition.y = _cameraBounds.CameraVisual.y + yOff;
            _respawnPosition.x = Random.Range(-_cameraBounds.CameraVisual.x + xOff, _cameraBounds.CameraVisual.x - xOff);
            transform.position = _respawnPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "laser")
        {
            GameObject.Destroy(collision.gameObject);
            GameObject.Destroy(this.gameObject);
        }
        else if (collision.tag == "Player")
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
