using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Referências")]
    public Transform target;

    [Header("Configurações")]
    public float smoothness = 5f; // Quão suave a câmera acompanha
    public Vector3 offset = new Vector3(0f, 0f, -10f); // deslocamento da câmera 

    [Header("Limites do Mapa")]
    public bool usarLimites = true; // Permite ativar/desativar os limites facilmente
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    void FixedUpdate()
    {
        if (target == null) return;

        // Calcula a posição ideal para onde a câmera deveria ir
        Vector3 targetPosition = target.position + offset;

        // Se a opção de limites estiver ativa, restringe o X e o Y da posição ideal
        if (usarLimites)
        {
            float clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);
            float clampedY = Mathf.Clamp(targetPosition.y, minY, maxY);

            // Atualiza a posição ideal com os valores limitados (mantendo o Z do offset)
            targetPosition = new Vector3(clampedX, clampedY, targetPosition.z);
        }

        // Interpola suavemente da posição atual da câmera para a posição ideal (já limitada)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothness * Time.deltaTime);

        // Aplica a nova posição à câmera
        transform.position = smoothedPosition;
    }
}