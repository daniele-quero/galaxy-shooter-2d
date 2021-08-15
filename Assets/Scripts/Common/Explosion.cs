using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public void Explode(string trigger, AudioSource audio)
    {
        Animator animator = GetComponent<Animator>();
        Utilities.CheckNullGrabbed(animator, name + " Animator");

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        GameObject.FindGameObjectWithTag("ppv").GetComponent<PostProcessingManager>().ExplosionBloom(clips[0].length);

        animator.SetTrigger(trigger);
        audio.Play();
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        GameObject.Destroy(gameObject, clips[0].length);
    }

    public void Explode(string trigger, AudioSource audio, CameraBounds camera)
    {
        Explode(trigger, audio);
        camera.CameraShake();
    }
}
