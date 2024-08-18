using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Transporter.Common.Constants;
using Transporter.Common.DTO;
using Transporter.Common.Enums;
using Transporter.Common.Helper;
using Transporter.Common.Models;
using Transporter.Common.VM;
using Transporter.DataAccess;
using Transporter.Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using Transporter.Services.Utils;
using DocumentFormat.OpenXml.Math;
using XLParser;

namespace Transporter.Services.Services
{
#pragma warning disable CS8600
    public class SecurityService : ISecurityService
    {
        private readonly SbDbContext _crmDbContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SecurityService(SbDbContext ctx, IServiceScopeFactory serviceScopeFactor)
        {
            this._crmDbContext = ctx;
            this._serviceScopeFactory = serviceScopeFactor;
        }

        /// <summary>
        /// method for checking action.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<bool> CheckPermission(string url)
        {
            //uncomment when implement action and permisson

            //Actions objAction = await this._crmDbContext.Actions.Where(x => x.ActionName == url && x.Status == (int)Enums.Status.Active).FirstOrDefaultAsync();
            //if (objAction != null)
            //{
            //    return true;
            //}

            return await Task.FromResult(true);
        }

        public async Task<ResponseMessage> Login(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                VMLogin objVMLogin = JsonConvert.DeserializeObject<VMLogin>(requestMessage.RequestObj.ToString());
                SystemUser existingSystemUser = new SystemUser();
                if (objVMLogin == null || string.IsNullOrEmpty(objVMLogin.Email) || string.IsNullOrEmpty(objVMLogin.Password))
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Invalid username or password";
                    return responseMessage;
                }

                if (objVMLogin.SSOLogin)
                {
                    var accessToken = objVMLogin.SSOAccessToken;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtSecurityToken = handler.ReadJwtToken(accessToken);
                    string email = jwtSecurityToken.Claims.First(claim => claim.Type == "unique_name").Value;
                    string firstName = jwtSecurityToken.Claims.First(claim => claim.Type == "given_name").Value;
                    existingSystemUser = _crmDbContext.SystemUser.Where(x => x.Email == email && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefault();

                    if (existingSystemUser == null)
                    {
                        SystemUser objSystemUser = new SystemUser();
                        objSystemUser.FirstName = firstName;
                        objSystemUser.Email = email;
                        objSystemUser.Password = BCrypt.Net.BCrypt.HashPassword(existingSystemUser.Password);
                        objSystemUser.CreatedBy = 0;
                        objSystemUser.CreatedDate = DateTime.Now;
                        objSystemUser.Status = (int)Enums.Status.Active;
                        _crmDbContext.SystemUser.Add(objSystemUser);
                        await _crmDbContext.SaveChangesAsync();
                        objVMLogin.SystemUserID = (objSystemUser != null) ? objSystemUser.SystemUserID : 0;
                    }
                }
                else
                {
                    //NOTE: below code is for userID = Email
                    //existingSystemUser = await _crmDbContext.SystemUser.Where(x => x.Email == objVMLogin.Email && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefaultAsync();

                    //NOTE: below code is for userID = Short Name
                    existingSystemUser = await _crmDbContext.SystemUser.Where(x => x.Email == objVMLogin.Email && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefaultAsync();

                    if (existingSystemUser == null || !BCrypt.Net.BCrypt.Verify(objVMLogin.Password, existingSystemUser.Password))
                    {
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                        responseMessage.Message = "Invalid username or password";
                        return responseMessage;
                    }
                    else if (!existingSystemUser.IsApproved)
                    {
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                        responseMessage.Message = "User is not approved, please contact with admin.";
                        return responseMessage;
                    }
                    else
                    {
                        objVMLogin.SystemUserID = existingSystemUser.SystemUserID;
                        objVMLogin.RoleID = existingSystemUser.Role;
                        objVMLogin.FirstName = existingSystemUser.FirstName;
                        objVMLogin.LastName = existingSystemUser.LastName;
                        objVMLogin.PhoneNumber = existingSystemUser.PhoneNumber;

                        if (existingSystemUser.ReferenceTypeID == (int)Enums.UserReferenceType.VendorAdmin
                            || existingSystemUser.ReferenceTypeID == (int)Enums.UserReferenceType.VendorEmployee)
                        {
                            objVMLogin.EmployeeId = 0;
                            objVMLogin.VendorId = existingSystemUser.ReferenceID;
                        }
                        else
                        {
                            objVMLogin.EmployeeId = existingSystemUser.ReferenceID;
                            objVMLogin.VendorId = 0;
                        }
                        objVMLogin.VendorOrEmployeeId = existingSystemUser.ReferenceID;
                        //objVMLogin .RoleName = existingSystemUser.Role
                    }
                }

                //get permission
                if (objVMLogin != null && objVMLogin.RoleID > 0)
                {
                    objVMLogin.RoleName = GetRoleName(existingSystemUser.Role);
                    objVMLogin.lstPermissions = await GetAllPermissionByRoleId(objVMLogin.RoleID);
                }

                //if (existingSystemUser != null && existingSystemUser.Role)
                //{
                //    objVMLogin.lstPermissions = _crmDbContext.Permission.OrderBy(x => x.Sequence).ToList();
                //}

                //objVMLogin.lstAideDepartment = _crmDbContext.VMDepartment.Where(d => d.DepartmentTypeID == (int)Enums.DepartmentType.Aide).ToList();
                //objVMLogin.lstPatientDepartment = _crmDbContext.VMDepartment.Where(d => d.DepartmentTypeID == (int)Enums.DepartmentType.Patient).ToList();

                //string sql = "Select * from Organization where OrganizationID in (Select OrganizationID from SystemUserOrganizationMapping where SystemUserID=" + objVMLogin.SystemUserID + ")";
                //objVMLogin.lstUserOrganization = _crmDbContext.Organization.FromSqlRaw<Organization>(sql).ToList();

                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                responseMessage.Message = MessageConstant.LoginSuccess;

                objVMLogin.Password = String.Empty;
                responseMessage.ResponseObj = objVMLogin;

                //Log write
                LogHelper.WriteLog(requestMessage.RequestObj, (int)Enums.ActionType.Login, objVMLogin.SystemUserID, "Login");
            }
            catch (Exception ex)
            {

                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ex.ToString(); // ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Login, 0, JsonConvert.SerializeObject(requestMessage.RequestObj), "Login");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        private async Task<List<Page>> GetAllPermissionByRoleId(int roleId)
        {
            List<Page> lstPermission = new List<Page>();
            List<RolePermissionMapping> lstRolePermissionMapping = await _crmDbContext.RolePermissionMapping.Where(r => r.RoleID == roleId).ToListAsync();
            List<int> lstPermissionIDs = lstRolePermissionMapping.Select(r => r.PermissionID).Distinct().ToList();
            lstPermission = _crmDbContext.Page.Where(p => lstPermissionIDs.Contains(p.PageID)).ToList();
            return lstPermission;
        }

        private string GetRoleName(int roleID)
        {
            List<Roles> lstRole = _crmDbContext.Role.Where(r => r.RoleID == roleID).ToList();
            if (lstRole.Count > 0)
            {
                return lstRole[0].RoleName;
            }

            return "";
        }

        /// <summary>
        /// Method for log out.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> Logout(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {

                List<UserSession> lstUserSession = new List<UserSession>();

                lstUserSession = await _crmDbContext.UserSession.AsNoTracking().Where(x => x.SystemUserID == requestMessage.UserID && x.Status == (int)Enums.Status.Active).ToListAsync();

                if (lstUserSession.Count > 0)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetService<SbDbContext>();
                        foreach (var item in lstUserSession)
                        {
                            item.SessionEnd = DateTime.Now;
                            item.Status = (int)Enums.Status.Inactive;
                        }
                        db.UserSession.UpdateRange(lstUserSession);
                        await db.SaveChangesAsync();
                    }
                }


                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                responseMessage.Message = MessageConstant.LogOutSuccessfully;
                return responseMessage;
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Logout, 0, JsonConvert.SerializeObject(requestMessage.RequestObj), "Logout");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;

        }


        /// <summary>
        /// method for save user.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> Register(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int actionType = (int)Enums.ActionType.Insert;
            try
            {
                VMRegister objVMRegister = JsonConvert.DeserializeObject<VMRegister>(requestMessage.RequestObj.ToString());

                if (objVMRegister == null)
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = MessageConstant.Invaliddatafound;
                    return responseMessage;
                }
                else
                {
                    if (CheckedValidation(objVMRegister, responseMessage))
                    {

                        if (objVMRegister.Password != objVMRegister.ConfirmPassword)
                        {
                            responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                            responseMessage.Message = MessageConstant.Confirmpasswordnotmatch;
                            return responseMessage;
                        }

                        SystemUser objSystemUser = new SystemUser();
                        objSystemUser.Status = (int)Enums.Status.Active;
                        objSystemUser.CreatedDate = DateTime.Now;
                        objSystemUser.CreatedBy = requestMessage.UserID;
                        objSystemUser.FirstName = objVMRegister.FirstName;
                        objSystemUser.LastName = objVMRegister.LastName;
                        objSystemUser.PhoneNumber = objVMRegister.PhoneNumber;
                        objSystemUser.Email = objVMRegister.Email;
                        objSystemUser.Password = BCrypt.Net.BCrypt.HashPassword(objVMRegister.Password);

                        await _crmDbContext.SystemUser.AddAsync(objSystemUser);
                        await _crmDbContext.SaveChangesAsync();

                        objSystemUser.Password = string.Empty;
                        responseMessage.ResponseObj = objSystemUser;
                        responseMessage.Message = MessageConstant.RegisterSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "Register");
                    }
                }
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Register, 0, JsonConvert.SerializeObject(requestMessage.RequestObj), "Register");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objVMRegister"></param>
        /// <returns></returns>
        private bool CheckedValidation(VMRegister objVMRegister, ResponseMessage responseMessage)
        {

            SystemUser existingSystemUser = new SystemUser();

            if (String.IsNullOrEmpty(objVMRegister.FirstName))
            {
                responseMessage.Message = MessageConstant.FirstNameRequired;
                return false;
            }
            if (String.IsNullOrEmpty(objVMRegister.LastName))
            {
                responseMessage.Message = MessageConstant.LastNameRequired;
                return false;
            }
            if (String.IsNullOrEmpty(objVMRegister.PhoneNumber))
            {
                responseMessage.Message = MessageConstant.PhoneRequired;
                return false;
            }
            if (String.IsNullOrEmpty(objVMRegister.Email))
            {
                responseMessage.Message = MessageConstant.EmailRequired;
                return false;
            }
            if (String.IsNullOrEmpty(objVMRegister.Password))
            {
                responseMessage.Message = MessageConstant.PasswordRequired;
                return false;
            }
            existingSystemUser = _crmDbContext.SystemUser.Where(x => x.PhoneNumber.ToLower() == objVMRegister.PhoneNumber.ToLower() && x.Email.ToLower() == objVMRegister.Email.ToLower() && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefault();
            if (existingSystemUser != null)
            {
                responseMessage.Message = MessageConstant.DuplicatePhoneAndEmail;
                return false;
            }
            //existingSystemUser = _crmDbContext.SystemUser.Where(x => x.Email == objVMRegister.Email && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefault();
            //if (existingSystemUser != null)
            //{
            //    responseMessage.Message = MessageConstant.EmailAlreadyExist;
            //    return false;
            //}
            //existingSystemUser = _crmDbContext.SystemUser.Where(x => x.PhoneNumber == objVMRegister.PhoneNumber && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefault();
            //if (existingSystemUser != null)
            //{
            //    responseMessage.Message = MessageConstant.PhoneNumberAlreadyExist;
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// Generate Token
        /// </summary>
        /// <param name="systemUser"></param>
        /// <returns></returns>
        private string GenerateToken(SystemUser systemUser)
        {
            string token = string.Empty;

            return token;
        }

    }

#pragma warning restore CS8600


}
