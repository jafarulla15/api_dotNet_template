using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.AuthInfo
{
    public class LoginInfo
    {
        private long sessionID = 0;
        [Key]
        [DataType("bigint")]
        public long SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }


        private string sessionKey = "";

        [DataType("nvarchar")]
        public string SessionKey
        {
            get { return sessionKey; }
            set { sessionKey = value; }
        }


        private string userID = "";

        [DataType("nvarchar")]
        public string UserID
        {
            get { return userID; }
            set { userID = value; }
        }


        private DateTime loginDateTime = DateTime.Now;

        [DataType("datetime")]
        public DateTime LoginDateTime
        {
            get { return loginDateTime; }
            set { loginDateTime = value; }
        }


        private DateTime logoutDate = DateTime.Now;

        [DataType("datetime")]
        public DateTime LogoutDate
        {
            get { return logoutDate; }
            set { logoutDate = value; }
        }


        private string iPAddress = "";

        [DataType("nvarchar")]
        public string IPAddress
        {
            get { return iPAddress; }
            set { iPAddress = value; }
        }


        private int companyId = 0;

        [DataType("int")]
        public int? CompanyId
        {
            get { return companyId; }
            set { companyId = (int)value; }
        }


        private int status = 0;

        [DataType("int")]
        public int Status
        {
            get { return status; }
            set { status = value; }
        }


        private string mACAddress = "";

        [DataType("nvarchar")]
        public string MACAddress
        {
            get { return mACAddress; }
            set { mACAddress = value; }
        }


        private string hostName = "";

        [DataType("nvarchar")]
        public string HostName
        {
            get { return hostName; }
            set { hostName = value; }
        }


        private string interfaceName = "";

        [DataType("nvarchar")]
        public string InterfaceName
        {
            get { return interfaceName; }
            set { interfaceName = value; }
        }


        private string protocol = "";

        [DataType("nvarchar")]
        public string Protocol
        {
            get { return protocol; }
            set { protocol = value; }
        }


        private string publicIP = "";

        [DataType("nvarchar")]
        public string PublicIP
        {
            get { return publicIP; }
            set { publicIP = value; }
        }


        private string interfaceDescription = "";

        [DataType("nvarchar")]
        public string InterfaceDescription
        {
            get { return interfaceDescription; }
            set { interfaceDescription = value; }
        }


    }

}
