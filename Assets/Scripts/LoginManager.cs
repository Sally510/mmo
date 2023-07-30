using Assets.Scripts.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
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

        public async void OnLoginClick()
        {
            ErrorLabel.text = string.Empty;

            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(2)
            };
            
            string json = JsonUtility.ToJson(new LoginRequest { email = EmailInputField.text, password = PasswordInputField.text });
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
            
            var authResponse = await client.PostAsync(
                requestUri: ConfigurationManager.Config.AuthHost + "/api/auth/login",
                content: requestContent,
                cancellationToken: destroyCancellationToken);


            if (!authResponse.IsSuccessStatusCode)
            {
                ErrorLabel.text = "Server error";
                return;
            }

            LoginResponse response = JsonUtility.FromJson<LoginResponse>(await authResponse.Content.ReadAsStringAsync());

            if (response.ok)
            {
                var loginModel = await ClientManager.LoginAsync(response.payload.accessToken, destroyCancellationToken);
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