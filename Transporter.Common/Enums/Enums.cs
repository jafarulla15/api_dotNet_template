namespace Transporter.Common.Enums
{
    public class Enums
    {
        public enum UserReferenceType
        {
            VendorAdmin = 1,
            VendorEmployee = 2,
            Employee = 3,
            AdminEmployee = 4,
            Admin = 5,
        }
        public enum BidStatus
        {
            Win = 1,
            Reject = 2,
        }
        public enum LogFixPriority
        {
            Low = 1,
            Medium = 2,
            High = 3
        }
        public enum StatusVendorUser
        {
            Pending,
            Active,
            Blocked,
            Suspended,
        }

        public enum StatusVendorRegistrationRequest
        {
            Pending = 1,
            Active = 2,
            Blocked = 3,
            Suspended = 4,
        }

        public enum Status
        {
            Active = 1,
            Inactive = 2,
            Delete = 9
        }

        public enum ResponseCode
        {
            Success = 200,
            InternalServerError = 500,
            Failed = 404,
            Warning = 400,
            UnAuthorize = 401,
            ValidationError = 405, //TODO: confirm this standard code.
        }

        public enum ActionType
        {
            Insert = 1,
            Update = 2,
            View = 3,
            Delete = 4,
            Login = 5,
            Register = 6,
            Logout = 7,
        }
        public enum Gender
        {
            Male = 1,
            Famale = 2
        }

        public enum Priority
        {
            LOW = 1,
            MEDIUM,
            HIGH,
            URGENT,
            IMPORTANT
        }

        public enum LoginStatus
        {
            Loggedin = 1,
            Loggedout = 0
        }

        public enum EntityType
        {
            BOE = 1,
            Anash = 2,
        }

        public enum MismatchHistoryStatus
        {
            Warning = 1,
            Accepted = 2,
            Rejected = 3
        }

       
    }
}
