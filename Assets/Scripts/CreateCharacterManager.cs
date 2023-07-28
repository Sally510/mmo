using System.Collections;
using System.Collections.Generic;
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

    }

    //IEnumerator Create(string name) {}
}
