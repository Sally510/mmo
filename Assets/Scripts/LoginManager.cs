using Assets.Scripts.Client;
using Assets.Scripts.Configuration;
using Firebase;
using Firebase.Auth;
using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Assets.Scripts.Client.Models.LoginModel;

namespace Assets.Scripts
{
    public class LoginManager : MonoBehaviour
    {
        [Header("Login Settings")]
        public GameObject loginPanel;
        public TMP_InputField emailInputField;
        public TMP_InputField passwordInputField;
        public TMP_Text loginErrorLabel;

        [Header("Logged-In Settings")]
        public GameObject loggedInPanel;
        public Button characterButton;
        public TMP_Text loggedInErrorLabel;

        private FirebaseAuth auth;
        private FirebaseUser user;

        

        async void Awake()
        {
            ConfigurationManager.Clear();
            State.Clear();

            if (ConfigurationManager.Config.DefaultLogin != null)
            {
                emailInputField.text = ConfigurationManager.Config.DefaultLogin.Username;
                passwordInputField.text = ConfigurationManager.Config.DefaultLogin.Password;
            }

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

        void InitializeFirebase()
        {
            auth = FirebaseAuth.DefaultInstance;
            if (auth.CurrentUser != null && auth.CurrentUser.IsValid())
            {
                LoadLoggedInPanel(auth.CurrentUser.Email);
            } else
            {
                LoadLogInPanel();
            }

            //auth.StateChanged += AuthStateChanged;
            //AuthStateChanged(this, null);
        }

        void LoadLogInPanel()
        {
            loginErrorLabel.text = string.Empty;
            loginPanel.SetActive(true);
            loggedInPanel.SetActive(false);
        }

        void LoadLoggedInPanel(string name)
        {
            loggedInErrorLabel.text = string.Empty;
            loginPanel.SetActive(false);
            loggedInPanel.SetActive(true);
            characterButton.GetComponentInChildren<TMP_Text>().text = name;
        }

        public async void OnLoggedInClick()
        {
            loggedInErrorLabel.text = await LogIn();
        }

        public void OnLogoutClick()
        {
            FirebaseAuth.DefaultInstance.SignOut();
            LoadLogInPanel();
        }

        private async Task<string> LogIn()
        {
            string token = await auth.CurrentUser.TokenAsync(false);
            var loginModel = await ClientManager.LoginAsync(token, destroyCancellationToken);
            switch (loginModel.Status)
            {
                case StatusType.Success:
                    State.CharacterOptions = loginModel.CharacterList;
                    SceneManager.LoadScene("CharacterSelectScene");
                    return null;
                case StatusType.AlreadyLoggedIn:
                    return "User already logged in.";
                case StatusType.BadToken:
                    return "Invalid information.";
            }
            return "Unknown error";
        }

        public async void OnLoginClick()
        {
            loginErrorLabel.text = string.Empty;

            try
            {
                AuthResult result = await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(emailInputField.text, passwordInputField.text);
                loginErrorLabel.text = await LogIn();
            }
            catch (FirebaseException firebaseEx)
            {
                Debug.LogException(firebaseEx);

                switch ((AuthError)firebaseEx.ErrorCode)
                {
                    case AuthError.MissingEmail:
                        loginErrorLabel.text = "Missing email";
                        break;
                    case AuthError.MissingPassword:
                        loginErrorLabel.text = "Missing password";
                        break;
                    case AuthError.WeakPassword:
                        loginErrorLabel.text = "Weak password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        loginErrorLabel.text = "Email already in use";
                        break;
                }
            }
            catch (AggregateException aggEx)
            {
                foreach (var e in aggEx.Flatten().InnerExceptions)
                {
                    if (e is FirebaseException fEx)
                    {
                        Debug.LogException(fEx);
                    }
                    else
                    {
                        Debug.LogException(e);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void OnCreateAccount()
        {
            SceneManager.LoadScene("CreateAccountScene");
        }
    }
}