using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject Panel;
    
    public void TogglePanel()
    {
        if(Panel != null)
        {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }
    }

    public void LogoutButton_Click()
    {
        SceneManager.LoadScene("LoginScene");

    }

    public void SelectButton_Click()
    {
        SceneManager.LoadScene("CharacterSelectScene");
    }

    public void ContinueButton_Click()
    {
        TogglePanel();
    }
}
