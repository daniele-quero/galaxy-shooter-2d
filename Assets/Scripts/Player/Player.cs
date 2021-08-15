using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private bool _hasTripleShot = false,
        _hasMegaBoost = false,
        _hasShields = false,
        _hasDeathRay = false,
        _hasTorpedo = false;

    public SpriteRenderer spriteRenderer;
    public CameraBounds cameraBounds = null;
    public GameObject shieldPrefab = null;
    public PlayerSoundManager psm;
    public UIManager uiman;

    private LvlManager _lvlManager;
    public PlayerMovement pm;
    public PlayerShootingSystem pss;

    [SerializeField]
    private int _kills = 0;

    public int Lives { get; set; } = 3;
    public bool HasMegaBoost { get => _hasDeathRay; set => _hasMegaBoost = value; }
    public int Ammo { get; set; } = 15;
    public int Score { get; set; } = 0;
    public int Kills { get => _kills; set => _kills = value; }
    public bool HasTripleShot { get => _hasTripleShot; set => _hasTripleShot = value; }
    public bool HasShields { get => _hasShields; set => _hasShields = value; }
    public bool HasDeathRay { get => _hasDeathRay; set => _hasDeathRay = value; }
    public bool HasTorpedo { get => _hasTorpedo; set => _hasTorpedo = value; }

    void Start()
    {
        psm = GetComponent<PlayerSoundManager>();
        pm = GetComponent<PlayerMovement>();
        pss = GetComponent<PlayerShootingSystem>();

        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            cameraBounds = camObj.GetComponent<CameraBounds>();
        else
            Utilities.LogNullGrabbed("Camera");

        spriteRenderer = GetComponent<SpriteRenderer>();

        _lvlManager = GameObject.Find("LevelManager").GetComponent<LvlManager>();
        Utilities.CheckNullGrabbed(_lvlManager, "LvlManager");

        uiman = GameObject.Find("Canvas").GetComponent<UIManager>();
        Utilities.CheckNullGrabbed(uiman, "UIManager");


        Score = PlayerPrefs.GetInt("Score", 0);
        Lives = PlayerPrefs.GetInt("Lives", 3);
        pss.Ammo = PlayerPrefs.GetInt("ammo", 15);

        uiman.UpdateAmmoText(pss.Ammo, pss.MaxAmmo);

        if (PlayerPrefs.GetInt("Engine0", 0) == 1)
            SetEngineFire(transform.position.x - 1);
        if (PlayerPrefs.GetInt("Engine1", 0) == 1)
            SetEngineFire(transform.position.x + 1);
    }

    void Update()
    {

    }

    public void Damage(int dmg, float x)
    {
        if (!HasShields)
        {
            Lives -= dmg;
            psm.sounds["damage"].Play();

            if (Lives < 2)
                SetEngineFire(x);

            PlayerPrefs.SetInt("Lives", Lives);
        }

        if (Lives < 0)
            playerDeath();

        uiman.UpdateLivesDisplay(Lives);
    }

    public void Repair(int up, int scoreValue)
    {
        Lives += up;
        if (Lives > 3)
        {
            Lives = 3;
            AddScore(scoreValue);
        }

        UnsetEngineFire();
        uiman.UpdateLivesDisplay(Lives);

    }

    private void playerDeath()
    {
        GameObject.Destroy(transform.Find("Thruster").gameObject);
        GameObject[] engines = new GameObject[2] { transform.Find("EngineFire0").gameObject, transform.Find("EngineFire1").gameObject };
        foreach (var eng in engines)
            GameObject.Destroy(eng);

        GetComponent<Explosion>().Explode("onPlayerDeath", psm.sounds["explosion"], cameraBounds);
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

    public void AddScore(int score)
    {
        Score += score;
        if (Score < 0)
            Score = 0;

        int max = PlayerPrefs.GetInt("MaxScore", 0);
        max = Score > max ? score : max;

        PlayerPrefs.SetInt("Score", Score);
        PlayerPrefs.SetInt("MaxScore", max);

        uiman.UpdateScoreText(Score);
    }

    public void AddKill(int kill)
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
                    Damage(2, collision.transform.position.x);
                    collision.GetComponent<Torpedo>().Destruct();
                    psm.sounds["damage"].Play();
                    break;
                }
            case "enemyLaser":
                Damage(1, collision.transform.position.x);
                GameObject.Destroy(collision.gameObject);
                psm.sounds["damage"].Play();
                break;
            default:
                break;
        }
    }

}
