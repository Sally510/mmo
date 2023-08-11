using Assets.Scripts.Client;
using Assets.Scripts.Configuration;
using Firebase;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Assets.Scripts.Client.Models.LoginModel;

namespace Assets.Scripts
{
    public class LoginManager : MonoBehaviour
    {
        private FirebaseAuth auth;
        private FirebaseUser user;

        public TMP_InputField EmailInputField;
        public TMP_InputField PasswordInputField;
        public TMP_Text ErrorLabel;

        async void Awake()
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Setting up Firebase Auth");
                InitializeFirebase();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase depenencies: {dependencyStatus}");
            }
        }

        async void InitializeFirebase()
        {
            auth = FirebaseAuth.DefaultInstance;
            bool loginSuccess = false;
            if (auth.CurrentUser != null && auth.CurrentUser.IsValid())
            {
                string token = await auth.CurrentUser.TokenAsync(false);
                var loginModel = await ClientManager.LoginAsync(token, destroyCancellationToken);
                switch (loginModel.Status)
                {
                    case StatusType.Success:
                        State.CharacterOptions = loginModel.CharacterList;
                        SceneManager.LoadScene("CharacterSelectScene");
                        loginSuccess = true;
                        break;
                    case StatusType.AlreadyLoggedIn:
                        ErrorLabel.text = "User already logged in.";
                        break;
                    case StatusType.BadToken:
                        ErrorLabel.text = "Invalid information.";
                        break;
                    default:
                        break;
                }
               
            }
            //SceneManager.LoadScene("CharacterSelectScene");

            //if(loginSuccess)
            //{
            //    UserButton.gameObject.SetActive(true);
            //    UserButton.GetComponentInChildren<TMP_Text>().text = auth.CurrentUser.UserId[..7] + "...";
            //    SignInButton.gameObject.SetActive(false);
            //}
            //else
            //{
            //    UserButton.gameObject.SetActive(false);
            //    SignInButton.gameObject.SetActive(true);
            //}
            //auth.StateChanged += AuthStateChanged;
            //AuthStateChanged(this, null);
        }

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