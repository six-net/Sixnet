using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sixnet.Serialization
{
    /// <summary>
    /// Binary serialization helper
    /// </summary>
    public static class SixnetBinarySerializer
    {
        #region Serialization an object to a string

        /// <summary>
        /// Serialization an object to a string
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <returns>Return the string value</returns>
        public static string SerializeDataToString<T>(T data)
        {
            byte[] buffer = SerializeToByte(data);
            if (buffer == null)
            {
                return string.Empty;
            }
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// Serialization an object to a string
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return the string value</returns>
        public static string SerializeObjectToString(object data)
        {
            byte[] buffer = SerializeToByte(data);
            if (buffer == null)
            {
                return string.Empty;
            }
            return Convert.ToBase64String(buffer);
        }

        #endregion

        #region Serialization an object to a bytes array

        /// <summary>
        /// Serialization an object to a bytes array
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <returns>Return thee bytes value</returns>
        public static byte[] SerializeToByte<T>(T data)
        {
            if (data == null)
            {
                return null;
            }
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, data);
                stream.Position = 0;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Flush();
                stream.Close();
                return buffer;
            }
        }

        #endregion

        #region Deserialization a string to an object

        /// <summary>
        /// Deserialization a string to an object
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">String value</param>
        /// <returns>Return the data object</returns>
        public static T DesrializeByString<T>(string value)
        {
            T data = default;
            if (string.IsNullOrWhiteSpace(value))
            {
                return default;
            }
            IFormatter formatter = new BinaryFormatter();
            byte[] buffer = Convert.FromBase64String(value);
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                data = (T)formatter.Deserialize(stream);
                stream.Flush();
                stream.Close();
            }
            return data;
        }

        #endregion

        #region Deserialization a bytes array to an object

        /// <summary>
        /// Deserialization a bytes array to an object
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="values">Bytes array</param>
        /// <returns>Return the data object</returns>
        public static T DesrializeByByte<T>(byte[] values)
        {
            T data = default;
            if (values == null || values.Length < 1)
            {
                return default;
            }
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(values))
            {
                data = (T)formatter.Deserialize(stream);
                stream.Flush();
                stream.Close();
            }
            return data;
        }

        #endregion
    }
}
