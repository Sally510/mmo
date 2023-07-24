using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraController: MonoBehaviour
    {
        private Transform target;
        [SerializeField] private float smoothSpeed;
        //[SerializeField] private float minX, maxX, minY, maxY;

        private void Awake()
        {
            target = GameObject
                .FindGameObjectWithTag("Player")
                .GetComponent<Transform>();
        }

        private void LateUpdate()
        {
            //Debug.Log(transform.position);
            //transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

            transform.position = Vector3.Lerp(
                transform.position,
                new Vector3(target.position.x, target.position.y, transform.position.z),
                smoothSpeed * Time.deltaTime);

            //transform.position = new Vector3(
            //    Mathf.Clamp(transform.position.x, minX, maxX),
            //    Mathf.Clamp(transform.position.y, minY, maxY),
            //    transform.position.z);
        }
    }
}
