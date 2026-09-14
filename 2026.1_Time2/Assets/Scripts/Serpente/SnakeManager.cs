using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    [Header("Distâncias Personalizadas")]
    [SerializeField] float distanciaPadrao = 0.2f; // Distância do corpo
    [SerializeField] float distanciaCabeca = 0.4f; // MAIOR: Entre a 1ª (cabeça) e a 2ª peça
    [SerializeField] float distanciaCauda = 0.1f;  // MENOR: Entre o antepenúltimo, penúltimo e último

    [Header("Configurações de Movimento")]
    [SerializeField] public float speed;
    [SerializeField] public float rotationSpeed;
    
    [Header("Arraste as partes do corpo para cá no Inspector")]
    [SerializeField] List<GameObject> bodyPartsPrefabs = new List<GameObject>(); 
    
    private List<GameObject> pendingBodyParts = new List<GameObject>();
    private List<GameObject> snakeBody = new List<GameObject>();
    
    private int totalPartsNoInicio; 

    float countUp = 0;

    void Start()
    {
        pendingBodyParts = new List<GameObject>(bodyPartsPrefabs);
        totalPartsNoInicio = bodyPartsPrefabs.Count;
        CreateBodyParts();
    }

    void FixedUpdate()
    {
        if (pendingBodyParts.Count > 0)
        {
            CreateBodyParts();
        }
        SnakeMovement();
    }

    void SnakeMovement()
    {
        if (snakeBody.Count == 0) return;

        snakeBody[0].GetComponent<Rigidbody2D>().velocity = snakeBody[0].transform.right * speed * Time.deltaTime;
        

        snakeBody[0].transform.Rotate(new Vector3(0, 0, -1 * rotationSpeed * Time.deltaTime));

        if(snakeBody.Count > 1)
        {
            for(int i = 1; i < snakeBody.Count; i++)
            {
                MarkerManager markM = snakeBody[i - 1].GetComponent<MarkerManager>();
                
                if (markM.markerList.Count > 0)
                {
                    snakeBody[i].transform.position = markM.markerList[0].position;
                    snakeBody[i].transform.rotation = markM.markerList[0].rotation;
                    markM.markerList.RemoveAt(0);
                }
            }
        }
    }

    // NOVA FUNÇÃO: Calcula a distância (tempo de espera) baseado em qual parte está nascendo
    float CalcularDistanciaNecessaria()
    {
        int indexAtual = snakeBody.Count; // A parte que vai nascer agora (1 = segunda parte, 2 = terceira...)

        // Se for a segunda peça (logoo depois da cabeça)
        if (indexAtual == 1)
        {
            return distanciaCabeca; 
        }
        // Se for a penúltima ou a última peça a nascer
        else if (indexAtual >= totalPartsNoInicio - 2)
        {
            return distanciaCauda;
        }
        
        // Qualquer outra peça no meio do corpo
        return distanciaPadrao;
    }

    void CreateBodyParts()
    {
        if (snakeBody.Count == 0)
        {
            Debug.Log("Creating first body part");
            GameObject temp1 = Instantiate(pendingBodyParts[0], transform.position, transform.rotation, transform);
            
            if(!temp1.GetComponent<MarkerManager>()) temp1.AddComponent<MarkerManager>();
            
            if(!temp1.GetComponent<Rigidbody2D>())
            {
                temp1.AddComponent<Rigidbody2D>();
                temp1.GetComponent<Rigidbody2D>().gravityScale = 0;
            }
            
            snakeBody.Add(temp1);
            pendingBodyParts.RemoveAt(0);
            return;
        }

        MarkerManager markM = snakeBody[snakeBody.Count - 1].GetComponent<MarkerManager>();
        
        if(countUp == 0)
        {
            markM.clearMarkerList();
        }
        
        countUp += Time.deltaTime;
        
        // Usa a nova função matemática para pegar a distância exata para ESSE segmento
        float distanciaNecessaria = CalcularDistanciaNecessaria();
        
        if(countUp >= distanciaNecessaria && pendingBodyParts.Count > 0)
        {
            if (markM.markerList.Count == 0) return;

            GameObject temp = Instantiate(pendingBodyParts[0], markM.markerList[0].position, markM.markerList[0].rotation, transform);
            
            if(!temp.GetComponent<MarkerManager>()) temp.AddComponent<MarkerManager>();
            
            if(!temp.GetComponent<Rigidbody2D>())
            {
                temp.AddComponent<Rigidbody2D>();
                temp.GetComponent<Rigidbody2D>().gravityScale = 0;
            }
            
            snakeBody.Add(temp);
            pendingBodyParts.RemoveAt(0); 
            temp.GetComponent<MarkerManager>().clearMarkerList();
            countUp = 0;
        }
    }
}