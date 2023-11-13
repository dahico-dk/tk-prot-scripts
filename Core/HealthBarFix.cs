using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarFix : MonoBehaviour
{
    GameObject maincam;

    // Start is called before the first frame update
    void Start()
    {
        maincam = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(maincam.transform);
    }
}
