using Assets.Scripts.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static Assets.Scripts.Client.Models.LoginModel;

namespace Assets.Scripts
{
    public class LoginManager : MonoBehaviour
    {
        public TMP_InputField EmailInputField;
        public TMP_InputField PasswordInputField;
        public TMP_Text ErrorLabel;

        private void Start()
        {
            ConfigurationManager.Clear();
            State.Clear();

            if(ConfigurationManager.Config.DefaultLogin != null)
            {
                EmailInputField.text = ConfigurationManager.Config.DefaultLogin.Username;
                PasswordInputField.text = ConfigurationManager.Config.DefaultLogin.Password;
            }
            
            ErrorLabel.text = string.Empty;
        }

        public void OnLoginClick()
        {
            ErrorLabel.text = string.Empty;

            StartCoroutine(Login(EmailInputField.text, PasswordInputField.text));
        }

        IEnumerator Login(string email, string password)
        {

            string json = JsonUtility.ToJson(new LoginRequest
            {
                email = email,
                password = password
            });

            UnityWebRequest www = UnityWebRequest.Post(ConfigurationManager.Config.AuthHost + "/api/auth/login", json, "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
                if (response.ok)
                {
                    yield return ClientManager.Login(response.payload.accessToken, loginModel =>
                    {
                        switch (loginModel.Status)
                        {
                            case StatusType.Success:
                                State.CharacterOptions = loginModel.CharacterList;
                                SceneManager.LoadScene("CharacterSelectScene");
                                break;
                            case StatusType.AlreadyLoggedIn:
                                ErrorLabel.text = "User already logged in.";
                                break;
                            case StatusType.InvalidInformation:
                                ErrorLabel.text = "Invalid information.";
                                break;
                            default:
                                break;
                        }
                    });

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
}