using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TornadoProjectile : MonoBehaviour
{
    [HideInInspector] public float speed;
    [HideInInspector] public float damage;
    public float lifetime = 3f;

    [Header("Visual")]
    public Sprite[] variacoesTornado; // arraste os 3 sprites fatiados aqui
    public float velocidadeRotacao = 180f; // graus por segundo, dá a sensação de giro

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

        // Escolhe uma das 3 variações aleatoriamente
        if (variacoesTornado != null && variacoesTornado.Length > 0)
        {
            spriteRenderer.sprite = variacoesTornado[Random.Range(0, variacoesTornado.Length)];
        }

        AjustarHitboxAoSprite();

        Destroy(gameObject, lifetime);
    }

    void AjustarHitboxAoSprite()
    {
        if (circleCollider == null || spriteRenderer == null || spriteRenderer.sprite == null) return;

        // Pega o raio real do sprite (considerando a escala do objeto) e aplica o multiplicador
        float raioBase = spriteRenderer.sprite.bounds.extents.x; // extents já é metade da largura, em unidades locais
        circleCollider.radius = raioBase * multiplicadorHitbox;

        // Centraliza o collider verticalmente no "corpo" do funil, não no topo largo do tornado
        circleCollider.offset = new Vector2(0f, spriteRenderer.sprite.bounds.center.y * 0.3f);
    }

    void Update()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
        transform.Rotate(0f, 0f, velocidadeRotacao * Time.deltaTime);
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