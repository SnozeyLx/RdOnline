using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;

public class PlayerSelector : MonoBehaviour
{
    // Start is called before the first frame update
    public enum GameState { WaitingForStart, CardsReceived, WinnerRevealed }
    private GameState currentState = GameState.WaitingForStart;
    public GameObject ObjectCard_1, ObjectCard_2;
    public GameObject Winner_Screen;   
    public Image Winner_Card; 
    private ApiAuth AA;
    public TMP_InputField inputField;
    public Sprite[] cardSprites;
    public Sprite back1, back2;
    private string cardChosen;


    public GraphicRaycaster raycaster; // Referência ao GraphicRaycaster do Canvas

    void Start()
    {
        
        
        AA = FindObjectOfType<ApiAuth>();
        if (AA != null)
        {
            //API.start();
        }
        SetState(GameState.WaitingForStart);
        Winner_Card = Winner_Screen.transform.GetChild(0).GetChild(1).GetComponent<Image>();
        Winner_Screen.SetActive(false);
        

        
    }

    // Update is called once per frame
    void Update()
    {
        
             if (Input.GetMouseButtonDown(0)) // Verifica clique esquerdo
        {
            CheckClick();
        }
        
        
    }

   
    

     void CheckClick()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        foreach (RaycastResult result in results)
        {
            if(AA.currentGame.card_left == 0 & AA.currentGame.card_right == 0)
            {/* 
                Image cardImage1 = Card_1.transform.GetChild(1).GetComponent<Image>();
                cardImage1.sprite = back1;
                Image cardImage2 = Card_2.transform.GetChild(1).GetComponent<Image>();
                cardImage2.sprite = back2; */
                return;
            }
            
            if (result.gameObject.CompareTag("card_1"))
            {
                ObjectCard_2.transform.GetChild(0).gameObject.SetActive(false);
                ObjectCard_1.transform.GetChild(0).gameObject.SetActive(true);
                cardChosen = "Card_1";

                return;
            }
             if (result.gameObject.CompareTag("card_2"))
            {
                ObjectCard_1.transform.GetChild(0).gameObject.SetActive(false);
                ObjectCard_2.transform.GetChild(0).gameObject.SetActive(true);
                cardChosen = "Card_2";

                return;
            }


            Debug.Log("nome fora de todos" + result.gameObject.name);
            return;
        } /*
        Debug.Log("saiu do if");
        Card_1.transform.GetChild(0).gameObject.SetActive(false);
        Card_2.transform.GetChild(0).gameObject.SetActive(false);
        dropdowncard1.value = 0;
        dropdowncard1.RefreshShownValue(); 
        dropdownslug1.value = 0;
        dropdownslug1.RefreshShownValue();
        dropdowncard2.value = 0;
        dropdowncard2.RefreshShownValue(); 
        dropdownslug2.value = 0;
        dropdownslug2.RefreshShownValue();  

        Debug.Log("Não clicou em um objeto com a tag específica.");*/
    } 
    
    

   /*  public void SelectCard(string card)
    {
        if(card == "card_A")
        {
            Card_2.transform.GetChild(0).gameObject.SetActive(false);
            Card_1.transform.GetChild(0).gameObject.SetActive(true);
            
           
        }  
        if(card == "card_B")
        {
            Card_1.transform.GetChild(0).gameObject.SetActive(false);
            Card_2.transform.GetChild(0).gameObject.SetActive(true);
            
        } 
           

    }  */
  

     public void DoBet()
    { 
        float betValue;
        //verifica se pode apostar
        if (float.TryParse(inputField.text, out betValue)){
            if(betValue != 0){ 
         AA.TryBet(betValue, cardChosen);
         return;
         }
         Debug.Log("valor da bet ta 0!");
        }
        Debug.Log("parsefloat deu ruim");
         
       
       // string nomeCartaEncontrada = cardIDs.FirstOrDefault(x => x.Value == cardID).Key;
         

        
        

    }
     void ShowBackCards()
    {
        Image cardImage1 = ObjectCard_1.transform.GetChild(1).GetComponent<Image>();
        cardImage1.sprite = back1;
        Image cardImage2 = ObjectCard_2.transform.GetChild(1).GetComponent<Image>();
        cardImage2.sprite = back2;
        Winner_Screen.SetActive(false);
        ObjectCard_1.transform.GetChild(0).gameObject.SetActive(false);
        ObjectCard_2.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void CardReceived(int card)
    {
        
    }
    public void WinnerReceived(int card)
    {
        Winner_Screen.SetActive(true);
        Winner_Card.sprite = cardSprites[card];
        Image cardImage1 = ObjectCard_1.transform.GetChild(1).GetComponent<Image>();
        cardImage1.sprite = back1;
        Image cardImage2 = ObjectCard_2.transform.GetChild(1).GetComponent<Image>();
        cardImage2.sprite = back2;
        AA.currentGame.card_winner = card;
       // AA.card_winner.id = winnerCard;
       // SetState(GameState.WinnerRevealed);
      

    }
    void UpdateCards()
    {
        if (AA.currentGame.card_left > 0 && AA.currentGame.card_right > 0)
        {
            ObjectCard_1.transform.GetChild(1).GetComponent<Image>().sprite = cardSprites[AA.currentGame.card_left];
            ObjectCard_2.transform.GetChild(1).GetComponent<Image>().sprite = cardSprites[AA.currentGame.card_right];
        }
    }
     void ShowWinner()
    {
        Winner_Card.sprite = cardSprites[AA.currentGame.card_winner];
        Winner_Screen.SetActive(true);       
    }
    public void SetState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.WaitingForStart:
                ShowBackCards();
                break;
            case GameState.CardsReceived:
                UpdateCards();
                break;
            case GameState.WinnerRevealed:
                ShowWinner();
                break;
        }
    }

     private Dictionary<string, int> cardIDs = new Dictionary<string, int>()
    {
         // Espadas (1-13)
    {"Ás de Paus", 1}, {"2 de Paus", 2}, {"3 de Paus", 3}, {"4 de Paus", 4}, {"5 de Paus", 5}, 
    {"6 de Paus", 6}, {"7 de Paus", 7}, {"8 de Paus", 8}, {"9 de Paus", 9}, {"10 de Paus", 10},
    {"J de Paus", 11}, {"Q de Paus", 12}, {"R de Paus", 13},

    // Copas
    {"Ás de Copas", 14}, {"2 de Copas", 15}, {"3 de Copas", 16}, {"4 de Copas", 17}, {"5 de Copas", 18},
    {"6 de Copas", 19}, {"7 de Copas", 20}, {"8 de Copas", 21}, {"9 de Copas", 22}, {"10 de Copas", 23},
    {"J de Copas", 24}, {"Q de Copas", 25}, {"R de Copas", 26},

    // Espadas
    {"Ás de Espadas", 27}, {"2 de Espadas", 28}, {"3 de Espadas", 29}, {"4 de Espadas", 30}, {"5 de Espadas", 31},
    {"6 de Espadas", 32}, {"7 de Espadas", 33}, {"8 de Espadas", 34}, {"9 de Espadas", 35}, {"10 de Espadas", 36},
    {"J de Espadas", 37}, {"Q de Espadas", 38}, {"R de Espadas", 39},

    // Ouros
    {"Ás de Ouros", 40}, {"2 de Ouros", 41}, {"3 de Ouros", 42}, {"4 de Ouros", 43}, {"5 de Ouros", 44},
    {"6 de Ouros", 45}, {"7 de Ouros", 46}, {"8 de Ouros", 47}, {"9 de Ouros", 48}, {"10 de Ouros", 49},
    {"J de Ouros", 50}, {"Q de Ouros", 51}, {"R de Ouros", 52}

    };
         

}

