//using System.Collections.Generic;
//using System.Linq;

//namespace Sixnet.Development.Message
//{
//    /// <summary>
//    /// Send message result
//    /// </summary>
//    public class SendMessageResult
//    {
//        /// <summary>
//        /// success
//        /// </summary>
//        public bool Success { get; set; }

//        /// <summary>
//        /// code
//        /// </summary>
//        public string Code { get; set; }

//        /// <summary>
//        /// message
//        /// </summary>
//        public string Message { get; set; }

//        /// <summary>
//        /// Return send failed result
//        /// </summary>
//        /// <param name="message">Result message</param>
//        /// <returns>Return send result</returns>
//        public static SendMessageResult SendFailed(string message = "")
//        {
//            return new SendMessageResult()
//            {
//                Success = false,
//                Message = message,
//            };
//        }

//        /// <summary>
//        /// Return send success result
//        /// </summary>
//        /// <returns>Return send result</returns>
//        public static SendMessageResult SendSuccess()
//        {
//            return new SendMessageResult()
//            {
//                Success = true,
//            };
//        }
//    }
//}
