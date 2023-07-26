using Assets.Scripts.Client;
using Assets.Scripts.Client.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CharacterSelectManager : MonoBehaviour
    {
        public GameObject buttonPrefab;
        public GameObject buttonParent;
        private int _selectedCharacterId;

        private void Awake()
        {
            foreach (var characterOption in State.CharacterOptions)
            {
                GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
                newButton.GetComponent<CharacterButton>().characterText.text = characterOption.Name;
                newButton.GetComponent<Button>().onClick.AddListener(() => SelectCharacter((int)characterOption.Id));
            }
        }

        private void SelectCharacter(int characterId)
        {
            _selectedCharacterId = characterId;
        }

        public void OnPlayClick()
        {
            StartCoroutine(Play(_selectedCharacterId));
        }

        IEnumerator Play(long characterId)
        {
            yield return ClientManager.LoginCharacter(characterId, packet =>
            {
                if (packet.Success)
                {
                    State.LoggedCharacter = packet.SelectedCharacter;
                    SceneManager.LoadScene("GameScene");
                }
            });
        }
    }
}