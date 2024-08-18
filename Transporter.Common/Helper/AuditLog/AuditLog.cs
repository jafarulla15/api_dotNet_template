using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Helper.AuditLog
{
    public class AuditLog
    {

        private int logID = 0;

        [Key]
        [DataType("bigint")]
        public int LogID
        {
            get { return logID; }
            set { logID = value; }
        }


        private string userID = "";

        [DataType("nvarchar")]
        public string UserID
        {
            get { return userID; }
            set { userID = value; }
        }


        private int moduleID = 0;

        [DataType("int")]
        public int ModuleID
        {
            get { return moduleID; }
            set { moduleID = value; }
        }

        private string formName = "";

        [DataType("nvarchar")]
        public string FormName
        {
            get { return formName; }
            set { formName = value; }
        }

        private string calledFuntion = "";
        [DataType("nvarchar")]
        public string CalledFunction
        {
            get { return calledFuntion; }
            set { calledFuntion = value; }
        }

        private int actionID = 0;

        [DataType("int")]
        public int ActionID
        {
            get { return actionID; }
            set { actionID = value; }
        }


        private int userRightID = 0;

        [DataType("int")]
        public int UserRightID
        {
            get { return userRightID; }
            set { userRightID = value; }
        }

        private int userTypeID = 0;

        [DataType("int")]
        public int UserTypeID
        {
            get { return userTypeID; }
            set { userTypeID = value; }
        }


        private string logMessage = "";

        [DataType("nvarchar")]
        public string LogMessage
        {
            get { return logMessage; }
            set { logMessage = value; }
        }


        private string logRefMessage = "";

        [DataType("nvarchar")]
        public string LogRefMessage
        {
            get { return logRefMessage; }
            set { logRefMessage = value; }
        }

        private bool isObj = false;

        [DataType("bool")]
        public bool IsObj
        {
            get { return isObj; }
            set { isObj = value; }
        }

        private int companyID = 0;

        [DataType("int")]
        public int CompanyID
        {
            get { return companyID; }
            set { companyID = value; }
        }


        private DateTime logTime = DateTime.Now;

        [DataType("datetime")]
        public DateTime LogTime
        {
            get { return logTime; }
            set { logTime = value; }
        }


        private int logTypeID = 0;

        [DataType("int")]
        public int LogTypeID
        {
            get { return logTypeID; }
            set { logTypeID = value; }
        }


        private int sessionID = 0;

        [DataType("bigint")]
        public int SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }




    }
}
