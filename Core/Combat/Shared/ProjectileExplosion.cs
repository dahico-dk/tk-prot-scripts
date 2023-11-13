using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplosion : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
       
    }
    public void StartCountDown()
    {
        StartCoroutine(FinalCountDown());
    }

   IEnumerator FinalCountDown()
    {
        yield return new WaitForSecondsRealtime(4f);
        gameObject.SetActive(false);
        yield return null;
    }
}
