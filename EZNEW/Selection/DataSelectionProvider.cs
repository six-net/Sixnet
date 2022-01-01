using System;
using System.Collections.Generic;
using System.Linq;
using EZNEW.Code;

namespace EZNEW.Selection
{
    /// <summary>
    /// Data selection provider
    /// </summary>
    [Serializable]
    public class DataSelectionProvider<T>
    {
        readonly ShuffleNet<T> shuffleNet = null;
        readonly List<T> values = null;
        int pollingIndex = -1;

        /// <summary>
        /// Initializes a new instance of the EZNEW.Selection.DataSelectionProvider class
        /// </summary>
        /// <param name="datas">Datas</param>
        public DataSelectionProvider(IEnumerable<T> datas)
        {
            values = datas?.ToList() ?? new List<T>(0);
            shuffleNet = new ShuffleNet<T>(values, true, true);
        }

        /// <summary>
        /// Gets the first data
        /// </summary>
        /// <returns>Return the selected data</returns>
        public T GetFirst()
        {
            return values.FirstOrDefault();
        }

        /// <summary>
        /// Gets the latest data
        /// </summary>
        /// <returns>Return the selected data</returns>
        public T GetLatest()
        {
            if (!values.IsNullOrEmpty())
            {
                return values[values.Count - 1];
            }
            return default;
        }

        /// <summary>
        /// Gets data by random
        /// </summary>
        /// <returns>Return the selected data</returns>
        public T GetByRandom()
        {
            if (!values.IsNullOrEmpty())
            {
                var maxIndex = values.Count - 1;
                var randomIndex = RandomNumberHelper.GetRandomNumber(maxIndex);
                return values[randomIndex];
            }
            return default;
        }

        /// <summary>
        /// Gets data by equiprobable random
        /// </summary>
        /// <returns>Return the selected data</returns>
        public T GetByEquiprobableRandom()
        {
            if (!values.IsNullOrEmpty())
            {
                return shuffleNet.TakeNextValues(1).FirstOrDefault();
            }
            return default;
        }

        /// <summary>
        /// Gets data by polling
        /// </summary>
        /// <returns>Return the selected data</returns>
        public T GetByPolling()
        {
            if (!values.IsNullOrEmpty())
            {
                pollingIndex++;
                if (pollingIndex == values.Count)
                {
                    pollingIndex = 0;
                }
                return values[pollingIndex];
            }
            return default;
        }

        /// <summary>
        /// Gets data by select match mode
        /// </summary>
        /// <param name="selectMatchMode">Select match mode</param>
        /// <returns>Return thhe selected data</returns>
        public T Get(SelectionMatchPattern selectMatchMode)
        {
            T data = default;
            switch (selectMatchMode)
            {
                case SelectionMatchPattern.First:
                    data = GetFirst();
                    break;
                case SelectionMatchPattern.Latest:
                    data = GetLatest();
                    break;
                case SelectionMatchPattern.Random:
                    data = GetByRandom();
                    break;
                case SelectionMatchPattern.EquiprobableRandom:
                    data = GetByEquiprobableRandom();
                    break;
                case SelectionMatchPattern.Polling:
                    data = GetByPolling();
                    break;
            }
            return data;
        }
    }
}
