using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomPitchPlay : MonoBehaviour
{
    AudioSource aud;
    [SerializeField]private Vector2 range;
    private void Awake()
    {
        aud = GetComponent<AudioSource>();
    }
    public void Play()
    {
        aud.pitch = Random.Range(range.x, range.y);
        aud.PlayOneShot(aud.clip);
    }
    public void Play(AudioClip clip)
    {
        aud.clip = clip;
        aud.pitch = Random.Range(range.x, range.y);
        aud.PlayOneShot(aud.clip);
    }
    public void JustPlay()
    {
        aud.pitch = Random.Range(range.x, range.y);
        aud.Play();
    }
}
