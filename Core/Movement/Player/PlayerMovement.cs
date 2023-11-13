using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Combat;
using Core.Animation;
using TMPro;
using UnityEngine.UI;
using System;

namespace Core.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] public float speed = 5f;
        [SerializeField] float jumpForce = 5f;
        Vector3 oldPosition;
        [SerializeField] float highSpeedMultiplier = 2f;
        Vector2 input;
        private Rigidbody rb;
        [SerializeField] private float speedholder;
        bool grounded = true;
        public Transform cam;
        public GameObject evadeParticle;
        PlayerCombat combatTest;
        PlayerAnimate animation;     
        private float EvadeCoolDown = 2;
        private float EvadePassed =100;
        float EvadeRad = 10;
        //bool jump = false;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            combatTest = GetComponent<PlayerCombat>();
            animation = GetComponent<PlayerAnimate>();          
        }
        private void Update()
        {
            EvadePassed += Time.deltaTime;
            //if (Input.GetKeyDown(KeyCode.Space) && grounded)
            //    Jump();

            if (Input.GetKey(KeyCode.LeftShift) && !combatTest.defending)
            {
                speedholder = speed * highSpeedMultiplier;
            }
            else { speedholder = speed; }

            if (Input.GetKeyDown(KeyCode.Space) && !combatTest.defending)
            {
                StartCoroutine(EvadeBehavior());
            }       
           
        }
        void FixedUpdate()
        {
            Move(speedholder);
        }
        private void OnCollisionStay(Collision collision)
        {
            grounded = true;
        }
        private void OnCollisionExit(Collision collision)
        {
            grounded = false;
        }               
        Tuple<Vector3,float> calculateInput()
        {
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            input = Vector2.ClampMagnitude(input, 1);
            Vector3 camF = cam.forward;
            Vector3 camR = cam.right;
            camF.y= camR.y = 0;           
            camF = camF.normalized;
            camR = camR.normalized;
            var angler = (camF * input.y + camR * input.x);
            return new Tuple<Vector3, float>(angler, Vector3.SignedAngle(angler, transform.forward, Vector3.up));
        }

        private void Move(float speedh,float evadeMultitude=1)
        {
            var angler = calculateInput();          
            var whereto = angler.Item1 * evadeMultitude * Time.deltaTime * speedh;
            if (evadeMultitude>1)
            {             
                while (Physics.Raycast(transform.position, whereto, evadeMultitude))
                {
                    evadeMultitude--;
                    whereto = angler.Item1 * evadeMultitude * Time.deltaTime * speedh;
                }               
            }
            transform.position += whereto;
            if (input.x != 0 || input.y != 0)
            {
                //print(angler.Item2);
                var deg = DegreeToVector2(angler.Item2);
                var rev = new Vector2(deg.y, deg.x);
                animation.Movement(rev);
            }
            else
            {
                animation.Movement(new Vector2(0, 0));
            }
        }

        private static void CheckEnemyOnEvade(ref float evadeMultitude, ref Vector3 whereto)
        {
            if (evadeMultitude > 1)
            {
                bool checkenemies = true;
                while (checkenemies)
                {
                    var checkradius = Physics.OverlapSphere(whereto * evadeMultitude, 0.5f);
                    int i = 0;
                    foreach (var item in checkradius)
                    {
                        i++;
                        if (item.gameObject.tag == "Enemy")
                        {
                            evadeMultitude++;
                            break;
                        }
                        if (i == checkradius.Length)
                        {
                            checkenemies = false;
                        }
                    }
                }
                whereto *= evadeMultitude;
            }
        }

        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }
        private void Jump()
        {
          
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            grounded = false;
        }                       
        private IEnumerator EvadeBehavior()
        {
           
            if (EvadePassed>EvadeCoolDown)
            {
                EvadePassed = 0;
                var rot = calculateInput();
                if (input.x != 0 || input.y != 0)
                {
                    print(rot.Item2);
                    var effect = Instantiate(evadeParticle, transform.position,Quaternion.identity,gameObject.transform);
                    var oldpos = transform.position;                    
                    effect.transform.localRotation = Quaternion.Euler(0,-1*rot.Item2, 0);
                    Move(speedholder, 10);
                    effect.transform.position = oldpos;
                    effect.GetComponent<ParticleSystem>().Play();                   
                    yield return new WaitForSeconds(1f);
                    //Destroy(effect);                  
                }
                yield return null;
            }
           
        }

    }


}