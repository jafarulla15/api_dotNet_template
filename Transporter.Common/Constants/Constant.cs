using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Constants
{
	public static class MessageConstant
	{
		public const string UserNotFound = "User not found.";
		public const string EnterValidEntry = "Enter valid entry.";
        public const string RejectSuccessfully = "Rejected successfully.";
        public const string RejectFailed = "Reject failed.";
        public const string ApprovedSuccessfully = "Approved successfully.";
		public const string ApprovedFailed = "Approved failed.";
		public const string RemovedSuccessfully = "Removed successfully.";
		public const string RemoveFailed = "Remove failed.";
		public const string FileUploadedSuccessfully = "File uploaded successfully.";
		public const string FileUploadFailed = "File upload failed.";
		public const string RevertSuccessfully = "Revert successfully.";
		public const string SavedSuccessfully = "Saved successfully.";
		public const string SetAsDefaultSuccessfully = "The default has been set successfully.";
		public const string RegisterSuccessfully = "Register successfully.";
		public const string SaveFailed = "Failed to save information.";
		public const string DeleteFailed = "Failed to delete.";
		public const string DeleteSuccess = "Delete successfully.";
		public const string SaveUserssuccess = "User save successfully.";
		public const string SaveUsersFail = "User save successfully.";
		public const string DuplicateUserIDOrEmail = "Duplicate user name or email.";
		public const string DuplicateEmail = "Duplicate user email.";
		public const string DuplicateUserName = "Duplicate user name";
		public const string DuplicatePhoneAndEmail = "Duplicate user with same email and phone no";
		public const string PhoneRequired = "Phone no is required";
		public const string EmailRequired = "Email must be required";
		public const string FirstNameRequired = "First name is required";
		public const string LastNameRequired = "Last name is required";
		public const string FileNameRequired = "FileName is Required.";
		public const string FileAttachmentNote = "File attachment note is required.";
		public const string FileNotFound = "File not found.";
		public const string FileTypeNotFound = "File type not found.";
		public const string FileFormatNotOK = "File format is not correct.";
		public const string FileDataSaveFailed = "File data failed to save.";
		public const string EnterNewPassword = "Enter new password.";
		public const string EnterPasswordValidLength = "Enter password between 4 and 40 characters.";
		public const string EnterConfirmPasswordMatched= "Confirm password is not matched.";
		public const string PasswordChangedSuccessfully = "Password changed successfully.";
		public const string StatusNotFound = "Sorry, Status not found.";
		public const string InvalidData = "Sorry, Invalid data.";
		public const string StatusChangedSuccessfully = "Status changed successfully.";
		public const string EnterValidStatusName = "Please enter status name correctly.";

		public const string DuplicateStatusName = "Duplicate status name.";
		public const string DuplicatePermissionName = "Duplicate permission name.";
		public const string DuplicateRoleName = "Duplicate role name.";

		public const string UsernameRequired = "Username must be required";
		public const string PasswordRequired = "Password must be required";
		public const string EmailAlreadyExist = "Email already exist";
		public const string PhoneNumberAlreadyExist = "Phone number already exist";
        public const string UserWithThisPhoneNumberAlreadyExist = "User with this phone number already exist";
        public const string RegistrationRequestWithThisPhoneNumberAlreadyExist = "Registration request with this phone number already exist";

        public const string ConfirmPasswordNotMatch = "Confirm password not matched";
		public const string DepartmentName = "Department name is required.";
		public const string DepartmentNameExist = "This department name is already exist.";
		public const string DepartmentStatusTitle = "Department status title is required.";
		public const string DepartmentTaskID = "Please select department task.";
		public const string TaskDescription = "Task description is required.";
		public const string KnowledgeDetail = "Knowledge detail is required.";
		public const string KnowledgeBase = "Please select knowledge.";
		public const string PermissionName = "Permission name is required.";
		public const string RoleName = "Role name is required.";
		public const string OrganizationName = "Organization name is required.";
		public const string TagTitle = "Tag title is required.";
		public const string TableName = "Table name is required.";
		public const string ColumnName = "Column name is required.";
		public const string ActionName = "Action name is required.";
		public const string DepartmentTaskDescription = "Department task description is required.";
		public const string DepartmentTypeName = "Department type  name is required.";
		public const string Email = "Email name is required.";
		public const string EmployeeName = "Employee name is required.";
		public const string LoginSuccess = "Login successfully";
		public const string Unauthorizerequest = "Unauthorize request.";
		public const string InternalServerError = "Internal server error.";
		public const string LogOutSuccessfully = "Log out successfully.";
		public const string Invaliddatafound = "Invalid data found.";
		public const string Confirmpasswordnotmatch = "Confirm password not match.";
		public const string PatientNote = "Patient Note is Required.";
		public const string CaregiverNote = "Care giver Note is required.";
		public const string Token = "Token is required.";
		public const string CustomFilterObject = "Filter object is required.";
		public const string TagColor = "Tag color is required.";
		public const string SelectUser = "Please select users.";
		public const string AnyuserNotMapping = "Any user not mapping.";

        

    }

    public static class ValidationMessage
    {
        public const string CompanyName = "Company name is required.";
        public const string Alias = "Alias is required.";
        public const string CompanyNameExist = "Company name already exist.";
        public const string AliasExist = "Alias already exist.";

        public const string FirstName = "First name is required.";
        public const string LicenseNO = "LicenseNO is required.";
        public const string VehicleType = "Vehicle type is required.";

        public const string Phone = "Phone number is required.";
        public const string Email = "Email is required.";
        public const string Role = "Role is required.";
        public const string Company = "Company is required.";
        public const string Password = "Password is required.";

        public const string FirstNameExist = "First name already exist.";
        public const string PhoneExist = "Phone already exist.";
        public const string EmailExist = "Email already exist.";
        public const string LicenseNOExist = "LicenseNO already exist.";

        public const string Driver_PhoneExist = "Driver with this phone no. already exist.";
        public const string Driver_FirstExist = "Driver with this first name already exist.";

    }

    public static class CommonConstant
	{
		public static DateTime DeafultDate = Convert.ToDateTime("1900/01/01");
		public static int SessionExpired = 30;
	}

	public static class CommonPath
	{
		public const string loginUrl = "/api/security/login";
		public const string registerUrl =  "/api/SystemUser/registerSystemUser";  // "/api/security/register";
        public const string registerUrl_Vendor = "/api/VendorRegistrationRequest/Save";  // "/api/security/register";
        public const string testUrl = "/api/Test/get";  // Test/get

    }
    public class HttpHeaders
	{
		public const string Token = "Authorization";
		public const string AuthenticationSchema = "Bearer";
	}

    public class JwtClaims
    {
        public const string UserId = "UserId";
        // public const string CompanyUserId = "CompanyUserId";
        public const string UserTypeId = "UserTypeId";
        public const string UserName = "admin"; // "UserName";
        public const string CompanyName = "Libas"; // "CompanyName";
        public const string CompanyId = "1"; // "CompanyId";
        public const string Name = "Name";
        public const string Email = "Email";
        public const string IsSuperAdmin = "IsSuperAdmin";
        public const string IssuedOn = "IssuedOn";
        public const string ExpiresDate = "ExpiresDate";
        public const string AccessRight = "AccessRight";
        public const string UniqueName = "UniqueName";
        public const string UniqueID = "UniqueID";
        public const string SessionId = "SessionId";

    }

}
