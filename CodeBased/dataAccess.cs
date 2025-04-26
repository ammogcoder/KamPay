using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Kpakam.Model;
using Kpakam.Helper;
using Kpakam.Interface;
using System.Security.Cryptography;

namespace Kpakam.CodeBased
{
    public class dataAccess : IdataAccess
    // : IDatabaseExtraction
    {
        appSettings _appSettings = null;
        // Dim conn As New SQLSeverConnection
        // Private ConnectionString As String = conn.connectionstring
        private static string ConnectionString;// ConfigurationSettings.AppSettings["ConnectionString"];
        public static List<string> ExemptState = new List<string>() { "438" };
        public int CurrentUserID;
        public string CurrentUserOffice;
        public bool IsHeadquarter;
        public string StateId;

        public dataAccess(IOptions<appSettings> AppSettings)
        {
            _appSettings = AppSettings.Value;
            ConnectionString = _appSettings.ConnectionString;
        }

        public dataAccess(string _ConnectionString)
        {
            ConnectionString = _ConnectionString;
        }

        public int ModifyProcess(Guid ProcID, Guid IntegratorID, DateTime datestamp, string Inflow, string Outflow)
        {
            var Param = new SqlParameter[] { new SqlParameter("@ProcID", ProcID),
                new SqlParameter("@IntegratorID", IntegratorID) ,
                new SqlParameter("@datestamp", datestamp) ,
                new SqlParameter("@Inflow", Inflow) ,
                new SqlParameter("@Outflow", Outflow) };
            try
            {
                return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "ModifyProcess", Param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetCurrentIntegrator(string IntKey)
        {
            var Param = new SqlParameter[] { new SqlParameter("@IntKey", IntKey) };
            try
            {
                var ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text,
                    "Select * from Integrators where IntKey = @IntKey", Param);
                if(ds != null && ds.Tables.Count>0)
                    return ds.Tables[0];
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}