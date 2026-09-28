using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    [Header("Distâncias Personalizadas")]
    [SerializeField] float distanciaPadrao = 0.2f; 
    [SerializeField] float distanciaCabeca = 0.4f; 
    [SerializeField] float distanciaCauda = 0.1f;

    [Header("Configurações de Movimento")]
    [SerializeField] public float speed;
    [SerializeField] public float rotationSpeed;
    private float baseSpeed;
    
    [Header("Partes Do Corpo da Cobra")]
    [SerializeField] List<GameObject> bodyPartsPrefabs = new List<GameObject>(); 
    
    private List<GameObject> pendingBodyParts = new List<GameObject>();
    private List<GameObject> snakeBody = new List<GameObject>();

    [Header("Formato da Arena")]
    [SerializeField] public float multiplicadorDaBorda = 2.0f;

    [Header("Controle Externo")]
    public bool estaOrbitando = true; 
    
    private int totalPartsNoInicio; 
    float countUp = 0;

    void Start()
    {
        baseSpeed = speed;
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

        Rigidbody2D headRb = snakeBody[0].GetComponent<Rigidbody2D>();
        headRb.velocity = snakeBody[0].transform.right * speed * Time.deltaTime;
        
        if (estaOrbitando)
        {
            Vector2 direcaoAtual = snakeBody[0].transform.right;
            float forcaDaCurva = 1f + (Mathf.Abs(direcaoAtual.y) * multiplicadorDaBorda);
            float giroFinal = -rotationSpeed * forcaDaCurva * Time.deltaTime;
            snakeBody[0].transform.Rotate(new Vector3(0, 0, giroFinal));
        }

        if(snakeBody.Count > 1)
        {
            float distanciaParaAndar = headRb.velocity.magnitude * Time.deltaTime;

            for(int i = 1; i < snakeBody.Count; i++)
            {
                MarkerManager markM = snakeBody[i - 1].GetComponent<MarkerManager>();
                float distanciaDisponivel = distanciaParaAndar;

                while (distanciaDisponivel > 0)
                {
                    if (markM.markerList.Count > 0)
                    {
                        float distanciaAtePonto = Vector3.Distance(snakeBody[i].transform.position, markM.markerList[0].position);

                        if (distanciaDisponivel >= distanciaAtePonto)
                        {
                            snakeBody[i].transform.position = markM.markerList[0].position;
                            snakeBody[i].transform.rotation = markM.markerList[0].rotation;
                            distanciaDisponivel -= distanciaAtePonto;
                            markM.markerList.RemoveAt(0);
                        }
                        else
                        {
                            snakeBody[i].transform.position = Vector3.MoveTowards(snakeBody[i].transform.position, markM.markerList[0].position, distanciaDisponivel);
                            snakeBody[i].transform.rotation = Quaternion.Lerp(snakeBody[i].transform.rotation, markM.markerList[0].rotation, distanciaDisponivel / distanciaAtePonto);
                            distanciaDisponivel = 0; 
                        }
                    }
                    else
                    {
                        snakeBody[i].transform.position += snakeBody[i].transform.right * distanciaDisponivel;
                        distanciaDisponivel = 0;
                    }
                }
            }
        }
    }

    float CalcularDistanciaNecessaria()
    {
        int indexAtual = snakeBody.Count; 
        if (indexAtual == 1) return distanciaCabeca; 
        else if (indexAtual >= totalPartsNoInicio - 2) return distanciaCauda;
        
        return distanciaPadrao;
    }

    void CreateBodyParts()
    {
        if (snakeBody.Count == 0)
        {
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
        if(countUp == 0) markM.clearMarkerList();
        
        countUp += Time.deltaTime;
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

    public Transform GetCauda()
    {
        if (snakeBody.Count > 0) return snakeBody[snakeBody.Count - 1].transform;
        return null; 
    }

    public Transform GetCabeca()
    {
        if (snakeBody.Count > 0) return snakeBody[0].transform;
        return null;
    }

    float ObterEspacamentoIdeal(int index)
    {
        float tempoEspera = distanciaPadrao;
        if (index == 1) tempoEspera = distanciaCabeca;
        else if (index >= totalPartsNoInicio - 2) tempoEspera = distanciaCauda;

        return (baseSpeed * Time.fixedDeltaTime) * tempoEspera;
    }

    public void ReposicionarCobra(Vector3 novaPosicao, Quaternion novaRotacao)
    {
        snakeBody[0].transform.position = novaPosicao;
        snakeBody[0].transform.rotation = novaRotacao;
        
        Vector3 direcaoTras = -snakeBody[0].transform.right;
        Vector3 posicaoAtual = novaPosicao;

        for(int i = 1; i < snakeBody.Count; i++)
        {
            MarkerManager markM = snakeBody[i - 1].GetComponent<MarkerManager>();
            if (markM != null) markM.clearMarkerList(); 
            
            float distanciaPerfeita = ObterEspacamentoIdeal(i);
            posicaoAtual += (direcaoTras * distanciaPerfeita);
            
            snakeBody[i].transform.position = posicaoAtual;
            snakeBody[i].transform.rotation = novaRotacao;
        }
        
        MarkerManager lastMarkM = snakeBody[snakeBody.Count - 1].GetComponent<MarkerManager>();
        if (lastMarkM != null) lastMarkM.clearMarkerList();
    }

    public List<GameObject> GetCorpoCompleto()
    {
        return snakeBody;
    }
}