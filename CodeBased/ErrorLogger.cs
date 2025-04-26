using System;
using System.Data.SqlClient;
using System.IO;
using Kpakam.Interface;

namespace Kpakam
{
    public class ErrorLogger : IErrorLogger
    {
        private string Destination;

        public void AddMatchIDs(string sourceID, string TargetID)
        {
            try
            {
                string FileName = string.Format("{0}/MatchIDs.txt", Destination);
                if (!File.Exists(FileName))
                {
                    File.WriteAllLines(FileName, new string[] { "Source,Target" });
                }

                File.AppendAllLines(FileName, new string[] { string.Format("{0},{1}", new[] { sourceID }, new[] { TargetID }) });
            }
            catch (Exception ex)
            {
                // ' logger.WriteLog(ex)
            }
        }

        public ErrorLogger(string _Destination)
        {
            Destination = _Destination;
            try
            {
                if (!Directory.Exists(_Destination))
                    Directory.CreateDirectory(_Destination);
            }
            catch (Exception ex)
            {
            }
        }

        public void WriteClientLog(string ex)
        {
            string file = Destination + "/" + DateTime.Now.Date.ToString("dd_MM_yyyy_") + "ClientErrorLog.txt";
            bool hasInnerException = false;
            var fs = File.Open(file, FileMode.Append, FileAccess.Write, FileShare.Write);
            using (var writer = new StreamWriter(fs))
            {
                writer.WriteLine(string.Format("======================================={0}=======================================", DateTime.Now.ToString()));
                writer.WriteLine(ex);
            }
        }

        public void WriteLog(Exception ex)
        {
            if (ExceptionMod.CustomError(ex) == "Thread was being aborted.")
            {
                return;
            }

            string file = Destination + "/" + DateTime.Now.Date.ToString("dd_MM_yyyy_") + "ErrorLog.txt";
            bool hasInnerException = false;
            var fs = File.Open(file, FileMode.Append, FileAccess.Write, FileShare.Write);
            using (var writer = new StreamWriter(fs))
            {
                writer.WriteLine(string.Format("======================================={0}=======================================", DateTime.Now.ToString()));
                writer.WriteLine(ExceptionMod.CustomError(ex));
                writer.WriteLine(string.Format("---------------Stack Trace---------------"));
                writer.WriteLine(ex.StackTrace);
                if (ex.InnerException is object)
                {
                    hasInnerException = true;
                    writer.WriteLine(string.Format("**********************Inner Exception**********************"));
                }
            }

            if (hasInnerException)
            {
                WriteLog(ex.InnerException);
            }
        }

        public void WriteActivity(string Activity)
        {
            string file = Destination + "/ActivityLog.txt";
            bool hasInnerException = false;
            var fs = File.Open(file, FileMode.Append, FileAccess.Write, FileShare.Write);
            using (var writer = new StreamWriter(fs))
            {
                writer.WriteLine(string.Format("======================================={0}=======================================", DateTime.Now.ToString()));
                writer.WriteLine(Activity);
            }
        }
    }

    public static class ExceptionMod
    {
        #region Error Translater
        /// <summary>
    /// Starts new custom error handle
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    /// <remarks></remarks>
        public static string CustomError(Exception ex)
        {
            return new CustomException(ex).GetMessage();
        }
        #endregion
    }

    public class CustomException
    {
        /// <summary>
    /// Represents the error code returned from stored procedure when entity could not be found.
    /// </summary>
    /// <remarks></remarks>
        private const int SQL_ERROR_CODE_ENTITY_NOT_FOUND = 50001;

        /// <summary>
    /// Represents the error code returned from stored procedure when entity to be updated has time mismatch
    /// </summary>
    /// <remarks></remarks>
        private const int SQL_ERROR_CODE_TIME_MISMATCH = 50002;

        /// <summary>
    /// Represents the error code returned from stored procedure when a persistence exception occurs
    /// </summary>
    /// <remarks></remarks>
        private const int SQL_ERROR_CODE_PERSISTENCE_ERROR = 50003;

        /// <summary>
    /// Represents the error code returned when dead lock occurs
    /// </summary>
    /// <remarks></remarks>
        private const int SQL_DEADLOCK_ERROR = 1205;

        /// <summary>
    /// Represents the error code returned when timeout occurs
    /// </summary>
    /// <remarks></remarks>
        private const int SQL_TIMEOUT_ERROR = -2;
        private const string DefaultSQLMessage = "Error found in database processing. Contact administrator";
        private string Message = "";

        public CustomException(Exception ex)
        {
            if (ReferenceEquals(ex.InnerException, typeof(SqlException))) // handle SQL Exception here
            {
                SqlException SQLExcep = (SqlException)ex.InnerException;
                switch (SQLExcep.Number)
                {
                    case SQL_ERROR_CODE_ENTITY_NOT_FOUND:
                        {
                            Message = "Invalid database transaction was sent";
                            break;
                        }

                    case SQL_ERROR_CODE_PERSISTENCE_ERROR:
                        {
                            Message = "Unending database process was detected";
                            break;
                        }

                    case SQL_ERROR_CODE_TIME_MISMATCH:
                        {
                            Message = "Date sent is in incorrect format";
                            break;
                        }

                    case SQL_DEADLOCK_ERROR:
                        {
                            Message = "The processing could not be completed due to unending transaction";
                            break;
                        }

                    case SQL_TIMEOUT_ERROR:
                        {
                            Message = "Database server is unreachable";
                            break;
                        }

                    default:
                        {
                            Message = DefaultSQLMessage;
                            break;
                        }
                }
            }
            else if (ReferenceEquals(ex.GetType(), typeof(OutOfMemoryException))) // handle local memory exception here
            {
                Message = "No enough memory to complete process. Release more memory by closing some application";
            }
            else if (ex.Message.ToLower().Contains("object reference"))
            {
                Message = "Error in processing, please try again";
            }
            else // other exceptions
            {
                Message = ex.Message;
            }
        }

        public string GetMessage()
        {
            return Message;
        }
    }
}