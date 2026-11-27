using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsVolumeController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string musicVolumeParam = "MusicVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";

    [SerializeField] private Slider musicSlider;
    [SerializeField] private TMP_Text musicValueText;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text sfxValueText;

    [SerializeField, Range(0f, 100f)] private float defaultMusicVolume = 80f;
    [SerializeField, Range(0f, 100f)] private float defaultSfxVolume = 70f;

    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }

    private bool isInitialized;

    private void Awake()
    {
        InitializeIfNeeded();

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }

    private void OnEnable()
    {
        InitializeIfNeeded();
        SyncUI();
    }

    private void OnDestroy()
    {
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        }
    }

    public void OnMusicVolumeChanged(float value)
    {
        InitializeIfNeeded();
        float clamped = Mathf.Clamp(value, 0f, 100f);
        MusicVolume = clamped / 100f;
        ApplyMixerVolume(musicVolumeParam, MusicVolume);
        UpdatePercentageText(musicValueText, clamped);
    }

    public void OnSFXVolumeChanged(float value)
    {
        InitializeIfNeeded();
        float clamped = Mathf.Clamp(value, 0f, 100f);
        SFXVolume = clamped / 100f;
        ApplyMixerVolume(sfxVolumeParam, SFXVolume);
        UpdatePercentageText(sfxValueText, clamped);
    }

    private void InitializeIfNeeded()
    {
        if (isInitialized)
        {
            return;
        }

        MusicVolume = Mathf.Clamp01(defaultMusicVolume / 100f);
        SFXVolume = Mathf.Clamp01(defaultSfxVolume / 100f);
        ApplyMixerVolume(musicVolumeParam, MusicVolume);
        ApplyMixerVolume(sfxVolumeParam, SFXVolume);
        isInitialized = true;
    }

    private void ApplyMixerVolume(string paramName, float normalized)
    {
        if (audioMixer == null || string.IsNullOrEmpty(paramName))
        {
            return;
        }

        float db = normalized <= 0.0001f ? -80f : Mathf.Log10(normalized) * 20f;
        audioMixer.SetFloat(paramName, db);
    }

    private void SyncUI()
    {
        float musicPercent = MusicVolume * 100f;
        float sfxPercent = SFXVolume * 100f;

        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(musicPercent);
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(sfxPercent);
        }

        UpdatePercentageText(musicValueText, musicPercent);
        UpdatePercentageText(sfxValueText, sfxPercent);
    }

    private static void UpdatePercentageText(TMP_Text targetText, float percent)
    {
        if (targetText != null)
        {
            targetText.text = $"{Mathf.RoundToInt(percent)}%";
        }
    }
}
