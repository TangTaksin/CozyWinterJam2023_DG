using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class Player : MonoBehaviour
{
    public enum PlayerState {Ground, Jump, Fall, Climb}
    PlayerState currentPlayerState;

    Camera mainCam;

    Animator animator;
    Rigidbody playerBody;
    Collider playerColl;

    Vector3 inputDir;

    bool isGrounded;

    [Header("Ground")]
    [SerializeField] float groundTopSpeed;
    [SerializeField] float acceleration;

    //

    public void ChangeState(PlayerState state)
    {
        currentPlayerState = state;

        switch (currentPlayerState)
        {
            case PlayerState.Ground:
                break;

            case PlayerState.Jump:
                break;

            case PlayerState.Fall:
                break;

            case PlayerState.Climb:
                break;

        }
    }

    void StateCondition()
    {

    }

    //

    private void Start()
    {
        //Get Camera
        mainCam = Camera.main;

        playerBody = GetComponent<Rigidbody>();
        playerColl = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        
        ChangeState(PlayerState.Ground);
    }

    private void Update()
    {
        InputProcess();

        //state check
        switch (currentPlayerState)
        {
            case PlayerState.Ground:
                Walking();
                break;

            case PlayerState.Jump:
                break;

            case PlayerState.Fall:
                break;

            case PlayerState.Climb:
                break;

        }

        GroundCheck();

        StateCondition();

    }

    //

    void InputProcess()
    {
        inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir.Normalize();
    }

    //

    void Walking()
    {
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

        //Animate
        animator.SetFloat("inputMagnitude", inputDir.sqrMagnitude);
    }

    void GroundCheck()
    {
        //Shoot SphereCast DownWard 
        var hit = Physics.SphereCastAll(transform.position, .5f, Vector3.down, .5f);
        isGrounded = (hit != null);
    }


}
