using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class PickupManager : MonoBehaviour
    {
        [SerializeField]GameObject[] pickups;
        private void Start()
        {
            //Health health = new Health();
            //health.DieMOFO += ArrangePickup;
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    var target = GameObject.FindGameObjectWithTag("Player");
            //    var dest = new Vector3(target.transform.position.x + 5, 0, target.transform.position.z + 5);
            //    var rndpickup = pickups[UnityEngine.Random.Range(0, pickups.Length)];
            //    Instantiate(rndpickup, dest, Quaternion.identity);
            //}
        }

        private void ArrangePickup(ObjectRef refe)
        {
            if (refe.tag=="Enemy")
            {
                if ((UnityEngine.Random.Range(0, 100)>70+refe.DropRateBonus)&&pickups.Length>0)
                {
                    var rndpickup = pickups[UnityEngine.Random.Range(0, pickups.Length)];
                    Instantiate(rndpickup, refe.lastLocation, Quaternion.identity);
                }
            }
        }
    }
}
