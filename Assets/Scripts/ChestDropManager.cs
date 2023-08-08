using Assets.Scripts.Client;
using Assets.Scripts.Helpers;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class ChestDropManager : MonoBehaviour
    {
        private void PacketEventHandler_ChestDropEvent(object sender, Client.Models.ChestDropModel e)
        {
            Vector3 worldPosition = e.IsoPosition.FromIsoToWorld();

            Debug.Log("New chest drop");
        }

        void OnEnable()
        {
            PacketEventHandler.ChestDropEvent += PacketEventHandler_ChestDropEvent;  
        }

        void OnDisable()
        {
            PacketEventHandler.ChestDropEvent -= PacketEventHandler_ChestDropEvent;
        }
    }
}