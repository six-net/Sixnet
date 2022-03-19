using System;
using EZNEW.Exceptions;

namespace EZNEW.Model
{
    /// <summary>
    /// Money
    /// </summary>
    [Serializable]
    public struct Money
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the EZNEW.ValueType.Money
        /// </summary>
        /// <param name="amount">amount</param>
        public Money(decimal amount)
        {
            Amount = amount;
            Currency = defaultCurrency;
        }

        /// <summary>
        /// Initializes a new instance of the EZNEW.ValueType.Money
        /// </summary>
        /// <param name="amount">amount</param>
        /// <param name="currency">currency</param>
        public Money(decimal amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Gets the default currency
        /// default value is Currency.CNY
        /// </summary>
        private static Currency defaultCurrency = Currency.CNY;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets amount value
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets currency
        /// </summary>
        public Currency Currency { get; }

        /// <summary>
        /// Gets or sets currencysign
        /// </summary>
        public string CurrencySign
        {
            get
            {
                return GetCurrencySign();
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Compare two money objects whether is equal
        /// </summary>
        /// <param name="moneyOne">First money</param>
        /// <param name="moneyTwo">Second money</param>
        /// <returns>Return whether is equal</returns>
        public static bool Equals(Money moneyOne, Money moneyTwo)
        {
            return moneyOne.Currency == moneyTwo.Currency && moneyOne.Amount == moneyTwo.Amount;
        }

        /// <summary>
        /// Verify whether can do calculate between two Money objects
        /// </summary>
        /// <param name="moneyOne">First money</param>
        /// <param name="moneyTwo">Second money</param>
        private static void CalculateVerify(Money moneyOne, Money moneyTwo)
        {
            if (moneyOne.Currency != moneyTwo.Currency)
            {
                throw new EZNEWException("Both money data don't hava the same currency");
            }
        }

        /// <summary>
        /// Sets default currency
        /// </summary>
        /// <param name="defaultCurrency">Default currency</param>
        public static void SetDefaultCurrency(Currency defaultCurrency)
        {
            Money.defaultCurrency = defaultCurrency;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets currency sign
        /// </summary>
        /// <returns></returns>
        string GetCurrencySign()
        {
            return string.Empty;
        }

        /// <summary>
        /// Compare two money objects whether equal
        /// </summary>
        /// <param name="data">Other money object</param>
        /// <returns>Return whether is equal</returns>
        public override bool Equals(object data)
        {
            return Equals(this, (Money)data);
        }

        /// <summary>
        /// Gets hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return string.Format("{0}_{1}", Currency, Amount).GetHashCode();
        }

        /// <summary>
        /// Do add operation between two Money objects,must be the same currency can do this operation
        /// </summary>
        /// <param name="one">First money</param>
        /// <param name="two">Second money</param>
        /// <returns>Return the calculate result</returns>
        public static Money operator +(Money one, Money two)
        {
            CalculateVerify(one, two);
            decimal newAmount = one.Amount + two.Amount;
            return new Money(newAmount, one.Currency);
        }

        /// <summary>
        /// Do subtraction operation between two Money objects,must be the same currency can do this operation
        /// </summary>
        /// <param name="one">First money</param>
        /// <param name="two">Second money</param>
        /// <returns>Return calculate result</returns>
        public static Money operator -(Money one, Money two)
        {
            CalculateVerify(one, two);
            decimal newAmount = one.Amount - two.Amount;
            return new Money(newAmount, one.Currency);
        }

        /// <summary>
        /// Do multiplication operation between two Money objects,must be the same currency can do this operation
        /// </summary>
        /// <param name="one">First money</param>
        /// <param name="two">Second money</param>
        /// <returns>Return calculate result</returns>
        public static Money operator *(Money one, Money two)
        {
            CalculateVerify(one, two);
            decimal newAmount = one.Amount * two.Amount;
            return new Money(newAmount, one.Currency);
        }

        /// <summary>
        /// Do division operation between two Money objects,must be the same currency can do this operation
        /// </summary>
        /// <param name="one">First money</param>
        /// <param name="two">Second money</param>
        /// <returns>Return calculate result</returns>
        public static Money operator /(Money one, Money two)
        {
            CalculateVerify(one, two);
            decimal newAmount = one.Amount / two.Amount;
            return new Money(newAmount, one.Currency);
        }

        /// <summary>
        /// Compare two Money objects whether equal
        /// </summary>
        /// <param name="one">First money</param>
        /// <param name="two">Second money</param>
        /// <returns>Whether is equal</returns>
        public static bool operator ==(Money one, Money two)
        {
            return Equals(one, two);
        }

        /// <summary>
        /// Compare two Money objects whether not equal
        /// </summary>
        /// <param name="one">First money</param>
        /// <param name="two">Second money</param>
        /// <returns>Whether is not equal</returns>
        public static bool operator !=(Money one, Money two)
        {
            return !Equals(one, two);
        }

        /// <summary>
        /// Determine whether the first money object less than second
        /// </summary>
        /// <param name="one">First money</param>
        /// <param name="two">Second money</param>
        /// <returns>Return determined result</returns>
        public static bool operator <(Money one, Money two)
        {
            CalculateVerify(one, two);
            return one.Amount < two.Amount;
        }

        /// <summary>
        /// Determine whether the first money object greater than second
        /// </summary>
        /// <param name="one">First money</param>
        /// <param name="two">Second money</param>
        /// <returns>Return determined result</returns>
        public static bool operator >(Money one, Money two)
        {
            CalculateVerify(one, two);
            return one.Amount > two.Amount;
        }

        /// <summary>
        /// Determine whether the first money object less than or equal to second
        /// </summary>
        /// <param name="one">First money</param>
        /// <param name="two">Second money</param>
        /// <returns>Return determined result</returns>
        public static bool operator <=(Money one, Money two)
        {
            CalculateVerify(one, two);
            return one.Amount <= two.Amount;
        }

        /// <summary>
        /// Determine whether the first money object greater than or equal to second
        /// </summary>
        /// <param name="one">First money</param>
        /// <param name="two">Second money</param>
        /// <returns>Return determined result</returns>
        public static bool operator >=(Money one, Money two)
        {
            CalculateVerify(one, two);
            return one.Amount >= two.Amount;
        }

        /// <summary>
        /// Add amount,minus amount if amount value is a negative number
        /// </summary>
        /// <param name="amount">Amount value</param>
        /// <returns>Return calculated money</returns>
        public Money AddAmount(decimal amount)
        {
            Amount += amount;
            return this;
        }

        /// <summary>
        /// Minus amount，add amount if amount value is a negative number
        /// </summary>
        /// <param name="amount">amount</param>
        /// <returns>Return calculated money</returns>
        public Money SubtractAmount(decimal amount)
        {
            Amount -= amount;
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Defines currency
    /// </summary>
    [Serializable]
    public enum Currency
    {
        CNY = 110,
        USD = 120,
    }
}
