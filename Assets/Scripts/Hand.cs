using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class Hand : MonoBehaviour
{
    [SerializeField] int index;

    public FastIKFabric IK;
    Camera mainCam;

    [Header("Grabing")]
    [SerializeField] float handLiftOffset;
    [SerializeField] float timeOut = 0.5f;
    float timer;
    [SerializeField] float surfaceGrabRange = .2f;
    [SerializeField] LayerMask raycastMask;

    public enum HandState { Idle, Lift, Placing, Grabing}
    HandState handState;

    public delegate void handEvent(int sideIndex, bool state);
    public static handEvent onLift;
    public static handEvent onGrab;
    public static handEvent onRelease;

    private void Start()
    {
        TryGetComponent<FastIKFabric>(out IK);
        mainCam = Camera.main;

        //Turn off Ik
        IK.enabled = false;

    }

    private void Update()
    {
        switch (handState)
        {
            case HandState.Idle:
                break;

            case HandState.Lift:
                LiftHand();
                break;

            case HandState.Placing:
                PlaceHand();
                break;

            case HandState.Grabing:
                break;
        }
    }

    //

    public void ChangeHandState(HandState state)
    {
        handState = state;
        timer = timeOut;

        switch (handState)
        {
            case HandState.Idle:
                IK.enabled = false;

                onLift?.Invoke(index, false);
                onGrab?.Invoke(index, false);

                break;

            case HandState.Lift:
                IK.enabled = true;

                onLift?.Invoke(index, true);
                onGrab?.Invoke(index, false);

                break;

            case HandState.Placing:
                break;

            case HandState.Grabing:

                onLift?.Invoke(index, false);
                onGrab?.Invoke(index, true);

                break;
        }
    }

    void LiftHand()
    {
        //Shoot ray
        var ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            IK.Target.position = hit.point + hit.normal * handLiftOffset;
        }
    }

    void PlaceHand()
    {   
        //Attemp to place hand at the target.
        //Shoot ray
        var ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            IK.Target.position = hit.point;
        }

        timer -= Time.deltaTime;
        if (timer > 0)
        {
            if (Physics.SphereCastAll(transform.position, surfaceGrabRange, transform.position - IK.Target.position, 0.2f, raycastMask).Length > 0)
            {
                ChangeHandState(HandState.Grabing);
            }
        }
        else
        {
            ChangeHandState(HandState.Idle);
        }
    }

}
