using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shields : MonoBehaviour
{
    [SerializeField]
    private int _shieldLives = 2;
    private int _maxLives;
    private AudioSource[] _sources;
    private Dictionary<string, AudioSource> _sounds;
    private UIManager _uimanager;

    public int ShieldLives { get => _shieldLives; set => _shieldLives = value; }

    void Start()
    {
        _sources = GetComponents<AudioSource>();
        _sounds = new Dictionary<string, AudioSource>()
        {
            ["shieldDamage"] = _sources[0]
        };

        _uimanager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _uimanager.GetShieldBar().UpdateBar(_shieldLives, _maxLives);
    }


    public void Destroy()
    {
        if(this != null && gameObject != null)
        {
            GetComponent<Collider2D>().enabled = false;
            GameObject.Destroy(this.gameObject); 
        }
    }

    public void Damage(int dmg)
    {
        _uimanager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _shieldLives -= dmg;
        _sounds["shieldDamage"].Play();
        _uimanager.GetShieldBar().UpdateBar(_shieldLives, _maxLives);
        if (_shieldLives < 0)
        {
            _uimanager.DeactivateShieldBarActivation();
            Destroy();
        }
    }

    public void SetOwner(Transform parent, string shieldsName)
    {
        name = shieldsName;
        transform.SetParent(parent);
        _maxLives = _shieldLives;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "laser":
            case "enemyLaser":
                Damage(1);
                GameObject.Destroy(collision.gameObject);
                break;
            case "enemy":
                Damage(1);
                collision.GetComponent<Enemy>().EnemyDeath();
                break;
            default:
                break;

        }
    }
}
