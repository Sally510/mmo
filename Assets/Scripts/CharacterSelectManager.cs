using Assets.Scripts.Client;
using Assets.Scripts.Client.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
        public TMP_Text ErrorLabel;
        private long? _selectedCharacterId;
        [SerializeField] private ConfirmationWindow _confirmationWindow;

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

        public async void OnPlayClick()
        {
            if (_selectedCharacterId.HasValue)
            {
                var response = await ClientManager.LoginCharacterAsync(_selectedCharacterId.Value, destroyCancellationToken);
                if (response.Success)
                {
                    State.InventoryItems = (await ClientManager.GetInventoryItemsAsync(destroyCancellationToken)).Items;
                    State.EquippedItems = (await ClientManager.GetEquippedItemsAsync(destroyCancellationToken)).Items;
                    State.LoggedCharacter = (response.SelectedCharacter, State.CharacterOptions.First(x => x.Id == _selectedCharacterId.Value));
                    SceneManager.LoadScene("GameScene");
                }
            }
        }

        private void OpenConfirmationWindow(string message)
        {
            _confirmationWindow.gameObject.SetActive(true);
            _confirmationWindow.yesButton.onClick.AddListener(OnYesClick);
            _confirmationWindow.cancelButton.onClick.AddListener(OnCancelClick);
            _confirmationWindow.messageText.text = message;
        }

        private async void OnYesClick()
        {
            _confirmationWindow.gameObject.SetActive(false);
            if (_selectedCharacterId.HasValue)
            {
                var response = await  ClientManager.DeleteCharacterAsync(_selectedCharacterId.Value, destroyCancellationToken);
                if (response.Success)
                {
                    State.CharacterOptions = response.CharacterList;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                else
                {
                    ErrorLabel.text = "Deletion failed.";
                }
            }
        }

        private void OnCancelClick()
        {
            _confirmationWindow.gameObject.SetActive(false);
        }

        public void OnDeleteClick()
        {
            OpenConfirmationWindow("Are you sure you wanna delete the character?");
        }

        public void OnCreateCharacterClick()
        {
            SceneManager.LoadScene("CreateCharacterScene");
        }



    }
}