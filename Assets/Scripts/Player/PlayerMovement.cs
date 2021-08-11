using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerInputManager pim;
    private Player _player;

    [SerializeField]
    private Vector3 _direction = Vector3.zero;
    private Vector3 _playerPosition = Vector3.zero;

    [SerializeField]
    private float _speed = 12f;

    public Thruster thruster;

    public float Speed { get => _speed; set => _speed = value; }
    public float DefaultSpeed { get; set; } = 12f;
    public float MiniBoost { get; set; } = 18f;
    public float MaxBoostFuel { get; set; } = 6f;
    public float CurrentBoostFuel { get; set; } = 6f;

    void Start()
    {
        thruster = transform.Find("Thruster").GetComponent<Thruster>();
        Utilities.CheckNullGrabbed(thruster, "Thruster Script");

        pim = GetComponent<PlayerInputManager>();
        _player = GetComponent<Player>();
    }

    void Update()
    {
        Move();
        CheckBounds();
        thruster.Boost();
    }

    private void Move()
    {
        _direction.x = pim.xAxis;
        _direction.y = pim.yAxis;

        transform.Translate(_direction * Time.deltaTime * Speed);
    }

    private void CheckBounds()
    {
        float yOff = _player.spriteRenderer.sprite.rect.size.y / 2 / _player.spriteRenderer.sprite.pixelsPerUnit * transform.lossyScale.y;
        float xOff = _player.spriteRenderer.sprite.rect.size.x / 2 / _player.spriteRenderer.sprite.pixelsPerUnit * transform.lossyScale.x;

        if (_player.cameraBounds != null)
            _playerPosition.Set(
                Mathf.Clamp(transform.position.x, -_player.cameraBounds.CameraVisual.x + xOff, _player.cameraBounds.CameraVisual.x - xOff),
                Mathf.Clamp(transform.position.y, -_player.cameraBounds.CameraVisual.y + yOff, _player.cameraBounds.CameraVisual.y - yOff),
                transform.position.z);

        transform.position = _playerPosition;
    }
}
