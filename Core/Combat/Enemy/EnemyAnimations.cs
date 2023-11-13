using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Core.Animation
{
    public class EnemyAnimations : MonoBehaviour
    {
        [SerializeField]ParticleSystem deatheffect;
        public bool dissolve;
        Renderer rend;
        float dissolveMat=0;
        bool countDownStarted = false;       
        private void Start()
        {
            rend = GetComponentInChildren<Renderer>();
            rend.material.shader = Shader.Find("DissolverShader/DissolveShader");
        }
        private void Update()
        {
            if (dissolve)
            {
                GetComponentInChildren<SkinnedMeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                dissolveMat = dissolveMat+Time.deltaTime<1?(dissolveMat + Time.deltaTime):1;
                rend.material.SetFloat("_DissolveAmount", dissolveMat);
                if (dissolveMat == 1 && !countDownStarted)
                {
                    gameObject.SetActive(false);
                }
            }
        }
        public void SmokeEffect()
        {
            var vect = transform.position;
            vect.y = 1;
            var part = Instantiate(deatheffect,vect, Quaternion.identity);
            part.Play();           
            var aut1 = new AutoDestroy();          
            aut1.StartCountDown(part.main.duration, part.gameObject);
            gameObject.SetActive(false);

        }

    }
}
