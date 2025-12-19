using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource m_AudioSource;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        m_AudioSource = GetComponent<AudioSource>();

        m_AudioSource.loop = true;
        m_AudioSource.playOnAwake = true;
    }

    private void Start()
    {
        Play();
    }

    public void Play()
    {
        if(!m_AudioSource.isPlaying)
        {
            m_AudioSource.Play();
        }
    }

    public void Stop()
    {
        m_AudioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        m_AudioSource.volume = volume;
    }
}
