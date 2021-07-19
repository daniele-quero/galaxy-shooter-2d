using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    PostProcessVolume _volume;
    Bloom _bloom;
    [SerializeField]
    private float _bloomVal = 20f,
        _timeStep = 0.2f;

    void Start()
    {
        _volume = GetComponent<PostProcessVolume>();
        Utilities.CheckNullGrabbed(_volume, "Volume");
        _volume.profile.TryGetSettings<Bloom>(out _bloom);
        Utilities.CheckNullGrabbed(_bloom, "Bloom");
    }

    public void ExplosionBloom(float t)
    {
        StartCoroutine(BloomCooldown(t * 0.75f));
    }

    private IEnumerator BloomCooldown(float t)
    {
        if(_bloom == null)
            _volume.profile.TryGetSettings<Bloom>(out _bloom);

        _bloom.intensity.value = _bloomVal;
        int steps = (int)(t / _timeStep);
        float bloomStep = _bloomVal / steps;

        while (_bloom.intensity.value > 0)
        {
            _bloom.intensity.value -= bloomStep;
            yield return new WaitForSeconds(_timeStep);
        }

        _bloom.intensity.value = 0f;
    }
}
