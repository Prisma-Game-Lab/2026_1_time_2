using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source - SFX")]
    public AudioSource sfxSource;

    [Header("Audio Source - Música")]
    public AudioSource musicaSource;

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

    [Header("Sons - UI / Menu")]
    public AudioClip somCliqueBotao;

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

    // Métodos de Controle de Volume
    public void SetVolumeGeral(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetVolumeMusica(float volume)
    {
        if (musicaSource != null)
            musicaSource.volume = volume;
    }

    public void SetVolumeSFX(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = volume;
    }

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

    public void PlayCliqueBotao() => PlaySFX(somCliqueBotao);
}