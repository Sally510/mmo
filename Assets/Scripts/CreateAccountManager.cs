using Assets.Scripts.Client;
using Firebase;
using Firebase.Auth;
using System;
using System.Collections;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class CreateAccountManager : MonoBehaviour
    {
        public TMP_InputField emailField;
        public TMP_InputField passwordField;
        public TMP_InputField confirmPasswordField;
        public TMP_Text errorLabel;

        async void Awake()
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Setting up Firebase Auth");
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase depenencies: {dependencyStatus}");
            }
        }

        public async void OnCreate()
        {
            
            errorLabel.text = string.Empty;

            string email = emailField.text;
            if(!IsEmailValid(email))
            {
                errorLabel.text = "Enter a valid email address";
                return;
            }

            string password = passwordField.text;
            string confirmPassword = confirmPasswordField.text;
            if (string.IsNullOrEmpty(password))
            {
                errorLabel.text = "Enter a password";
                return;
            }

            if(password != confirmPassword)
            {
                errorLabel.text = "Passwords do not match";
                return;
            }

            if(password.Length < 8)
            {
                errorLabel.text = "Password has to be atleast 8 characters long";
                return;
            }

            try
            {
                AuthResult result = await FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password);
                Debug.Log($"New user created with email {result.User.Email} and id {result.User.UserId}");
                SceneManager.LoadScene("LoginScene");
            } 
            catch(FirebaseException firebaseEx)
            {
                Debug.LogException(firebaseEx);

                switch ((AuthError)firebaseEx.ErrorCode)
                {
                    case AuthError.MissingEmail:
                        errorLabel.text = "Missing email";
                        break;
                    case AuthError.MissingPassword:
                        errorLabel.text = "Missing password";
                        break;
                    case AuthError.WeakPassword:
                        errorLabel.text = "Weak password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        errorLabel.text = "Email already in use";
                        break;
                }
            }
            catch (AggregateException aggEx)
            {
                foreach (var e in aggEx.Flatten().InnerExceptions)
                {
                    Debug.LogException(e);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void OnBackButton()
        {
            SceneManager.LoadScene("LoginScene");
        }

        private static bool IsEmailValid(string email)
        {
            var valid = true;

            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }

            return valid;
        }
    }
}