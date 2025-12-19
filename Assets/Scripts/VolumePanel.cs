using UnityEngine;
using UnityEngine.UI;

public class VolumePanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject volumePanel;
    [SerializeField] private Slider bgmSlider;

    private const string BGM_VOLUME_KEY = "BGM_VOLUME";

    private void Start()
    {
        float saved = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.5f);

        bgmSlider.minValue = 0f;
        bgmSlider.maxValue = 1f;
        bgmSlider.value = saved;

        ApplyVolume(saved);

        bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);

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
        ApplyVolume(value);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    private void ApplyVolume(float value)
    {
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBgmVolume(value);
        }
    }
}
