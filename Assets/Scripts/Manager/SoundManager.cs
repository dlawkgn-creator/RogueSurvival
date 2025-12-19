using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip uiClickSfx;
    [SerializeField] private AudioClip foodSfx;
    [SerializeField] private AudioClip moveSfx;
    [SerializeField] private AudioClip hitSfx;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource.loop = true;
        bgmSource.playOnAwake = true;

        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
    }

    private void Start()
    {
        PlayBgm();
    }

    public void PlayBgm()
    {
        if(!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    public void StopBgm()
    {
        bgmSource.Stop();
    }

    public void SetBgmVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        if(sfxSource == null)
        {
            return;
        }
        sfxSource.volume = volume;
    }

    public void PlaySfx(AudioClip clip)
    {
        if(clip == null || sfxSource == null)
        {
            return;
        }
        sfxSource.PlayOneShot(clip);
    }

    public void PlayEatFood()
    {
        PlaySfx(foodSfx);
    }

    public void PlayMove()
    {
        if(moveSfx == null || sfxSource == null)
        {
            return;
        }
        sfxSource.PlayOneShot(moveSfx);
    }

    public void PlayUiClick()
    {
        PlaySfx(uiClickSfx);
    }

    public void PlayHit()
    {
        PlaySfx(hitSfx);
    }
}
