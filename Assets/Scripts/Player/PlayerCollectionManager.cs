using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCollectionManager : MonoBehaviour
{
    private Player _player;
    private bool _canAttract = true;

    void Start()
    {
        _player = GetComponent<Player>();
    }

    // Update is called once per frame
    private void Update()
    {
        AttractPowerUps();
    }

    private IEnumerator PowerUpCooldown(PowerUp powerup)
    {
        switch (powerup.tag)
        {
            case "tripleShotPowerUp":
                {
                    _player.HasTripleShot = true;
                    yield return new WaitForSeconds(powerup.duration);
                    _player.HasTripleShot = false;
                    break;
                }
            case "deathRayPowerUp":
                {
                    _player.HasDeathRay = true;
                    _player.pss.FireRate = 2.6f;
                    yield return new WaitForSeconds(powerup.duration);
                    _player.HasDeathRay = false;
                    _player.pss.RestoreDefaultFireRate();
                    break;
                }
            case "torpedoPowerUp":
                {
                    _player.HasTorpedo = true;
                    yield return new WaitForSeconds(powerup.duration);
                    _player.HasTorpedo = false;
                    break;
                }
            case "speedPowerUp":
                {
                    if (_player.HasMegaBoost)
                    {
                        _player.AddScore(powerup.scoreValue);
                        yield return null;
                        break;
                    }

                    _player.pm.Speed *= powerup.magnitude;
                    _player.HasMegaBoost = true;
                    _player.pm.thruster.ThrusterVFX(new Vector3(1.2f, 1.8f, 1), Color.red);
                    yield return new WaitForSeconds(powerup.duration);
                    _player.pm.Speed = _player.pm.DefaultSpeed;
                    _player.HasMegaBoost = false;
                    _player.pm.thruster.RestoreThrusterVFX(true);
                    break;
                }
            case "shieldPowerUp":
                {
                    if (_player.HasShields)
                    {
                        _player.AddScore(powerup.scoreValue);
                        yield return null;
                        break;
                    }

                    Shields shields = Instantiate(_player.shieldPrefab, this.transform.position, Quaternion.identity)
                        .GetComponent<Shields>();
                    shields.SetOwner(transform, "Shields");
                    _player.HasShields = true;
                    shields.ShieldLives = (int)powerup.magnitude;
                    _player.uiman.ActivateShieldBarActivation();

                    float time = powerup.duration;
                    while (time > 0)
                    {
                        time -= 0.1f;
                        if (time < 0)
                            time = 0f;
                        _player.uiman.GetShieldBarText().text = "Shield Time: " + time.ToString("N1");
                        yield return new WaitForSeconds(0.1f);
                    }

                    _player.uiman.DeactivateShieldBarActivation();
                    _player.HasShields = false;
                    shields.Destroy();
                    break;
                }
            case "ammo":
                _player.pss.Ammo += (int)powerup.magnitude;
                if (_player.pss.Ammo > 30)
                {
                    _player.pss.Ammo = 30;
                    _player.AddScore(powerup.scoreValue);
                }
                _player.uiman.UpdateAmmoText(_player.pss.Ammo, _player.pss.MaxAmmo);
                PlayerPrefs.SetInt("ammo", _player.pss.Ammo);
                goto default;
            case "1up":
                _player.Repair((int)powerup.magnitude, powerup.scoreValue);
                goto default;
            case "fireRatePowerDown":
                _player.pss.FireRate = powerup.magnitude;
                yield return new WaitForSeconds(powerup.duration);
                _player.pss.RestoreDefaultFireRate();
                break;
            default:
                yield return null;
                break;
        }
    }

    public void ActivatePowerUp(PowerUp powerup)
    {
        StartCoroutine(PowerUpCooldown(powerup));
    }

    public void AttractPowerUps()
    {
        if (_canAttract && _player.Score >0 && _player.pm.pim.collectButtonHeld)
        { 
            List<Transform> pups = GameObject.Find("PowerUpContainer").GetComponentsInChildren<Transform>().ToList();
            pups.RemoveAll(p => p.name == "PowerUpContainer");

            foreach(var p in pups)
            {
                Vector3 dir = transform.position - p.position;
                p.Translate(dir * Time.deltaTime * p.GetComponent<PowerUp>().Speed*0.1f);
                
            }
            _player.AddScore(Mathf.CeilToInt(-Time.deltaTime*75));
        }
    }


}
