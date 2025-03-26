using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class JWTdecoder : MonoBehaviour
{
    public static string DecodePayload(string token)
    {
        if (string.IsNullOrEmpty(token)) return null;

        string[] parts = token.Split('.');
        if (parts.Length < 2) return null;

        string payload = parts[1]; // Pega a parte do meio (Payload)
        payload = payload.Replace('-', '+').Replace('_', '/'); // Corrige formato Base64
        switch (payload.Length % 4) // Ajusta padding
        {
            case 2: payload += "=="; break;
            case 3: payload += "="; break;
        }

        byte[] decodedBytes = Convert.FromBase64String(payload);
        string decodedString = Encoding.UTF8.GetString(decodedBytes);
        return decodedString;


       /*  string payload = JWTDecoder.DecodePayload(token);
        Debug.Log("Payload decodificado: " + payload); */
    }
}
