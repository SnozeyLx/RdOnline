using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;


public class DealerSelector : MonoBehaviour
{
    // Start is called before the first frame update
    public string naipeSel;
    public string cartaSel;
    public bool waitingWinner = false;
    public GameObject Card_1, Card_2;
    public GameObject Btn_Card, Btn_Winner, Btn_NewGame;
    public Sprite back1, back2;
    public TMP_Dropdown dropdownslug1;
    public TMP_Dropdown dropdowncard1;
    public TMP_Dropdown dropdownslug2;
    public TMP_Dropdown dropdowncard2;
    public string cardChosen;
    private ApiAuth AA;
    public TMP_InputField inputField;
    public Sprite[] cardSprites;

    public string nomeCarta;

    public GraphicRaycaster raycaster; // Referência ao GraphicRaycaster do Canvas


   

    void Start()
    {
        cardChosen = null;
        /* Image cardImage = Card_1.transform.GetChild(1).GetComponent<Image>();
        cardImage.sprite = cardSprites[cardID]; */


        

        AA = FindObjectOfType<ApiAuth>();
        if (AA != null)
        {
            //API.start();
        }
        // Adiciona um listener para ativar o segundo dropdown ao escolher um naipe
        dropdowncard1.onValueChanged.RemoveAllListeners();
        dropdowncard1.onValueChanged.AddListener(delegate { OnCartaSelected("card_1"); });
        dropdownslug1.onValueChanged.RemoveAllListeners();
        dropdownslug1.onValueChanged.AddListener(delegate { OnNaipeSelected("card_1"); });
        dropdowncard2.onValueChanged.RemoveAllListeners();
        dropdowncard2.onValueChanged.AddListener(delegate { OnCartaSelected("card_2"); });
        dropdownslug2.onValueChanged.RemoveAllListeners();
        dropdownslug2.onValueChanged.AddListener(delegate { OnNaipeSelected("card_2"); });

        dropdowncard1.gameObject.SetActive(false);
        dropdownslug1.gameObject.SetActive(false);
        dropdowncard2.gameObject.SetActive(false);
        dropdownslug2.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
           // CheckClick();
        
        
    }

   
    

   /*  void CheckClick()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("card_1"))
            {
                
                Card_2.transform.GetChild(0).gameObject.SetActive(false);
                Card_1.transform.GetChild(0).gameObject.SetActive(true);

                dropdowncard2.value = 0;
                dropdowncard2.RefreshShownValue(); 
                dropdownslug2.value = 0;
                dropdownslug2.RefreshShownValue(); 
                dropdowncard2.gameObject.SetActive(false);
                dropdownslug2.gameObject.SetActive(false);
                return;
            }
             if (result.gameObject.CompareTag("card_2"))
            {
                cardChosen = result.gameObject.name;
               
                Card_1.transform.GetChild(0).gameObject.SetActive(false);
                Card_2.transform.GetChild(0).gameObject.SetActive(true);
                dropdowncard1.value = 0;
                dropdowncard1.RefreshShownValue(); 
                dropdownslug1.value = 0;
                dropdownslug1.RefreshShownValue(); 
                dropdowncard1.gameObject.SetActive(false);
                dropdownslug1.gameObject.SetActive(false);
                return;
            }
             if (result.gameObject.CompareTag("drop"))
            {
                Debug.Log("drop");
                return;
            }
            Debug.Log("nome fora de todos" + result.gameObject.name);
            return;
        }/* 
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

        Debug.Log("Não clicou em um objeto com a tag específica.");
    } 
    
    */ 

    public void SelectCard(string card)
    {
        if(card == "card_A" & waitingWinner == false)
        {
            Card_2.transform.GetChild(0).gameObject.SetActive(false);
            Card_1.transform.GetChild(0).gameObject.SetActive(true);
            
            dropdownslug1.SetValueWithoutNotify(0);
            dropdowncard1.SetValueWithoutNotify(0);
            dropdownslug2.SetValueWithoutNotify(0);
            dropdowncard2.SetValueWithoutNotify(0);
           
            dropdownslug2.gameObject.SetActive(false);
            dropdowncard2.gameObject.SetActive(false);
            dropdownslug1.gameObject.SetActive(true);
        }  
        if(card == "card_B" & waitingWinner == false)
        {
            Card_1.transform.GetChild(0).gameObject.SetActive(false);
            Card_2.transform.GetChild(0).gameObject.SetActive(true);
            
            dropdownslug1.SetValueWithoutNotify(0);
            dropdowncard1.SetValueWithoutNotify(0);
            dropdownslug2.SetValueWithoutNotify(0);
            dropdowncard2.SetValueWithoutNotify(0); 

            dropdownslug1.gameObject.SetActive(false);
            dropdowncard1.gameObject.SetActive(false);
            dropdownslug2.gameObject.SetActive(true);
        } 
        if(card == "card_A" & waitingWinner == true)
        {
            Card_2.transform.GetChild(0).gameObject.SetActive(false);
            Card_1.transform.GetChild(0).gameObject.SetActive(true);
            cardChosen = "Card_1";
        }

        if(card == "card_B" & waitingWinner == true)
        {
            Card_1.transform.GetChild(0).gameObject.SetActive(false);
            Card_2.transform.GetChild(0).gameObject.SetActive(true);
            cardChosen = "Card_2";
        }
           

    }

    public void OnNaipeSelected(string card)
    {
        // Ativa o Dropdown de Cartas quando um naipe for escolhido
        if (card == "card_1" & dropdownslug1.value != 0) // Garante que o primeiro item não seja usado (caso seja um "Escolha um naipe")
        {
            dropdowncard1.gameObject.SetActive(true);
        }
        if (card == "card_2" & dropdownslug2.value != 0) // Garante que o primeiro item não seja usado (caso seja um "Escolha um naipe")
        {
            dropdowncard2.gameObject.SetActive(true);
        }
    }

    public void OnCartaSelected(string card)
    {
        
        if (card == "card_1" & dropdownslug1.value != 0 & dropdowncard1.value != 0){ 
        string naipeSelecionado = dropdownslug1.options[dropdownslug1.value].text;
        string cartaSelecionada = dropdowncard1.options[dropdowncard1.value].text;
        naipeSel = naipeSelecionado;
        cartaSel = cartaSelecionada;
        nomeCarta = $"{cartaSelecionada} de {naipeSelecionado}";   
        dropdowncard1.SetValueWithoutNotify(0);
        dropdowncard1.SetValueWithoutNotify(0);     
        dropdownslug1.gameObject.SetActive(false);
        dropdowncard1.gameObject.SetActive(false);

        }
         if (card == "card_2" & dropdownslug2.value != 0 & dropdowncard2.value != 0){ 
        string naipeSelecionado = dropdownslug2.options[dropdownslug2.value].text;
        string cartaSelecionada = dropdowncard2.options[dropdowncard2.value].text;
        naipeSel = naipeSelecionado;
        cartaSel = cartaSelecionada;
        nomeCarta = $"{cartaSelecionada} de {naipeSelecionado}";
        dropdownslug2.SetValueWithoutNotify(0);
        dropdowncard2.SetValueWithoutNotify(0);
        dropdownslug2.gameObject.SetActive(false);
        dropdowncard2.gameObject.SetActive(false);      
        
        }

        if (cardIDs.TryGetValue(nomeCarta, out int cardID))
        {
            if (card == "card_1" ){
                Card_1.transform.GetChild(0).gameObject.SetActive(false);
                Image cardImage = Card_1.transform.GetChild(1).GetComponent<Image>();
                cardImage.sprite = cardSprites[cardID];
                AA.currentGame.card_left = cardID; 
                AA.currentGame.card_left_name = cartaSel;
                AA.currentGame.card_left_slug = naipeSel; 
                /*int index = nomeCarta.IndexOf("de ");
                 string naipe = nomeCarta.Substring(index + 3); // Pega tudo depois de "de "
                Debug.Log(naipe);    */          
                Debug.Log($"Carta Escolhida 1: {nomeCarta}, ID: {cardID}");
                 
            }
            if (card == "card_2" ){
                Card_2.transform.GetChild(0).gameObject.SetActive(false);
                Image cardImage = Card_2.transform.GetChild(1).GetComponent<Image>();
                cardImage.sprite = cardSprites[cardID];
                AA.currentGame.card_right = cardID; 
                AA.currentGame.card_right_name = cartaSel;
                AA.currentGame.card_right_slug = naipeSel; 
                Debug.Log($"Carta Escolhida 2: {nomeCarta}, ID: {cardID}");
                
            }
        }
        else
        {
            Debug.Log("Carta não encontrada no banco de dados.");
        }
    }

     public void SendCard()
    { 
        if(AA.currentGame.card_left != 0 & AA.currentGame.card_right != 0){
        dropdownslug1.gameObject.SetActive(false);
        dropdowncard1.gameObject.SetActive(false);
        dropdownslug2.gameObject.SetActive(false);
        dropdowncard2.gameObject.SetActive(false);   
        Card_1.transform.GetChild(0).gameObject.SetActive(false);
        Card_2.transform.GetChild(0).gameObject.SetActive(false);
        AA.TrySendCards();

        waitingWinner = true;

        Btn_Card.SetActive(false);
        Btn_Winner.SetActive(true);
        //botao de sendwinner aparece

        return;

        }
        Debug.Log("falta escolher as cartas");
        //verifica se pode apostar
        //int betValue = int.Parse(inputField.text);
       // string betValue = inputField.text;
       
       // string textoDigitado = inputField.text;
       // Debug.Log("Texto digitado: " + textoDigitado);

    }

      public void SendWinnerCard()
    { 
        if(waitingWinner & cardChosen != null){
            AA.TryFinish(cardChosen);
            waitingWinner = false;
            cardChosen = null;
            Btn_Winner.SetActive(false);
            Btn_NewGame.SetActive(true);
            //return;
        }
        Debug.Log("waiting winner falso ainda");

    }

     public void SendNewGame()
    {
        Btn_NewGame.SetActive(false);
        AA.NewGame();
        Btn_Card.SetActive(true); 
        Image cardImage1 = Card_1.transform.GetChild(1).GetComponent<Image>();
        cardImage1.sprite = back1;
        Image cardImage2 = Card_2.transform.GetChild(1).GetComponent<Image>();
        cardImage2.sprite = back2;       
        Card_2.transform.GetChild(0).gameObject.SetActive(false);
        Card_1.transform.GetChild(0).gameObject.SetActive(false);    
        Debug.Log("foi");

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

