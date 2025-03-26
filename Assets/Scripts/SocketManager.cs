using System.Collections;
using System.Collections.Generic;
using System.Text;
using NativeWebSocket;
using UnityEngine;

public class SocketManager : MonoBehaviour
{
    
    public WebSocket websocket;
    public string serverUrl = "ws://SEU-IP-PUBLICO:3000";
    public string roomId = "partida1";
    public string playerName = "Jogador1";
    private bool isDealer = false;
 
    async void Start()
    {
        websocket = new WebSocket(serverUrl);

        websocket.OnOpen += () =>
        {
            Debug.Log("Conectado ao servidor WebSocket!");

            if (isDealer)
            {
                SendMessage(new { type = "create_room", roomId, dealerName = playerName });
            }
            else
            {
                SendMessage(new { type = "join_room", roomId, playerName });
            }
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = Encoding.UTF8.GetString(bytes);
            Debug.Log("Mensagem recebida: " + message);
        };

        websocket.OnError += (error) =>
        {
            Debug.LogError("Erro no WebSocket: " + error);
        };

        websocket.OnClose += (close) =>
        {
            Debug.Log("Conexão fechada.");
        };

        await websocket.Connect();
    }

    public async void SendMessage(object data)
    {
        string json = JsonUtility.ToJson(data);
        await websocket.SendText(json);
    }

    public void SetAsDealer()
    {
        isDealer = true;
    }

    public void SendCard(string card)
    {
        if (isDealer)
        {
            SendMessage(new { type = "deal_card", roomId, card });
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
        await websocket.Close();
        }
    } 
}
