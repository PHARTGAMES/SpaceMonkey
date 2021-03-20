using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using CMCustomUDP;


namespace GenericTelemetryProvider
{
    public class FilterModuleCustom
    {

        public static FilterModuleCustom Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new FilterModuleCustom();
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }
        static FilterModuleCustom instance;

        public enum FilterType
        {
            Smooth,
            Kalman,
            SavitzkyGolay,
            Butterworth,
            FIR,
            Median,
            KalmanVelocity,

            Max
        }


        public List<FilterBase>[] filters;

        string configFilename;

        FilterConfigData configData;

        List<CMCustomUDPData> filteredData = new List<CMCustomUDPData>();
        List<CMCustomUDPData> rawData = new List<CMCustomUDPData>();
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

            if (filters == null)
                filters = new List<FilterBase>[(int)CMCustomUDPData.DataKey.Max];

            foreach(List<FilterBase> filterList in filters)
            {
                if(filterList != null)
                    filterList.Clear();
            }

            for (int i = 0; i < filters.Length; ++i)
                filters[i] = null;

            for(int i = 0; i < configData.keys.Count; ++i)
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
                    else
                    if (filterData is MedianFilterData)
                    {
                        MedianFilterData fData = (MedianFilterData)filterData;
                        MedianFilterWrapper newFilter = new MedianFilterWrapper();
                        newFilter.SetParameters(fData.sampleCount);

                        filterList.Add(newFilter);
                    }
                }

            }

        }

        public void SaveConfig()
        {

            configData = new FilterConfigData();

            for (int i = 0; i < (int)CMCustomUDPData.DataKey.Max; ++i)
            {

                List<FilterBase> filterList = filters[i];
                if (filterList == null)
                {
                    continue;
                }
                else
                {
                    FilterDataKey newConfig = new FilterDataKey(i);
                    configData.keys.Add(newConfig);

                    foreach (FilterBase filter in filterList)
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
                        else
                        if (filter is MedianFilterWrapper)
                        {
                            MedianFilterData newFilterData = new MedianFilterData();
                            MedianFilterWrapper filterW = (MedianFilterWrapper)filter;
                            newFilterData.sampleCount = filterW.GetSampleCount();

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

        public void Filter(CMCustomUDPData dataIn, ref CMCustomUDPData dataOut, int keyMask = int.MaxValue, bool newHistory = true)
        {
            if (filters == null)
                return;

            mutex.WaitOne();

            CMCustomUDPData newRawData;
            CMCustomUDPData newFiltered;
            if (newHistory == true)
            {
                dataOut.Copy(dataIn);

                //copy to raw history
                newRawData = new CMCustomUDPData();
                newRawData.Copy(dataIn);
                rawData.Add(newRawData);
                if (rawData.Count > maxHistorySamples)
                    rawData.RemoveAt(0);

                //add new filtered history
                newFiltered = new CMCustomUDPData();
                newFiltered.Copy(dataIn);
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
                CMCustomUDPData.DataKey key = (CMCustomUDPData.DataKey)i;
                List<FilterBase> filterList = filters[i];

                if (!dataIn.IsValid(key))
                    continue;

                object value = dataIn.GetValue(key);

                if (filterList == null || filterList.Count == 0 || !dataIn.IsFloat(key))
                {
                    newFiltered.SetValue(key, value);
                    newRawData.SetValue(key, value);
                    continue;
                }

                //ignore filter if not in keyMask
                if (((1 << i) & keyMask) == 0)
                    continue;

                //get float val
                float floatVal = (float)value;

                //do filters for key
                foreach (FilterBase filter in filterList)
                {
                    floatVal = filter.Filter(floatVal);
                }

                //filtered
                newFiltered.SetValue(key, floatVal);
                dataOut.SetValue(key, floatVal);
            }

            mutex.ReleaseMutex();
        }

        public void GetFilteredHistory(out List<CMCustomUDPData> data)
        {
            mutex.WaitOne();

            data = new List<CMCustomUDPData>();
            data.AddRange(filteredData);

            mutex.ReleaseMutex();
        }

        public void GetRawHistory(out List<CMCustomUDPData> data)
        {
            mutex.WaitOne();

            data = new List<CMCustomUDPData>();
            data.AddRange(rawData);

            mutex.ReleaseMutex();
        }

        public void DeleteFilter(FilterBase filter, CMCustomUDPData.DataKey key)
        {
            filters[(int)key].Remove(filter);
        }

        public void MoveFilter(FilterBase filter, CMCustomUDPData.DataKey key, int direction)
        {
            int index = filters[(int)key].IndexOf(filter);

            int newIndex = Math.Min(filters[(int)key].Count-1, Math.Max(0, index + direction));

            filters[(int)key].RemoveAt(index);
            filters[(int)key].Insert(newIndex, filter);
        }


        public FilterBase AddFilter(FilterType filterType, CMCustomUDPData.DataKey key, bool updateUI = false)
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
                case FilterType.KalmanVelocity:
                    {
                        KalmanVelocityNoiseFilter newKalmanFilter = new KalmanVelocityNoiseFilter();
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
                case FilterType.Median:
                    {

                        MedianFilterWrapper newFilterW = new MedianFilterWrapper();
                        newFilterW.SetParameters(9);

                        filterList.Add(newFilterW);
                        newFilter = newFilterW;

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



}
