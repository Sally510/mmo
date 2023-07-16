using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkingManager : MonoBehaviour
{

    public TMP_InputField EmailInputField;
    public TMP_InputField PasswordInputField;

    public void OnLoginClick()
    {
        StartCoroutine(Login(EmailInputField.text, PasswordInputField.text));
    }

    IEnumerator Login(string email, string password)
    {
        
        string json = JsonUtility.ToJson(new LoginModel
        {
            email = email,
            password = password
        });

        UnityWebRequest www = UnityWebRequest.Post("http://89.142.170.95/api/auth/login", json, "application/json");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            SceneManager.LoadScene("GameScene");
        }
    }

    [Serializable]
    class LoginModel
    {
        public string email;
        public string password;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
