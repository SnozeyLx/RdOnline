using System.Collections;
using System.Collections.Generic;
using UnityEngine;
  
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int room;
    public string token;
    public string role;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Impede que seja destruído ao trocar de cena
        }
        else
        {
            Destroy(gameObject); // Garante que apenas um GameManager exista
        }
    }
}

