using Assets.Scripts.Client;
using Assets.Scripts.Client.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CharacterSelectManager : MonoBehaviour
    {
        public GameObject buttonPrefab;
        public GameObject buttonParent;
        private long? _selectedCharacterId;

        private void Awake()
        {
            _selectedCharacterId = null;
            foreach (var characterOption in State.CharacterOptions)
            {
                GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
                newButton.GetComponent<CharacterButton>().characterText.text = characterOption.Name;
                newButton.GetComponent<Button>().onClick.AddListener(() => SelectCharacter(characterOption.Id));
            }
        }

        private void SelectCharacter(long characterId)
        {
            if (characterId <= 0)
            {
                _selectedCharacterId = null;
            } else
            {
                _selectedCharacterId = _selectedCharacterId != characterId ? characterId : null;
            }
        }

        public void OnPlayClick()
        {
            if (_selectedCharacterId.HasValue)
            {
                StartCoroutine(Play(_selectedCharacterId.Value));
            }
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
        
        public void OnDeleteClick()
        {
            if (_selectedCharacterId.HasValue)
            {
                StartCoroutine(Delete(_selectedCharacterId.Value));
            }
        }

        IEnumerator Delete(long characterId) 
        {
            yield return ClientManager.DeleteCharacter(characterId, model =>
            {
                if(model.Success)
                {
                    State.CharacterOptions = model.CharacterList;
                    //TODO: refresh the character buttons displayed
                } else
                {
                    //TODO: write an error that the deletion wasn't successful
                }
            });
        }

        public void OnCreateCharacterClick()
        {
            SceneManager.LoadScene("CreateCharacterScene");
        }



    }
}