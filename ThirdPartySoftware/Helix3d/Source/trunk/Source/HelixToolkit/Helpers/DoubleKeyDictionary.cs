using System;
using System.Collections;
using System.Collections.Generic;

namespace HelixToolkit
{
    // from http://noocyte.wordpress.com/2008/02/18/double-key-dictionary/
    // added Remove method

    public class DoubleKeyDictionary<K, T, V> :
        IEnumerable<DoubleKeyPairValue<K, T, V>>,
        IEquatable<DoubleKeyDictionary<K, T, V>>
    {
        private Dictionary<T, V> m_innerDictionary;

        public DoubleKeyDictionary()
        {
            OuterDictionary = new Dictionary<K, Dictionary<T, V>>();
        }

        private Dictionary<K, Dictionary<T, V>> OuterDictionary { get; set; }

        public V this[K index1, T index2]
        {
            get { return OuterDictionary[index1][index2]; }
            set { Add(index1, index2, value); }
        }

        #region IEnumerable<DoubleKeyPairValue<K,T,V>> Members

        public IEnumerator<DoubleKeyPairValue<K, T, V>> GetEnumerator()
        {
            foreach (var outer in OuterDictionary)
            {
                foreach (var inner in outer.Value)
                {
                    yield return new DoubleKeyPairValue<K, T, V>(outer.Key, inner.Key, inner.Value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEquatable<DoubleKeyDictionary<K,T,V>> Members

        public bool Equals(DoubleKeyDictionary<K, T, V> other)
        {
            if (OuterDictionary.Keys.Count != other.OuterDictionary.Keys.Count)
                return false;

            bool isEqual = true;

            foreach (var innerItems in OuterDictionary)
            {
                if (!other.OuterDictionary.ContainsKey(innerItems.Key))
                    isEqual = false;

                if (!isEqual)
                    break;

                // here we can be sure that the key is in both lists, 
                // but we need to check the contents of the inner dictionary
                Dictionary<T, V> otherInnerDictionary = other.OuterDictionary[innerItems.Key];
                foreach (var innerValue in innerItems.Value)
                {
                    if (!otherInnerDictionary.ContainsValue(innerValue.Value))
                        isEqual = false;
                    if (!otherInnerDictionary.ContainsKey(innerValue.Key))
                        isEqual = false;
                }

                if (!isEqual)
                    break;
            }

            return isEqual;
        }

        #endregion

        public void Remove(K key1, T key2)
        {
            OuterDictionary[key1].Remove(key2);
            if (OuterDictionary[key1].Count == 0)
                OuterDictionary.Remove(key1);
        }

        public void Add(K key1, T key2, V value)
        {
            if (OuterDictionary.ContainsKey(key1))
            {
                if (m_innerDictionary.ContainsKey(key2))
                    OuterDictionary[key1][key2] = value;
                else
                {
                    m_innerDictionary = OuterDictionary[key1];
                    m_innerDictionary.Add(key2, value);
                    OuterDictionary[key1] = m_innerDictionary;
                }
            }
            else
            {
                m_innerDictionary = new Dictionary<T, V>();
                m_innerDictionary[key2] = value;
                OuterDictionary.Add(key1, m_innerDictionary);
            }
        }

        public bool ContainsKey(K index1, T index2)
        {
            if (!OuterDictionary.ContainsKey(index1))
                return false;
            if (!OuterDictionary[index1].ContainsKey(index2))
                return false;
            return true;
        }
    }

    public class DoubleKeyPairValue<K, T, V>
    {
        public DoubleKeyPairValue(K key1, T key2, V value)
        {
            Key1 = key1;
            Key2 = key2;
            Value = value;
        }

        public K Key1 { get; set; }

        public T Key2 { get; set; }

        public V Value { get; set; }

        public override string ToString()
        {
            return Key1.ToString() + " - " + Key2.ToString() + " - " + Value.ToString();
        }
    }
}