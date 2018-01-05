using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMovement : MonoBehaviour {



    private MovementMotor motor;

    [Header("Movement")]
    public float move_Magitude = 0.05f;
    public float speed = 0.7f;
    public float speed_Move_WhileAttack = 1.5f;
    public float speed_Attack = 1.5f;
    public float turnSpeed = 10f;
    public float speed_Jump = 20f;

    private float speed_Move_Multiplier = 1f;

    private bool canMove = true;

    

    private Vector3 direction;


    // ANIMATION ---------------------------------------------
    private Animator anim;
    private Camera mainCamera;

    public float rotationSpeed = 3f;
    private float rotateY;
    [Header("tools Animation")]
    private bool digging;
    

    void Awake ()
    {
        motor = GetComponent<MovementMotor>();
        anim = GetComponent<Animator>();
    }

   

    


    //START---------------------------------------------------------------
    void Start ()
    {
        anim.applyRootMotion = false;

        mainCamera = Camera.main;

        
    }

  
	
	// Update is called once per frame
	void Update () {

        MovementAndJumping();
        GetInput();
        



    }

    private Vector3 MoveDirection
    {
        get { return direction; }

        set
        {
            direction = value * speed_Move_Multiplier;

            if ( direction.magnitude > 0.1f)
            {
                var newRotation = Quaternion.LookRotation (direction);

                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * turnSpeed);
            }

            direction *= speed * (Vector3.Dot(transform.forward, direction) + 1f) * 5f;
            motor.Move(direction);

            AnimationMove(motor.charController.velocity.magnitude * 0.1f);
        }
    }

    void Moving(Vector3 dir, float mult)
    {
        speed_Move_Multiplier = 1 * mult;
        MoveDirection = dir;
    }

    void Jump()
    {
        motor.Jump(speed_Jump);
    }



    void MovementAndJumping()
    {



        if (!digging)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }


            Vector3 moveInput = Vector3.zero;
            Vector3 forward = Quaternion.AngleAxis(-90, Vector3.up) * mainCamera.transform.right;

            moveInput += forward * Input.GetAxis("Vertical");
            moveInput += mainCamera.transform.right * Input.GetAxis("Horizontal");

            moveInput.Normalize();
            Moving(moveInput.normalized, 1f);

        } 


    }



    


    //ANIMATION--------------------------------------------------------

    void GetInput()
    {
        if (Input.GetKeyDown("f"))
        {
            DigB();
            anim.SetTrigger("punch");
            print("digging");
            


        }
        else if (Input.GetMouseButtonUp(0))
        {
            speed = 0.15f;
        }
    }






    void AnimationMove(float magnitude)
    {


        if (magnitude <= move_Magitude)
        {
            anim.SetInteger("Condition", 0);
            return;
        }


        else if (!digging)
        {
            //Walk
            if (magnitude > move_Magitude)
            {

                anim.SetInteger("Condition", 1);







                //run
                if (Input.GetKey("left shift"))
                {
                    if (!digging)
                    {
                        anim.SetInteger("Condition", 2);
                    }


                    speed = .3f;

                }
                else if (Input.GetKeyUp("left shift"))
                {
                    speed = .15f;
                }


            }
        }
         


    }

    void DigB()
    {

        if (digging) return;

        
        anim.SetInteger("Condition", 3);
        StartCoroutine(AnimationRoutine());
        
        
    }


    

        
        
    

    IEnumerator AnimationRoutine()
    {
        digging = true;
        yield return new WaitForSeconds(0.1f);
        anim.SetInteger("Condition", 0);
        yield return new WaitForSeconds(1);
        digging = false;
        
    }


} //class
