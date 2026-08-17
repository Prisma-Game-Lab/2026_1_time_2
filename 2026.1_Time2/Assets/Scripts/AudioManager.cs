using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source - SFX")]
    public AudioSource sfxSource;

    [Header("Audio Source - Música")]
    public AudioSource musicaSource;

    [Header("Sons - UI / Menu")]
    public AudioClip somCliqueBotao;

    [Header("Sons - Player")]
    public AudioClip somPlayerDano;

    [Header("Sons - Atlatl")]
    public AudioClip somAtlatlVoando;
    public AudioClip somAtlatlAcerto;

    [Header("Sons - Macuahuitl")]
    public AudioClip somMacuahuitlErro;
    public AudioClip somMacuahuitlAcerto;

    [Header("Sons - Serpente")]
    public AudioClip somSerpenteAviso;
    public AudioClip somSerpenteMorte;

    [Header("Músicas das Fases")]
    public AudioClip musicaFaseSerpente;
    public AudioClip musicaFaseTlaloc;
    public AudioClip musicaMenu;

    [Header("Sons - Boss Tlaloc")]
    public AudioClip somTlalocLava;
    public AudioClip somTlalocRaio;
    public AudioClip somTlalocPorrada;
    public AudioClip somTlalocMorte;
    public AudioClip somTlaloqueMorte;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        CarregarVolumes();
    }

    private void CarregarVolumes()
    {
        float volGeral = PlayerPrefs.GetFloat("VolGeral", 1f);
        float volMusica = PlayerPrefs.GetFloat("VolMusica", 1f);
        float volSFX = PlayerPrefs.GetFloat("VolSFX", 1f);

        AudioListener.volume = volGeral;
        if (musicaSource != null) musicaSource.volume = volMusica;
        if (sfxSource != null) sfxSource.volume = volSFX;
    }

    public float GetVolumeGeral() => PlayerPrefs.GetFloat("VolGeral", 1f);
    public float GetVolumeMusica() => PlayerPrefs.GetFloat("VolMusica", 1f);
    public float GetVolumeSFX() => PlayerPrefs.GetFloat("VolSFX", 1f);

    public void SetVolumeGeral(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("VolGeral", volume);
        PlayerPrefs.Save();
    }

    public void SetVolumeMusica(float volume)
    {
        if (musicaSource != null)
            musicaSource.volume = volume;

        PlayerPrefs.SetFloat("VolMusica", volume);
        PlayerPrefs.Save();
    }

    public void SetVolumeSFX(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = volume;

        PlayerPrefs.SetFloat("VolSFX", volume);
        PlayerPrefs.Save();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void TocarMusica(AudioClip clip, bool loop = true)
    {
        if (clip == null || musicaSource == null) return;
        if (musicaSource.clip == clip && musicaSource.isPlaying) return;

        musicaSource.clip = clip;
        musicaSource.loop = loop;
        musicaSource.Play();
    }

    public void PararMusica()
    {
        if (musicaSource != null)
            musicaSource.Stop();
    }

    public void PlayCliqueBotao() => PlaySFX(somCliqueBotao);
    public void PlayPlayerDano() => PlaySFX(somPlayerDano);
    public void PlayAtlatlVoando() => PlaySFX(somAtlatlVoando);
    public void PlayAtlatlAcerto() => PlaySFX(somAtlatlAcerto);
    public void PlayMacuahuitlErro() => PlaySFX(somMacuahuitlErro);
    public void PlayMacuahuitlAcerto() => PlaySFX(somMacuahuitlAcerto);
    public void PlaySerpenteAviso() => PlaySFX(somSerpenteAviso);
    public void PlaySerpenteMorte() => PlaySFX(somSerpenteMorte);
    public void PlayTlalocLava() => PlaySFX(somTlalocLava);
    public void PlayTlalocRaio() => PlaySFX(somTlalocRaio);
    public void PlayTlalocPorrada() => PlaySFX(somTlalocPorrada);
    public void PlayTlalocMorte() => PlaySFX(somTlalocMorte);
    public void PlayTlaloqueMorte() => PlaySFX(somTlaloqueMorte);
}