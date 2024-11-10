using UnityEngine;
using UnityEngine.Serialization;

namespace SpyRunners.Managers
{
    [System.Serializable]
    public class CursorData
    {
        public CursorLockMode LockState = CursorLockMode.None;
        public bool IsVisible = true;

        public CursorData(CursorLockMode lockState)
        {
            LockState = lockState;
            IsVisible = LockState is not CursorLockMode.Locked;
        }

        public CursorData(bool isVisible)
        {
            LockState = CursorLockMode.None;
            IsVisible = isVisible;
        }
        
        public CursorData(CursorLockMode lockState, bool isVisible)
        {
            LockState = lockState;
            IsVisible = isVisible;
        }
    }
}