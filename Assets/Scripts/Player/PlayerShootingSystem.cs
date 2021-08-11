using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootingSystem : MonoBehaviour
{
    private Player _player;
    private PlayerInputManager _pim;
    private Vector2 _laserSpawnPosition = new Vector2();

    [SerializeField]
    private GameObject
        _laserPrefab = null,
        _tripleShotPrefab = null,
        _deathRayPrefab = null,
        _torpedoPrefab = null,
        _shot;

    [SerializeField]
    private float _fireRate = 0.2f, _defaultFireRate = 0.2f;

    private float _nextFireTime = -1f;

    [SerializeField]
    private int _maxAmmo = 30, _ammo = 15;

    public int Ammo { get => _ammo; set => _ammo = value; }
    public int MaxAmmo { get => _maxAmmo; set => _maxAmmo = value; }
    public float FireRate { get => _fireRate; set => _fireRate = value; }
    public float DefaultFireRate { get => _defaultFireRate; set => _defaultFireRate = value; }

    void Start()
    {
        _player = GetComponent<Player>();
        _pim = GetComponent<PlayerInputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        ShootLaser();
    }

    private void ShootLaser()
    {
        if (_pim.shootButtonPressed)
        {
            _laserSpawnPosition = transform.position;
            _laserSpawnPosition.y += _player.spriteRenderer.sprite.rect.size.y / 2 / _player.spriteRenderer.sprite.pixelsPerUnit * transform.lossyScale.y;

            _shot = _laserPrefab;
            AudioSource shootSound = _player.psm.sounds["laser"];
            int ammoConsumed = 1;

            if (_player.HasTripleShot)
            {
                _shot = _tripleShotPrefab;
                ammoConsumed = 3;
            }

            else if (_player.HasDeathRay)
            {
                _shot = _deathRayPrefab;
                ammoConsumed = 0;
            }

            else if (_player.HasTorpedo)
            {
                _shot = _torpedoPrefab;
                shootSound = _player.psm.sounds["torpedo"];
                ammoConsumed = 2;
            }

            if (_shot != null && Time.time > _nextFireTime)
            {
                if (Ammo >= ammoConsumed)
                {
                    Ammo -= ammoConsumed;
                    PlayerPrefs.SetInt("ammo", Ammo);
                    _nextFireTime = Time.time + _fireRate;
                    Instantiate(_shot, _laserSpawnPosition, Quaternion.identity);
                    shootSound.Play();
                    _player.uiman.UpdateAmmoText(Ammo, MaxAmmo);
                }
                else
                    _player.psm.sounds["noammo"].Play();
            }
        }
    }

    public void RestoreDefaultFireRate()
    {
        _fireRate = _defaultFireRate;
    }
}
