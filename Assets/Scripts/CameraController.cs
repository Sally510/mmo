using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts
{
    public class CameraController: MonoBehaviour
    {
        Transform followTarget;
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
        }
    }
}
