using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveRagdoll : MonoBehaviour
{
    [SerializeField] ConfigurableJoint[] RagdollJoints;
    List<Transform> RagdollInitRotation = new List<Transform>();

    [SerializeField] Transform[] AnimatorJoints;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var joint in RagdollJoints)
        {
            RagdollInitRotation.Add(joint.transform);
        }
           
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < RagdollJoints.Length; i++)
        {
            ConfigurableJointExtensions.SetTargetRotationLocal(RagdollJoints[i], AnimatorJoints[i].localRotation, RagdollInitRotation[i].rotation);
        }
    }
}
