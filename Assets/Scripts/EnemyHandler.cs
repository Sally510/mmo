using UnityEngine;
using static Assets.Scripts.Client.PacketQueue;

namespace Assets.Scripts
{
    public class EnemyHandler : MonoBehaviour
    {
        [SerializeField]
        private GameObject enemyPrefab;


        void Start()
        {

        }

        async void Update()
        {
            PacketList list = await Client.Client.Instance.GetPacketQueueAsync(Client.PacketType.MonsterInfo, destroyCancellationToken);
        }
    }
}