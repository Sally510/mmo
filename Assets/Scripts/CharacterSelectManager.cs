using Assets.Scripts.Client;
using Assets.Scripts.Client.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class CharacterSelectManager : MonoBehaviour
    {
        public void OnPlayClick()
        {
            StartCoroutine(Play(State.CharacterOptions[0].Id));
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