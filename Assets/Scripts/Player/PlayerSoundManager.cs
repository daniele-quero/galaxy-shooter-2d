using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    private AudioSource[] _sources;
    public Dictionary<string, AudioSource> sounds;

    void Start()
    {
        _sources = GetComponents<AudioSource>();
        sounds = new Dictionary<string, AudioSource>()
        {
            ["laser"] = _sources[0],
            ["explosion"] = _sources[1],
            ["damage"] = _sources[2],
            ["noammo"] = _sources[3],
            ["torpedo"] = _sources[4]
        };
    }

}
