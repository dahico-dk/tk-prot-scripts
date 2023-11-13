using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    class AutoDestroy:MonoBehaviour
    {                      
        public void StartCountDown(float counter,GameObject victim)
        {
           StartCoroutine(Disabler(victim, counter));
        }

        private IEnumerator Disabler(GameObject victim, float counter)
        {
            yield return new WaitForSecondsRealtime(counter);
            victim.SetActive(false);
        }
    }
}
