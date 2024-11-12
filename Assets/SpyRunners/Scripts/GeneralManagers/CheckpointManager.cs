using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public Transform latestCheckpoint;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Checkpoint")
        {
            latestCheckpoint = other.transform;
        }
    }

}
