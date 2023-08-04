using Assets.Scripts.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        Client.Instance.SetEnabled(true);
    }

    private void Reset()
    {
        Client.Instance.SetEnabled(false);
    }
}
