using Assets.Scripts;
using Assets.Scripts.Helpers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterBadgeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text characterName; 
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text healthValue;
    [SerializeField] private TMP_Text expValue;
    [SerializeField] private ProgressBar characterHealthBar;
    [SerializeField] private ProgressBar expBar;

    private void Start()
    {
        characterName.text = State.LoggedCharacter.option.Name;
    }

    private void Update()
    {
        levelText.text = "LV" + State.LoggedCharacter.character.Level.ToString();
        characterHealthBar.SetMaxValue(State.LoggedCharacter.character.MaxHealth);
        characterHealthBar.SetValue(State.LoggedCharacter.character.Health);
        var (start, end) = ExperienceHelpers.CalculateExperienceRangeAtLevel(State.LoggedCharacter.character.Level);
        expBar.SetMaxValue((int)end - (int)start);
        expBar.SetValue((int)(State.LoggedCharacter.character.Experience - start));
        healthValue.text = State.LoggedCharacter.character.Health.ToString() + " / " + State.LoggedCharacter.character.MaxHealth.ToString();
        expValue.text = (State.LoggedCharacter.character.Experience - start).ToString() + " / " + ((int)end - (int)start).ToString();
    }
}
