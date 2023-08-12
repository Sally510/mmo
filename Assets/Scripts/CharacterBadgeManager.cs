using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterBadgeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text characterName; 
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private ProgressBar characterHealthBar;
    [SerializeField] private ProgressBar expBar;
    private void Start()
    {
        characterName.text = State.LoggedCharacter.option.Name;
        //expBar.SetHealth((int)State.LoggedCharacter.option.Experience);
    }


}
