using Assets.Scripts.Client;
using Assets.Scripts.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TMPro;
using UnityEngine;
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

            if (ConfigurationManager.Config.DefaultLogin != null)
            {
                EmailInputField.text = ConfigurationManager.Config.DefaultLogin.Username;
                PasswordInputField.text = ConfigurationManager.Config.DefaultLogin.Password;
            }

            ErrorLabel.text = string.Empty;
        }

        public async void OnLoginClick()
        {
            ErrorLabel.text = string.Empty;

            using HttpClient client = new(new HttpClientHandler
            {
                Proxy = System.Net.WebRequest.DefaultWebProxy
            })
            {
                BaseAddress = new Uri(ConfigurationManager.Config.AuthHost)
            };

            string json = JsonUtility.ToJson(new LoginRequest { email = EmailInputField.text, password = PasswordInputField.text });
            using StringContent requestContent = new(json, Encoding.UTF8, "application/json");

            using var authResponse = await client.PostAsync(
                requestUri: "/api/auth/login",
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

        public void OnCreateAccount()
        {
            SceneManager.LoadScene("CreateAccountScene");
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