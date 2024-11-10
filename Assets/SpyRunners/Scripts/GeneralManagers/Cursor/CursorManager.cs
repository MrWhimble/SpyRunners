using UnityEngine;

namespace SpyRunners.Managers
{
    public class CursorManager : MonoBehaviour
    {
        private static CursorManager _instance;

        [SerializeField] private CursorData _defaultCursorData;
        
        private PriorityList<CursorData> _cursors;
        public static PriorityList<CursorData> Cursors => _instance ? _instance._cursors : null;

        private void Awake()
        {
            _instance = this;

            _cursors = new (_defaultCursorData);
        }

        private void Update()
        {
            if (Cursor.lockState != _cursors.Value.LockState)
                Cursor.lockState = _cursors.Value.LockState;
            
            if (Cursor.visible != _cursors.Value.IsVisible)
                Cursor.visible = _cursors.Value.IsVisible;
        }

        private void OnDestroy()
        {
            _cursors.Clear();
        }
    }
}