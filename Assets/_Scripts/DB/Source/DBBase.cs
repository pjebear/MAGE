using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.DB.Internal
{
    abstract class DBBase<KeyType, ValueType> 
        where ValueType : DBEntryBase, new()
    {
        public List<KeyType> Keys { get { return new List<KeyType>(mDB.Keys); } }
        protected string TAG = "";
        protected Dictionary<KeyType, ValueType> mDB = new Dictionary<KeyType, ValueType>();
        protected Dictionary<object, DBUpdateCB<KeyType>> mDBUpdateListeners = new Dictionary<object, DBUpdateCB<KeyType>>();
        
        protected DBBase(string dbName)
        {
            TAG = dbName;
        }

        public void Write(KeyType key, ValueType value)
        {
            string message = "";

            if (mDB.ContainsKey(key))
            {
                message = string.Format("Updating entry. [{0}, {1}]", key.ToString(), value.ToString());
            }
            else
            {
                message = string.Format("Inserting entry. [{0}, {1}]", key.ToString(), value.ToString());
                mDB.Add(key, new ValueType());
            }

            mDB[key].Set(value);

            Logger.Log(LogTag.DB, TAG, message);
            NotifyEntryChanged(key);
        }

        public ValueType Load(KeyType key)
        {
            ValueType entry = new ValueType();

            if (mDB.ContainsKey(key))
            {
                mDB[key].CopyTo(entry);
                Logger.Log(LogTag.DB, TAG, string.Format("Loading entry. [{0}, {1}]", key, entry.ToString()));
            }
            else
            {
                Logger.Log(LogTag.DB, TAG, string.Format("Failed to find entry. [{0}]", key), LogLevel.Warning);
            }

            return entry;
        }

        public void Clear(KeyType key)
        {
            string message = "";

            if (mDB.ContainsKey(key))
            {
                ValueType entry = mDB[key];
                mDB.Remove(key);
                message = string.Format("Cleared entry. [{0}, {1}]", key, entry.ToString());
            }
            else
            {
                message = string.Format("Failed to find entry. [{0}]", key);
            }

            Logger.Log(LogTag.DB, TAG, message);
        }

        public bool ContainsEntry(KeyType key)
        {
            return mDB.ContainsKey(key);
        }

        public ValueType EmptyEntry()
        {
            return new ValueType();
        }

        public void RegisterUpdateListener(object listener, DBUpdateCB<KeyType> cb)
        {
            Logger.Assert(!mDBUpdateListeners.ContainsKey(listener), LogTag.DB, TAG, "Listener already registered");
            if (!mDBUpdateListeners.ContainsKey(listener))
            {
                mDBUpdateListeners.Add(listener, cb);
            }
        }

        public void UnRegisterUpdateListener(object listener)
        {
            Logger.Assert(mDBUpdateListeners.ContainsKey(listener), LogTag.DB, TAG, "Listener isn't registered");
            if (mDBUpdateListeners.ContainsKey(listener))
            {
                mDBUpdateListeners.Remove(listener);
            }
        }

        protected void NotifyEntryChanged(KeyType entry)
        {
            Dictionary<object, DBUpdateCB<KeyType>> listenersCopy = new Dictionary<object, DBUpdateCB<KeyType>>(mDBUpdateListeners);
            foreach (DBUpdateCB<KeyType> cb in listenersCopy.Values)
            {
                cb(entry);
            }
        }

        public abstract void Save(string path);
        public abstract void Load(string path);
    }
}
