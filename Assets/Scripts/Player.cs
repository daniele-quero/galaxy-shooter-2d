using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float
        _speed = 4f,
        _defaultSpeed = 4f,
        _miniBoost = 6f,
        _maxBoostFuel = 5f,
        _currentBoostFuel = 5f;

    [SerializeField]
    private int _ammo = 15,
        _maxAmmo = 15;


    [SerializeField]
    private Vector2 _direction = Vector2.zero;

    [SerializeField]
    private GameObject
        _laserPrefab = null,
        _tripleShotPrefab = null,
        _shieldPrefab = null,
        _deathRayPrefab = null,
        _shot;

    [SerializeField]
    private bool _hasTripleShot = false,
        _hasMegaBoost = false,
        _hasShields = false,
        _hasDeathRay = false,
        _hasTorpedo = false;

    private SpriteRenderer _spriteRenderer;
    private CameraBounds _cameraBounds = null;
    private AudioSource[] _sources;
    private Dictionary<string, AudioSource> _sounds;
    private LvlManager _lvlManager;
    private UIManager _uiman;
    private Thruster _thruster;

    private Vector3 _playerPosition = Vector3.zero;
    private Vector2 _laserSpawnPosition = new Vector2();

    [SerializeField]
    private float _fireRate = 0.2f,
        _defaultFireRate = 0.2f;
    private float _nextFireTime = -1f;

    [SerializeField]
    private int _lives = 3;

    public int Score = 0;
    public int Kills = 0;

    [SerializeField]
    private GameObject _torpedoPrefab;

    public int Lives { get => _lives; set => _lives = value; }
    //public bool HasTripleShot { get => _hasTripleShot; set => _hasTripleShot = value; }
    public float Speed { get => _speed; set => _speed = value; }
    public float DefaultSpeed { get => _defaultSpeed; set => _defaultSpeed = value; }
    public float MiniBoost { get => _miniBoost; set => _miniBoost = value; }
    public float MaxBoostFuel { get => _maxBoostFuel; set => _maxBoostFuel = value; }
    public float CurrentBoostFuel { get => _currentBoostFuel; set => _currentBoostFuel = value; }
    public bool HasMegaBoost { get => _hasMegaBoost; set => _hasMegaBoost = value; }

    void Start()
    {
        _sources = GetComponents<AudioSource>();
        _sounds = new Dictionary<string, AudioSource>()
        {
            ["laser"] = _sources[0],
            ["explosion"] = _sources[1],
            ["damage"] = _sources[2],
            ["noammo"] = _sources[3],
            ["torpedo"] = _sources[4]
        };

        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            _cameraBounds = camObj.GetComponent<CameraBounds>();
        else
            Utilities.LogNullGrabbed("Camera");

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _sources = GetComponents<AudioSource>();
        _thruster = transform.Find("Thruster").GetComponent<Thruster>();
        Utilities.CheckNullGrabbed(_thruster, "Thruster Script");
        _lvlManager = GameObject.Find("LevelManager").GetComponent<LvlManager>();
        _uiman = GameObject.Find("Canvas").GetComponent<UIManager>();
        Utilities.CheckNullGrabbed(_uiman, "UIManager");
        _uiman.UpdateAmmoText(_ammo, _maxAmmo);

        Score = PlayerPrefs.GetInt("Score", 0);
        _lives = PlayerPrefs.GetInt("Lives", 3);
        PlayerPrefs.GetInt("ammo", 15);

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
        _thruster.Boost();
    }

    private void Move()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        _direction.x = xInput;
        _direction.y = yInput;
        transform.Translate(_direction * Time.deltaTime * Speed);
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
            AudioSource shootSound = _sounds["laser"];
            int ammoConsumed = 1;

            if (_hasTripleShot)
            {
                _shot = _tripleShotPrefab;
                ammoConsumed = 3;
            }

            else if (_hasDeathRay)
            {
                _shot = _deathRayPrefab;
                ammoConsumed = 0;
            }

            else if (_hasTorpedo)
            {
                _shot = _torpedoPrefab;
                shootSound = _sounds["torpedo"];
                ammoConsumed = 2;
            }

            if (_shot != null && Time.time > _nextFireTime)
            {
                if (_ammo >= ammoConsumed)
                {
                    _ammo -= ammoConsumed;
                    PlayerPrefs.SetInt("ammo", _ammo);
                    _nextFireTime = Time.time + _fireRate;
                    Instantiate(_shot, _laserSpawnPosition, Quaternion.identity);
                    shootSound.Play();
                    _uiman.UpdateAmmoText(_ammo, _maxAmmo);
                }
                else
                    _sounds["noammo"].Play();
            }
        }
    }

    public void Damage(int dmg, float x)
    {
        if (!_hasShields)
        {
            _lives -= dmg;
            _sounds["damage"].Play();

            if (_lives < 2)
                SetEngineFire(x);

            PlayerPrefs.SetInt("Lives", _lives);
        }

        if (_lives < 0)
            playerDeath();

        _uiman.UpdateLivesDisplay(_lives);
    }

    public void Repair(int up, int scoreValue)
    {
        _lives += up;
        if (_lives > 3)
        {
            _lives = 3;
            AddScore(scoreValue);
        }

        UnsetEngineFire();
        _uiman.UpdateLivesDisplay(_lives);

    }

    private void playerDeath()
    {
        GameObject.Destroy(transform.Find("Thruster").gameObject);
        GameObject[] engines = new GameObject[2] { transform.Find("EngineFire0").gameObject, transform.Find("EngineFire1").gameObject };
        foreach (var eng in engines)
            GameObject.Destroy(eng);

        _speed = 0;

        Animator animator = GetComponent<Animator>();
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        GameObject.FindGameObjectWithTag("ppv").GetComponent<PostProcessingManager>().ExplosionBloom(clips[0].length);
        Utilities.CheckNullGrabbed(animator, "Player Animator");

        _cameraBounds.CameraShake();
        animator.SetTrigger("onPlayerDeath");
        GetComponent<Collider2D>().enabled = false;
        _sounds["explosion"].Play();
        GameObject.Destroy(this.gameObject, clips[0].length);
        _lvlManager.PlayerPrefClear();
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

    private void UnsetEngineFire()
    {
        GameObject[] engines = new GameObject[2] { transform.Find("EngineFire0").gameObject, transform.Find("EngineFire1").gameObject };
        foreach (var eng in engines)
        {
            if (eng.activeInHierarchy)
            {
                eng.SetActive(false);
                return;
            }
        }
    }


    private IEnumerator PowerUpCooldown(PowerUp powerup)
    {
        switch (powerup.tag)
        {
            case "tripleShotPowerUp":
                {
                    _hasTripleShot = true;
                    yield return new WaitForSeconds(powerup.duration);
                    _hasTripleShot = false;
                    break;
                }
            case "deathRayPowerUp":
                {
                    _hasDeathRay = true;
                    _fireRate = 2.6f;
                    yield return new WaitForSeconds(powerup.duration);
                    _hasDeathRay = false;
                    _fireRate = _defaultFireRate;
                    break;
                }
            case "torpedoPowerUp":
                {
                    _hasTorpedo = true;
                    yield return new WaitForSeconds(powerup.duration);
                    _hasTorpedo = false;
                    break;
                }
            case "speedPowerUp":
                {
                    if (_hasMegaBoost)
                    {
                        AddScore(powerup.scoreValue);
                        yield return null;
                        break;
                    }

                    Speed *= powerup.magnitude;
                    _hasMegaBoost = true;
                    _thruster.ThrusterVFX(new Vector3(1.2f, 1.8f, 1), Color.red);
                    yield return new WaitForSeconds(powerup.duration);
                    Speed = DefaultSpeed;
                    _hasMegaBoost = false;
                    _thruster.RestoreThrusterVFX(true);
                    break;
                }
            case "shieldPowerUp":
                {
                    if (_hasShields)
                    {
                        AddScore(powerup.scoreValue);
                        yield return null;
                        break;
                    }

                    Shields shields = Instantiate(_shieldPrefab, this.transform.position, Quaternion.identity)
                        .GetComponent<Shields>();
                    shields.SetOwner(transform, "Shields");
                    _hasShields = true;
                    shields.ShieldLives = (int)powerup.magnitude;
                    _uiman.ActivateShieldBarActivation();

                    float time = powerup.duration;
                    while (time > 0)
                    {
                        time -= 0.1f;
                        if (time < 0)
                            time = 0f;
                        _uiman.GetShieldBarText().text = "Shield Time: " + time.ToString("N1");
                        yield return new WaitForSeconds(0.1f);
                    }

                    _uiman.DeactivateShieldBarActivation();
                    _hasShields = false;
                    shields.Destroy();
                    break;
                }
            case "ammo":
                _ammo += (int)powerup.magnitude;
                if (_ammo > 30)
                {
                    _ammo = 30;
                    AddScore(powerup.scoreValue);
                }
                _uiman.UpdateAmmoText(_ammo, _maxAmmo);
                PlayerPrefs.SetInt("ammo", _ammo);
                break;
            case "1up":
                Repair((int)powerup.magnitude, powerup.scoreValue);
                break;
            case "fireRatePowerDown":
                _fireRate = powerup.magnitude;
                yield return new WaitForSeconds(powerup.duration);
                _fireRate = _defaultFireRate;
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
        PlayerPrefs.SetInt("Score", Score);
        _uiman.UpdateScoreText(Score);
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
            case "enemyTorpedo":
                {
                    Damage(1, collision.transform.position.x);
                    goto case "enemyLaser";
                }
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
