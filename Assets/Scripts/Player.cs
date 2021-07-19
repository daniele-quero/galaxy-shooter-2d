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
    public int Kills = 0;

    private AudioSource[] _sources;
    private Dictionary<string, AudioSource> _sounds;
    private LvlManager _lvlManager;
    public int Lives { get => _lives; set => _lives = value; }
    public bool HasTripleShot { get => _hasTripleShot; set => _hasTripleShot = value; }

    void Start()
    {
        _sources = GetComponents<AudioSource>();
        _sounds = new Dictionary<string, AudioSource>()
        {
            ["laser"] = _sources[0],
            ["explosion"] = _sources[1],
            ["damage"] = _sources[2],
            ["shieldDamage"] = _sources[3]
        };

        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            _cameraBounds = camObj.GetComponent<CameraBounds>();
        else
            Utilities.LogNullGrabbed("Camera");

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _sources = GetComponents<AudioSource>();

        _lvlManager = GameObject.Find("LevelManager").GetComponent<LvlManager>();
        //_lvlManager.LoadLevel1Data();

        Score = PlayerPrefs.GetInt("Score", 0);
        _lives = PlayerPrefs.GetInt("Lives", 3);

        if (PlayerPrefs.GetInt("Engine0", 0) == 1)
            SetEngineFire(transform.position.x - 1);
        if (PlayerPrefs.GetInt("Engine1", 0) == 1)
            SetEngineFire(transform.position.x + 1);
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
                _sounds["laser"].Play();
            }
        }
    }

    public void Damage(int dmg, float x)
    {
        if (_shields > 0)
        {
            _shields -= dmg;
            _sounds["shieldDamage"].Play();
            if (_shields <= 0)
                DestroyShield();
        }
        else
        {
            _lives -= dmg;
            _sounds["damage"].Play();

            if (_lives < 2)
                SetEngineFire(x);

            PlayerPrefs.SetInt("Lives", _lives);
        }

        if (_lives < 0)
            playerDeath();

        UIManager ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (ui != null)
            ui.UpdateLivesDisplay(_lives);
        else
            Utilities.LogNullGrabbed("UIManager");

    }

    private void playerDeath()
    {
        GameObject.Destroy(transform.Find("Thruster").gameObject);
        Animator animator = GetComponent<Animator>();
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        GameObject.FindGameObjectWithTag("ppv").GetComponent<PostProcessingManager>().ExplosionBloom(clips[0].length);
        Utilities.CheckNullGrabbed(animator, "Player Animator");
        animator.SetTrigger("onPlayerDeath");
        GetComponent<Collider2D>().enabled = false;
        _sounds["explosion"].Play();
        GameObject.Destroy(this.gameObject, clips[0].length);
    }

    private void DestroyShield()
    {
        Transform shieldTr;
        if ((shieldTr = transform.Find("Shields")) != null)
            GameObject.Destroy(shieldTr.gameObject);
    }

    private void SetEngineFire(float x)
    {
        GameObject[] engines = new GameObject[2] { transform.Find("EngineFire0").gameObject, transform.Find("EngineFire1").gameObject };
        int hurt = 0;

        if (transform.position.x < x)
            hurt = 1;

        if (engines[hurt].activeInHierarchy)
            hurt = Utilities.Flip01(hurt);

        engines[hurt].SetActive(true);
        GameObject.FindGameObjectWithTag("ppv").GetComponent<PostProcessingManager>().ExplosionBloom(2f);

        for (int i = 0; i < 2; i++)
            PlayerPrefs.SetInt("Engine" + i, engines[i].activeInHierarchy ? 1 : 0);
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
                transform.Find("Thruster").localScale = new Vector3(1, 1.8f, 1); ;
                yield return new WaitForSeconds(powerup.duration);
                _speed = _defaultSpeed;
                transform.Find("Thruster").localScale = new Vector3(1, 1, 1);
                break;
            case "shieldPowerUp":
                if(_shields > 0)
                    GameObject.Destroy(transform.Find("Shields").gameObject);

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

    public void AddScore(int score)
    {
        Score += score;
        PlayerPrefs.SetInt("Score", Kills);
        UIManager ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (ui != null)
            ui.UpdateScoreText(Score);
    }

    public void addKill(int kill)
    {
        Kills += kill;
        PlayerPrefs.Save();
        _lvlManager.checkKillCount(Kills);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "enemyLaser":
                Damage(1, collision.transform.position.x);
                GameObject.Destroy(collision.gameObject);
                _sounds["damage"].Play();
                break;
            default:
                break;
        }
    }
}
