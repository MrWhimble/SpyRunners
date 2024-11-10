using System.Collections.Generic;

namespace SpyRunners
{
    public class PriorityList<T>
    {
        public readonly struct Element
        {
            public readonly int ID;
            public readonly int Priority;
            public readonly T Value;

            public Element(int id, int priority, T value)
            {
                ID = id;
                Priority = priority;
                Value = value;
            }
        }

        public PriorityList(T defaultValue)
        {
            _defaultValue = defaultValue;
            _list = new List<Element>();
        }

        public delegate void ValueUpdatedDelegate(T newValue);
        public event ValueUpdatedDelegate NewValue;
        
        private readonly T _defaultValue;
        private readonly List<Element> _list;

        public void Add(int id, int priority, T value)
        {
            Element element = new Element(id, priority, value);
            for (int i = 0; i < _list.Count; ++i)
            {
                if (element.Priority < _list[i].Priority)
                {
                    continue;
                }
                _list.Insert(i, element);
                if (i == 0)
                    NewValue?.Invoke(element.Value);
                return;
            }
            _list.Add(element);
            NewValue?.Invoke(element.Value);
        }
        
        public void Add(int id, int priority, T value, bool mustBeFirst)
        {
            Element element = new Element(id, priority, value);
            for (int i = 0; i < _list.Count; ++i)
            {
                if (element.Priority < _list[i].Priority)
                {
                    if (mustBeFirst && i == 0)
                        return;
                    continue;
                }
                _list.Insert(i, element);
                if (i == 0)
                    NewValue?.Invoke(element.Value);
                return;
            }
            _list.Add(element);
            NewValue?.Invoke(element.Value);
        }

        public void Change(int id, T newValue)
        {
            for (int i = 0; i < _list.Count; ++i)
            {
                if (_list[i].ID != id)
                    continue;
                _list[i] = new Element(_list[i].ID, _list[i].Priority, newValue);
                if (i == 0)
                    NewValue?.Invoke(_list.Count == 0 ? _defaultValue : _list[0].Value);
                return;
            }
        }

        public void Remove(int id)
        {
            for (int i = 0; i < _list.Count; ++i)
            {
                if (_list[i].ID != id)
                    continue;
                _list.RemoveAt(i);
                if (i == 0)
                    NewValue?.Invoke(_list.Count == 0 ? _defaultValue : _list[0].Value);
                return;
            }
        }
        
        public T Get(int id)
        {
            for (int i = 0; i < _list.Count; ++i)
            {
                if (_list[i].ID != id)
                    continue;
                return _list[i].Value;
            }
            return default;
        }

        public T Value => _list.Count == 0 ? _defaultValue : _list[0].Value;
        public T this[int index] => _list[index].Value;

        public int Count => _list.Count;

        public bool IsCurrent(int id) => _list.Count != 0 && _list[0].ID == id;
        public void Clear()
        {
            _list.Clear();
            NewValue?.Invoke(_defaultValue);
        }
    }
}