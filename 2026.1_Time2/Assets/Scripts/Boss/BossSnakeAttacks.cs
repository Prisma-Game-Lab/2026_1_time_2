using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSnakeAttacks : MonoBehaviour
{
    public Transform player;
    public GameObject tornadoPrefab;

    public int tornadoCount = 1;
    public float tornadoSpawnRadius = 2f;
    public float tornadoMinSize = 0.5f;
    public float tornadoMaxSize = 2f;
    public float tornadoMinSpeed = 1.5f;
    public float tornadoMaxSpeed = 5f;
    public float tornadoDamage = 10f;

    public float biteDashSpeed = 20f;
    public float biteStunDuration = 3f;
    public float biteDamage = 20f;
    private int missedBitesCount = 0;

    public float dashOutSpeed = 15f;
    public float dashOutDamage = 25f;
    public float dashTiredDuration = 4f;
    public float offscreenOffset = 3f;

    public Animator animator;

    public Color corStun = Color.red;
    public float velocidadePiscada = 0.15f;
    private SpriteRenderer spriteRenderer;
    private Color corOriginal;
    private Coroutine stunBlinkCoroutine;

    [Header("Hitbox Animada")]
    public PolygonCollider2D bossCollider;

    public Vector2[] pontosVoando = new Vector2[] {
        new Vector2(1.23731f, -17.8334f), new Vector2(1.344902f, -17.51063f), new Vector2(1.855965f, -17.34924f), new Vector2(2.501518f, -16.99957f), new Vector2(3.17397f, -16.11193f), new Vector2(3.30846f, -15.35878f), new Vector2(3.819523f, -14.55184f), new Vector2(3.44295f, -14.22907f), new Vector2(3.819523f, -13.58351f), new Vector2(3.819523f, -13.36833f), new Vector2(3.792625f, -13.26074f), new Vector2(3.093275f, -13.23384f), new Vector2(3.227766f, -12.91106f), new Vector2(3.335358f, -12.31931f), new Vector2(3.254664f, -12.23861f), new Vector2(2.689805f, -12.26551f), new Vector2(2.662907f, -10.89371f), new Vector2(2.582212f, -10.78612f), new Vector2(1.506291f, -11.16269f), new Vector2(1.398698f, -10.14056f), new Vector2(1.291106f, -8.876355f), new Vector2(1.560087f, -7.719739f), new Vector2(1.291106f, -7.558351f), new Vector2(1.264208f, -5.998264f), new Vector2(1.23731f, -5.971366f), new Vector2(1.506291f, -0.6993492f), new Vector2(2.017354f, -0.6993492f), new Vector2(2.07115f, -0.6455531f), new Vector2(2.098048f, 0.4303688f), new Vector2(1.855965f, 0.6455531f), new Vector2(1.3718f, 0.6455531f), new Vector2(1.748373f, 6.321041f), new Vector2(1.855965f, 6.832104f), new Vector2(1.829067f, 7.370065f), new Vector2(1.398698f, 7.450759f), new Vector2(1.398698f, 10.19436f), new Vector2(2.07115f, 10.14056f), new Vector2(2.178742f, 11.35098f), new Vector2(2.07115f, 11.45857f), new Vector2(1.183514f, 11.45857f), new Vector2(1.129718f, 13.20694f), new Vector2(1.129718f, 15.97744f), new Vector2(1.667679f, 16.00434f), new Vector2(1.748373f, 16.11193f), new Vector2(1.748373f, 16.99957f), new Vector2(1.694577f, 17.10716f), new Vector2(0.5917571f, 17.18785f), new Vector2(0.4034707f, 18.93623f), new Vector2(0.5917571f, 19.74317f), new Vector2(0.3496746f, 19.93145f), new Vector2(0.2151844f, 21.19566f), new Vector2(0.9414316f, 21.19566f), new Vector2(1.049024f, 21.24946f), new Vector2(1.049024f, 21.9757f), new Vector2(0.9952278f, 22.56746f), new Vector2(0.9145336f, 22.64816f), new Vector2(-0.1613883f, 22.64816f), new Vector2(-0.05379609f, 22.94403f), new Vector2(0.5110629f, 23.02473f), new Vector2(0.4572668f, 23.61649f), new Vector2(0.4034707f, 23.75097f), new Vector2(-0.2420824f, 24.07375f), new Vector2(-0.2689805f, 24.55792f), new Vector2(-0.3496746f, 24.63861f), new Vector2(-0.8338395f, 24.63861f), new Vector2(-0.9952278f, 24.58481f), new Vector2(-0.9683297f, 21.62603f), new Vector2(-1.022126f, 24.63861f), new Vector2(-1.506291f, 24.63861f), new Vector2(-1.667679f, 24.61171f), new Vector2(-1.748373f, 24.53102f), new Vector2(-1.748373f, 24.04685f), new Vector2(-1.667679f, 23.96616f), new Vector2(-1.829067f, 23.75097f), new Vector2(-2.367028f, 23.72408f), new Vector2(-2.47462f, 23.69718f), new Vector2(-2.420824f, 22.89024f), new Vector2(-1.909761f, 22.94403f), new Vector2(-1.802169f, 22.62126f), new Vector2(-2.286334f, 22.62126f), new Vector2(-2.878091f, 22.59436f), new Vector2(-3.012581f, 22.59436f), new Vector2(-2.904989f, 21.03427f), new Vector2(-2.20564f, 21.14186f), new Vector2(-2.178742f, 21.22256f), new Vector2(-2.178742f, 21.89501f), new Vector2(-2.232538f, 21.94881f), new Vector2(-2.151844f, 22.0295f), new Vector2(-2.178742f, 21.62603f), new Vector2(-2.124946f, 21.41084f), new Vector2(-2.393926f, 19.55488f), new Vector2(-2.232538f, 19.33969f), new Vector2(-2.34013f, 17.02646f), new Vector2(-3.469848f, 17.13406f), new Vector2(-3.658134f, 16.64989f), new Vector2(-3.469848f, 15.92364f), new Vector2(-2.904989f, 15.97744f), new Vector2(-2.878091f, 11.40477f), new Vector2(-3.738828f, 11.45857f), new Vector2(-3.873319f, 11.37787f), new Vector2(-3.900217f, 10.27505f), new Vector2(-3.416052f, 10.19436f), new Vector2(-3.120173f, 10.22126f), new Vector2(-3.120173f, 7.396963f), new Vector2(-3.389154f, 7.396963f), new Vector2(-3.496746f, 7.316269f), new Vector2(-3.496746f, 6.428633f), new Vector2(-3.416052f, 6.347939f), new Vector2(-3.120173f, 6.347939f), new Vector2(-3.711931f, 0.6455531f), new Vector2(-3.846421f, 0.564859f), new Vector2(-3.873319f, -0.6455531f), new Vector2(-3.792625f, -0.7262473f), new Vector2(-3.147072f, -0.6993492f), new Vector2(-3.120173f, -4.787852f), new Vector2(-3.093275f, -7.585249f), new Vector2(-3.200868f, -7.558351f), new Vector2(-3.523644f, -7.773536f), new Vector2(-3.281562f, -8.419088f), new Vector2(-3.200868f, -9.118438f), new Vector2(-3.362256f, -11.29718f), new Vector2(-4.196095f, -10.83991f), new Vector2(-4.438178f, -11.0013f), new Vector2(-4.384382f, -12.4f), new Vector2(-4.976139f, -12.3462f), new Vector2(-5.056833f, -12.4269f), new Vector2(-5.487202f, -13.42213f), new Vector2(-5.487202f, -13.7449f), new Vector2(-5.029935f, -14.41735f), new Vector2(-5.325813f, -14.57874f), new Vector2(-5.406507f, -14.65944f), new Vector2(-5.325813f, -14.90152f), new Vector2(-4.949241f, -15.25119f), new Vector2(-4.922343f, -15.54707f), new Vector2(-4.81475f, -16.11193f), new Vector2(-4.599566f, -16.46161f), new Vector2(-3.980911f, -16.75748f), new Vector2(-4.088503f, -17.08026f), new Vector2(-3.819523f, -17.32234f), new Vector2(-2.904989f, -17.53753f), new Vector2(-2.824295f, -17.941f), new Vector2(-2.420824f, -18.66724f), new Vector2(-1.990456f, -20.17353f), new Vector2(-1.694577f, -20.55011f), new Vector2(-1.560087f, -20.57701f), new Vector2(0.05379609f, -20.57701f), new Vector2(0.2151844f, -20.52321f), new Vector2(0.4572668f, -20.14664f), new Vector2(0.8069414f, -18.85553f)
    };

    public Vector2[] pontosTornado = new Vector2[] {
        new Vector2(-1.730019f, -8.735742f), new Vector2(-1.199023f, -7.416816f), new Vector2(-1.079121f, -6.628886f), new Vector2(-1.061992f, -6.183535f), new Vector2(-0.8564453f, -6.149277f), new Vector2(-0.4453515f, -6.029375f), new Vector2(-0.3254492f, -5.875215f), new Vector2(0.1199023f, -5.755312f), new Vector2(0.2055469f, -5.669668f), new Vector2(0.2055469f, -5.584023f), new Vector2(0.2226758f, -5.378476f), new Vector2(0.9592187f, -4.881738f), new Vector2(0.7536718f, -4.556289f), new Vector2(1.09625f, -4.179453f), new Vector2(1.061992f, -4.059551f), new Vector2(1.336055f, -3.802617f), new Vector2(1.55873f, -3.528554f), new Vector2(1.541601f, -3.44291f), new Vector2(1.40457f, -3.340137f), new Vector2(1.216152f, -3.168848f), new Vector2(1.644375f, -2.34666f), new Vector2(1.592988f, -2.278144f), new Vector2(0.907832f, -2.261015f), new Vector2(1.130508f, -1.507344f), new Vector2(1.130508f, -1.353184f), new Vector2(1.09625f, -1.301797f), new Vector2(0.4110937f, -1.370312f), new Vector2(0.4624805f, -1.301797f), new Vector2(0.4967383f, -0.5995117f), new Vector2(0.4967383f, 0f), new Vector2(0.3425781f, 0.08564453f), new Vector2(-0.3939648f, -0.2569336f), new Vector2(-0.3939648f, -0.05138671f), new Vector2(-0.4282226f, 0.4282226f), new Vector2(-0.4453515f, 0.4624805f), new Vector2(5.515508f, 0.4282226f), new Vector2(5.532637f, 0.06851562f), new Vector2(5.549765f, -0.1541601f), new Vector2(5.652539f, -0.2398047f), new Vector2(6.697402f, -0.2226758f), new Vector2(6.783047f, -0.188418f), new Vector2(6.800175f, 0.1027734f), new Vector2(6.73166f, 0.8393164f), new Vector2(7.365429f, 0.7708007f), new Vector2(7.725136f, 0.7879297f), new Vector2(7.810781f, 0.8393164f), new Vector2(7.810781f, 1.644375f), new Vector2(7.793652f, 1.849922f), new Vector2(7.725136f, 1.918437f), new Vector2(7.125625f, 1.952695f), new Vector2(7.074238f, 1.901309f), new Vector2(7.074238f, 1.592988f), new Vector2(7.125625f, 1.541601f), new Vector2(7.331172f, 1.40457f), new Vector2(7.03998f, 1.421699f), new Vector2(7.074238f, 1.764277f), new Vector2(7.074238f, 11.90459f), new Vector2(7.057109f, 11.99023f), new Vector2(7.03998f, 12.23004f), new Vector2(7.331172f, 12.12726f), new Vector2(7.125625f, 12.11014f), new Vector2(7.074238f, 12.05875f), new Vector2(7.074238f, 11.75043f), new Vector2(7.108496f, 11.68191f), new Vector2(7.519589f, 11.69904f), new Vector2(7.708007f, 11.7333f), new Vector2(7.793652f, 11.80182f), new Vector2(7.810781f, 12.04162f), new Vector2(7.810781f, 12.46984f), new Vector2(7.793652f, 12.79529f), new Vector2(7.759394f, 12.86381f), new Vector2(7.536718f, 12.88094f), new Vector2(6.577499f, 12.81242f), new Vector2(6.594628f, 13.51471f), new Vector2(6.577499f, 13.72025f), new Vector2(6.508984f, 13.75451f), new Vector2(5.755312f, 13.77164f), new Vector2(5.601152f, 13.77164f), new Vector2(5.532637f, 13.66887f), new Vector2(5.532637f, 13.155f), new Vector2(5.566894f, 13.12074f), new Vector2(5.858086f, 13.12074f), new Vector2(5.892344f, 13.155f), new Vector2(5.995117f, 13.08648f), new Vector2(5.721055f, 13.12074f), new Vector2(3.220234f, 13.12074f), new Vector2(3.048945f, 13.17213f), new Vector2(1.747148f, 13.155f), new Vector2(1.747148f, 13.12074f), new Vector2(-3.28875f, 13.12074f), new Vector2(-3.391523f, 13.10361f), new Vector2(-3.579941f, 13.18926f), new Vector2(-3.425781f, 13.12074f), new Vector2(-3.13459f, 13.12074f), new Vector2(-3.083203f, 13.13787f), new Vector2(-3.100332f, 13.70312f), new Vector2(-3.168848f, 13.77164f), new Vector2(-3.854004f, 13.77164f), new Vector2(-4.076679f, 13.75451f), new Vector2(-4.128066f, 13.72025f), new Vector2(-4.145195f, 13.53184f), new Vector2(-4.076679f, 12.624f), new Vector2(-4.110937f, 12.53836f), new Vector2(-4.573418f, 12.52123f), new Vector2(-6.508984f, 12.3842f), new Vector2(-6.800175f, 12.52123f), new Vector2(-6.902949f, 12.53836f), new Vector2(-7.091367f, 12.3842f), new Vector2(-7.519589f, 12.33281f), new Vector2(-7.536718f, 12.28143f), new Vector2(-8.13623f, 12.33281f), new Vector2(-8.119102f, 12.94945f), new Vector2(-8.119102f, 13.05223f), new Vector2(-8.204745f, 13.12074f), new Vector2(-8.872773f, 13.13787f), new Vector2(-9.472284f, 13.13787f), new Vector2(-9.5408f, 13.06936f), new Vector2(-9.557929f, 12.88094f), new Vector2(-9.557929f, 11.95598f), new Vector2(-10.14031f, 11.95598f), new Vector2(-9.951894f, 12.16152f), new Vector2(-9.934765f, 12.33281f), new Vector2(-9.917636f, 12.52123f), new Vector2(-9.917636f, 12.55549f), new Vector2(-9.986152f, 12.624f), new Vector2(-10.22596f, 12.65826f), new Vector2(-10.65418f, 12.64113f), new Vector2(-10.67131f, 12.58975f), new Vector2(-10.68844f, 12.29855f), new Vector2(-10.9625f, 11.64766f), new Vector2(-10.9625f, 11.92172f), new Vector2(-11.03102f, 11.95598f), new Vector2(-11.51062f, 11.9731f), new Vector2(-11.59627f, 11.87033f), new Vector2(-11.59627f, 11.30508f), new Vector2(-11.44211f, 11.23656f), new Vector2(-10.9625f, 11.25369f), new Vector2(-8.821386f, 11.2023f), new Vector2(-8.084844f, 11.18517f), new Vector2(-7.776523f, 11.11666f), new Vector2(-10.05467f, 11.16805f), new Vector2(-10.82547f, 11.18517f), new Vector2(-11.52775f, 11.18517f), new Vector2(-11.59627f, 11.15092f), new Vector2(-11.59627f, 10.53428f), new Vector2(-11.56201f, 10.50002f), new Vector2(-11.03102f, 10.48289f), new Vector2(-10.97963f, 10.58566f), new Vector2(-10.97963f, 10.79121f), new Vector2(-10.72269f, 10.63705f), new Vector2(-10.72269f, 9.831991f), new Vector2(-10.70557f, 9.797734f), new Vector2(-10.27734f, 9.729218f), new Vector2(-10.02041f, 9.780605f), new Vector2(-9.969023f, 9.814862f), new Vector2(-9.986152f, 10.26021f), new Vector2(-10.00328f, 10.39725f), new Vector2(-9.609316f, 10.38012f), new Vector2(-9.626445f, 9.318125f), new Vector2(-9.609316f, 9.23248f), new Vector2(-9.198222f, 9.198222f), new Vector2(-8.273261f, 9.198222f), new Vector2(-8.187616f, 9.249609f), new Vector2(-8.187616f, 9.900507f), new Vector2(-8.221874f, 9.934765f), new Vector2(-8.92416f, 9.969023f), new Vector2(-8.975547f, 9.900507f), new Vector2(-9.129706f, 9.677832f), new Vector2(-9.112577f, 10.00328f), new Vector2(-8.615839f, 10.02041f), new Vector2(-7.690878f, 10.00328f), new Vector2(-7.570976f, 9.917636f), new Vector2(-7.091367f, 9.900507f), new Vector2(-7.03998f, 9.797734f), new Vector2(-6.971465f, 9.694961f), new Vector2(-6.783047f, 9.643574f), new Vector2(-6.560371f, 9.797734f), new Vector2(-6.029375f, 9.797734f), new Vector2(-5.515508f, 9.746347f), new Vector2(-4.076679f, 9.660703f), new Vector2(-4.162324f, 8.752871f), new Vector2(-4.145195f, 8.581581f), new Vector2(-4.076679f, 8.547324f), new Vector2(-3.716972f, 8.530195f), new Vector2(-3.151719f, 8.530195f), new Vector2(-3.117461f, 8.615839f), new Vector2(-3.083203f, 8.838515f), new Vector2(-3.100332f, 9.163964f), new Vector2(2.603594f, 9.163964f), new Vector2(2.603594f, 4.796093f), new Vector2(-2.312402f, 4.796093f), new Vector2(-2.449434f, 4.778965f), new Vector2(-2.65498f, 4.744707f), new Vector2(-3.185976f, 4.641933f), new Vector2(-3.425781f, 4.53916f), new Vector2(-3.991035f, 4.282226f), new Vector2(-4.282226f, 4.042422f), new Vector2(-4.402129f, 4.487773f), new Vector2(-4.53916f, 4.590547f), new Vector2(-5.00164f, 4.590547f), new Vector2(-5.104414f, 4.556289f), new Vector2(-5.190058f, 4.522031f), new Vector2(-5.361347f, 4.419258f), new Vector2(-5.327089f, 4.128066f), new Vector2(-5.121542f, 3.528554f), new Vector2(-5.087285f, 3.528554f), new Vector2(-5.224316f, 1.935566f), new Vector2(-5.378476f, -0.2226758f), new Vector2(-5.63541f, -0.2740625f), new Vector2(-5.823828f, -0.188418f), new Vector2(-6.337695f, 0.03425781f), new Vector2(-6.440468f, 0.05138671f), new Vector2(-6.491855f, 0f), new Vector2(-6.526113f, -0.3939648f), new Vector2(-6.508984f, -1.318926f), new Vector2(-7.108496f, -1.318926f), new Vector2(-7.177011f, -1.353184f), new Vector2(-7.159883f, -1.507344f), new Vector2(-6.971465f, -2.209629f), new Vector2(-7.622363f, -2.278144f), new Vector2(-7.673749f, -2.34666f), new Vector2(-7.468203f, -2.80914f), new Vector2(-7.211269f, -3.254492f), new Vector2(-7.588105f, -3.44291f), new Vector2(-7.485332f, -3.648457f), new Vector2(-7.108496f, -4.008164f), new Vector2(-6.988594f, -4.093808f), new Vector2(-7.074238f, -4.265098f), new Vector2(-6.748789f, -4.641933f), new Vector2(-6.937207f, -4.84748f), new Vector2(-6.954336f, -4.881738f), new Vector2(-6.783047f, -5.035898f), new Vector2(-6.234921f, -5.361347f), new Vector2(-6.25205f, -5.686797f), new Vector2(-5.977988f, -5.806699f), new Vector2(-5.686797f, -5.858086f), new Vector2(-5.566894f, -5.977988f), new Vector2(-5.549765f, -6.063632f), new Vector2(-4.984511f, -6.149277f), new Vector2(-4.950253f, -6.714531f), new Vector2(-4.915996f, -6.971465f), new Vector2(-4.813222f, -7.536718f), new Vector2(-4.556289f, -8.101973f), new Vector2(-4.145195f, -9.438026f), new Vector2(-4.025293f, -9.677832f), new Vector2(-3.888262f, -9.797734f), new Vector2(-3.665586f, -9.866249f), new Vector2(-2.329531f, -9.866249f), new Vector2(-2.243887f, -9.831991f), new Vector2(-2.123984f, -9.780605f), new Vector2(-1.935566f, -9.506542f)
    };

    public Vector2[] pontosMordida = new Vector2[] {
        new Vector2(-2.238152f, -7.213003f), new Vector2(-2.004791f, -7.02207f), new Vector2(-1.941146f, -6.831138f), new Vector2(-1.665355f, -5.918905f), new Vector2(-1.410779f, -5.240035f), new Vector2(-1.177417f, -4.730881f), new Vector2(-1.156202f, -4.476305f), new Vector2(-1.071343f, -3.797434f), new Vector2(-1.050128f, -3.500428f), new Vector2(-0.8804107f, -3.479213f), new Vector2(-0.6682635f, -3.457999f), new Vector2(-0.5409752f, -3.394354f), new Vector2(-0.4561163f, -3.33071f), new Vector2(-0.4985458f, -3.118563f), new Vector2(-0.3924722f, -3.224637f), new Vector2(-0.09546621f, -3.182207f), new Vector2(0.1591104f, -3.076133f), new Vector2(0.2439692f, -3.033704f), new Vector2(0.1803251f, -2.800342f), new Vector2(0.3076133f, -2.673054f), new Vector2(0.8167665f, -2.333619f), new Vector2(0.9016253f, -2.227545f), new Vector2(0.9016253f, -2.163901f), new Vector2(0.7531223f, -1.88811f), new Vector2(1.007699f, -1.612318f), new Vector2(1.007699f, -1.548674f), new Vector2(0.9440548f, -1.463815f), new Vector2(1.071343f, -1.357742f), new Vector2(1.559281f, -0.8698033f), new Vector2(1.559281f, -0.7849444f), new Vector2(1.304705f, -0.6364414f), new Vector2(1.113773f, -0.594012f), new Vector2(1.219846f, -0.5303679f), new Vector2(1.516852f, -0.02121471f), new Vector2(1.665355f, 0.3394354f), new Vector2(1.580496f, 0.3606502f), new Vector2(0.9016253f, 0.4242943f), new Vector2(1.028914f, 0.8061591f), new Vector2(1.113773f, 1.12438f), new Vector2(1.134987f, 1.294098f), new Vector2(1.113773f, 1.357742f), new Vector2(0.7319077f, 1.336527f), new Vector2(0.3924722f, 1.272883f), new Vector2(0.4773311f, 1.357742f), new Vector2(0.4985458f, 1.972968f), new Vector2(0.5197605f, 2.121471f), new Vector2(0.4773311f, 2.503336f), new Vector2(0.5834047f, 2.736698f), new Vector2(1.198631f, 2.948845f), new Vector2(3.977759f, 3.903507f), new Vector2(4.020188f, 3.839863f), new Vector2(4.211121f, 3.755004f), new Vector2(4.465697f, 3.797434f), new Vector2(4.614201f, 3.861078f), new Vector2(4.720274f, 3.818649f), new Vector2(4.889992f, 3.670146f), new Vector2(5.186997f, 3.73379f), new Vector2(5.144568f, 3.415569f), new Vector2(5.186997f, 3.351925f), new Vector2(5.462789f, 3.267066f), new Vector2(5.802224f, 3.182207f), new Vector2(6.09923f, 3.076133f), new Vector2(6.629598f, 2.97006f), new Vector2(6.735672f, 3.160992f), new Vector2(6.82053f, 3.818649f), new Vector2(6.841745f, 4.09444f), new Vector2(6.799316f, 4.158084f), new Vector2(7.053893f, 4.264158f), new Vector2(6.990248f, 4.45509f), new Vector2(6.650813f, 5.240035f), new Vector2(6.438666f, 5.706758f), new Vector2(6.990248f, 8.698032f), new Vector2(7.690334f, 12.49547f), new Vector2(7.923696f, 13.76835f), new Vector2(8.029769f, 13.83199f), new Vector2(8.369205f, 13.78956f), new Vector2(8.602567f, 13.78956f), new Vector2(8.666211f, 13.87442f), new Vector2(8.75107f, 14.129f), new Vector2(8.835929f, 14.65937f), new Vector2(8.835929f, 14.8503f), new Vector2(8.75107f, 14.91394f), new Vector2(8.687426f, 14.93516f), new Vector2(8.34799f, 14.97759f), new Vector2(7.605475f, 15.06245f), new Vector2(7.308469f, 15.06245f), new Vector2(7.26604f, 14.95637f), new Vector2(7.22361f, 14.68058f), new Vector2(7.075107f, 14.82909f), new Vector2(7.096322f, 15.14731f), new Vector2(7.075107f, 15.25338f), new Vector2(7.032678f, 15.29581f), new Vector2(6.693243f, 15.31702f), new Vector2(6.120445f, 15.31702f), new Vector2(6.056801f, 15.25338f), new Vector2(6.035586f, 15.06245f), new Vector2(5.823439f, 14.87151f), new Vector2(5.802224f, 14.95637f), new Vector2(5.674936f, 15.10488f), new Vector2(5.526433f, 15.16852f), new Vector2(5.271856f, 15.16852f), new Vector2(5.144568f, 15.12609f), new Vector2(4.953636f, 15.12609f), new Vector2(4.762703f, 15.29581f), new Vector2(4.571771f, 15.25338f), new Vector2(4.486912f, 15.21095f), new Vector2(4.529342f, 15.4231f), new Vector2(4.529342f, 15.55039f), new Vector2(4.444483f, 15.61403f), new Vector2(4.25355f, 15.67767f), new Vector2(3.744397f, 15.80496f), new Vector2(3.447391f, 15.88982f), new Vector2(3.320103f, 15.93225f), new Vector2(3.065526f, 15.93225f), new Vector2(3.023097f, 15.88982f), new Vector2(2.917023f, 15.40188f), new Vector2(2.853379f, 15.02002f), new Vector2(2.853379f, 14.8503f), new Vector2(2.81095f, 14.82909f), new Vector2(2.216938f, 14.8503f), new Vector2(1.050128f, 14.87151f), new Vector2(1.028914f, 14.89273f), new Vector2(-0.1166809f, 14.93516f), new Vector2(-0.7106929f, 14.95637f), new Vector2(-1.856287f, 14.9988f), new Vector2(-2.4503f, 15.02002f), new Vector2(-2.577588f, 15.06245f), new Vector2(-2.577588f, 15.14731f), new Vector2(-2.598803f, 15.59282f), new Vector2(-2.620017f, 15.63524f), new Vector2(-2.683661f, 15.65646f), new Vector2(-2.938238f, 15.69889f), new Vector2(-3.574679f, 15.69889f), new Vector2(-3.617109f, 15.61403f), new Vector2(-3.638324f, 15.44431f), new Vector2(-3.617109f, 14.93516f), new Vector2(-3.595894f, 14.48965f), new Vector2(-3.956544f, 14.44722f), new Vector2(-4.911206f, 14.38358f), new Vector2(-5.42036f, 14.34115f), new Vector2(-6.162875f, 14.2775f), new Vector2(-6.184089f, 14.25629f), new Vector2(-6.714457f, 14.2775f), new Vector2(-6.926604f, 14.38358f), new Vector2(-7.032678f, 14.40479f), new Vector2(-7.138752f, 14.36236f), new Vector2(-7.329684f, 14.129f), new Vector2(-7.753978f, 14.17143f), new Vector2(-8.072199f, 14.17143f), new Vector2(-8.199487f, 14.08657f), new Vector2(-8.772285f, 14.08657f), new Vector2(-8.75107f, 14.65937f), new Vector2(-8.729855f, 14.89273f), new Vector2(-8.857143f, 14.93516f), new Vector2(-10.13003f, 14.93516f), new Vector2(-10.19367f, 14.8503f), new Vector2(-10.21488f, 14.17143f), new Vector2(-10.21488f, 13.76835f), new Vector2(-10.76647f, 13.91685f), new Vector2(-10.63918f, 13.91685f), new Vector2(-10.57553f, 13.95928f), new Vector2(-10.55432f, 14.40479f), new Vector2(-10.59675f, 14.42601f), new Vector2(-10.70282f, 14.44722f), new Vector2(-11.25441f, 14.44722f), new Vector2(-11.31805f, 14.38358f), new Vector2(-11.33926f, 14.02293f), new Vector2(-11.63627f, 13.76835f), new Vector2(-12.14542f, 13.81078f), new Vector2(-12.23028f, 13.66228f), new Vector2(-12.23028f, 13.11069f), new Vector2(-12.12421f, 13.06826f), new Vector2(-11.59384f, 13.04705f), new Vector2(-10.74525f, 13.02584f), new Vector2(-9.854235f, 13.00462f), new Vector2(-8.963217f, 13.00462f), new Vector2(-8.942002f, 12.98341f), new Vector2(-8.220702f, 12.96219f), new Vector2(-8.284346f, 12.91976f), new Vector2(-9.005646f, 12.94098f), new Vector2(-10.87254f, 12.96219f), new Vector2(-10.89376f, 12.98341f), new Vector2(-12.18785f, 12.98341f), new Vector2(-12.2515f, 12.96219f), new Vector2(-12.2515f, 12.36818f), new Vector2(-12.18785f, 12.30453f), new Vector2(-11.6787f, 12.28332f), new Vector2(-11.61506f, 12.36818f), new Vector2(-11.61506f, 12.58033f), new Vector2(-11.38169f, 12.32575f), new Vector2(-11.38169f, 11.64688f), new Vector2(-11.31805f, 11.58323f), new Vector2(-10.97861f, 11.56202f), new Vector2(-10.72404f, 11.56202f), new Vector2(-10.61796f, 11.60445f), new Vector2(-10.61796f, 12.07117f), new Vector2(-10.78768f, 12.1136f), new Vector2(-10.76647f, 12.19846f), new Vector2(-10.27853f, 12.15603f), new Vector2(-10.27853f, 11.11651f), new Vector2(-10.21488f, 11.03165f), new Vector2(-9.854235f, 11.01044f), new Vector2(-9.090505f, 10.98922f), new Vector2(-8.920788f, 10.98922f), new Vector2(-8.814713f, 11.03165f), new Vector2(-8.814713f, 11.71052f), new Vector2(-8.878358f, 11.73174f), new Vector2(-9.366297f, 11.77417f), new Vector2(-9.599658f, 11.77417f), new Vector2(-9.620872f, 11.71052f), new Vector2(-9.620872f, 11.49837f), new Vector2(-9.769376f, 11.49837f), new Vector2(-9.748161f, 11.83781f), new Vector2(-9.281438f, 11.8166f), new Vector2(-8.305561f, 11.8166f), new Vector2(-8.178272f, 11.68931f), new Vector2(-7.520616f, 11.68931f), new Vector2(-7.075107f, 11.56202f), new Vector2(-6.926604f, 11.56202f), new Vector2(-6.735672f, 11.66809f), new Vector2(-6.184089f, 11.68931f), new Vector2(-6.162875f, 11.66809f), new Vector2(-5.823439f, 11.64688f), new Vector2(-5.208212f, 11.60445f), new Vector2(-4.932421f, 11.58323f), new Vector2(-4.274765f, 11.5408f), new Vector2(-3.723182f, 11.49837f), new Vector2(-3.786827f, 11.07408f), new Vector2(-3.829256f, 10.50128f), new Vector2(-3.765612f, 10.45885f), new Vector2(-2.832164f, 10.39521f), new Vector2(-2.747305f, 10.5225f), new Vector2(-2.747305f, 10.75586f), new Vector2(-2.641232f, 11.01044f), new Vector2(-2.068435f, 11.01044f), new Vector2(-0.9228401f, 10.96801f), new Vector2(-0.3288281f, 10.94679f), new Vector2(0.8167665f, 10.90436f), new Vector2(1.410779f, 10.88315f), new Vector2(2.556373f, 10.86193f), new Vector2(2.76852f, 10.62857f), new Vector2(2.301796f, 8.104021f), new Vector2(2.216938f, 7.913088f), new Vector2(0.4985458f, 7.319077f), new Vector2(-2.259367f, 6.364414f), new Vector2(-3.1716f, 6.046194f), new Vector2(-3.553465f, 5.855261f), new Vector2(-3.786827f, 5.706758f), new Vector2(-4.147477f, 5.452181f), new Vector2(-4.677845f, 4.921813f), new Vector2(-4.805133f, 4.709667f), new Vector2(-4.953636f, 4.709667f), new Vector2(-5.208212f, 4.646022f), new Vector2(-5.356715f, 4.433875f), new Vector2(-5.335501f, 4.349016f), new Vector2(-5.186997f, 4.052011f), new Vector2(-5.293071f, 3.118563f), new Vector2(-5.356715f, 2.630625f), new Vector2(-5.399145f, 2.269974f), new Vector2(-5.78101f, 2.482121f), new Vector2(-6.184089f, 2.630625f), new Vector2(-6.396236f, 2.715483f), new Vector2(-6.438666f, 2.715483f), new Vector2(-6.50231f, 2.630625f), new Vector2(-6.523525f, 2.142686f), new Vector2(-6.523525f, 1.994183f), new Vector2(-6.50231f, 1.357742f), new Vector2(-6.438666f, 1.272883f), new Vector2(-6.778101f, 1.336527f), new Vector2(-7.117537f, 1.336527f), new Vector2(-7.159966f, 1.294098f), new Vector2(-7.159966f, 1.188024f), new Vector2(-7.075107f, 0.8698033f), new Vector2(-6.947819f, 0.4667237f), new Vector2(-6.884175f, 0.3818648f), new Vector2(-7.350898f, 0.3818648f), new Vector2(-7.605475f, 0.3606502f), new Vector2(-7.647904f, 0.3182207f), new Vector2(-7.62669f, 0.1909324f), new Vector2(-7.26604f, -0.5091531f), new Vector2(-7.287254f, -0.6364414f), new Vector2(-7.563046f, -0.7849444f), new Vector2(-7.563046f, -0.8485886f), new Vector2(-7.520616f, -0.9546621f), new Vector2(-7.117537f, -1.378956f), new Vector2(-6.947819f, -1.378956f), new Vector2(-7.053893f, -1.548674f), new Vector2(-7.053893f, -1.591104f), new Vector2(-6.756886f, -1.88811f), new Vector2(-6.650813f, -1.909324f), new Vector2(-6.926604f, -2.185116f), new Vector2(-6.926604f, -2.269974f), new Vector2(-6.756886f, -2.397263f), new Vector2(-6.565954f, -2.524551f), new Vector2(-6.247733f, -2.715483f), new Vector2(-6.205304f, -2.842772f), new Vector2(-6.247733f, -3.012489f), new Vector2(-6.205304f, -3.076133f), new Vector2(-5.78101f, -3.224637f), new Vector2(-5.568862f, -3.182207f), new Vector2(-5.568862f, -3.33071f), new Vector2(-5.505219f, -3.394354f), new Vector2(-5.250642f, -3.457999f), new Vector2(-4.974851f, -3.479213f), new Vector2(-4.974851f, -3.712575f), new Vector2(-4.953636f, -4.136869f), new Vector2(-4.911206f, -4.433875f), new Vector2(-4.826347f, -4.81574f), new Vector2(-4.677845f, -5.155176f), new Vector2(-4.550556f, -5.494611f), new Vector2(-4.25355f, -6.364414f), new Vector2(-4.126262f, -6.809923f), new Vector2(-4.020188f, -7.02207f), new Vector2(-3.914115f, -7.128144f), new Vector2(-3.744397f, -7.191788f), new Vector2(-3.638324f, -7.213003f), new Vector2(-3.447391f, -7.213003f), new Vector2(-3.107956f, -7.191788f), new Vector2(-2.662447f, -7.213003f)
    };

    public Vector2[] pontosIdle = new Vector2[] {
        new Vector2(-1.185432f, -10.68543f), new Vector2(-0.9538595f, -10.57516f), new Vector2(-0.8215322f, -10.34359f), new Vector2(-0.6230412f, -9.615786f), new Vector2(-0.3694138f, -8.854904f), new Vector2(-0.1709228f, -8.402785f), new Vector2(0.02756819f, -8.01683f), new Vector2(0.04962275f, -7.961694f), new Vector2(0.06065002f, -7.575739f), new Vector2(0.1268137f, -7.487521f), new Vector2(0.1929774f, -7.553685f), new Vector2(0.2811956f, -7.553685f), new Vector2(0.5127684f, -7.498549f), new Vector2(0.578932f, -7.45444f), new Vector2(0.5899593f, -7.388276f), new Vector2(0.5348229f, -7.200812f), new Vector2(0.7002321f, -7.300058f), new Vector2(0.733314f, -7.300058f), new Vector2(0.7774231f, -7.289031f), new Vector2(0.8105049f, -7.278003f), new Vector2(0.9759141f, -7.233894f), new Vector2(1.174405f, -7.178758f), new Vector2(1.273651f, -7.112594f), new Vector2(1.284678f, -7.057457f), new Vector2(1.174405f, -6.792803f), new Vector2(1.251596f, -6.781775f), new Vector2(1.538305f, -6.605339f), new Vector2(1.946314f, -6.285548f), new Vector2(1.968369f, -6.241439f), new Vector2(1.769878f, -5.998839f), new Vector2(1.791933f, -5.910621f), new Vector2(1.92426f, -5.778294f), new Vector2(2.056587f, -5.623911f), new Vector2(2.078642f, -5.535693f), new Vector2(2.023505f, -5.46953f), new Vector2(1.825014f, -5.381311f), new Vector2(2.133778f, -5.326175f), new Vector2(2.321242f, -5.171793f), new Vector2(2.563842f, -4.918166f), new Vector2(2.596924f, -4.752757f), new Vector2(2.343297f, -4.598375f), new Vector2(2.16686f, -4.521184f), new Vector2(2.277133f, -4.432966f), new Vector2(2.431515f, -4.135229f), new Vector2(2.696169f, -3.60592f), new Vector2(2.641033f, -3.517701f), new Vector2(2.056587f, -3.506674f), new Vector2(1.946314f, -3.429483f), new Vector2(2.111724f, -2.889147f), new Vector2(2.133778f, -2.800928f), new Vector2(2.188915f, -2.525247f), new Vector2(2.133778f, -2.481138f), new Vector2(1.869124f, -2.481138f), new Vector2(1.725769f, -2.492165f), new Vector2(1.549332f, -2.536274f), new Vector2(1.549332f, -1.98491f), new Vector2(1.538305f, -1.389437f), new Vector2(1.527278f, -1.168891f), new Vector2(1.516251f, -1.13581f), new Vector2(5.585316f, -1.13581f), new Vector2(5.739698f, -1.124782f), new Vector2(5.816889f, -1.113755f), new Vector2(6.037435f, -1.146837f), new Vector2(5.89408f, -1.37841f), new Vector2(5.89408f, -1.190946f), new Vector2(5.872025f, -1.146837f), new Vector2(5.541207f, -1.13581f), new Vector2(5.497098f, -1.179919f), new Vector2(5.53018f, -1.709228f), new Vector2(5.585316f, -1.775392f), new Vector2(6.169762f, -1.775392f), new Vector2(6.577771f, -1.764364f), new Vector2(6.62188f, -1.720255f), new Vector2(6.632907f, -1.554846f), new Vector2(6.588798f, -0.9593731f), new Vector2(6.721126f, -0.9924549f), new Vector2(7.493035f, -1.036564f), new Vector2(7.768717f, -1.036564f), new Vector2(7.823853f, -0.9924549f), new Vector2(7.834881f, -0.6175275f), new Vector2(7.834881f, -0.07719094f), new Vector2(7.724608f, -0.02205455f), new Vector2(7.250435f, -0.02205455f), new Vector2(7.206326f, -0.06616367f), new Vector2(7.184271f, -0.3639002f), new Vector2(7.34968f, -0.396982f), new Vector2(7.316598f, -0.4962275f), new Vector2(7.162217f, -0.4962275f), new Vector2(7.184271f, -0.2536274f), new Vector2(7.195299f, -0.0992455f), new Vector2(7.195299f, 8.66744f), new Vector2(7.184271f, 8.83285f), new Vector2(7.162217f, 9.053394f), new Vector2(7.437899f, 8.954149f), new Vector2(7.22838f, 8.954149f), new Vector2(7.195299f, 8.865932f), new Vector2(7.206326f, 8.634358f), new Vector2(7.22838f, 8.590249f), new Vector2(7.768717f, 8.601276f), new Vector2(7.834881f, 8.66744f), new Vector2(7.834881f, 9.295995f), new Vector2(7.823853f, 9.56065f), new Vector2(7.801799f, 9.615786f), new Vector2(7.548172f, 9.626813f), new Vector2(7.140162f, 9.615786f), new Vector2(7.195299f, 10.28845f), new Vector2(7.184271f, 10.47591f), new Vector2(7.151189f, 10.49797f), new Vector2(6.467498f, 10.49797f), new Vector2(6.423389f, 10.46489f), new Vector2(6.401335f, 10.00174f), new Vector2(6.456471f, 9.979686f), new Vector2(6.577771f, 9.979686f), new Vector2(6.654962f, 9.990713f), new Vector2(6.677016f, 10.01277f), new Vector2(6.765235f, 10.17818f), new Vector2(6.765235f, 9.957631f), new Vector2(6.544689f, 9.979686f), new Vector2(2.817469f, 9.979686f), new Vector2(2.806442f, 9.990713f), new Vector2(1.527278f, 9.990713f), new Vector2(1.516251f, 9.979686f), new Vector2(-0.1598955f, 9.979686f), new Vector2(-0.2260592f, 9.968658f), new Vector2(-0.2701683f, 9.957631f), new Vector2(-0.3914683f, 9.957631f), new Vector2(-0.3032501f, 10.17818f), new Vector2(-0.3032501f, 10.0238f), new Vector2(-0.2701683f, 9.990713f), new Vector2(-0.1047591f, 9.979686f), new Vector2(-0.08270458f, 9.979686f), new Vector2(-0.01654092f, 10.01277f), new Vector2(-0.03859547f, 10.4318f), new Vector2(-0.08270458f, 10.49797f), new Vector2(-0.7774231f, 10.49797f), new Vector2(-0.8105049f, 10.46489f), new Vector2(-0.7994776f, 10.0238f), new Vector2(-0.7663958f, 9.726059f), new Vector2(-0.8987231f, 9.726059f), new Vector2(-2.012478f, 9.63784f), new Vector2(-2.519733f, 9.593731f), new Vector2(-2.839524f, 9.593731f), new Vector2(-3.026988f, 9.626813f), new Vector2(-3.170342f, 9.737086f), new Vector2(-3.269588f, 9.737086f), new Vector2(-3.401915f, 9.63784f), new Vector2(-3.42397f, 9.56065f), new Vector2(-4.085606f, 9.56065f), new Vector2(-4.217934f, 9.538595f), new Vector2(-4.251016f, 9.516541f), new Vector2(-4.262043f, 9.494486f), new Vector2(-5.441961f, 9.494486f), new Vector2(-5.662507f, 9.505513f), new Vector2(-5.838943f, 9.483459f), new Vector2(-5.838943f, 9.814277f), new Vector2(-5.684562f, 9.814277f), new Vector2(-5.684562f, 9.527568f), new Vector2(-5.662507f, 9.505513f), new Vector2(-5.232443f, 9.494486f), new Vector2(-4.912652f, 9.494486f), new Vector2(-4.87957f, 9.582705f), new Vector2(-4.901625f, 9.593731f), new Vector2(-4.890597f, 10.29948f), new Vector2(-4.945734f, 10.33256f), new Vector2(-6.280035f, 10.33256f), new Vector2(-6.335171f, 10.25537f), new Vector2(-6.324143f, 9.119558f), new Vector2(-6.654962f, 9.119558f), new Vector2(-6.919617f, 9.295995f), new Vector2(-6.765235f, 9.295995f), new Vector2(-6.732153f, 9.307022f), new Vector2(-6.710098f, 9.759141f), new Vector2(-6.74318f, 9.792222f), new Vector2(-7.173244f, 9.803249f), new Vector2(-7.426871f, 9.781195f), new Vector2(-7.459953f, 9.748114f), new Vector2(-7.482008f, 9.483459f), new Vector2(-7.470981f, 8.777713f), new Vector2(-7.746663f, 9.009286f), new Vector2(-7.801799f, 9.086476f), new Vector2(-8.320081f, 9.075449f), new Vector2(-8.375217f, 9.009286f), new Vector2(-8.375217f, 8.435867f), new Vector2(-8.309053f, 8.391758f), new Vector2(-4.262043f, 8.391758f), new Vector2(-4.372315f, 8.292513f), new Vector2(-8.298026f, 8.292513f), new Vector2(-8.375217f, 8.248404f), new Vector2(-8.375217f, 7.663958f), new Vector2(-8.36419f, 7.630876f), new Vector2(-8.264944f, 7.586767f), new Vector2(-7.801799f, 7.597794f), new Vector2(-7.746663f, 7.674985f), new Vector2(-7.757689f, 7.917585f), new Vector2(-7.470981f, 7.708067f), new Vector2(-7.482008f, 7.189785f), new Vector2(-7.459953f, 6.92513f), new Vector2(-7.437899f, 6.903076f), new Vector2(-6.952698f, 6.881021f), new Vector2(-6.820371f, 6.892048f), new Vector2(-6.754208f, 6.903076f), new Vector2(-6.710098f, 6.92513f), new Vector2(-6.721126f, 7.377249f), new Vector2(-6.897562f, 7.41033f), new Vector2(-6.886535f, 7.542658f), new Vector2(-6.533662f, 7.531631f), new Vector2(-6.324143f, 7.531631f), new Vector2(-6.324143f, 6.43993f), new Vector2(-6.313117f, 6.406848f), new Vector2(-6.269007f, 6.362739f), new Vector2(-6.01538f, 6.351712f), new Vector2(-4.945734f, 6.351712f), new Vector2(-4.890597f, 6.395821f), new Vector2(-4.890597f, 6.506094f), new Vector2(-4.901625f, 7.04643f), new Vector2(-4.912652f, 7.079512f), new Vector2(-5.65148f, 7.079512f), new Vector2(-5.718219f, 6.814857f), new Vector2(-6.328429f, 6.295153f), new Vector2(-5.24347f, 7.16773f), new Vector2(-5.24347f, 7.189785f), new Vector2(-4.251016f, 7.189785f), new Vector2(-4.27307f, 7.134648f), new Vector2(-4.162797f, 7.079512f), new Vector2(-3.479106f, 7.112594f), new Vector2(-3.291642f, 6.903076f), new Vector2(-3.104179f, 6.92513f), new Vector2(-2.905688f, 7.04643f), new Vector2(-2.685142f, 7.035403f), new Vector2(-2.310215f, 7.035403f), new Vector2(-1.064132f, 6.947185f), new Vector2(-0.7774231f, 6.903076f), new Vector2(-0.7994776f, 6.583285f), new Vector2(-0.7994776f, 6.31863f), new Vector2(-0.7774231f, 6.274521f), new Vector2(-0.08270458f, 6.252466f), new Vector2(-0.03859547f, 6.340684f), new Vector2(-0.02756819f, 6.737667f), new Vector2(-0.06065002f, 6.781775f), new Vector2(-0.2701683f, 6.781775f), new Vector2(-0.3032501f, 6.748694f), new Vector2(-0.3914683f, 6.594312f), new Vector2(-0.3804411f, 6.814857f), new Vector2(-0.08270458f, 6.781775f), new Vector2(3.269588f, 6.781775f), new Vector2(3.269588f, 2.789901f), new Vector2(-0.08270458f, 2.789901f), new Vector2(-0.1157864f, 2.811956f), new Vector2(-0.2150319f, 2.856065f), new Vector2(-0.4245502f, 2.856065f), new Vector2(-0.578932f, 2.822983f), new Vector2(-0.6340684f, 2.789901f), new Vector2(-1.648578f, 2.789901f), new Vector2(-1.747823f, 2.778874f), new Vector2(-2.012478f, 2.734765f), new Vector2(-2.255078f, 2.690656f), new Vector2(-2.607951f, 2.59141f), new Vector2(-2.949797f, 2.437028f), new Vector2(-3.192397f, 2.34881f), new Vector2(-3.346779f, 2.271619f), new Vector2(-3.346779f, 2.492165f), new Vector2(-3.368833f, 2.701683f), new Vector2(-3.434997f, 2.811956f), new Vector2(-3.534242f, 2.845038f), new Vector2(-3.820952f, 2.856065f), new Vector2(-4.074579f, 2.789901f), new Vector2(-4.206906f, 2.635519f), new Vector2(-4.15177f, 2.448056f), new Vector2(-3.997388f, 2.205456f), new Vector2(-3.986361f, 1.852583f), new Vector2(-4.041497f, 0.7608821f), new Vector2(-4.129715f, 0.1543819f), new Vector2(-4.295125f, -1.157864f), new Vector2(-4.306152f, -1.510737f), new Vector2(-5.100116f, -1.113755f), new Vector2(-5.342716f, -1.025537f), new Vector2(-5.40888f, -1.003482f), new Vector2(-5.475043f, -1.235055f), new Vector2(-5.464016f, -1.86361f), new Vector2(-5.452989f, -2.437028f), new Vector2(-5.419907f, -2.525247f), new Vector2(-5.640452f, -2.492165f), new Vector2(-5.783807f, -2.481138f), new Vector2(-6.037435f, -2.481138f), new Vector2(-6.092571f, -2.525247f), new Vector2(-6.103598f, -2.580383f), new Vector2(-6.037435f, -2.878119f), new Vector2(-5.838943f, -3.495647f), new Vector2(-6.37928f, -3.506674f), new Vector2(-6.533662f, -3.528729f), new Vector2(-6.577771f, -3.561811f), new Vector2(-6.588798f, -3.594893f), new Vector2(-6.390307f, -4.035984f), new Vector2(-6.213871f, -4.388856f), new Vector2(-6.103598f, -4.532211f), new Vector2(-6.511607f, -4.730702f), new Vector2(-6.456471f, -4.885084f), new Vector2(-6.368253f, -5.006384f), new Vector2(-6.037435f, -5.315148f), new Vector2(-5.783807f, -5.370284f), new Vector2(-5.938189f, -5.46953f), new Vector2(-6.01538f, -5.579803f), new Vector2(-5.717643f, -5.899593f), new Vector2(-5.574289f, -5.932675f), new Vector2(-5.89408f, -6.208357f), new Vector2(-5.805861f, -6.373766f), new Vector2(-5.883011f, -6.391938f), new Vector2(-5.419907f, -6.783017f), new Vector2(-5.100116f, -6.825885f), new Vector2(-5.177307f, -7.035403f), new Vector2(-5.177307f, -7.079512f), new Vector2(-5.155252f, -7.134648f), new Vector2(-5.136325f, -7.304562f), new Vector2(-4.692107f, -7.289031f), new Vector2(-4.603889f, -7.300058f), new Vector2(-4.493616f, -7.443412f), new Vector2(-4.460534f, -7.498549f), new Vector2(-4.195879f, -7.564713f), new Vector2(-4.107661f, -7.553685f), new Vector2(-3.953279f, -7.531631f), new Vector2(-3.953279f, -8.01683f), new Vector2(-3.743761f, -8.42484f), new Vector2(-3.534242f, -8.876958f), new Vector2(-3.379861f, -9.373186f), new Vector2(-3.148288f, -10.07893f), new Vector2(-3.115206f, -10.27742f), new Vector2(-3.004933f, -10.48694f), new Vector2(-2.872606f, -10.60824f), new Vector2(-2.696169f, -10.68543f), new Vector2(-2.40946f, -10.68543f), new Vector2(-2.111724f, -10.66338f), new Vector2(-2.001451f, -10.66338f), new Vector2(-1.494196f, -10.68543f)
    };

    private float arenaLeft = -15.05f;
    private float arenaRight = 14.95f;
    private float arenaBottom = -10f;
    private float arenaTop = 10f;

    private Vector3 originPosition;
    private bool isAttacking = false;
    private bool isStunned = false;

    public bool IsAttacking() => isAttacking;
    public bool IsStunned() => isStunned;

    [Header("Aviso Visual de Ataque")]
    public GameObject avisoAtaquePrefab;
    public float tempoAviso = 0.6f;

    void Start()
    {
        originPosition = transform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            corOriginal = spriteRenderer.color;

        HitboxParaIdle();
    }

    void SetEstadoAnimacao(int estado)
    {
        if (animator != null)
            animator.SetInteger("estadoBoss", estado);
    }

    public void AtualizarHitbox(Vector2[] novosPontos)
    {
        if (bossCollider == null || novosPontos == null || novosPontos.Length == 0) return;
        bossCollider.points = novosPontos;
    }

    public void HitboxParaIdle() => AtualizarHitbox(pontosIdle);
    public void HitboxParaMordida() => AtualizarHitbox(pontosMordida);
    public void HitboxParaVoando() => AtualizarHitbox(pontosVoando);
    public void HitboxParaTornado() => AtualizarHitbox(pontosTornado);

    [ContextMenu("Debug - Imprimir Pontos Atuais")]
    void ImprimirPontosAtuais()
    {
        if (bossCollider == null) return;
        string resultado = "";
        foreach (Vector2 p in bossCollider.points)
            resultado += $"new Vector2({p.x}f, {p.y}f), ";
        Debug.Log(resultado);
    }

    [ContextMenu("Debug - Forçar Hitbox Voando")]
    void DebugForcarHitboxVoando()
    {
        Debug.Log($"[DEBUG] bossCollider é null? {bossCollider == null}");
        Debug.Log($"[DEBUG] pontosVoando tem {pontosVoando.Length} pontos");

        AtualizarHitbox(pontosVoando);

        Debug.Log($"[DEBUG] Depois de aplicar, o collider tem {bossCollider.points.Length} pontos");
    }

    public void AttackTornado()
    {
        if (isAttacking || isStunned || tornadoPrefab == null) return;
        StartCoroutine(TornadoCoroutine());
    }

    IEnumerator TornadoCoroutine()
    {
        isAttacking = true;
        SetEstadoAnimacao(1);
        HitboxParaTornado();

        for (int i = 0; i < tornadoCount; i++)
        {
            Vector2 spawnOffset = Random.insideUnitCircle.normalized * tornadoSpawnRadius;
            Vector3 spawnPos = transform.position + (Vector3)spawnOffset;

            float size = Random.Range(tornadoMinSize, tornadoMaxSize);
            float t = Mathf.InverseLerp(tornadoMinSize, tornadoMaxSize, size);
            float speed = Mathf.Lerp(tornadoMaxSpeed, tornadoMinSpeed, t);

            Vector2 direction = (player.position - spawnPos).normalized;

            GameObject tornado = Instantiate(tornadoPrefab, spawnPos, Quaternion.identity);
            tornado.transform.localScale = Vector3.one * size;

            TornadoProjectile proj = tornado.GetComponent<TornadoProjectile>();
            if (proj != null) proj.Init(direction, speed, tornadoDamage);

            yield return new WaitForSeconds(0.3f);
        }

        SetEstadoAnimacao(0);
        HitboxParaIdle();
        isAttacking = false;
    }

    public void AttackBite()
    {
        if (isAttacking || isStunned) return;
        StartCoroutine(BiteCoroutine());
    }

    IEnumerator BiteCoroutine()
    {
        isAttacking = true;
        SetEstadoAnimacao(2);
        HitboxParaMordida();

        Vector3 targetPos = player.position;
        Vector3 startPos = transform.position;
        Vector3 direction = (targetPos - startPos).normalized;
        transform.up = -direction;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySerpenteAviso();

        float distanceTraveled = 0f;
        float totalDistance = Vector3.Distance(startPos, targetPos);
        bool hitPlayer = false;

        while (distanceTraveled < totalDistance)
        {
            float step = biteDashSpeed * Time.deltaTime;
            transform.position += direction * step;
            distanceTraveled += step;

            Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.5f);
            if (hit != null && hit.CompareTag("Player"))
            {
                hitPlayer = true;
                Player p = hit.GetComponent<Player>();
                if (p != null) p.TakeDamage((int)biteDamage);
                break;
            }
            yield return null;
        }

        if (!hitPlayer)
        {
            missedBitesCount++;
            Debug.Log($"Serpente errou o bote! Erros acumulados: {missedBitesCount}/3");

            if (missedBitesCount >= 3)
            {
                Debug.Log("Serpente bateu a cabeça e está DESNORTEADA!");
                isStunned = true;
                //IniciarStunVisual();
                SetEstadoAnimacao(0);
                HitboxParaIdle();
                yield return new WaitForSeconds(biteStunDuration);
                isStunned = false;
                //PararStunVisual();
                missedBitesCount = 0;
                isAttacking = false;
                yield break;
            }
        }

        SetEstadoAnimacao(0);
        HitboxParaIdle();
        isAttacking = false;
    }

    public void AttackDashThrough(float healthPercent)
    {
        if (isAttacking || isStunned) return;
        StartCoroutine(DashThroughCoroutine(healthPercent));
    }

    IEnumerator DashThroughCoroutine(float healthPercent)
    {
        isAttacking = true;
        SetEstadoAnimacao(3);
        HitboxParaVoando();

        int minPasses = 5, maxPasses = 7;
        if (healthPercent > 0.66f) { minPasses = 3; maxPasses = 5; }
        else if (healthPercent > 0.33f) { minPasses = 4; maxPasses = 6; }

        int passes = Random.Range(minPasses, maxPasses + 1);

        for (int i = 0; i < passes; i++)
        {
            Vector2 enterDirection = GetRandomAllDirections();
            Vector3 entryPoint = GetOffscreenPosition(enterDirection);
            Vector3 exitPoint = GetOffscreenPosition(-enterDirection);
            Vector3 avisoPos = GetBordaArenaPosition(enterDirection);

            if (avisoAtaquePrefab != null)
            {
                Vector3 direcaoAtaque = (exitPoint - entryPoint).normalized;
                float angulo = Mathf.Atan2(direcaoAtaque.y, direcaoAtaque.x) * Mathf.Rad2Deg;

                Quaternion rotacaoAviso = Quaternion.Euler(0, 0, angulo + 180f);

                // Instancia o aviso
                GameObject aviso = Instantiate(avisoAtaquePrefab, avisoPos, rotacaoAviso);
                Destroy(aviso, tempoAviso);
            }

            DispararAnimacaoFolhas(enterDirection);

            yield return new WaitForSeconds(tempoAviso);

            transform.position = entryPoint;
            Vector3 direction = (exitPoint - entryPoint).normalized;
            float totalDist = Vector3.Distance(entryPoint, exitPoint);
            transform.up = -direction;

            float traveled = 0f;
            while (traveled < totalDist)
            {
                float step = dashOutSpeed * Time.deltaTime;
                transform.position += direction * step;
                traveled += step;

                Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.5f);
                if (hit != null && hit.CompareTag("Player"))
                {
                    Player p = hit.GetComponent<Player>();
                    if (p != null) p.TakeDamage((int)dashOutDamage);
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
        }

        transform.position = originPosition;
        transform.rotation = Quaternion.identity;

        Debug.Log("Serpente terminou os mergulhos e está EXAUSTA!");
        isStunned = true;
        //IniciarStunVisual();
        SetEstadoAnimacao(0);
        HitboxParaIdle();

        yield return new WaitForSeconds(dashTiredDuration);
        isStunned = false;
        //PararStunVisual();

        isAttacking = false;
    }

    Vector3 GetBordaArenaPosition(Vector2 dir)
    {
        float targetX = (arenaLeft + arenaRight) / 2f;
        float targetY = (arenaTop + arenaBottom) / 2f;

        if (dir.x > 0.1f) targetX = arenaRight - 1.5f;
        else if (dir.x < -0.1f) targetX = arenaLeft + 1.5f;

        if (dir.y > 0.1f) targetY = arenaTop - 1.5f;
        else if (dir.y < -0.1f) targetY = arenaBottom + 1.5f;

        return new Vector3(targetX, targetY, 0f);
    }

    Vector2 GetRandomAllDirections()
    {
        Vector2[] directions = {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
            new Vector2(1, 1).normalized, new Vector2(-1, 1).normalized,
            new Vector2(1, -1).normalized, new Vector2(-1, -1).normalized
        };
        return directions[Random.Range(0, directions.Length)];
    }

    Vector3 GetOffscreenPosition(Vector2 dir)
    {
        float centerX = (arenaLeft + arenaRight) / 2f;
        float centerY = (arenaTop + arenaBottom) / 2f;

        float targetX = centerX;
        float targetY = centerY;

        if (dir.x > 0.1f) targetX = arenaRight + offscreenOffset;
        else if (dir.x < -0.1f) targetX = arenaLeft - offscreenOffset;

        if (dir.y > 0.1f) targetY = arenaTop + offscreenOffset;
        else if (dir.y < -0.1f) targetY = arenaBottom - offscreenOffset;

        return new Vector3(targetX, targetY, 0f);
    }

    void DispararAnimacaoFolhas(Vector2 direcaoEntrada)
    {
        Debug.Log($"[VFX] Balançar árvores na direção: {direcaoEntrada}");
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySerpenteAviso();
    }

    /*
    void IniciarStunVisual()
    {
        if (stunBlinkCoroutine != null)
            StopCoroutine(stunBlinkCoroutine);
        stunBlinkCoroutine = StartCoroutine(BlinkVermelho());
    }

    
    void PararStunVisual()
    {
        if (stunBlinkCoroutine != null)
        {
            StopCoroutine(stunBlinkCoroutine);
            stunBlinkCoroutine = null;
        }
        if (spriteRenderer != null)
            spriteRenderer.color = corOriginal;
    }
    */

    IEnumerator BlinkVermelho()
    {
        while (true)
        {
            if (spriteRenderer != null) spriteRenderer.color = corStun;
            yield return new WaitForSeconds(velocidadePiscada);
            if (spriteRenderer != null) spriteRenderer.color = corOriginal;
            yield return new WaitForSeconds(velocidadePiscada);
        }
    }
}