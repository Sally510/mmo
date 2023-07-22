using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkingManager : MonoBehaviour
{
    private const string BASE_URL = "http://89.142.170.95";

    public TMP_InputField EmailInputField;
    public TMP_InputField PasswordInputField;

    public void OnLoginClick()
    {
        StartCoroutine(Login(EmailInputField.text, PasswordInputField.text));
    }

    IEnumerator Login(string email, string password)
    {
        
        string json = JsonUtility.ToJson(new LoginRequest
        {
            email = email,
            password = password
        });

        UnityWebRequest www = UnityWebRequest.Post(BASE_URL + "/api/auth/login", json, "application/json");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
            if(response.ok) {
                var builder = new PacketBuilder(PacketType.Login).SetBreakString(email).SetBreakString(password);
                Client.Instance.Send(builder);
                SceneManager.LoadScene("GameScene");
            }
        }
    }

    [Serializable]
    class LoginRequest
    {
        public string email;
        public string password;
    }

    [Serializable]
    class LoginResponse
    {
        public bool ok;
        public PayloadModel payload;
        public List<string> errors;

        [Serializable]
        public class PayloadModel
        {
            public string accessToken;
            public int accessExpiry;
            public string refreshToken;
            public int refreshExpiry;
        }
    }
}
