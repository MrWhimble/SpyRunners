using System.Collections;
using System.Collections.Generic;
using SpyRunners;
using SpyRunners.Player;
using UnityEngine;

[RequireComponent(typeof(CheckpointManager))]
public class PlayerHealth : MonoBehaviour, IDependent
{
    [SerializeField] private int _maxHealth = 1;
    
    public delegate void DamagedDelegate(PlayerCharacter playerCharacter);
    public event DamagedDelegate Damaged;
    
    public delegate void DiedDelegate(PlayerCharacter playerCharacter);
    public event DiedDelegate Died;
    
    private PlayerCharacter _playerCharacter;

    private int _health;

    private bool _isInitialized = false;
    private bool _isSubscribed = false;
    private bool _isCleanedUp = false;
    private bool _isFinished = false;
    
    private void Awake()
    {
        _playerCharacter = GetComponent<PlayerCharacter>();
        _playerCharacter.AddDependent(this);
    }
    
    public void Initialize()
    {
        if (_isInitialized)
            return;

        _health = _maxHealth;

        _isInitialized = true;
    }

    public void SubscribeToEvents()
    {
        if (_isSubscribed)
            return;

        

        _isSubscribed = false;
    }
    
    public void Damage()
    {
        _health--;
        if (_health <= 0)
        {
            Died?.Invoke(_playerCharacter);
            return;
        }
        
        Damaged?.Invoke(_playerCharacter);
    }

    public void CleanUp()
    {
        if (_isCleanedUp)
            return;



        _isCleanedUp = true;
    }

    public void UnsubscribeFromEvents()
    {
        if (!_isSubscribed)
            return;
        
        
        
        _isSubscribed = false;
    }

    public void Finish()
    {
        if (_isFinished)
            return;



        _isFinished = true;
    }

    
    
    //private void Start()
    //{
    //    checkpointManager = GetComponent<CheckpointManager>();
    //}
    //public void OnDeath()
    //{
    //    transform.position = checkpointManager.latestCheckpoint.position;
    //}

    
}
