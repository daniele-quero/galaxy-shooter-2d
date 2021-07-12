using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _angSpeed = 10f;

    [SerializeField]
    private int _lives = 1,
        _score = 5;
    
    public bool isWaveAsteroid = false;

    private Animator _animator;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        Utilities.CheckNullGrabbed(_animator, "Animator");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _angSpeed * Time.deltaTime);
    }

    public void AsteroidDestruction()
    {
        _animator.SetTrigger("onAsteroidDestruction");
        _angSpeed *= 0.2f;
        AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
        GetComponent<Collider2D>().enabled = false;
        GameObject.Destroy(this.gameObject, clips[0].length);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "laser":
                {
                    _lives--;
                    if (_lives < 0)
                        AsteroidDestruction();

                    GameObject.Destroy(collision.gameObject);
                    Player player = collision.GetComponent<Player>();
                    if (player != null)
                        player.Score += _score;

                    break;
                }
            case "enemy":
                {
                    _lives--;
                    if (_lives < 0)
                        AsteroidDestruction();
                    Enemy enemy = collision.GetComponent<Enemy>();
                    if (enemy != null)
                        enemy.EnemyDeath();
                    break;
                }
            case "Player":
                {
                    Player player = collision.GetComponent<Player>();
                    if (player != null)
                        player.Damage(1);
                    break;
                }
            case "shields":
                AsteroidDestruction();
                break;
            default:
                break;
        }
    }
}
