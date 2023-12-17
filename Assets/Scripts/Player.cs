using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class Player : MonoBehaviour
{
    public enum PlayerState {Walking, Climbing, Ragdoll, StandUp}
    PlayerState currentPlayerState;

    Camera mainCam;

    Animator animator;
    Rigidbody playerBody;
    Collider playerColl;

    Vector3 inputDir;
    Vector3 mouseInput;
    Vector3 mousePos;

    bool isGrounded;
    bool[] activeHands = {false, false};
    int aHandCount;
    bool[] grabingHands = { false, false };
    int gHandCount;

    [Header("Ground")]
    [SerializeField] float groundTopSpeed;
    [SerializeField] float acceleration;

    [Header("IKs")]
    [SerializeField] Hand[] handIKs;
    [SerializeField] FastIKFabric[] footIKs;
    [SerializeField] Transform[] footTarget;

    [Header("BodyPart")]
    [SerializeField] Rigidbody[] ragdollParts;
    [SerializeField] Rigidbody movePart;

    //

    public void ChangeState(PlayerState state)
    {
        currentPlayerState = state;

        switch (currentPlayerState)
        {
            case PlayerState.Walking:
                animator.enabled = true;
                playerColl.enabled = true;
                playerBody.isKinematic = false;
                break;

            case PlayerState.Climbing:
                animator.enabled = false;
                playerColl.enabled = false;
                playerBody.isKinematic = true;
                break;

            case PlayerState.Ragdoll:
                animator.enabled = false;
                playerColl.enabled = false;
                playerBody.isKinematic = false;

                break;

            case PlayerState.StandUp:
                animator.enabled = true;
                playerColl.enabled = true;
                playerBody.isKinematic = false;
                break;

        }
    }

    void StateCondition()
    {
        if (gHandCount > 0)
        {
            ChangeState(PlayerState.Climbing);
        }

        if (gHandCount <= 0)
        {
            if (isGrounded)
                ChangeState(PlayerState.Walking);
            else
                ChangeState(PlayerState.Ragdoll);
        }
    }

    //

    private void Start()
    {
        Hand.onGrab += onHandGrab;
        Hand.onLift += onHandActive;

        //Get Camera
        mainCam = Camera.main;

        playerBody = GetComponent<Rigidbody>();
        playerColl = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        
        ChangeState(PlayerState.Walking);

        //init hand and foot targets position and turn off IK
        for (int i = 0; i < handIKs.Length; i++)
        {
            footTarget[i].position = footIKs[i].transform.position;

            footIKs[i].enabled = false;
        }
    }

    private void Update()
    {
        InputProcess();

        Walking();
        GroundCheck();

        ClimbingLogic();

        StateCondition();

    }

    //

    void InputProcess()
    {
        inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir.Normalize();

        mouseInput = new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mousePos = Input.mousePosition;
        
        //Return Hands
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (Hand hand in handIKs)
            {
                hand.ChangeHandState(Hand.HandState.Idle);
            }
        }

        //Mouse Click
        if (Input.GetMouseButton(0)) // L
        {
            handIKs[0].ChangeHandState(Hand.HandState.Lift);
        }

        if (Input.GetMouseButton(1)) // R
        {
            handIKs[1].ChangeHandState(Hand.HandState.Lift);
        }

        //Mouse Release
        if (Input.GetMouseButtonUp(0)) // L
        {
            handIKs[0].ChangeHandState(Hand.HandState.Placing);
        }

        if (Input.GetMouseButtonUp(1)) // R
        {
            handIKs[1].ChangeHandState(Hand.HandState.Placing);
        }

    }

    //

    void Walking()
    {
        if (currentPlayerState != PlayerState.Walking)
            return;

        //set XZ direction
        var baseVelo = new Vector3(inputDir.x, 0, inputDir.y);

        // make movement relative to camera.
        var forward = mainCam.transform.forward;
        var right = mainCam.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        var camVelo = baseVelo.x * right + baseVelo.z * forward;

        //set final velocity
        playerBody.velocity = new Vector3(camVelo.x * groundTopSpeed, playerBody.velocity.y, camVelo.z * groundTopSpeed);

        //Turn player based on input direction when the input happened.
        var targetRotate = Quaternion.LookRotation(camVelo,Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotate, inputDir.sqrMagnitude);
    }

    void GroundCheck()
    {
        //Shoot SphereCast DownWard 
        var hit = Physics.SphereCastAll(transform.position, .5f, Vector3.down, .5f);
        isGrounded = (hit != null);
    }

    //

    void onHandGrab(int index, bool state)
    {
        grabingHands[index] = state;
        GrabbingHandCounter();
    }

    void onHandActive(int index, bool state)
    {
        activeHands[index] = state;
        ActiveHandCounter();
    }

    //

    void ActiveHandCounter() 
    {
        aHandCount = 0;

        foreach (bool aHand in activeHands)
        {
            if (aHand)
                aHandCount++;
        }
    }

    void GrabbingHandCounter()
    {
        gHandCount = 0;

        foreach (bool gHand in grabingHands)
        {
            if (gHand)
                gHandCount++;
        }
    }

    //

    void ClimbingLogic()
    {
        if (currentPlayerState == PlayerState.Climbing)
        {
            // while there is one hand grabbing the wall amd the other lift...
            if (gHandCount > 0 && aHandCount > 0)
            {
                // translate player body between two hand.
                var midPoint = Vector3.Lerp(handIKs[0].transform.position, handIKs[1].transform.position, 0.5f);

                movePart.MovePosition(midPoint);
            }

        }
    }    
}
