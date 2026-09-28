using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private float volumeGeral = 0.5f;

    [Header("Audio Source - SFX")]
    public AudioSource sfxSource;

    [Header("Audio Source - Música")]
    public AudioSource musicaSource;
    public AudioClip musicaMenuIntro;
    public AudioClip musicaMenuLoop;

    [Header("Audio Source - Ambiente")]
    public AudioSource ambienteSource;

    private Coroutine rotinaMusica;

    [Header("Sons - Player (Eztli)")]
    public AudioClip somPlayerDano;
    public AudioClip somPlayerDash;

    [Header("Sons - Atlatl")]
    public AudioClip somAtlatlVoando;
    public AudioClip somAtlatlAcerto;

    [Header("Sons - Macuahuitl")]
    public AudioClip somMacuahuitlErro;
    public AudioClip somMacuahuitlAcerto;

    [Header("Sons - Serpente")]
    public AudioClip somSerpenteAviso;
    public AudioClip somSerpenteMato;
    public AudioClip somSerpenteMorte;

    [Header("Sons - Mulher")]
    public AudioClip somMulherGrito;
    public AudioClip somMulherMorte;
    public AudioClip somMulherGiro;
    public AudioClip somMulherDano1;
    public AudioClip somMulherDano2;

    [Header("Sons - Boss Tlaloc")]
    public AudioClip somTlalocLava;
    public AudioClip somTlalocRaio;
    public AudioClip somTlalocPorrada;
    public AudioClip somTlalocMorte;
    public AudioClip somTlaloqueMorte;
    public AudioClip somTlalocRiso;
    public AudioClip somTlalocDor1;
    public AudioClip somTlalocDor2;
    public AudioClip somTlalocTempestadeAmbiente;

    [Header("Músicas das Fases e Mapa")]
    public AudioClip musicaFaseSerpente;
    public AudioClip musicaFaseTlaloc;
    public AudioClip musicaFaseMulher;
    public AudioClip musicaMenu;
    public AudioClip musicaWorldMap;

    [Header("Sons - UI / Recompensa")]
    public AudioClip somCliqueBotao;
    public AudioClip somRecompensa;

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

        if (ambienteSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length > 2)
                ambienteSource = sources[2];
        }

        CarregarVolumes();
    }

    private void CarregarVolumes()
    {
        float volGeral = PlayerPrefs.GetFloat("VolGeral", volumeGeral);
        float volMusica = PlayerPrefs.GetFloat("VolMusica", volumeGeral);
        float volSFX = PlayerPrefs.GetFloat("VolSFX", volumeGeral);

        AudioListener.volume = volGeral;
        if (musicaSource != null) musicaSource.volume = volMusica;
        if (sfxSource != null) sfxSource.volume = volSFX;
        if (ambienteSource != null) ambienteSource.volume = volSFX;
    }

    public void TocarMusicaComIntro(AudioClip introClip, AudioClip loopClip)
    {
        if (musicaSource == null || introClip == null) return;

        if (rotinaMusica != null)
            StopCoroutine(rotinaMusica);

        PararMusica();
        rotinaMusica = StartCoroutine(RotinaTocarIntroELoop(introClip, loopClip));
    }

    private IEnumerator RotinaTocarIntroELoop(AudioClip intro, AudioClip loop)
    {
        musicaSource.clip = intro;
        musicaSource.loop = false;
        musicaSource.Play();

        yield return new WaitForSeconds(intro.length);

        if (loop != null)
        {
            musicaSource.clip = loop;
            musicaSource.loop = true;
            musicaSource.Play();
        }
    }

    public float GetVolumeGeral() => PlayerPrefs.GetFloat("VolGeral", volumeGeral);
    public float GetVolumeMusica() => PlayerPrefs.GetFloat("VolMusica", volumeGeral);
    public float GetVolumeSFX() => PlayerPrefs.GetFloat("VolSFX", volumeGeral);

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

    public void TocarAmbiente(AudioClip clip, bool loop = true)
    {
        if (clip == null || ambienteSource == null) return;
        ambienteSource.clip = clip;
        ambienteSource.loop = loop;
        ambienteSource.Play();
    }

    public void PararAmbiente()
    {
        if (ambienteSource != null)
            ambienteSource.Stop();
    }

    public void SetVolumeGeral(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("VolGeral", volume);
        PlayerPrefs.Save();
    }

    public void SetVolumeMusica(float volume)
    {
        if (musicaSource != null) musicaSource.volume = volume;
        PlayerPrefs.SetFloat("VolMusica", volume);
        PlayerPrefs.Save();
    }

    public void SetVolumeSFX(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
        if (ambienteSource != null) ambienteSource.volume = volume;
        PlayerPrefs.SetFloat("VolSFX", volume);
        PlayerPrefs.Save();
    }

    // Player
    public void PlayPlayerDano() => PlaySFX(somPlayerDano);
    public void PlayPlayerDash() => PlaySFX(somPlayerDash);

    // Armas
    public void PlayAtlatlVoando() => PlaySFX(somAtlatlVoando);
    public void PlayAtlatlAcerto() => PlaySFX(somAtlatlAcerto);
    public void PlayMacuahuitlErro() => PlaySFX(somMacuahuitlErro);
    public void PlayMacuahuitlAcerto() => PlaySFX(somMacuahuitlAcerto);

    // Serpente
    public void PlaySerpenteAviso() => PlaySFX(somSerpenteAviso);
    public void PlaySerpenteMato() => PlaySFX(somSerpenteMato);
    public void PlaySerpenteMorte() => PlaySFX(somSerpenteMorte);

    // Mulher
    public void PlayMulherGrito() => PlaySFX(somMulherGrito);
    public void PlayMulherMorte() => PlaySFX(somMulherMorte);
    public void PlayMulherGiro() => PlaySFX(somMulherGiro);
    public void PlayMulherDanoAleatorio()
    {
        AudioClip[] clips = { somMulherDano1, somMulherDano2 };
        AudioClip sorteado = clips[Random.Range(0, clips.Length)];
        if (sorteado != null) PlaySFX(sorteado);
        else PlaySFX(somMulherDano1 != null ? somMulherDano1 : somMulherDano2);
    }

    // Tlaloc
    public void PlayTlalocLava() => PlaySFX(somTlalocLava);
    public void PlayTlalocRaio() => PlaySFX(somTlalocRaio);
    public void PlayTlalocPorrada() => PlaySFX(somTlalocPorrada);
    public void PlayTlalocMorte() => PlaySFX(somTlalocMorte);
    public void PlayTlaloqueMorte() => PlaySFX(somTlaloqueMorte);
    public void PlayTlalocRiso() => PlaySFX(somTlalocRiso);
    public void PlayTlalocDorAleatoria()
    {
        AudioClip[] clips = { somTlalocDor1, somTlalocDor2 };
        AudioClip sorteado = clips[Random.Range(0, clips.Length)];
        if (sorteado != null) PlaySFX(sorteado);
        else PlaySFX(somTlalocDor1 != null ? somTlalocDor1 : somTlalocDor2);
    }

    // UI / Recompensa
    public void PlayCliqueBotao() => PlaySFX(somCliqueBotao);
    public void PlayRecompensa() => PlaySFX(somRecompensa);
}