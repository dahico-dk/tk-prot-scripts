using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Movement
{
    public class PlayerRotation : MonoBehaviour
    {

        // Update is called once per frame
        Rigidbody rb;
       
        private void Start()
        {
            //Cursor.visible = false;
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            Plane plane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist;
            if (plane.Raycast(ray, out dist))
            {
               // Debug.DrawLine(transform.position, ray.GetPoint(dist));
                transform.LookAt(ray.GetPoint(dist));
            }
        }      
        void depr()
        {
            //print(Input.mousePosition);
            // Debug.DrawRay(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Ray ray2 = Camera.main.ScreenPointToRay(Camera.main.ScreenToWorldPoint(Input.mousePosition)); ;
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawLine(transform.position, hit.point);
                Debug.DrawLine(ray.origin, hit.point, Color.blue);
                var target = hit.point;
                Vector3 targetPostition = new Vector3(target.x,
                                        transform.position.y,
                                        target.z);

                transform.LookAt(targetPostition);
            }
        }

    }

}
