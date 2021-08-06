using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInterfaces;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int _scoreValue = 10,
        _lives = 0;

    private AudioSource[] _sources;

    public Animator animator;
    public Dictionary<string, AudioSource> _sounds;
    public LvlManager lvlManager;
    public EnemyMovement movement;

    void Start()
    {
        animator = GetComponent<Animator>();
        Utilities.CheckNullGrabbed(animator, "Animator");
        movement = GetComponent<EnemyMovement>();

        _sources = GetComponents<AudioSource>();
        _sounds = new Dictionary<string, AudioSource>()
        {
            ["collision"] = _sources[0],
            ["explosion"] = _sources[1],
            ["laser"] = _sources[2],
        };

        lvlManager = GameObject.Find("LevelManager").GetComponent<LvlManager>();

        _lives = lvlManager.enemyLives;
        _scoreValue = lvlManager.enemyScore;
    }

    private void EnemyDamageBase()
    {
        _lives--;
        _sounds["collision"].Play();
        if (_lives < 0)
            EnemyDeath();
    }

    public void EnemyKill(bool isRam)
    {
        EnemyDamageBase();
        if (_lives < 0)
        {
            Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            if (player != null)
            {
                player.AddScore(_scoreValue);
                player.addKill(1);
                if (isRam)
                    player.Damage(1, transform.position.x);
            }
        }
    }

    public void EnemyKill()
    {
        EnemyKill(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "laser":
                {
                    GameObject.Destroy(collision.gameObject);
                    EnemyKill();
                    break;
                }
            case "Player":
                {
                    EnemyKill(true);
                    break;
                }
            case "shields":
                {
                    collision.GetComponent<AudioSource>().Play();
                    Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                    if (player != null)
                    {
                        player.AddScore(_scoreValue);
                        player.addKill(1);
                    }

                    EnemyDeath();
                    break;
                }
            default:
                break;
        }
    }

    public void EnemyDeath()
    {
        if (name.Contains("Fighter"))
            transform.localScale *= 2f;

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        GameObject.FindGameObjectWithTag("ppv").GetComponent<PostProcessingManager>().ExplosionBloom(clips[0].length);
        animator.SetTrigger("onEnemyDeath");
        movement.cameraBounds.CameraShake();
        GetComponent<Collider2D>().enabled = false;
        movement.ScaleSpeed(0.75f);
        _sounds["explosion"].Play();
        SelfDestroy(clips[0].length);
    }

    private void SelfDestroy()
    {
        GameObject.Destroy(this.gameObject);
    }

    private void SelfDestroy(float time)
    {
        GameObject.Destroy(this.gameObject, time);
    }
}
