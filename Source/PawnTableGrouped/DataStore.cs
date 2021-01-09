using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    public class ValueContainer<T> : IExposable
    {
        public T Value;

        public ValueContainer() { }

        public ValueContainer(T value)
        {
            Value = value;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref Value, "value", default(T));
        }

        public static implicit operator ValueContainer<T>(T value)
        {
            return new ValueContainer<T>(value);
        }

        public static implicit operator T(ValueContainer<T> value)
        {
            return value.Value;
        }
    }
 
    public class DictionaryContainer<K,V> : IExposable
    {
        public Dictionary<K, V> Value;


        public DictionaryContainer() { }

        public DictionaryContainer(Dictionary<K,V> value)
        {
            Value = value;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref Value, "value");
        }

        public static implicit operator DictionaryContainer<K, V>(Dictionary<K, V> value)
        {
            return new DictionaryContainer<K, V>(value);
        }

        public static implicit operator Dictionary<K, V>(DictionaryContainer<K, V> value)
        {
            return value.Value;
        }
    }

    public class DataContainer : IExposable
    {
        public string Key;
        public Dictionary<string, IExposable> Data;

        public void ExposeData()
        {
            Scribe_Values.Look(ref Key, "key", "");
            Scribe_Collections.Look(ref Data, "data");
        }
    }

    public class DataStore : GameComponent
    {
        public DataStore(Game game)
        {
            "DataStore created".Log();
        }


        List<Verse.WeakReference<IDataOwner>> dataOwners = new List<Verse.WeakReference<IDataOwner>>();
        Dictionary<string, Dictionary<string, IExposable>> storedData = new Dictionary<string, Dictionary<string, IExposable>>();

        public Dictionary<string, IExposable> GetData(string key)
        {
            if (storedData.TryGetValue(key, out var data))
            {
                return data;
            } 
            else
            {
                return null;
            }
        }

        public void RegisterModel(PawnTableGroupedModel pawnTableGroupedModel)
        {
            dataOwners.Add(new Verse.WeakReference<IDataOwner>(pawnTableGroupedModel));
        }

        public void UpdateData(string key, Dictionary<string, IExposable> data)
        {
            storedData[key] = data;
        }

        private void TrimDataOwners()
        {
            int index = 0;
            while (index < dataOwners.Count)
            {
                if (dataOwners[index].IsAlive)
                {
                    index++;
                } 
                else
                {
                    dataOwners.RemoveAt(index);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                TrimDataOwners();

                foreach (var dataOwner in dataOwners)
                {
                    if (dataOwner.IsAlive)
                    {
                        string key = null;
                        var value = dataOwner.Target?.SaveData(out key);
                        storedData[key] = value;
                    }
                }
            }

            var data = storedData.Select(kvp => new DataContainer
            {
                Key = kvp.Key,
                Data = kvp.Value,
            }).ToList();

            Scribe_Collections.Look(ref data, "storedData");

            storedData.Clear();
            if (data != null)
            {
                foreach (var d in data)
                {
                    storedData[d.Key] = d.Data;
                }
            }
        }
    }
}
