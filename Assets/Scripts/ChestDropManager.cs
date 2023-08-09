using Assets.Scripts.Client;
using Assets.Scripts.Client.Models;
using Assets.Scripts.Helpers;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class ChestDropManager : MonoBehaviour
    {
        private void PacketEventHandler_ChestDropEvent(object sender, ChestDropModel e)
        {
            Vector3 worldPosition = e.IsoPosition.FromIsoToWorld();

            Debug.Log("New chest drop");
        }

        private void PacketEventHandler_ChestItemsEvent(object sender, ChestItemsModel e)
        {
            foreach(ChestItemsModel.DroppedItemModel item in e.Items)
            {
                //TODO: add items to opened chest
            }

            //TODO: open chest
        }

        void OnEnable()
        {
            PacketEventHandler.ChestDropEvent += PacketEventHandler_ChestDropEvent;
            PacketEventHandler.ChestItemsEvent += PacketEventHandler_ChestItemsEvent;
        }

        void OnDisable()
        {
            PacketEventHandler.ChestDropEvent -= PacketEventHandler_ChestDropEvent;
            PacketEventHandler.ChestItemsEvent -= PacketEventHandler_ChestItemsEvent;
        }
    }
}