/*
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published by the MIT license.
 */

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配

namespace Honoo.Collections.ObjectModel
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配
{
    /// <summary>
    /// Represents a observable dictionary of keys and values.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged where TKey : notnull
    {
        //private int _index;

        /// <summary>
        /// Gets a number of key/value pairs contained in the <see cref="Dictionary&lt;TKey, TValue&gt;"/>.
        /// </summary>
        public new int Count => base.Count;

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="Dictionary&lt;TKey, TValue&gt;"/>.
        /// </summary>
        public new KeyCollection Keys => base.Keys;

        /// <summary>
        /// Gets a collection containing the Values in the <see cref="Dictionary&lt;TKey, TValue&gt;"/>.
        /// </summary>
        public new ValueCollection Values => base.Values;

        #region Event

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        /// <param name="e"></param>
        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Event

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new TValue this[TKey key]
        {
            get { return base[key]; }
            set { AddOrUpdate(key, value); }
        }

        #region Construction

        /// <summary>
        /// Initializes a new instance of the ObservableDictionary class.
        /// </summary>
        public ObservableDictionary() : base()
        {
        }

        #endregion Construction

        /// <summary>
        /// Add the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this[key], base.Count - 1));
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
            OnPropertyChanged(nameof(Count));
        }

        /// <summary>
        /// Add or update the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddOrUpdate(TKey key, TValue value)
        {
            if (TryGet(key, out int index, out KeyValuePair<TKey, TValue> pair))
            {
                base[key] = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, base[key], pair, index));
                OnPropertyChanged(nameof(Values));
                //OnPropertyChanged("Item[]");
            }
            else
            {
                Add(key, value);
            }
        }

        /// <summary>
        /// Remove all keys and values from this <see cref="Dictionary&lt;TKey, TValue&gt;"/>.
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
            OnPropertyChanged(nameof(Count));
        }

        /// <summary>
        /// Remove the value with the specified key from the <see cref="Dictionary&lt;TKey, TValue&gt;"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new bool Remove(TKey key)
        {
            if (TryGet(key, out int index, out KeyValuePair<TKey, TValue> pair))
            {
                base.Remove(key);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, pair, index));
                OnPropertyChanged(nameof(Keys));
                OnPropertyChanged(nameof(Values));
                OnPropertyChanged(nameof(Count));
                return true;
            }
            return false;
        }

        private bool TryGet(TKey key, out int index, out KeyValuePair<TKey, TValue> pair)
        {
            index = -1;
            foreach (var item in this)
            {
                index++;
                if (item!.Key.Equals(key))
                {
                    pair = item;
                    return true;
                }
            }
            index = -1;
            pair = default;
            return false;
        }
    }
}