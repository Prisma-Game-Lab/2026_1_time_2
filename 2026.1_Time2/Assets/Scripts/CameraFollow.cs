using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Referências")]
    public Transform target;

    [Header("Configurações")]
    public float smoothness = 5f; // Quão suave a câmera acompanha
    public Vector3 offset = new Vector3(0f, 0f, -10f); // deslocamento da câmera 

    void FixedUpdate()
    {
        if (target == null) return;

        // Calcula a posição ideal para onde a câmera deveria ir
        Vector3 targetPosition = target.position + offset;

        // Interpola suavemente da posição atual da câmera para a posição ideal
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothness * Time.deltaTime);

        // Aplica a nova posição à câmera
        transform.position = smoothedPosition;
    }
}