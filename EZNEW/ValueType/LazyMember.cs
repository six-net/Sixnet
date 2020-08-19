using System;

namespace EZNEW.ValueType
{
    /// <summary>
    /// Lazy member
    /// </summary>
    public class LazyMember<T>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the EZNEW.ValueType.LazyMember<>
        /// </summary>
        /// <param name="valueFactory">value factory</param>
        public LazyMember(Func<T> valueFactory)
        {
            ValueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets Value
        /// </summary>
        public T Value
        {
            get
            {
                return GetValue();
            }
            private set
            {
                SetValue(value, false);
            }
        }

        /// <summary>
        /// Gets or sets value factory
        /// </summary>
        protected Func<T> ValueFactory { get; set; }

        /// <summary>
        /// Gets or sets whether value is created
        /// </summary>
        protected bool IsCreatedValue { get; set; }

        /// <summary>
        /// Gets the current value
        /// </summary>
        public T CurrentValue { get; private set; } = default;

        #endregion

        #region Methods

        /// <summary>
        /// Gets value
        /// </summary>
        /// <returns>Return data</returns>
        T GetValue()
        {
            if (IsCreatedValue)
            {
                return CurrentValue;
            }
            var newValue = ValueFactory();
            SetValue(newValue, true);
            return CurrentValue;
        }

        /// <summary>
        /// Sets value
        /// </summary>
        /// <param name="value">New value</param>
        /// <param name="createdValue">Whether inited or not</param>
        public void SetValue(T value, bool createdValue = true)
        {
            IsCreatedValue = createdValue;
            CurrentValue = value;
        }

        /// <summary>
        /// Implicit convert to data
        /// </summary>
        /// <param name="lazyMember">Lazy member</param>
        public static implicit operator T(LazyMember<T> lazyMember)
        {
            if (lazyMember == null)
            {
                return default;
            }
            return lazyMember.Value;
        }

        /// <summary>
        /// Implicit convert to LazyMember<T>
        /// </summary>
        /// <param name="value">Data object</param>
        public static implicit operator LazyMember<T>(T value)
        {
            var lazyMember = new LazyMember<T>(() => default);
            lazyMember.SetValue(value, true);
            return lazyMember;
        }

        #endregion
    }
}
