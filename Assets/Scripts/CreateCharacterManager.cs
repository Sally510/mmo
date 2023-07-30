using Assets.Scripts;
using Assets.Scripts.Client;
using Assets.Scripts.Client.Models;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateCharacterManager : MonoBehaviour
{
    public TMP_InputField NameInputField;
    public TMP_Text ErrorLabel;

    public void BackButton_Click()
    {
        SceneManager.LoadScene("CharacterSelectScene");
    }

    public async void OnCreateClick()
    {
        ErrorLabel.text = string.Empty;

        string name = NameInputField.text;
        if (!string.IsNullOrEmpty(name))
        {
            var response = await ClientManager.CreateCharacterAsync(name, destroyCancellationToken);
            switch (response.Status)
            {
                case CreateCharacterModel.StatusType.Success:
                    State.CharacterOptions = response.CharacterList;
                    SceneManager.LoadScene("CharacterSelectScene");
                    break;
                case CreateCharacterModel.StatusType.NameExists:
                    ErrorLabel.text = "Name already exists";
                    break;
                case CreateCharacterModel.StatusType.TooManyCharacters:
                    ErrorLabel.text = "No empty character slots left";
                    break;
                case CreateCharacterModel.StatusType.Unknown:
                default:
                    ErrorLabel.text = "Unknown error";
                    break;
            }
        }
        else
        {
            ErrorLabel.text = "Enter the character name";
        }
    }
}
