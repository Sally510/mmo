using Assets.Scripts.Client;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        Client.Instance.SetProcessPacketState(true);
    }
}
