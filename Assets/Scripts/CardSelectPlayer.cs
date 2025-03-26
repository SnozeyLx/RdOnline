using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardSelectPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    
    public GameObject Card_1;
    public GameObject Card_2;
    public TMP_Dropdown dropdownslug1;
    public TMP_Dropdown dropdowncard1;
    public TMP_Dropdown dropdownslug2;
    public TMP_Dropdown dropdowncard2;
    private string cardChosen;
    private GameController GC;
    public TMP_InputField inputField;
    public Sprite[] cardSprites;

    public string nomeCarta;

    public GraphicRaycaster raycaster; // Referência ao GraphicRaycaster do Canvas

    void Start()
    {
        GC = FindObjectOfType<GameController>();
        if (GC != null)
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
        if(card == "card_A")
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
        if(card == "card_B")
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
        nomeCarta = $"{cartaSelecionada} de {naipeSelecionado}";   
        dropdowncard1.SetValueWithoutNotify(0);
        dropdowncard1.SetValueWithoutNotify(0);     
        dropdownslug1.gameObject.SetActive(false);
        dropdowncard1.gameObject.SetActive(false);

        }
         if (card == "card_2" & dropdownslug2.value != 0 & dropdowncard2.value != 0){ 
        string naipeSelecionado = dropdownslug2.options[dropdownslug2.value].text;
        string cartaSelecionada = dropdowncard2.options[dropdowncard2.value].text;
        nomeCarta = $"{cartaSelecionada} de {naipeSelecionado}";
        dropdownslug2.SetValueWithoutNotify(0);
        dropdowncard2.SetValueWithoutNotify(0);
        dropdownslug2.gameObject.SetActive(false);
        dropdowncard2.gameObject.SetActive(false);      
        
        Debug.Log($"Carta Escolhida 1: ");
        }

        if (cardIDs.TryGetValue(nomeCarta, out int cardID))
        {
            if (card == "card_1" ){
                Card_1.transform.GetChild(0).gameObject.SetActive(false);
                Image cardImage = Card_1.transform.GetChild(1).GetComponent<Image>();
                cardImage.sprite = cardSprites[cardID];
                Debug.Log($"Carta Escolhida 1: {nomeCarta}, ID: {cardID}");
                 
            }
            if (card == "card_2" ){
                Card_2.transform.GetChild(0).gameObject.SetActive(false);
                Image cardImage = Card_2.transform.GetChild(1).GetComponent<Image>();
                cardImage.sprite = cardSprites[cardID];
                Debug.Log($"Carta Escolhida 2: {nomeCarta}, ID: {cardID}");
                
            }
        }
        else
        {
            Debug.Log("Carta não encontrada no banco de dados.");
        }
    }

     public void DoBet()
    { 
        //verifica se pode apostar
        //int betValue = int.Parse(inputField.text);
        string betValue = inputField.text;
        GC.MakeBet("",betValue);
        string textoDigitado = inputField.text;
        Debug.Log("Texto digitado: " + textoDigitado);


    }

     private Dictionary<string, int> cardIDs = new Dictionary<string, int>()
    {
         // Espadas (1-13)
     {"Ás de Paus", 0}, {"2 de Paus", 1}, {"3 de Paus", 2}, {"4 de Paus", 3}, {"5 de Paus", 4},
    {"6 de Paus", 5}, {"7 de Paus", 6}, {"8 de Paus", 7}, {"9 de Paus", 8}, {"10 de Paus", 9},
    {"J de Paus", 10}, {"Q de Paus", 11}, {"R de Paus", 12},

    // Copas
    {"Ás de Copas", 13}, {"2 de Copas", 14}, {"3 de Copas", 15}, {"4 de Copas", 16}, {"5 de Copas", 17},
    {"6 de Copas", 18}, {"7 de Copas", 19}, {"8 de Copas", 20}, {"9 de Copas", 21}, {"10 de Copas", 22},
    {"J de Copas", 23}, {"Q de Copas", 24}, {"R de Copas", 25},

    // Espadas
    {"Ás de Espadas", 26}, {"2 de Espadas", 27}, {"3 de Espadas", 28}, {"4 de Espadas", 29}, {"5 de Espadas", 30},
    {"6 de Espadas", 31}, {"7 de Espadas", 32}, {"8 de Espadas", 33}, {"9 de Espadas", 34}, {"10 de Espadas", 35},
    {"J de Espadas", 36}, {"Q de Espadas", 37}, {"R de Espadas", 38},

    // Ouros
    {"Ás de Ouros", 39}, {"2 de Ouros", 40}, {"3 de Ouros", 41}, {"4 de Ouros", 42}, {"5 de Ouros", 43},
    {"6 de Ouros", 44}, {"7 de Ouros", 45}, {"8 de Ouros", 46}, {"9 de Ouros", 47}, {"10 de Ouros", 48},
    {"J de Ouros", 49}, {"Q de Ouros", 50}, {"R de Ouros", 51}

    };
         

}

