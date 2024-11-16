using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectManager : MonoBehaviour
{
    private GameObject currentEffect;
    [SerializeField] private Transform cameraObject;
    private void Start()
    {
        cameraObject = Camera.main.transform;
    }
    public void SpawnEffect(GameObject newEffect)
    {
        currentEffect = Instantiate(newEffect, cameraObject);
    }

    public void RemoveEffect()
    {
        if (currentEffect == null)
            return;
        Destroy(currentEffect);
        currentEffect = null;
    }
}
