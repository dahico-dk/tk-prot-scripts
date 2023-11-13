using Core.Movement.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateCompanion : MonoBehaviour
{
    [SerializeField] float lifeTime;
    float totalLife;
    private GameManager gmg;
    public bool targeted = false;
    Vector3 defaultScale;
    private void Awake()
    {
        defaultScale = transform.localScale;
    }

    void Start()
    {
        var mng = GameObject.FindGameObjectWithTag("GameManager");
        gmg = mng.GetComponent<GameManager>();
    }
    private void OnEnable()
    {
        RendererStatus(true);
        totalLife = 0;
    }

    private void RendererStatus(bool status)
    {
        var meshrenderer = GetComponentsInChildren<MeshRenderer>();
        if (meshrenderer.Length>0)
        {
            foreach (var item in meshrenderer)
            {
                item.enabled = status;
            }          
        }
        var skinnedmeshrenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (skinnedmeshrenderer.Length > 0)
        {
            foreach (var item in skinnedmeshrenderer)
            {
                item.enabled = status;
            }
        }
    }

    private void OnDisable()
    {
        totalLife = 0;
    }
    // Update is called once per frame
    void Update()
    {
        totalLife += Time.deltaTime;
        if (totalLife > lifeTime)
        {
            gmg.ultiState = false;            
            StartCoroutine(StartDisabler());
        }
    }

    IEnumerator StartDisabler()
    {
        var mover = gameObject.GetComponent<AIMovement>();           
        if (!mover.Portal.isPlaying)
        {            
            mover.Portal.Play();
        }
        yield return new WaitForSecondsRealtime(0.2f);
        RendererStatus(false);
        yield return new WaitForSecondsRealtime(mover.Portal.main.duration-0.2f);
        gameObject.SetActive(false);
    }

}
