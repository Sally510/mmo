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

    public void OnCreateClick()
    {
        ErrorLabel.text = string.Empty;

        string name = NameInputField.text;
        if (!string.IsNullOrEmpty(name))
        {
            StartCoroutine(Create(name));
        }
        else
        {
            ErrorLabel.text = "Enter the character name";
        }
    }

    IEnumerator Create(string name)
    {
        yield return ClientManager.CreateCharacter(name, model =>
        {
            switch (model.Status)
            {
                case CreateCharacterModel.StatusType.Success:
                    State.CharacterOptions = model.CharacterList;
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
        });
    }
}
