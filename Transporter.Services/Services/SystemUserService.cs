using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Transporter.Common.Constants;
using Transporter.Common.DTO;
using Transporter.Common.Enums;
using Transporter.Common.Helper;
using Transporter.Common.Helper.AuditLog;
using Transporter.Common.Models;
using Transporter.Common.VM;
using Transporter.DataAccess;
using Transporter.Repository;
using Transporter.Services.Interface;
using Transporter.Services.Services.Log;
using Transporter.Services.Utils;
using static Transporter.Common.Enums.Enums;

namespace Transporter.Services
{
#pragma warning disable CS8600
    public class SystemUserService : ISystemUserService
    {
        private readonly IUnitOfWork<SbDbContext> _unitOfWork;
        private readonly IExceptionLogService _exceptionLog;
        private readonly SbDbContext _crmDbContext;

        public SystemUserService(SbDbContext ctx, IUnitOfWork<SbDbContext> unitOfWork, IExceptionLogService exceptionLog)
        {
            this._crmDbContext = ctx;
            _unitOfWork = unitOfWork;
            _exceptionLog = exceptionLog;
        }

        /// <summary>
        /// Get all System User
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllSystemUser(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<SystemUser> lstSystemUser = new List<SystemUser>();
                string searchtext = string.Empty;
                searchtext = requestMessage?.RequestObj?.ToString();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                IQueryable<SystemUser> lstUsers = (!string.IsNullOrEmpty(searchtext)) ? _crmDbContext.SystemUser.Where(x => x.FirstName.ToLower().Contains(searchtext.ToLower()) || x.LastName.ToLower().Contains(searchtext.ToLower()) ||
                 x.Email.ToLower().Contains(searchtext.ToLower()) || x.PhoneNumber.ToLower().Contains(searchtext.ToLower())) : _crmDbContext.SystemUser;

                //lstSystemUser = await lstUsers.Where(x => x.Status == (int)Enums.Status.Active).OrderBy(x => x.SystemUserID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                lstSystemUser = await lstUsers.Where(x => x.Status == (int)Enums.Status.Active).OrderBy(x => x.FirstName).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstSystemUser.Count();

                #region ""
                //foreach (SystemUser user in lstSystemUser)
                //{
                //    user.Password = string.Empty;

                //    // get all department of each user
                //    List<SystemUserDepartmentMapping> lstSystemUserDepartmentMappings = await _crmDbContext.SystemUserDepartmentMapping.AsNoTracking().Where(x => x.SystemUserID == user.SystemUserID).ToListAsync();

                //    if (lstSystemUserDepartmentMappings.Count > 0)
                //    {
                //        foreach (SystemUserDepartmentMapping objSystemUserDepartmentMapping in lstSystemUserDepartmentMappings)
                //        {
                //            Department objDepartment = await _crmDbContext.Department.AsNoTracking().Where(x => x.DepartmentID == objSystemUserDepartmentMapping.DepartmentID).FirstOrDefaultAsync();

                //            if (objDepartment != null)
                //            {
                //                user.lstDepartment.Add(objDepartment);
                //            }

                //        }
                //    }

                //    // get all organization of each user
                //    List<SystemUserOrganizationMapping> lstSystemUserOrganizationMappings = await _crmDbContext.SystemUserOrganizationMapping.AsNoTracking().Where(x => x.SystemUserID == user.SystemUserID).ToListAsync();

                //    if (lstSystemUserOrganizationMappings.Count > 0)
                //    {
                //        foreach (SystemUserOrganizationMapping objSystemUserOrganizationMapping in lstSystemUserOrganizationMappings)
                //        {
                //            Organization objOrganization = await _crmDbContext.Organization.AsNoTracking().Where(x => x.OrganizationID == objSystemUserOrganizationMapping.OrganizationID).FirstOrDefaultAsync();

                //            if (objOrganization != null)
                //            {
                //                user.lstOrganization.Add(objOrganization);
                //            }

                //        }
                //    }

                //}
                #endregion

                foreach (SystemUser user in lstSystemUser)
                {
                    user.Password = string.Empty;
                }

                responseMessage.ResponseObj = lstSystemUser;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllSystemUser");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllSystemUser");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        public async Task<ResponseMessage> GetAllEmployeeSystemUserIncludingExtendedData(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                var filtersData = JsonConvert.DeserializeObject<FilterVM>(requestMessage.RequestObj.ToString());

                List<SystemUser> lstSystemUser = new List<SystemUser>();
                List<VMSystemUser> lstVMSystemUserTemp = new List<VMSystemUser>();

                string searchtext = string.Empty;
                searchtext = requestMessage?.RequestObj?.ToString();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;
                string sql = string.Empty;
                if (filtersData.FilterColumns.Any() || filtersData.SearchText != "")
                {
                    sql = @"select u.*, CASE WHEN RoleName IS NULL or Role = 0
                                                THEN 'Not Assigned'
                                                ELSE RoleName
                                                END AS RoleName
                                            , 
                                            CASE WHEN sa.StatusName IS NULL 
                                            THEN 'Not Assigned'
                                            ELSE sa.StatusName
                                            END AS StatusName
                                            , sa.ColorCode
                            from SystemUser  as u
                            left join Role as r on r.RoleID = u.Role
                            left join StatusOfApp as sa on sa.StatusOfAppID = u.StatusOfUser
                            where u.Status = 1 and ReferenceTypeID <> 1 and ReferenceTypeID <> 2 "
                            + " and (u.FirstName LIKE '%" + filtersData.SearchText.Trim() + "%' or u.LastName LIKE '%"
                            + filtersData.SearchText.Trim() + "%'or u.Email LIKE '%" + filtersData.SearchText.Trim() + "%' or u.PhoneNumber LIKE '%"
                            + filtersData.SearchText.Trim() + "%')";
                }
                else
                {
                    sql = @"select u.*, CASE WHEN RoleName IS NULL or Role = 0
                                                THEN 'Not Assigned'
                                                ELSE RoleName
                                                END AS RoleName
                                            , 
                                            CASE WHEN sa.StatusName IS NULL 
                                            THEN 'Not Assigned'
                                            ELSE sa.StatusName
                                            END AS StatusName
                                            , sa.ColorCode
                            from SystemUser  as u
                            left join Role as r on r.RoleID = u.Role
                            left join StatusOfApp as sa on sa.StatusOfAppID = u.StatusOfUser
                            where u.Status = 1 and ReferenceTypeID <> 1 and ReferenceTypeID <> 2 ";
                }


                //List<VMSystemUser> lstVMSystemUser = _crmDbContext.VMSystemUser.FromSqlRaw<VMSystemUser>(sql).ToList();
                List<VMSystemUser> lstVMSystemUser = _crmDbContext.VMSystemUser.FromSqlRaw(sql).ToList();

                //IQueryable<SystemUser> lstUsers = (!string.IsNullOrEmpty(searchtext)) ? _crmDbContext.SystemUser.Where(x => x.FirstName.ToLower().Contains(searchtext.ToLower()) || x.LastName.ToLower().Contains(searchtext.ToLower()) ||
                // x.Email.ToLower().Contains(searchtext.ToLower()) || x.PhoneNumber.ToLower().Contains(searchtext.ToLower())) : _crmDbContext.SystemUser;

                //lstSystemUser = await lstUsers.Where(x => x.Status == (int)Enums.Status.Active).OrderBy(x => x.SystemUserID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstVMSystemUser.Count();

                foreach (VMSystemUser user in lstVMSystemUser)
                {
                    user.Password = string.Empty;
                }

                responseMessage.ResponseObj = lstVMSystemUser;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllSystemUser");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllSystemUser");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        public async Task<ResponseMessage> GetAllSystemUserIncludingExtendedData(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                var filtersData = JsonConvert.DeserializeObject<FilterVM>(requestMessage.RequestObj.ToString());

                List<SystemUser> lstSystemUser = new List<SystemUser>();
                List<VMSystemUser> lstVMSystemUserTemp = new List<VMSystemUser>();

                string searchtext = string.Empty;
                searchtext = requestMessage?.RequestObj?.ToString();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;
                string sql = string.Empty;
                if (filtersData.FilterColumns.Any() || filtersData.SearchText != "")
                {
                    sql = @"select u.*, CASE WHEN RoleName IS NULL or Role = 0
                                                THEN 'Not Assigned'
                                                ELSE RoleName
                                                END AS RoleName
                                            , 
                                            CASE WHEN sa.StatusName IS NULL 
                                            THEN 'Not Assigned'
                                            ELSE sa.StatusName
                                            END AS StatusName
                                            , sa.ColorCode
                            from SystemUser  as u
                            left join Role as r on r.RoleID = u.Role
                            left join StatusOfApp as sa on sa.StatusOfAppID = u.StatusOfUser
                            where u.Status = 1 " + " and (u.FirstName LIKE '%" + filtersData.SearchText.Trim() + "%' or u.LastName LIKE '%"
                            + filtersData.SearchText.Trim() + "%'or u.Email LIKE '%" + filtersData.SearchText.Trim() + "%' or u.PhoneNumber LIKE '%"
                            + filtersData.SearchText.Trim() + "%')";
                }
                else
                {
                    sql = @"select u.*, CASE WHEN RoleName IS NULL or Role = 0
                                                THEN 'Not Assigned'
                                                ELSE RoleName
                                                END AS RoleName
                                            , 
                                            CASE WHEN sa.StatusName IS NULL 
                                            THEN 'Not Assigned'
                                            ELSE sa.StatusName
                                            END AS StatusName
                                            , sa.ColorCode
                            from SystemUser  as u
                            left join Role as r on r.RoleID = u.Role
                            left join StatusOfApp as sa on sa.StatusOfAppID = u.StatusOfUser
                            where u.Status = 1 ";
                }


                //List<VMSystemUser> lstVMSystemUser = _crmDbContext.VMSystemUser.FromSqlRaw<VMSystemUser>(sql).ToList();
                List<VMSystemUser> lstVMSystemUser = _crmDbContext.VMSystemUser.FromSqlRaw(sql).ToList();

                //IQueryable<SystemUser> lstUsers = (!string.IsNullOrEmpty(searchtext)) ? _crmDbContext.SystemUser.Where(x => x.FirstName.ToLower().Contains(searchtext.ToLower()) || x.LastName.ToLower().Contains(searchtext.ToLower()) ||
                // x.Email.ToLower().Contains(searchtext.ToLower()) || x.PhoneNumber.ToLower().Contains(searchtext.ToLower())) : _crmDbContext.SystemUser;

                //lstSystemUser = await lstUsers.Where(x => x.Status == (int)Enums.Status.Active).OrderBy(x => x.SystemUserID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstVMSystemUser.Count();

                foreach (VMSystemUser user in lstVMSystemUser)
                {
                    user.Password = string.Empty;
                }

                responseMessage.ResponseObj = lstVMSystemUser;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllSystemUser");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllSystemUser");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseMessage> GetSystemUserById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                SystemUser objSystemUser = new SystemUser();
                int systemUserID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objSystemUser = await _crmDbContext.SystemUser.FirstOrDefaultAsync(x => x.SystemUserID == systemUserID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objSystemUser;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetSystemUserById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetSystemUserById");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseMessage> GetSystemUserByPhoneNo(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                SystemUser objSystemUser = new SystemUser();

                string userPhone = requestMessage?.RequestObj?.ToString();

                objSystemUser = await _crmDbContext.SystemUser.FirstOrDefaultAsync(x => x.PhoneNumber.ToLower() == userPhone.ToLower() && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objSystemUser;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetSystemUserByPhone");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetSystemUserByPhone");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseMessage> GetSystemUserByEmail(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                SystemUser objSystemUser = new SystemUser();
                string email = requestMessage?.RequestObj.ToString();

                objSystemUser = await _crmDbContext.SystemUser.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower() && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objSystemUser;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetSystemUserByEmail");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetSystemUserById");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }



        public async Task<ResponseMessage> ChangePassword(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                ChangePassword objChangePassword = JsonConvert.DeserializeObject<ChangePassword>(requestMessage?.RequestObj.ToString());
                if (objChangePassword != null)
                {
                    if (CheckedValidationOfChangePassword(objChangePassword, responseMessage))
                    {
                        if (requestMessage.UserID > 0)
                        {
                            SystemUser existingSystemUser = await this._crmDbContext.SystemUser.AsNoTracking().FirstOrDefaultAsync(x => x.SystemUserID == requestMessage.UserID && x.Status == (int)Enums.Status.Active);
                            if (existingSystemUser != null)
                            {
                                existingSystemUser.Password = (string.IsNullOrEmpty(objChangePassword.NewPassword)) ? existingSystemUser.Password : BCrypt.Net.BCrypt.HashPassword(objChangePassword.NewPassword);
                                existingSystemUser.UpdatedDate = DateTime.Now;
                                existingSystemUser.UpdatedBy = requestMessage.UserID;
                                _crmDbContext.SystemUser.Update(existingSystemUser);
                            }
                            else
                            {
                                // Failed
                                responseMessage.ResponseObj = null;
                                responseMessage.Message = MessageConstant.UserNotFound;
                                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                                return responseMessage;
                            }
                        }
                        else
                        {
                            // Failed
                            responseMessage.ResponseObj = null;
                            responseMessage.Message = MessageConstant.UserNotFound;
                            responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                            return responseMessage;
                        }

                        await _crmDbContext.SaveChangesAsync();

                        responseMessage.ResponseObj = null;
                        responseMessage.Message = MessageConstant.PasswordChangedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, (int)Enums.ActionType.Update, requestMessage.UserID, "UpdatePassword");
                    }
                    else
                    {
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                    }
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = MessageConstant.SaveFailed;
                }
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Update, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "UpdatePassword");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update System user
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveSystemUser(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {
                SystemUser objSystemUser = JsonConvert.DeserializeObject<SystemUser>(requestMessage?.RequestObj.ToString());
                if (objSystemUser != null)
                {
                    if (CheckedValidation(objSystemUser, responseMessage))
                    {
                        if (objSystemUser.SystemUserID > 0)
                        {
                            SystemUser existingSystemUser = await this._crmDbContext.SystemUser.AsNoTracking().FirstOrDefaultAsync(x => x.SystemUserID == objSystemUser.SystemUserID && x.Status == (int)Enums.Status.Active);
                            if (existingSystemUser != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objSystemUser.Password = (string.IsNullOrEmpty(objSystemUser.Password)) ? existingSystemUser.Password : BCrypt.Net.BCrypt.HashPassword(objSystemUser.Password);
                                objSystemUser.CreatedDate = existingSystemUser.CreatedDate;
                                objSystemUser.CreatedBy = existingSystemUser.CreatedBy;
                                objSystemUser.UpdatedDate = DateTime.Now;
                                objSystemUser.UpdatedBy = requestMessage.UserID;
                                _crmDbContext.SystemUser.Update(objSystemUser);
                            }
                        }
                        else
                        {
                            objSystemUser.Status = (int)Enums.Status.Active;
                            objSystemUser.StatusOfUser = (int)Enums.Status.Active;
                            objSystemUser.CreatedDate = DateTime.Now;
                            objSystemUser.CreatedBy = requestMessage.UserID;
                            objSystemUser.Password = BCrypt.Net.BCrypt.HashPassword(objSystemUser.Password);
                            await _crmDbContext.SystemUser.AddAsync(objSystemUser);

                        }

                        await _crmDbContext.SaveChangesAsync();

                        objSystemUser.Password = string.Empty;
                        responseMessage.ResponseObj = objSystemUser;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveSystemUser");
                    }
                    else
                    {
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                    }
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = MessageConstant.SaveFailed;
                }

            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveSystemUser");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }



        /// <summary>
        /// Update Profile data of an user
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> UpdateProfile(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            //int actionType = (int)Enums.ActionType.Insert;

            try
            {
                SystemUser objSystemUser = JsonConvert.DeserializeObject<SystemUser>(requestMessage?.RequestObj.ToString());
                if (objSystemUser != null)
                {
                    objSystemUser.SystemUserID = requestMessage.UserID; // set ID

                    if (CheckedValidation(objSystemUser, responseMessage))
                    {
                        if (objSystemUser.SystemUserID > 0)
                        {
                            SystemUser existingSystemUser = await this._crmDbContext.SystemUser.AsNoTracking().FirstOrDefaultAsync(x => x.SystemUserID == objSystemUser.SystemUserID && x.Status == (int)Enums.Status.Active);
                            if (existingSystemUser != null)
                            {
                                // actionType = (int)Enums.ActionType.Update;
                                existingSystemUser.FirstName = objSystemUser.FirstName;
                                existingSystemUser.LastName = objSystemUser.LastName;
                                existingSystemUser.PhoneNumber = objSystemUser.PhoneNumber;
                                existingSystemUser.Email = objSystemUser.Email;

                                existingSystemUser.UpdatedDate = DateTime.Now;
                                existingSystemUser.UpdatedBy = requestMessage.UserID;
                                _crmDbContext.SystemUser.Update(existingSystemUser);

                                await _crmDbContext.SaveChangesAsync();

                                existingSystemUser.Password = string.Empty;
                                responseMessage.ResponseObj = existingSystemUser;
                                responseMessage.Message = MessageConstant.SavedSuccessfully;
                                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                            }
                            else
                            {
                                responseMessage.Message = MessageConstant.UserNotFound;
                                responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                            }
                        }
                        else
                        {
                            responseMessage.Message = MessageConstant.UserNotFound;
                            responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                        }
                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, (int)Enums.ActionType.Update, requestMessage.UserID, "UpdateProfile");
                    }
                    else
                    {
                        //** Check-Validation => will return message and code.
                        //responseMessage.Message = MessageConstant.SavedSuccessfully;
                        //responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                    }
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = MessageConstant.SaveFailed;
                }
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Update, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "UpdateProfile");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Approve User
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> ApproveSystemUser(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            //int actionType = (int)Enums.ActionType.Insert;
            try
            {
                SystemUser objSystemUser = JsonConvert.DeserializeObject<SystemUser>(requestMessage?.RequestObj.ToString());
                if (objSystemUser != null)
                {
                    if (objSystemUser.SystemUserID > 0)
                    {
                        SystemUser existingSystemUser = await this._crmDbContext.SystemUser.AsNoTracking().FirstOrDefaultAsync(x => x.SystemUserID == objSystemUser.SystemUserID && x.Status == (int)Enums.Status.Active);
                        if (existingSystemUser != null)
                        {
                            existingSystemUser.IsApproved = true;
                            existingSystemUser.UpdatedDate = DateTime.Now;
                            existingSystemUser.UpdatedBy = requestMessage.UserID;
                            _crmDbContext.SystemUser.Update(existingSystemUser);

                            await _crmDbContext.SaveChangesAsync();

                            existingSystemUser.Password = string.Empty;
                            responseMessage.ResponseObj = existingSystemUser;
                            responseMessage.Message = MessageConstant.ApprovedSuccessfully;
                            responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                        }
                        else
                        {
                            responseMessage.Message = MessageConstant.UserNotFound;
                            responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                        }
                    }
                    else
                    {
                        responseMessage.Message = MessageConstant.UserNotFound;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                    }
                    //Log write
                    LogHelper.WriteLog(requestMessage.RequestObj, (int)Enums.ActionType.Update, requestMessage.UserID, "ApproveSystemUser");
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = MessageConstant.ApprovedFailed;
                }
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Update, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "ApproveSystemUser");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        public async Task<ResponseMessage> RejectSystemUser(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            //int actionType = (int)Enums.ActionType.Insert;
            try
            {
                SystemUser objSystemUser = JsonConvert.DeserializeObject<SystemUser>(requestMessage?.RequestObj.ToString());
                if (objSystemUser != null)
                {
                    if (objSystemUser.SystemUserID > 0)
                    {
                        SystemUser existingSystemUser = await this._crmDbContext.SystemUser.AsNoTracking().FirstOrDefaultAsync(x => x.SystemUserID == objSystemUser.SystemUserID && x.Status == (int)Enums.Status.Active);
                        if (existingSystemUser != null)
                        {
                            existingSystemUser.IsApproved = false;
                            existingSystemUser.UpdatedDate = DateTime.Now;
                            existingSystemUser.UpdatedBy = requestMessage.UserID;
                            _crmDbContext.SystemUser.Update(existingSystemUser);

                            await _crmDbContext.SaveChangesAsync();

                            existingSystemUser.Password = string.Empty;
                            responseMessage.ResponseObj = existingSystemUser;
                            responseMessage.Message = MessageConstant.RejectSuccessfully;
                            responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                        }
                        else
                        {
                            responseMessage.Message = MessageConstant.UserNotFound;
                            responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                        }
                    }
                    else
                    {
                        responseMessage.Message = MessageConstant.UserNotFound;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                    }
                    //Log write
                    LogHelper.WriteLog(requestMessage.RequestObj, (int)Enums.ActionType.Update, requestMessage.UserID, "RejectSystemUser");
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = MessageConstant.RejectFailed;
                }
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Update, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "ApproveSystemUser");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }


        /// <summary>
        /// Remove User
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> RemoveSystemUser(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                SystemUser objSystemUser = JsonConvert.DeserializeObject<SystemUser>(requestMessage?.RequestObj.ToString());
                if (objSystemUser != null)
                {
                    if (objSystemUser.SystemUserID > 0)
                    {
                        SystemUser existingSystemUser = await this._crmDbContext.SystemUser.AsNoTracking().FirstOrDefaultAsync(x => x.SystemUserID == objSystemUser.SystemUserID && x.Status == (int)Enums.Status.Active);
                        if (existingSystemUser != null)
                        {
                            existingSystemUser.Status = (int)Enums.Status.Delete;
                            existingSystemUser.UpdatedDate = DateTime.Now;
                            existingSystemUser.UpdatedBy = requestMessage.UserID;
                            _crmDbContext.SystemUser.Update(existingSystemUser);

                            await _crmDbContext.SaveChangesAsync();

                            existingSystemUser.Password = string.Empty;
                            responseMessage.ResponseObj = existingSystemUser;
                            responseMessage.Message = MessageConstant.RemovedSuccessfully;
                            responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                        }
                        else
                        {
                            responseMessage.Message = MessageConstant.UserNotFound;
                            responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                        }
                    }
                    else
                    {
                        responseMessage.Message = MessageConstant.UserNotFound;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                    }
                    //Log write
                    LogHelper.WriteLog(requestMessage.RequestObj, (int)Enums.ActionType.Update, requestMessage.UserID, "ApproveSystemUser");
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = MessageConstant.RemoveFailed;
                }
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Update, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "ApproveSystemUser");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// method for get system user by role.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetSystemUsersByRole(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<SystemUser> lstSystemUser = new List<SystemUser>();
                int role = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstSystemUser = await _crmDbContext.SystemUser.Where(x => x.Role == role && x.Status == (int)Enums.Status.Active)
                                        .OrderBy(x => x.SystemUserID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();

                foreach (SystemUser user in lstSystemUser)
                {
                    user.Password = string.Empty;
                }
                responseMessage.ResponseObj = lstSystemUser;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetSystemUserByRole");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetSystemUserByRole");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update System user
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> DeleteSystemUser(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int actionType = (int)Enums.ActionType.Delete;

            try
            {
                SystemUser objSystemUser = JsonConvert.DeserializeObject<SystemUser>(requestMessage?.RequestObj.ToString());

                if (objSystemUser != null)
                {
                    SystemUser existingSystemUser = await _crmDbContext.SystemUser.AsNoTracking().FirstOrDefaultAsync(x => x.SystemUserID == objSystemUser.SystemUserID);

                    if (existingSystemUser?.SystemUserID > 0)
                    {
                        existingSystemUser.Status = (int)Enums.Status.Delete;
                        existingSystemUser.UpdatedBy = requestMessage.UserID;
                        existingSystemUser.UpdatedDate = DateTime.Now;
                        _crmDbContext.SystemUser.Update(existingSystemUser);
                    }

                    await _crmDbContext.SaveChangesAsync();

                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = MessageConstant.DeleteFailed;
                }
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteSystemUser");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }
            return responseMessage;
        }

        #region Methods Private
        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objSystemUser"></param>
        /// <returns></returns>
        private bool CheckedValidation(SystemUser objSystemUser, ResponseMessage responseMessage)
        {

            bool result = true;
            SystemUser existingSystemUser = new SystemUser();

            if (string.IsNullOrEmpty(objSystemUser?.FirstName?.Trim()) || string.IsNullOrEmpty(objSystemUser?.LastName?.Trim()) ||
                string.IsNullOrEmpty(objSystemUser?.PhoneNumber?.Trim()) || string.IsNullOrEmpty(objSystemUser?.Email?.Trim()))
            {
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                responseMessage.Message = MessageConstant.EnterValidEntry;
                result = false;
            }
            //existingSystemUser = _crmDbContext.SystemUser.Where(x => x.Email == objSystemUser.Email && x.PhoneNumber == objSystemUser.PhoneNumber && x.SystemUserID != objSystemUser.SystemUserID && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefault();
            existingSystemUser = _crmDbContext.SystemUser.Where(x => x.Email == objSystemUser.Email && x.SystemUserID != objSystemUser.SystemUserID && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefault();
            if (existingSystemUser != null)
            {
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                responseMessage.Message = MessageConstant.DuplicateEmail;
                result = false;
            }

            return result;
        }

        private bool CheckedValidationOfChangePassword(ChangePassword objChangePassword, ResponseMessage responseMessage)
        {
            bool result = true;

            if (string.IsNullOrEmpty(objChangePassword.NewPassword.Trim()) || objChangePassword.NewPassword.Trim().Length < 4 ||
                objChangePassword.NewPassword.Trim().Length > 40)
            {
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                responseMessage.Message = MessageConstant.EnterPasswordValidLength;
                result = false;
            }
            if (objChangePassword.NewPassword.Trim() != objChangePassword.ConfirmNewPassword.Trim())
            {
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                responseMessage.Message = MessageConstant.EnterConfirmPasswordMatched;
                result = false;
            }

            return result;
        }

        public async Task<SystemUser> GetSystemUserByVendorUserId(int vendorUserID)
        {
            try
            {
                Expression<Func<SystemUser, bool>> expression = x => x.ReferenceID == vendorUserID
                && (x.ReferenceTypeID == (int)Enums.UserReferenceType.VendorAdmin || x.ReferenceTypeID == (int)Enums.UserReferenceType.VendorEmployee);
                //&& x.Status == (int)Enums.Status.Active;
                SystemUser details = await _unitOfWork.Repository<SystemUser>().GetFirstOrDefaultAsync(predicate: expression);
                return details;
            }
            catch (Exception ex)
            {
                await _exceptionLog.SaveExceptionLog((int)Enums.LogFixPriority.Medium, 0, ex.Message, ex.StackTrace.ToString(),
                                       vendorUserID, "SystemUserService", "GetSystemUserByVendorUserId", (int)Enums.ActionType.View, "");
            }

            return null;
        }

        public async Task<SystemUser> GetSystemUserByEmployeeId(int employeeID)
        {
            try
            {
                Expression<Func<SystemUser, bool>> expression = x => x.ReferenceID == employeeID
                && (x.ReferenceTypeID == (int)Enums.UserReferenceType.AdminEmployee || x.ReferenceTypeID == (int)Enums.UserReferenceType.Employee);
                //&& x.Status == (int)Enums.Status.Active;
                SystemUser details = await _unitOfWork.Repository<SystemUser>().GetFirstOrDefaultAsync(predicate: expression);
                return details;
            }
            catch (Exception ex)
            {
                await _exceptionLog.SaveExceptionLog((int)Enums.LogFixPriority.Medium, 0, ex.Message, ex.StackTrace.ToString(),
                                       employeeID, "SystemUserService", "GetSystemUserByEmployeeId", (int)Enums.ActionType.View, "");
            }

            return null;
        }

        public async Task<bool> CreateUpdateSystemUserForEmployee(EmployeeInformation empObj, int userID, bool isUpdate)
        {
            try
            {
                if (isUpdate)
                {
                    //1. Update
                    SystemUser objSystemUser = await GetSystemUserByEmployeeId(empObj.EmployeeInformationID);
                    if (objSystemUser != null)
                    {
                        objSystemUser.PhoneNumber = empObj.Phone;
                        objSystemUser.FirstName = empObj.FirstName;
                        objSystemUser.LastName = empObj.LastName;
                        //objSystemUser.Email = empObj.Email;   // NOTE: Short Name - as UserID
                        objSystemUser.Email = empObj.Alias;

                        objSystemUser.Password = (string.IsNullOrEmpty(empObj.Password)) ? objSystemUser.Password : BCrypt.Net.BCrypt.HashPassword(empObj.Password);
                        objSystemUser.CreatedDate = objSystemUser.CreatedDate;
                        objSystemUser.CreatedBy = objSystemUser.CreatedBy;
                        objSystemUser.UpdatedDate = DateTime.Now;
                        objSystemUser.UpdatedBy = userID;

                        objSystemUser.IsApproved = true; 
                        objSystemUser.StatusOfUser = 1; // Hardcode : jafar
                        objSystemUser.Role = empObj.RoleId;

                        _unitOfWork.Repository<SystemUser>().Update(objSystemUser);
                    }
                }
                else
                {
                    // 2. insert
                    SystemUser objSystemUser = new SystemUser();
                    objSystemUser.PhoneNumber = empObj.Phone;
                    objSystemUser.FirstName = empObj.FirstName;
                    objSystemUser.LastName = empObj.LastName;
                    //objSystemUser.Email = empObj.Email;  // NOTE: Short Name - as UserID
                    objSystemUser.Email = empObj.Alias;
                    objSystemUser.Password = BCrypt.Net.BCrypt.HashPassword(empObj.Password);
                    objSystemUser.CreatedDate = DateTime.Now;
                    objSystemUser.CreatedBy = userID;
                    objSystemUser.Role = empObj.RoleId;  
                    objSystemUser.ReferenceTypeID = (int)Enums.UserReferenceType.Employee;
                    objSystemUser.ReferenceID = empObj.EmployeeInformationID;

                    await _unitOfWork.Repository<SystemUser>().InsertAsync(objSystemUser);
                }

                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw;

            }

            return false;
        }


        public async Task<SystemUser> GetSystemUserByEmail(string  email) // NOTE: for Vendor this field value is PHONE no
        {
            try
            {
                Expression<Func<SystemUser, bool>> expression = x => x.Email == email
                   && x.Status != (int)Enums.Status.Delete;
                SystemUser details = await _unitOfWork.Repository<SystemUser>().GetFirstOrDefaultAsync(predicate: expression);
                return details;
            }
            catch (Exception ex)
            {
                await _exceptionLog.SaveExceptionLog((int)Enums.LogFixPriority.Medium, 0, ex.Message, ex.StackTrace.ToString(),
                                       email, "SystemUserService", "GetSystemUserByEmail", (int)Enums.ActionType.View, "");
            }

            return null;
        }

        public async Task<SystemUser> GetSystemUserByVendorUserID(int vendorUserID) // NOTE: for Vendor this field value is PHONE no
        {
            try
            {
                Expression<Func<SystemUser, bool>> expression = x => x.ReferenceID == vendorUserID 
                && (x.ReferenceTypeID == (int)Enums.UserReferenceType.VendorAdmin 
                || x.ReferenceTypeID == (int)Enums.UserReferenceType.VendorEmployee)
                  && x.Status != (int)Enums.Status.Delete;
                SystemUser details = await _unitOfWork.Repository<SystemUser>().GetFirstOrDefaultAsync(predicate: expression);
                return details;
            }
            catch (Exception ex)
            {
                await _exceptionLog.SaveExceptionLog((int)Enums.LogFixPriority.Medium, 0, ex.Message, ex.StackTrace.ToString(),
                                       vendorUserID, "systemUserService", "GetSystemUserByVendorUserID", (int)Enums.ActionType.View, "");
            }

            return null;
        }


        #endregion
    }
}
