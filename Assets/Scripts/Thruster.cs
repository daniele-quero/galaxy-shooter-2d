using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    private Vector3 _defaultScale = new Vector3(1, 1, 1);
    private Color _white = Color.white;
    private Player _player;
    private SimpleHealthBar _bar;
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Utilities.CheckNullGrabbed(_player, "Player");

        _bar = GameObject.FindGameObjectWithTag("thrusterBar").GetComponentInChildren<SimpleHealthBar>();
        Utilities.CheckNullGrabbed(_player, "Thruster Bar");
    }

    public void ThrusterVFX(Vector3 rescale, Color color)
    {
        StopCoroutine(FadeThrusterVFX());
        transform.localScale = rescale;
        transform.GetComponent<SpriteRenderer>().color = color;
        if(_player.HasMegaBoost)
        {
            _bar.additionalText = "Mega Boost: ON ";
            _bar.UpdateBar(_player.CurrentBoostFuel, _player.MaxBoostFuel); 
        }
    }

    public void RestoreThrusterVFX()
    {
        ThrusterVFX(_defaultScale, _white);
    }
    public void RestoreThrusterVFX(bool fade)
    {
        if (fade)
            StartCoroutine(FadeThrusterVFX());
        else
            RestoreThrusterVFX();

        _bar.additionalText = "Fuel: ";
        _bar.UpdateBar(_player.CurrentBoostFuel, _player.MaxBoostFuel);
    }

    public IEnumerator FadeThrusterVFX()
    {
        Vector3 scale = transform.localScale;
        Color color = transform.GetComponent<SpriteRenderer>().color;
        float x = 1f;
        float y = 1f;
        float r = 1f;
        float g = 1f;
        float b = 1f;

        while (!scale.Equals(_defaultScale) && !color.Equals(_white))
        {
            if (scale.x > 1f)
            {
                x = scale.x - 0.025f;
                if (x < 1f)
                    x = 1f;
            }

            if (scale.y > 1f)
            {
                y = scale.y - 0.05f;
                if (y < 1f)
                    y = 1f;
            }

            if (color.r < 1f)
                r = color.r + 0.05f;
            if (color.g < 1f)
                g = color.g + 0.05f;
            if (color.b < 1f)
                b = color.b + 0.05f;

            scale = transform.localScale;
            color = transform.GetComponent<SpriteRenderer>().color;
            transform.GetComponent<SpriteRenderer>().color = new Color(r, g, b);
            transform.localScale = new Vector3(x, y, 1f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void Boost()
    {
        if (!_player.HasMegaBoost)
        {
            if (_player.CurrentBoostFuel > 0 && Input.GetKeyDown(KeyCode.LeftShift))
            {
                _player.Speed = _player.MiniBoost;
                ThrusterVFX(new Vector3(1f, 1.4f, 1f), Color.white);
                StopCoroutine(RechargeFuel(0.05f));
                StartCoroutine(ConsumeFuel(0.2f));
            }

            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _player.Speed = _player.DefaultSpeed;
                RestoreThrusterVFX();
                StopCoroutine(ConsumeFuel(0.2f));
                StartCoroutine(RechargeFuel(0.05f));
            }
        }
    }

    private IEnumerator ConsumeFuel(float f)
    {
        while (Input.GetKey(KeyCode.LeftShift))
        {
            _player.CurrentBoostFuel -= f;
            if (_player.CurrentBoostFuel <= 0)
            {
                _player.CurrentBoostFuel = 0;
                _bar.UpdateBar(_player.CurrentBoostFuel, _player.MaxBoostFuel);
                _player.Speed = _player.DefaultSpeed;
                RestoreThrusterVFX();
                break;
            }
            _bar.UpdateBar(_player.CurrentBoostFuel, _player.MaxBoostFuel);
            yield return new WaitForSeconds(0.1f);
        }
        RestoreThrusterVFX();
    }

    private IEnumerator RechargeFuel(float f)
    {
        while (!Input.GetKey(KeyCode.LeftShift) && _player.CurrentBoostFuel < _player.MaxBoostFuel)
        {
            _player.CurrentBoostFuel += f;
            if (_player.CurrentBoostFuel > _player.MaxBoostFuel)
                _player.CurrentBoostFuel = _player.MaxBoostFuel;

            _bar.UpdateBar(_player.CurrentBoostFuel, _player.MaxBoostFuel);
            yield return new WaitForSeconds(0.1f);
        }
    }


}
