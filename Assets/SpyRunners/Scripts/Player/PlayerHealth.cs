using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CheckpointManager))]
public class PlayerHealth : MonoBehaviour
{
    CheckpointManager checkpointManager;
    private void Start()
    {
        checkpointManager = GetComponent<CheckpointManager>();
    }
    public void OnDeath()
    {
        transform.position = checkpointManager.latestCheckpoint.position;
    }
}
