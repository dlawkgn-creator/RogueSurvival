using UnityEngine;
using UnityEngine.UI;

public class VolumePanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject volumePanel;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private const string BGM_VOLUME_KEY = "BGM_VOLUME";
    private const string SFX_VOLUME_KEY = "SFX_VOLUME";

    private void Start()
    {
        float bgmSaved = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.5f);
        float sfxSaved = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.5f);

        bgmSlider.minValue = 0f;
        bgmSlider.maxValue = 1f;
        bgmSlider.value = bgmSaved;
        ApplyBgmVolume(bgmSaved);
        bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);

        sfxSlider.minValue = 0f;
        sfxSlider.maxValue = 1f;
        sfxSlider.value = sfxSaved;
        ApplySfxVolume(sfxSaved);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);

        volumePanel.SetActive(false);
    }

    public void Open()
    {
        volumePanel.SetActive(true);
    }

    public void Close()
    {
        volumePanel.SetActive(false);
    }

    private void OnBgmSliderChanged(float value)
    {
        ApplyBgmVolume(value);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    private void OnSfxSliderChanged(float value)
    {
        ApplySfxVolume(value);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    private void ApplyBgmVolume(float value)
    {
        SoundManager.Instance?.SetBgmVolume(value);
    }

    private void ApplySfxVolume(float value)
    {
        SoundManager.Instance?.SetSfxVolume(value);
    }
}
