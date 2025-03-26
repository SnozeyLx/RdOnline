
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

using NativeWebSocket;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;


using System.Runtime.InteropServices;

public class ApiAuth : MonoBehaviour
{
     [DllImport("__Internal")]
    private static extern string getReactData(); // pegar informações da pagina react 
    public NativeWebSocket.WebSocket websocket; //websocket
    public string tokenteste;
    public int roomteste;
    private string token; // token JWT
    private string wsUrl = "ws://3.129.253.32:8080";  // URL do WebSocket ("ws://12.34.56.78:8080");
    private string apiUrl = "https://ronda-api.artevitdevs.com.br"; // URL da API https://ronda-api.artevitdevs.com.br local = 127.0.0.1:8000

    //chaves de acesso
    private string refresh = "/refresh";   
    private string profile = "/users/profile";  
    private string getCard = "/cards";
    private string showGame = "/games/{game}";
    private string setCards = "/games/{game}/set-cards";
    private string finishGame = "/games/{game}/finish";
    private string startGame = "/games";
    private string listRooms = "/rooms";
    private string bet = "/bets";

    
    public string type = "refresh_game"; //tipo de mensagem enviada entre os endpoints
    public GameObject capa; //proteção capa preta quando não tem sala

    public GameObject dealerBoard, userBoard; //interface dos usuarios

    //contrutores do cliente     
    public LoginResponse currentPlayer;
    public RoomResponse roomResponse;
    public Game currentGame;

    public PlayerSelector playerSelector;
   
   
    //texto tela
    public TMP_Text screenLobby;
    public TMP_Text screenName;
    public TMP_Text screenRole;


    IEnumerator EsperarEExecutar(float t)
    {
        Debug.Log("Antes de esperar...");
        yield return new WaitForSeconds(t); // Espera 3 segundos
        Debug.Log("Depois de esperar!");
    }
    async void Start()
    {
         #if UNITY_WEBGL && !UNITY_EDITOR
           string data = getReactData();
           Debug.Log("Dados do React: " + data);
           ReactResponse json = JsonUtility.FromJson<ReactResponse>(data);
           currentPlayer.token = json.token;
           currentGame.room_id = json.room_id;
        #endif   

        /* currentPlayer.token = tokenteste; 
        currentGame.room_id = roomteste;  */
        screenLobby.text = currentGame.room_id.ToString(); // necessario para tirar a tela preta
        if(currentPlayer.token != ""){ 
           
/* 
            screenLobby.text = GameManager.Instance.room.ToString(); 
            currentPlayer.token = GameManager.Instance.token;
            currentPlayer.profile.role = GameManager.Instance.role;
            currentGame.room_id = GameManager.Instance.room;
 */
            await GetProfile();
        }

        if(currentPlayer.token == ""){ 
           
           screenLobby.text = GameManager.Instance.room.ToString(); 
            currentPlayer.token = GameManager.Instance.token;
            currentPlayer.profile.role = GameManager.Instance.role;
            currentGame.room_id = GameManager.Instance.room;

            await GetProfile();
        }
        

        websocket = new WebSocket(wsUrl);

         websocket.OnOpen += async () =>
        {
           
            //EsperarEExecutar(2.0f);
            Debug.Log("Conectado ao servidor WebSocket!");
            
            
            if (currentPlayer.profile.role == "dealer")
            {
                // string json = JsonUtility.ToJson(player.user.email);
                Debug.Log("Conectado Dealer");                 

                await CreateRoom();                
            }

            if (currentPlayer.profile.role == "user")
            {
                Debug.Log("Conectado Jogador"); 

                await JoinGame();
            }
            
            // fazer um caso seja admin não conseguir entrar na partida
            Debug.Log("role para o websocket : " + currentPlayer.profile.role);
        };

        websocket.OnMessage += async (bytes) =>
        {
             
            string message = Encoding.UTF8.GetString(bytes);
            Debug.Log("recebeu mensagem do servidor node" + message);
            Game data1 = JsonUtility.FromJson<Game>(message);
            
            Debug.Log(data1.type);
            if(data1.type == "joined"){
             Debug.Log("entrou no joined");
            }

            
            if(data1.type == "refresh_game"){

            await RefreshGame(); 

            Debug.Log("Atualizou o game pela msg do node"); 
            return;

            }

            if(data1.type == "receive_card"){

            currentGame.card_left = data1.card_left;            
            currentGame.card_right = data1.card_right; 

            Debug.Log(currentGame.card_left + currentGame.card_right); 
            return;

            }

            if(data1.type == "receive_winner"){

              //  await GetWinner();

                if(currentGame.card_winner != 0){
                    
                    playerSelector.WinnerReceived(currentGame.card_winner);                

                    EsperarEExecutar(10.0f);
                    playerSelector.Winner_Screen.SetActive(false);

                }

                return;

            }

         /*    CardData data = JsonUtility.FromJson<CardData>(message);
            data.type = "receive_card";
            card_1.id = int.Parse(data.card_left);            
            card_2.id = int.Parse(data.card_right);

           
             Image cardImage1 = selectPlayer.Card_1.transform.GetChild(1).GetComponent<Image>();
                cardImage1.sprite = selectPlayer.cardSprites[card_1.id];

            Image cardImage2 = selectPlayer.Card_1.transform.GetChild(1).GetComponent<Image>();
                cardImage2.sprite = selectPlayer.cardSprites[card_2.id];
             */


        };

        websocket.OnError += (error) =>
        {
            Debug.LogError("Erro no WebSocket: " + error);
        };

          websocket.OnClose += (close) =>
        {
            Debug.Log("Conexão fechada."); // Mensagem de log

                // Verifica se o WebSocket ainda está aberto antes de enviar mensagem
                if (websocket.State == WebSocketState.Open)
                {
                    NodeC neww = new NodeC();
                    neww.room_id = currentGame.room_id;
                    SendMessageWs(neww);
                }
                else
                {
                    Debug.Log("Não foi possível enviar mensagem: WebSocket fechado.");
                }
            Debug.Log("Conexão fechada."); // quando o Dealer fechar a sala ou a sala cair "voce foi desconectado do servidor node"
        };

            
        await websocket.Connect();
        // get token primeira coisa pra iniciar pois ele serve como id do usuario e pagar seus dados
 
       /*  player = new LoginResponse();
        player.user.email = "richard_bocao_@homtail.com";
        player.user.password = "123456789"; */
        //string json = "{\"email\": \" admin@rondaonlinebr.com\", \"password\": \"@Ronda2025\"}";
        //jogador@rondaonlinebr.com  senha: @Ronda2025

      //  string json = JsonUtility.ToJson(player.user.email);

       // StartCoroutine(PostRequest(apiUrl + login, json)); //login

      /*   //Get request
         StartCoroutine(GetRequest(apiUrl));

        //Post request
         string json = "{\"title\": \"foo\", \"body\": \"bar\", \"userId\": 1}";
         StartCoroutine(PostRequest("https://jsonplaceholder.typicode.com/posts", json));
   
        //Delete request
         StartCoroutine(DeleteRequest("https://jsonplaceholder.typicode.com/posts/1")); */
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
        Debug.Log("sala encerrada"); // voce saiu do jogo
        await websocket.Close();
        }
    } 

     void Update()
    {      

         /* #if !UNITY_WEBGL || UNITY_EDITOR
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            websocket.DispatchMessageQueue();
        }
        #endif */
          /* if (websocket != null)
        {
            websocket.DispatchMessageQueue();
        }   */

        if(screenLobby.text != ""){
            capa.gameObject.SetActive(false);

        } 
       if(screenLobby.text == ""){
            capa.gameObject.SetActive(true);

        }  

    }

    public async void SendMessageWs(object data)
    {
        string json = JsonUtility.ToJson(data);
        
        Debug.Log("enviou para o servidor node: "+json);
        //string json = JsonUtility.ToJson(data);
        await websocket.SendText(json);
    }

    public async Task SendCard()
    {
        if (currentPlayer.profile.role == "dealer")
        {
            currentGame.type = "refresh_game";

            setCards = "/games/"+ currentGame.id + "/set-cards";

            string json = JsonUtility.ToJson(currentGame);

            string response = await PostProtectedData(setCards, json);

            SendMessageWs(currentGame);

            Debug.Log("enviou cards API");
        }
    } 

    public async Task SendFinish(string cardChosen){
   
        currentGame.type = "refresh_game";
        if(cardChosen == "Card_1") currentGame.card_winner = currentGame.card_left;
        if(cardChosen == "Card_2") currentGame.card_winner = currentGame.card_right;

        finishGame = "/games/"+ currentGame.id + "/finish"; 

        string json = JsonUtility.ToJson(currentGame);

        string response = await PostProtectedData(finishGame, json);
        
        SendMessageWs(currentGame);
        
        Debug.Log("finalizou game API");
    
        EsperarEExecutar(35.0f);

        await NewGame();
    } 

 /*    public void SendFullCards(string data){
    Room rom = JsonUtility.FromJson<Room>(data);
   
    setCards = "/games/"+ currentGame.id + "/set-cards";
    PostProtectedData(apiUrl+setCards, data);
    } */
   
    public async Task TryBet(float valorBet, string cardChosen){
       
    Bet currentBet = new Bet();

    if(cardChosen == "Card_1") currentBet.card_id = currentGame.card_left; 
    if(cardChosen == "Card_2") currentBet.card_id = currentGame.card_right;

    currentBet.game_id = currentGame.id;
    currentBet.room_id = currentGame.room_id;
    currentBet.value = valorBet;

    string json = JsonUtility.ToJson(currentBet);
    Debug.Log(currentBet.value +"<valor - room >" + currentBet.room_id  + currentBet.game_id + "<game - card >" + currentBet.card_id );
    string response = await PostProtectedData(bet, json);

    Debug.Log("apostou na API");
    }


    public async Task TrySendCards(){
        
    currentGame.type = "refresh_game";

    setCards = "/games/"+ currentGame.id + "/set-cards";

    string json = JsonUtility.ToJson(currentGame);

    
    string response = await PutProtectedData(setCards, json);
    SendMessageWs(currentGame);
    Debug.Log("enviou cards API");
    }


    public async Task TryFinish(string cardChosen){
   
    currentGame.type = "refresh_game";
    if(cardChosen == "Card_1") currentGame.card_winner = currentGame.card_left;
    if(cardChosen == "Card_2") currentGame.card_winner = currentGame.card_right;

    finishGame = "/games/"+ currentGame.id + "/finish"; 

    string json = JsonUtility.ToJson(currentGame);

    
    string response = await PostProtectedData(finishGame, json);
    SendMessageWs(currentGame);
    Debug.Log("finalizou game API" + response);
    
    //EsperarEExecutar(350.0f);
    // await NewGame();
    }


    public async Task CreateRoom(){

    NodeC nodeC = new NodeC();
    nodeC.room_id = currentGame.room_id;
    nodeC.player = currentPlayer.profile;
    nodeC.type = type = "create_room";

    string json = JsonUtility.ToJson(nodeC);

    
    Debug.Log("criou game no Node");
    type = "refresh_game";
    finishGame = "/rooms/"+ currentGame.room_id + "/games"; 
    
    string response2 = await PostProtectedData(startGame, json);
    Debug.Log(response2);

    string response = await GetProtectedData(finishGame); 
    Debug.Log(response);
    SendMessageWs(nodeC);
    //PostProtectedData(startGame, json);

    //Debug.Log("criou game API");
    }


    public async Task NewGame(){

    NodeC nodeC = new NodeC();
    nodeC.room_id = currentGame.room_id;
    nodeC.player = currentPlayer.profile;
    nodeC.type = type = "refresh_game";

    string json = JsonUtility.ToJson(nodeC);

    finishGame = "/rooms/"+ currentGame.room_id + "/games"; 

    
    Debug.Log("criou game no Node");
    string response2 = await PostProtectedData(startGame, json);
    Debug.Log(response2);
   
    string response = await GetProtectedData(finishGame); // Agora podemos esperar a função terminar

    Debug.Log("Dados recebidos: " + response);
    SendMessageWs(nodeC);
    }

    public async Task JoinGame(){

    NodeC nodeC = new NodeC();
    nodeC.room_id = currentGame.room_id;
    nodeC.player = currentPlayer.profile;
    nodeC.type = "join_room";

    string json = JsonUtility.ToJson(nodeC);

    SendMessageWs(nodeC);
    Debug.Log("entrou game no Node");

    EsperarEExecutar(5.0f);
    type = "refresh_game";
    finishGame = "/rooms/"+ currentGame.room_id + "/games"; 

    //finishGame = "/games/"+ currentGame.id;

    string response = await GetProtectedData(finishGame); // Agora podemos esperar a função terminar

    Debug.Log("Dados recebidos: " + response);

    }

    public async Task RefreshGame(){

    NodeC nodeC = new NodeC();
    nodeC.room_id = currentGame.room_id;
    nodeC.player = currentPlayer.profile;
    nodeC.type = type = "refresh_room";

    string json = JsonUtility.ToJson(nodeC);

    
    type = "refresh_game";
    finishGame = "/rooms/"+ currentGame.room_id + "/games"; 

    //finishGame = "/games/"+ currentGame.id;

    string response = await GetProtectedData(finishGame); // Agora podemos esperar a função terminar

    Debug.Log("Dados recebidos: " + response);
    SendMessageWs(nodeC);
    }

    /* public async Task GetWinner(){
        type = "showGame";
        finishGame = "/games/"+ currentGame.id;
        string response = await GetProtectedData(finishGame); // Agora podemos esperar a função terminar

        Debug.Log("Dados recebidos: " + response);
    } */
    public async Task GetProfile(){
        type = "profile";
        string response = await GetProtectedData(profile); // Agora podemos esperar a função terminar

        Debug.Log("Dados recebidos: " + response);
    }

   /*  public async Task GetRooms(){
        type = "rooms";
        string response = await GetProtectedData(listRooms); // Agora podemos esperar a função terminar

        Debug.Log("Dados recebidos: " + response);
    } */

      public void ChangeRoom(string json)
    {       
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
      


     async Task<string> GetProtectedData(string url)
    {
       // string url = "https://suaapi.com/dados-protegidos"; // Endpoint protegido

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + url))
        {
            request.SetRequestHeader("Authorization", "Bearer " + currentPlayer.token);

             var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield(); // Aguarda a conclusão sem bloquear a Unity
            }

            if (request.result == UnityWebRequest.Result.Success)
            {               
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Resposta recebida: " + jsonResponse);
                switch(type){

                    case "profile":
                    LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonResponse);
                    //player.token = response.token;
                    currentPlayer.profile = response.profile;
                    currentPlayer.success = response.success;
                    screenName.text = currentPlayer.profile.name;
                    screenRole.text = currentPlayer.profile.role;

                    if(currentPlayer.profile.role == "dealer"){
                        dealerBoard.SetActive(true);
                    }
                    if(currentPlayer.profile.role == "user"){
                        userBoard.SetActive(true);
                    }
                    //yield return response;
                    break;

                    case "rooms":
                    roomResponse = JsonUtility.FromJson<RoomResponse>(jsonResponse);  
                     

                    break;

                    case "refresh_game":
                    GameResponse game = JsonUtility.FromJson<GameResponse>(jsonResponse);  
                    currentGame = game.games.data[0];
                    Debug.Log("resultado do get game" + game.games.data[0]);
                    Debug.Log("current game" + currentGame.id);
                  /*   Debug.Log("primario :" + currentGame.id + currentGame.room_id);
                    string jsonForm = JsonUtility.ToJson(currentGame);
                    Debug.Log("secundario :" + jsonForm); */
                    if(currentPlayer.profile.role == "user"){

                    if(currentGame.card_winner != 0){ // chegou no game e ja tem a carta vencedora
                        Debug.Log("2x2");
                        playerSelector.SetState(PlayerSelector.GameState.WinnerRevealed);
                        return "fim do game, foi encontrada a card_winner";
                       // await JoinGame();
                    }    

                    if(currentGame.card_left != 0 || currentGame.card_right != 0){ //chegou no game e as cartas já foram escolhidas
                    playerSelector.SetState(PlayerSelector.GameState.CardsReceived);
                    Debug.Log("1x1"); 
                    return "meio do game, foram encontradas card_left e card_right";

                    }

                    if(currentGame.card_left == 0 || currentGame.card_right == 0){ //chegou no game e as cartas ainda n foram escolhidas
                    playerSelector.SetState(PlayerSelector.GameState.WaitingForStart);
                    Debug.Log("0x0");
                    return "começo do game, não foram encontradas card_left e card_right";

                    }

                    

                   
                    
                    }
                    break;

                    default: 
                    break;
                
                }
                return "foi";
            }

            else
            {
                Debug.LogError("Erro ao acessar endpoint protegido: " + request.error);
                 return null;
            }
        }
    }

    async Task<string> PostProtectedData(string urlAd, string jsonData)
    { 
        // Endpoint protegido
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData); // converte json pra byte
    

        
        using (UnityWebRequest request = new UnityWebRequest(apiUrl + urlAd, "POST"))
        {
            
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + currentPlayer.token);
            Debug.Log("mandou pra api");
            //string response = request.SendWebRequest().ToString();

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                    {
                        await Task.Yield(); // Aguarda a conclusão sem bloquear a Unity
                    }

            if (request.result == UnityWebRequest.Result.Success)
            {
                 string jsonResponse = request.downloadHandler.text;

            try
            {
                RoomResponse response = JsonUtility.FromJson<RoomResponse>(jsonResponse);
                Debug.Log(response);
                return "foi";
            }
            catch (System.Exception ex)
            {
                // Debug.LogError("Erro ao desserializar JSON: " + ex.Message);
                return "erro_json";
            }
                 
            }
            else
            {
                Debug.LogError("Erro ao acessar endpoint protegido: " + request.result);
                 return "null";
            }
        }
    }

    async Task<string> PutProtectedData(string urlAd, string jsonData)
    { 
        // Endpoint protegido
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData); // converte json pra byte
    

        
        using (UnityWebRequest request = new UnityWebRequest(apiUrl + urlAd, "PUT"))
        {
            
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + currentPlayer.token);
            Debug.Log("mandou pra api");
            //string response = request.SendWebRequest().ToString();

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                    {
                        await Task.Yield(); // Aguarda a conclusão sem bloquear a Unity
                    }

            if (request.result == UnityWebRequest.Result.Success)
            {
                 string jsonResponse = request.downloadHandler.text;
                 Debug.Log(jsonResponse);
            try
            {
                RoomResponse response = JsonUtility.FromJson<RoomResponse>(jsonResponse);
                Debug.Log(response);
                return "foi";
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Erro ao desserializar JSON: " + ex.Message);
                return "erro_json";
            }
                 
            }
            else
            {
                Debug.LogError("Erro ao acessar endpoint protegido: " + request.error);
                 return "null";
            }
        }
    }
      

    IEnumerator PostRequest(string url, string jsonData)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Erro: " + request.error);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonResponse);
                currentPlayer.token = response.token; // Armazena o token JWT
                //player.user = response.user;
                Debug.Log("Token recebido: " + token);
                Debug.Log("Resposta: " + response.success + request.downloadHandler.text);
            }
        }
    }

    IEnumerator DeleteRequest(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Erro: " + request.error);
            }
            else
            {
                Debug.Log("Item deletado com sucesso!");
            }
        }
    }

   
    /* public void dogmataDados(string nameF, int scoreF){

        PlayerData player = new PlayerData();
        player.name = nameF;

        string jsonForm = JsonUtility.ToJson(player);
        PostRequest(apiUrl, jsonForm);
    }  */


    //CONTRUTORES

    [System.Serializable]
    public class CardData
    {
        public string type;
        public string card_left;
        public string card_right;
    }

    [System.Serializable]
    public class PlayerData
    {
        public string id;
        public string name;
        public string email;
        public string password;
        public string balance;
        public string role = "admin";
        public string image_profile = "string";

    }
    [System.Serializable]
    public class LoginResponse
    {
        public bool success;
        public PlayerData profile;
        public string token;
    }
    [System.Serializable]
    public class Card
    {
        public int id = 0;
        public string name;
        public string slug;
    }

    [System.Serializable]
    public class Room
    {
        public int id;
        public int room_id;
        public int user_id;
        public string live_url;
        public string created_at;
        public string updated_at;
    }

    

    [System.Serializable]
    public class RoomResponse
    {
        public bool success;
        public Room[] rooms;
    }
    [System.Serializable]
    public class RoomChange
    {
        public int room;
        public string token;
        public string role;
    }

     [System.Serializable]
    public class Games
    {
        public int current_page;
        public Game[] data;

    }

    [System.Serializable]
    public class Game
    {
        public int id;
        public int card_left;
        public string card_left_name;
        public string card_left_slug;
        public int card_right;
        public string card_right_name;
        public string card_right_slug;
        public int card_winner=10000;
        public int user_id;
        public int room_id;
        public string finished_at;
        public string created_at;
        public string updated_at;  
        public string type;  

    }

    [System.Serializable]
    public class GameResponse
    {
        public bool success;
        public Games games;
    }

    [System.Serializable]
    public class ReactResponse
    {
        public string token;
        public int room_id;
    }

    [System.Serializable]
    public class Bet
    {   
        public int id;
        public int game_id;
        public int user_id;
        public int room_id;
        public int card_id;
        public float value;
        public string created_at;
        public string updated_at;
    }    

    [System.Serializable]
    public class BetResponse
    {
        public bool success;
        public string message;
        public Bet bet;
    }
    [System.Serializable]
    public class NodeC
    {   
        public int id;
        public int game_id;
        public int user_id;
        public int room_id;
        public int room;
        public int card_id;
        public string type;
        public string dealerName;
        public PlayerData player;
    }    


   
}
/* 

const WebSocket = require("ws");

const server = new WebSocket.Server({ port: 3000 });
let rooms = {}; // Armazena as salas
let player = { new   
        var id;
        var name;
        var string balance;
        var role;
};
server.on("connection", (ws) => {
    console.log("Novo jogador conectado!");

    ws.on("message", (message) => {
        const data = JSON.parse(message);

        if (data.type === "create_room") {
            let { roomId, dealerName } = data;
            if (rooms[roomId]) {
                ws.send(JSON.stringify({ type: "error", message: "Sala já existe!" }));
                return;
            }

            rooms[roomId] = { dealer: ws, players: [] };
            ws.send(JSON.stringify({ type: "room_created", roomId, message: "Você é o Dealer!" }));
            console.log(`Dealer ${dealerName} criou a sala ${roomId}`);
        }

        if (data.type === "join_room") {
            if (!rooms[data.room]) {
                ws.send(JSON.stringify({ type: "error", message: "Sala não encontrada!" }));
                return;
            }

            rooms[data.room].players.push(ws);
            ws.send(JSON.stringify({ type: "joined", data.room, message: "Você entrou na sala!" }));
            console.log(`${playerName} entrou na sala ${roomId}`);
        }

        if (data.type === "deal_card") {
            let { roomId, card } = data;
            if (!rooms[roomId] || rooms[roomId].dealer !== ws) {
                ws.send(JSON.stringify({ type: "error", message: "Você não é o Dealer!" }));
                return;
            }

            rooms[data.room].players.forEach(player => {
                player.send(JSON.stringify({ type: "receive_card", card }));
            });

            console.log(`Dealer enviou carta ${card} para todos na sala ${roomId}`);
        }
    });

    ws.on("close", () => {
        console.log("Um jogador desconectou");
    });
});

console.log("Servidor WebSocket rodando na porta 3000");
 */