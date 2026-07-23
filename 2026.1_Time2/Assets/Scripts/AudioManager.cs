using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    public AudioSource sfxSource;

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

    public void PlayPlayerDano() => PlaySFX(somPlayerDano);
    public void PlayAtlatlVoando() => PlaySFX(somAtlatlVoando);
    public void PlayAtlatlAcerto() => PlaySFX(somAtlatlAcerto);
    public void PlayMacuahuitlErro() => PlaySFX(somMacuahuitlErro);
    public void PlayMacuahuitlAcerto() => PlaySFX(somMacuahuitlAcerto);
    public void PlaySerpenteAviso() => PlaySFX(somSerpenteAviso);
    public void PlaySerpenteMorte() => PlaySFX(somSerpenteMorte);
}