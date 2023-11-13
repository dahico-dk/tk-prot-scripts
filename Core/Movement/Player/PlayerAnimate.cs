using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Core.Animation
{
    public class PlayerAnimate : MonoBehaviour
    {
        Animator animController;
        // Start is called before the first frame update
        void Start()
        {
            animController = GetComponent<Animator>();
        }

        public void Movement(Vector2 dir)
        {
           
            animController.SetFloat("2d x", dir.x);
            animController.SetFloat("2d y", dir.y);
        }
       public void AttackAnimation(int anim)
        {
           
            string attackname = "Attack " + anim.ToString();
           // print(attackname);
            animController.SetTrigger(attackname);
        }

        public void Block(bool blocking)
        {
            animController.SetBool("Blocking", blocking);

        }
        public void SwitchAnimation()
        {
            animController.SetTrigger("Switch");

        }
        public void Special(bool Ulti)
        {
            if (Ulti)
            {
                animController.SetTrigger("ULTİ");
            }
            else
            {
                animController.SetTrigger("AOE");
            }
        }


    }

}
