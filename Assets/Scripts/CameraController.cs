using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour
    {
        Transform followTarget;
        public Tilemap tileMap;
        private Vector3 targetPos;
        public float moveSpeed;

        void Start()
        {
            followTarget = GameObject
                .FindGameObjectWithTag("Player")
                .GetComponent<Transform>();
        }

        void Update()
        {
            if (followTarget != null)
            {
                targetPos = new Vector3(followTarget.position.x, followTarget.position.y, transform.position.z);
                Vector3 velocity = (targetPos - transform.position) * moveSpeed;
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1.0f, Time.deltaTime);
            }

            if (Input.GetButtonDown("Fire1"))
            {
                Vector3 mousePos = Input.mousePosition;
                {
                    var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                    Debug.Log($"{worldPos.x} {worldPos.y}");
                    //var cell = tileMap.WorldToCell(worldPos);
                    //Debug.Log($"{cell.x} {cell.y}");
                }
            }
        }
    }
}
