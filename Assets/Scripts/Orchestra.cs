using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orchestra : MonoBehaviour
{
    public AudioSource[] sand;
    public AudioSource[] sprint;
    public AudioSource[] bubbles;
    public AudioSource laughter;
    public AudioSource scream;
    public AudioSource[] crash;
    public AudioSource[] knock;
    public AudioSource gong;

    public enum Sfx
    {
        Sand,
        Sprint,
        Bubbles,
        Laughter,
        Scream,
        Crash,
        Knock,
        Gong
    }

    public static Orchestra instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            instance.PlayDefault();
        }
    }

    private void PlayDefault()
    {
        Play(Sfx.Bubbles, 3);
        Play(Sfx.Gong);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private Dictionary<Sfx, float> sparselyPlaying = new Dictionary<Sfx, float>();

    public const float VeryVerySparsely = 1f;
    public const float VerySparsely = 0.6f;
    public const float Sparsely = 0.3f;

    public static void Play(Sfx sfx, float interval)
    {
        instance.PlaySfx(sfx, interval);
    }

    public static void Play(Sfx sfx)
    {
        instance.PlaySfx(sfx, 0);
    }

    private void PlaySfx(Sfx sfx, float interval) {

        interval = UnityEngine.Random.Range(interval / 2, 1.5f * interval);

        if (!sparselyPlaying.ContainsKey(sfx))
        {
            sparselyPlaying.Add(sfx, Time.time);
        }
        else if (sparselyPlaying[sfx] + interval > Time.time)
        {
            return;
        }
        else
        {
            sparselyPlaying[sfx] = Time.time;
        }

        switch (sfx)
        {
            case Sfx.Sand:
                PlaySfx(sand);
                break;

            case Sfx.Sprint:
                PlaySfx(sprint);
                break;

            case Sfx.Bubbles:
                PlaySfx(bubbles);
                break;

            case Sfx.Laughter:
                PlaySfx(laughter);
                break;

            case Sfx.Scream:
                PlaySfx(scream);
                break;

            case Sfx.Crash:
                PlaySfx(crash);
                break;

            case Sfx.Knock:
                PlaySfx(knock);
                break;

            case Sfx.Gong:
                PlaySfx(gong);
                break;
        }
    }

    private void PlaySfx(AudioSource sfx)
    {
        PlaySfxIfNotPlaying(sfx);        
    }

    private void PlaySfx(AudioSource[] sfxGroup)
    {
        int i = UnityEngine.Random.Range(0, sfxGroup.Length);
        PlaySfxIfNotPlaying(sfxGroup[i]);
    }

    private void PlaySfxIfNotPlaying(AudioSource sfx)
    {
        if (!sfx.isPlaying)
        {
            sfx.Play();
            Debug.Log("Playing SFX " + sfx.name);
        }
    }    

}
