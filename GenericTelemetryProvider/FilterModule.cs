using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;


namespace GenericTelemetryProvider
{
    public class FilterModule
    {

        public static FilterModule Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new FilterModule();
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }
        static FilterModule instance;

        public enum FilterType
        {
            Smooth,
            Kalman,
            SavitzkyGolay,
            Butterworth,
            FIR,

            Max
        }


        public List<FilterBase>[] filters = new List<FilterBase>[(int)GenericProviderData.DataKey.Max];

        string configFilename;

        FilterConfigData configData;

        List<GenericProviderData> filteredData = new List<GenericProviderData>();
        List<GenericProviderData> rawData = new List<GenericProviderData>();
        Mutex mutex = new Mutex(false);

        public int maxHistorySamples = 200;


        public void InitFromConfig(string filename)
        {
            configFilename = filename;

            if (!File.Exists(configFilename))
                return;

            configData = JsonConvert.DeserializeObject<FilterConfigData>(File.ReadAllText(configFilename), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            foreach(List<FilterBase> filterList in filters)
            {
                if(filterList != null)
                    filterList.Clear();
            }

            for (int i = 0; i < filters.Length; ++i)
                filters[i] = null;

            for(int i = 0; i < configData.keys.Length; ++i)
            {
                if (configData.keys[i] == null)
                    continue;

                FilterDataKey key = configData.keys[i];

                List<FilterBase> filterList = filters[(int)key.key] = new List<FilterBase>();

                foreach(FilterData filterData in key.filters)
                {
                    if(filterData is SmoothFilterData)
                    {
                        SmoothFilterData smoothFilterData = (SmoothFilterData)filterData;
                        NestedSmoothFilter newFilter = new NestedSmoothFilter();
                        newFilter.SetParameters(smoothFilterData.nestCount, smoothFilterData.sampleCount, smoothFilterData.maxDelta);

                        filterList.Add(newFilter);
                    }
                    else
                    if (filterData is KalmanFilterData)
                    {
                        KalmanFilterData kalmanFilterData = (KalmanFilterData)filterData;
                        KalmanNoiseFilter newFilter = new KalmanNoiseFilter();
                        newFilter.SetParameters(kalmanFilterData.a, kalmanFilterData.h, kalmanFilterData.q, kalmanFilterData.r, kalmanFilterData.p, kalmanFilterData.x);

                        filterList.Add(newFilter);
                    }
                }

            }

        }

        public void SaveConfig()
        {

            configData = new FilterConfigData();

            for (int i = 0; i < (int)GenericProviderData.DataKey.Max; ++i)
            {

                List<FilterBase> filterList = filters[i];
                if (filterList == null)
                {
                    configData.keys[i] = null;
                }
                else
                {
                    FilterDataKey newConfig = configData.keys[i] = new FilterDataKey((GenericProviderData.DataKey)i);

                    foreach(FilterBase filter in filterList)
                    {
                        if(filter is NestedSmoothFilter)
                        {
                            SmoothFilterData newFilterData = new SmoothFilterData();
                            NestedSmoothFilter nestSmoothFilter = (NestedSmoothFilter)filter;
                            newFilterData.nestCount = nestSmoothFilter.GetNestCount();
                            newFilterData.sampleCount = nestSmoothFilter.GetSampleCount();
                            newFilterData.maxDelta = nestSmoothFilter.GetMaxDelta();

                            newConfig.filters.Add(newFilterData);
                        }
                        else
                        if (filter is KalmanNoiseFilter)
                        {
                            KalmanFilterData newFilterData = new KalmanFilterData();
                            KalmanNoiseFilter kalmanNoiseFilter = (KalmanNoiseFilter)filter;
                            newFilterData.a = kalmanNoiseFilter.GetA();
                            newFilterData.h = kalmanNoiseFilter.GetH();
                            newFilterData.q = kalmanNoiseFilter.GetQ();
                            newFilterData.r = kalmanNoiseFilter.GetR();
                            newFilterData.p = kalmanNoiseFilter.GetP();
                            newFilterData.x = kalmanNoiseFilter.GetX();

                            newConfig.filters.Add(newFilterData);
                        }

                    }    
                }    
            }    


            string outputString = JsonConvert.SerializeObject(configData, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });


            File.WriteAllText(configFilename, outputString);

        }

        public void Filter(GenericProviderData dataIn, ref GenericProviderData dataOut, int keyMask = int.MaxValue, bool newHistory = true)
        {
            mutex.WaitOne();

            GenericProviderData newRawData;
            GenericProviderData newFiltered;
            if (newHistory == true)
            {
                dataOut.Copy(dataIn);

                //copy to raw history
                newRawData = new GenericProviderData(dataIn.version);
                newRawData.Copy(dataIn);
                rawData.Add(newRawData);
                if (rawData.Count > maxHistorySamples)
                    rawData.RemoveAt(0);

                //add new filtered history
                newFiltered = new GenericProviderData(dataIn.version);
                filteredData.Add(newFiltered);
                if (filteredData.Count > maxHistorySamples)
                    filteredData.RemoveAt(0);
            }
            else
            {
                newFiltered = filteredData[filteredData.Count - 1];
                newRawData = rawData[rawData.Count - 1];
            }


            //do filter lists for each key if it exists
            for (int i = 0; i < filters.Length; ++i)
            {
                List<FilterBase> filterList = filters[i];

                float value = dataIn.data[i];
                if (filterList == null || filterList.Count == 0)
                {
                    newFiltered.data[i] = value;
                    newRawData.data[i] = value;
                    continue;
                }

                //ignore filter if not in keyMask
                if (((1 << i) & keyMask) == 0)
                    continue;

                //do filters for key
                foreach(FilterBase filter in filterList)
                {
                    value = filter.Filter(value);
                }

                //filtered
                newFiltered.data[i] = dataOut.data[i] = value;
            }

            mutex.ReleaseMutex();
        }

        public void GetFilteredHistory(out List<GenericProviderData> data)
        {

            mutex.WaitOne();

            data = new List<GenericProviderData>();
            data.AddRange(filteredData);

            mutex.ReleaseMutex();
        }

        public void GetRawHistory(out List<GenericProviderData> data)
        {

            mutex.WaitOne();

            data = new List<GenericProviderData>();
            data.AddRange(rawData);

            mutex.ReleaseMutex();
        }

        public void DeleteFilter(FilterBase filter, GenericProviderData.DataKey key)
        {
            filters[(int)key].Remove(filter);
        }

        public void MoveFilter(FilterBase filter, GenericProviderData.DataKey key, int direction)
        {
            int index = filters[(int)key].IndexOf(filter);

            int newIndex = Math.Min(filters[(int)key].Count-1, Math.Max(0, index + direction));

            filters[(int)key].RemoveAt(index);
            filters[(int)key].Insert(newIndex, filter);
        }


        public FilterBase AddFilter(FilterType filterType, GenericProviderData.DataKey key, bool updateUI = false)
        {
            List<FilterBase> filterList = filters[(int)key];
            
            if(filterList == null)
                filterList = filters[(int)key] = new List<FilterBase>();

            FilterBase newFilter = null;

            switch (filterType)
            {
                case FilterType.Smooth:
                    {

                        NestedSmoothFilter newSmoothFilter = new NestedSmoothFilter();
                        newSmoothFilter.SetParameters(1, 3, 0.5f);

                        filterList.Add(newSmoothFilter);
                        newFilter = newSmoothFilter;

                        break;
                    }
                case FilterType.Kalman:
                    {
                        KalmanNoiseFilter newKalmanFilter = new KalmanNoiseFilter();
                        newKalmanFilter.SetParameters(1, 1, 0.02f, 1, 0.02f, 0.0f);

                        filterList.Add(newKalmanFilter);
                        newFilter = newKalmanFilter;

                        break;
                    }
                case FilterType.SavitzkyGolay:
                    {
                        break;
                    }
                case FilterType.FIR:
                    {
                        break;
                    }
            }

            if (updateUI && FilterUI.Instance != null)
            {
                FilterUI.Instance.InitChartForKey(key);
                    
            }



            return newFilter;
        }


    }



    [System.Serializable]
    public class FilterConfigData
    {
        public FilterDataKey[] keys = new FilterDataKey[(int)GenericProviderData.DataKey.Max];
    }

    public class FilterDataKey
    {
        public FilterDataKey(GenericProviderData.DataKey _key)
        {
            key = _key;
        }
        public GenericProviderData.DataKey key;
        public List<FilterData> filters = new List<FilterData>();

    }


    [System.Serializable]
    public abstract class FilterData
    {
        public string type;
       
    }

    [System.Serializable]
    public class SmoothFilterData : FilterData
    {
        public int nestCount;
        public int sampleCount;
        public float maxDelta;
    }

    [System.Serializable]
    public class KalmanFilterData : FilterData
    {
        public float a;
        public float h;
        public float q;
        public float r;
        public float p;
        public float x;
    }


}
