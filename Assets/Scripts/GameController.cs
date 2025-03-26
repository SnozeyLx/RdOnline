using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private ApiAuth API;
   
    // Start is called before the first frame update
    void Awake()
    {  
        API = FindObjectOfType<ApiAuth>();
        if (API != null)
        {
            //API.start();
        }
        
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeBet(string card_id, string value)
    {

        Bet bet = new Bet();
        bet.room_id = API.roomResponse.rooms[0].id;
        bet.game_id = API.roomResponse.rooms[0].user_id;
        //if(card_id == "card_A") bet.card_id = API.card_A.id;
        //if(card_id == "card_B") bet.card_id = API.card_B.id;
        bet.value = value;

        string json = JsonUtility.ToJson(bet);
        
      //API.TryBet(json);

    }
   public class Bet
    {   
        public int id;
        public int game_id;
        public int user_id;
        public int room_id;
        public int card_id;
        public string value;
        public string created_at;
        public string updated_at;
    }    

}
