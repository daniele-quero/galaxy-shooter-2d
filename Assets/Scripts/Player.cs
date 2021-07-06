using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f,
        _defaultSpeed = 4f;

    [SerializeField]
    private Vector2 _direction = Vector2.zero;

    [SerializeField]
    private GameObject _laserPrefab = null;

    [SerializeField]
    private GameObject _tripleShotPrefab = null;

    [SerializeField]
    private GameObject _shieldPrefab = null;

    private GameObject _shot;

    [SerializeField]
    private bool _hasTripleShot = false;

    [SerializeField]
    private int _shields = 0;

    private CameraBounds _cameraBounds = null;
    private Vector3 _playerPosition = Vector3.zero;

    private SpriteRenderer _spriteRenderer;
    private Vector2 _laserSpawnPosition = new Vector2();

    [SerializeField]
    private float _fireRate = 0.2f;
    private float _nextFireTime = -1f;

    [SerializeField]
    private int _lives = 3;

    public int Score = 0;

    public int Lives { get => _lives; set => _lives = value; }
    public bool HasTripleShot { get => _hasTripleShot; set => _hasTripleShot = value; }

    void Start()
    {
        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            _cameraBounds = camObj.GetComponent<CameraBounds>();
        else
            Debug.LogError("Camera not found");

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CheckBounds();
        Move();
        ShootLaser();
    }

    private void Move()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        _direction.x = xInput;
        _direction.y = yInput;
        transform.Translate(_direction * Time.deltaTime * _speed);
    }

    private void CheckBounds()
    {
        float yOff = _spriteRenderer.sprite.rect.size.y / 2 / _spriteRenderer.sprite.pixelsPerUnit * transform.lossyScale.y;
        float xOff = _spriteRenderer.sprite.rect.size.x / 2 / _spriteRenderer.sprite.pixelsPerUnit * transform.lossyScale.x;

        if (_cameraBounds != null)
            _playerPosition.Set(
                Mathf.Clamp(transform.position.x, -_cameraBounds.CameraVisual.x + xOff, _cameraBounds.CameraVisual.x - xOff),
                Mathf.Clamp(transform.position.y, -_cameraBounds.CameraVisual.y + yOff, _cameraBounds.CameraVisual.y - yOff),
                transform.position.z);

        transform.position = _playerPosition;
    }

    private void ShootLaser()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _laserSpawnPosition = transform.position;
            _laserSpawnPosition.y += _spriteRenderer.sprite.rect.size.y / 2 / _spriteRenderer.sprite.pixelsPerUnit * transform.lossyScale.y;

            _shot = _laserPrefab;
            if (_hasTripleShot)
                _shot = _tripleShotPrefab;

            if (_shot != null && Time.time > _nextFireTime)
            {
                _nextFireTime = Time.time + _fireRate;
                Instantiate(_shot, _laserSpawnPosition, Quaternion.identity);
            }
        }
    }

    public void Damage(int dmg)
    {
        if (_shields > 0)
        {
            _shields -= dmg;
            if (_shields <= 0)
                DestroyShield();
        }
        else
            _lives -= dmg;

        if (_lives < 0)
            GameObject.Destroy(this.gameObject);

        UIManager ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (ui != null)
            ui.UpdateLivesDisplay(_lives);
    }

    private void DestroyShield()
    {
        Transform shieldTr;
        if ((shieldTr = transform.Find("Shields")) != null)
            GameObject.Destroy(shieldTr.gameObject);
    }

    private IEnumerator PowerUpCooldown(PowerUp powerup)
    {
        switch (powerup.tag)
        {
            case "tripleShotPowerUp":
                _hasTripleShot = true;
                yield return new WaitForSeconds(powerup.duration);
                _hasTripleShot = false;
                break;
            case "speedPowerUp":
                _speed *= powerup.boost;
                yield return new WaitForSeconds(powerup.duration);
                _speed = _defaultSpeed;
                break;
            case "shieldPowerUp":
                _shields = powerup.shields;
                GameObject shieldObj = Instantiate(_shieldPrefab, this.transform.position, Quaternion.identity);
                shieldObj.transform.SetParent(this.transform);
                shieldObj.name = "Shields";
                yield return new WaitForSeconds(powerup.duration);
                GameObject.Destroy(shieldObj);
                _shields = 0;
                break;
            default:
                break;
        }
    }

    public void ActivatePowerUp(PowerUp powerup)
    {
        StartCoroutine(PowerUpCooldown(powerup));
    }
}
