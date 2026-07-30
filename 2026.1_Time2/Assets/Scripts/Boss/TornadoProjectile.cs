using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TornadoProjectile : MonoBehaviour
{
    [HideInInspector] public float speed;
    [HideInInspector] public float damage;
    public float lifetime = 3f;

    [Header("Visual - Animação")]
    public Sprite[] framesAnimacao; // arraste os 3 frames fatiados aqui, na ordem
    public float tempoEntreFrames = 0.1f; // controla a velocidade do "giro"

    [Header("Hitbox")]
    [Range(0.1f, 1f)]
    public float multiplicadorHitbox = 0.5f; // reduz a hitbox em relação ao sprite (tornado tem muito espaço vazio/transparente nas bordas)

    private float arenaLeft = -15.05f;
    private float arenaRight = 14.95f;
    private float arenaBottom = -10f;
    private float arenaTop = 10f;

    private Vector2 moveDirection;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void Init(Vector2 direction, float spd, float dmg)
    {
        moveDirection = direction.normalized;
        speed = spd;
        damage = dmg;

        if (framesAnimacao != null && framesAnimacao.Length > 0)
        {
            spriteRenderer.sprite = framesAnimacao[0];
            AjustarHitboxAoSprite();
            StartCoroutine(AnimarFrames());
        }

        Destroy(gameObject, lifetime);
    }

    IEnumerator AnimarFrames()
    {
        int indiceAtual = 0;
        while (true)
        {
            spriteRenderer.sprite = framesAnimacao[indiceAtual];
            AjustarHitboxAoSprite();
            indiceAtual = (indiceAtual + 1) % framesAnimacao.Length;
            yield return new WaitForSeconds(tempoEntreFrames);
        }
    }

    void AjustarHitboxAoSprite()
    {
        if (circleCollider == null || spriteRenderer == null || spriteRenderer.sprite == null) return;

        float raioBase = spriteRenderer.sprite.bounds.extents.x;
        circleCollider.radius = raioBase * multiplicadorHitbox;
        circleCollider.offset = new Vector2(0f, spriteRenderer.sprite.bounds.center.y * 0.3f);
    }

    void Update()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
        Ricochet();
    }

    void Ricochet()
    {
        Vector3 pos = transform.position;
        if (pos.x <= arenaLeft || pos.x >= arenaRight)
        {
            moveDirection.x *= -1;
            pos.x = Mathf.Clamp(pos.x, arenaLeft, arenaRight);
            transform.position = pos;
        }
        if (pos.y <= arenaBottom || pos.y >= arenaTop)
        {
            moveDirection.y *= -1;
            pos.y = Mathf.Clamp(pos.y, arenaBottom, arenaTop);
            transform.position = pos;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage((int)damage);
            }
        }
    }
}