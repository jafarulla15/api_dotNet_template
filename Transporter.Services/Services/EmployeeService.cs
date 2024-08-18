using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Transporter.Common.Constants;
using Transporter.Common.DTO;
using Transporter.Common.Enums;
using Transporter.Common.Models;
using Transporter.DataAccess;
using Transporter.Repository;
using Transporter.Services.Interface;
using Transporter.Services.Services.Log;
using Transporter.Services.Utils;
using static Transporter.Common.Enums.Enums;

namespace Transporter.Services
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly IUnitOfWork<SbDbContext> _unitOfWork;
        private readonly IExceptionLogService _exceptionLog;
        private readonly ISystemUserService _systemUserService;
        private readonly SbDbContext _sbDbContext;
        private readonly IRoleService _roleService;
        public EmployeeServices(IUnitOfWork<SbDbContext> unitOfWork, IExceptionLogService exceptionLog
                                , IRoleService roleService, ISystemUserService systemUserService, SbDbContext sbDbContext)
        {
            _unitOfWork = unitOfWork;
            _exceptionLog = exceptionLog;
            _systemUserService = systemUserService;
            _sbDbContext = sbDbContext;
            _roleService = roleService;
        }

        public async Task<ResponseMessage> GetAll(RequestMessage requestMessage)
        {
            try
            {
                Expression<Func<EmployeeInformation, bool>> expression = x => x.Status == (int)Enums.Status.Active;
                List<EmployeeInformation> lstEmployee = (await _unitOfWork.Repository<EmployeeInformation>().GetAsync(predicate: expression)).ToList();
                return new ResponseMessage
                {
                    ResponseCode = (int)Enums.ResponseCode.Success,
                    Message = "Success",
                    ResponseObj = lstEmployee,
                    TotalCount = lstEmployee.Count
                };
            }
            catch (Exception ex)
            {
                await _exceptionLog.SaveExceptionLog((int)Enums.LogFixPriority.Medium, 0, ex.Message, ex.StackTrace.ToString(),
                                       "", "EmployeeController", "GetById", (int)Enums.ActionType.View, "");
                return new ResponseMessage
                {
                    ResponseCode = (int)Enums.ResponseCode.InternalServerError,
                    Message = ex.Message
                };
            }
        }

        public async Task<ResponseMessage> Search(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<EmployeeInformation> lstEmployee = new();
                string searchtext = string.Empty;
                searchtext = requestMessage?.RequestObj?.ToString();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                IQueryable<EmployeeInformation> lst = (!string.IsNullOrEmpty(searchtext)) ? _sbDbContext.EmployeeInformation.Where
                    (x => x.Status == (int)Enums.Status.Active && (x.FirstName.ToLower().Contains(searchtext.ToLower())
                    || x.LastName.ToLower().Contains(searchtext.ToLower()) || x.MiddleName.ToLower().Contains(searchtext.ToLower())
                    || x.Alias.ToLower().Contains(searchtext.ToLower()) || x.EmployeeCode.ToLower().Contains(searchtext.ToLower())
                    || x.Phone.ToLower().Contains(searchtext.ToLower()) || x.Address.ToLower().Contains(searchtext.ToLower())
                    || x.Email.ToLower().Contains(searchtext.ToLower()) || x.PhoneAlter.ToLower().Contains(searchtext.ToLower())))
                    : _sbDbContext.EmployeeInformation.Where(x => x.Status != (int)Enums.Status.Delete);

                lstEmployee = await lst.OrderBy(x => x.FirstName).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lst.Count();

                //Get all role
                string sqlSystemUser = "select * from SystemUser as su left " +
                                       " join Role as r on r.RoleID = su.Role where " +
                                       " ReferenceTypeID = " + (int) Enums.UserReferenceType.Employee
                                       + " or ReferenceTypeID = " + (int)Enums.UserReferenceType.AdminEmployee
                                       + " or ReferenceTypeID = " + (int)Enums.UserReferenceType.Admin;


                List<SystemUser> lstSystemUser = _unitOfWork.RawSqlQuery<SystemUser>(sqlSystemUser);
                List<SystemUser> lstSystemUserFiltered = new List<SystemUser>();

                //List<Roles> lstRole = await _roleService.GetAllRoles();
                //List<Roles> lstRoleFiltered = new List<Roles>();

                foreach (EmployeeInformation emp in lstEmployee)
                {
                    //1. set role name
                    lstSystemUserFiltered = lstSystemUser.FindAll(o => o.ReferenceID == emp.EmployeeInformationID);
                    if (lstSystemUserFiltered != null && lstSystemUserFiltered.Count > 0)
                    {
                        emp.RoleName = lstSystemUserFiltered[0].RoleName;
                        emp.RoleId = lstSystemUserFiltered[0].Role;
                    }

                    //2. set Status
                    emp.StatusName = emp.Status == 1 ? "Active" : "Inactive";
                }

                responseMessage.ResponseObj = lstEmployee;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                return responseMessage;
            }
            catch (Exception ex)
            {
                await _exceptionLog.SaveExceptionLog((int)Enums.LogFixPriority.Medium, 0, ex.Message, ex.StackTrace.ToString(),
                                       "", "EmployeeController", "Search", (int)Enums.ActionType.View, "");
                return new ResponseMessage
                {
                    ResponseCode = (int)Enums.ResponseCode.InternalServerError,
                    Message = ex.Message
                };
            }
        }
        //public async Task<ResponseMessage> Search2(RequestMessage requestMessage)
        //{
        //    try
        //    {
        //        List<Vendor> lstVendor = new();
        //        string searchtext = string.Empty;
        //        searchtext = requestMessage?.RequestObj?.ToString();
        //        int totalSkip = 0;
        //        totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

        //        IQueryable<Vendor> lst = (!string.IsNullOrEmpty(searchtext)) ? _sbDbContext.Vendor.Where(x => x.Status == (int)Enums.Status.Active && (x.FirstName.ToLower().Contains(searchtext.ToLower()) || x.LastName.ToLower().Contains(searchtext.ToLower()) || x.MiddleName.ToLower().Contains(searchtext.ToLower()) ||
        //         x.NID.ToLower().Contains(searchtext.ToLower()) || x.TIN.ToLower().Contains(searchtext.ToLower()) || x.Phone.ToLower().Contains(searchtext.ToLower()) || x.Address.ToLower().Contains(searchtext.ToLower()) || x.PhoneAlter.ToLower().Contains(searchtext.ToLower()))) : _sbDbContext.Vendor.Where(x => x.Status == (int)Enums.Status.Active);

        //        lstVendor = await lst.OrderByDescending(x => x.VendorID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
        //        responseMessage.TotalCount = lst.Count();

        //        responseMessage.ResponseObj = lstVendor;
        //        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
        //        return responseMessage;

        //        CommonSearch search = JsonConvert.DeserializeObject<CommonSearch>(requestMessage?.RequestObj.ToString());
        //        string sql = "select * from EmployeeInformation where Status = " + (int)Enums.Status.Active;
        //        if (search != null && !string.IsNullOrEmpty(search.SearchText.Trim()))
        //        {
        //            string searchText = search.SearchText.Trim();
        //            sql += "   and ( Alias like '\\" +  searchText +"\\' or FirstName like '\\" +  searchText +"\\' or MiddleName like '\\" +  searchText +"\\' or LastName like '\\" +  searchText +"\\' or Phone like '\\" +  searchText +"\\' or Email like '\\" +  searchText +"\\' ) ";
        //        }
        //        if (search != null && search.CompanyID > 0)
        //        {
        //            sql += "   and ( EmployeeInformationID IN (select EmployeeID from CompanyEmployeeMapping where CompanyID = '" + search.CompanyID + "') ) ";
        //        }
        //        sql = sql.Replace('\\','%');

        //        List<EmployeeInformation> lstEmployee = _unitOfWork.RawSqlQuery<EmployeeInformation>(sql);

        //        if (lstEmployee != null)
        //        {
        //            return new ResponseMessage
        //            {
        //                ResponseCode = (int)Enums.ResponseCode.Success,
        //                Message = "Success",
        //                ResponseObj = lstEmployee
        //            };
        //        }
        //        else
        //        {
        //            return new ResponseMessage
        //            {
        //                ResponseCode = (int)Enums.ResponseCode.Success,
        //                Message = "Success",
        //                ResponseObj = null
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await _exceptionLog.SaveExceptionLog((int)Enums.LogFixPriority.Medium, 0, ex.Message, ex.StackTrace.ToString(),
        //                               requestMessage?.RequestObj.ToString(), "EmployeeController", "GetById", (int)Enums.ActionType.View, "");
        //        return new ResponseMessage
        //        {
        //            ResponseCode = (int)Enums.ResponseCode.InternalServerError,
        //            Message = ex.Message
        //        };
        //    }
        //}

        public async Task<ResponseMessage> GetById(RequestMessage requestMessage)
        {
            try
            {
                //1. Get employee Detail

                int id = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());
                Expression<Func<EmployeeInformation, bool>> expression = x => x.EmployeeInformationID == id && x.Status != (int)Enums.Status.Delete;
                EmployeeInformation details = await _unitOfWork.Repository<EmployeeInformation>().GetFirstOrDefaultAsync(predicate: expression);

                //2. Get company Mapping
                string sql = "select CompanyId, CompanyName from Company where CompanyID in (select CompanyId from [dbo].[CompanyEmployeeMapping] where EmployeeID = '" + id + "') ";
                List<Company> lstCompany = _unitOfWork.RawSqlQuery<Company>(sql);

                //3. Get Role Mapping
                string sqlSystemUser = "( select * from SystemUser where ReferenceTypeID = " + (int)Enums.UserReferenceType.Employee + " and ReferenceID = '" + id + "' and Status = 1) ";
                List<SystemUser> lstSystemUser = _unitOfWork.RawSqlQuery<SystemUser>(sqlSystemUser);

                if (details != null)
                {
                    details.ListCompany = lstCompany;
                    if (lstSystemUser != null && lstSystemUser.Count > 0)
                    {
                        details.RoleId = lstSystemUser[0].Role;
                    }
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Success,
                        Message = "Success",
                        ResponseObj = details
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Success,
                        Message = "Success",
                        ResponseObj = null
                    };
                }
            }
            catch (Exception ex)
            {
                await _exceptionLog.SaveExceptionLog((int)Enums.LogFixPriority.Medium, 0, ex.Message, ex.StackTrace.ToString(),
                                       requestMessage?.RequestObj.ToString(), "EmployeeController", "GetById", (int)Enums.ActionType.View, "");
                return new ResponseMessage
                {
                    ResponseCode = (int)Enums.ResponseCode.InternalServerError,
                    Message = ex.Message
                };
            }
        }
        public async Task<ResponseMessage> Save(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int actionType = (int)Enums.ActionType.Insert;
            bool isUpdate = false;
            try
            {
                EmployeeInformation obj = JsonConvert.DeserializeObject<EmployeeInformation>(requestMessage?.RequestObj.ToString());

                if (obj != null)
                {
                    if (CheckedValidation(obj, responseMessage))
                    {
                        if (obj.EmployeeInformationID > 0)
                        {
                            //1.Update

                            actionType = (int)Enums.ActionType.Update;
                            isUpdate = true;
                            obj.UpdatedDate = DateTime.Now;
                            obj.UpdatedBy = requestMessage.UserID;

                            _unitOfWork.Repository<EmployeeInformation>().Update(obj);

                            _unitOfWork.DbContext.Entry(obj).Property(x => x.CreatedBy).IsModified = false;
                            _unitOfWork.DbContext.Entry(obj).Property(x => x.CreatedDate).IsModified = false;
                            _unitOfWork.DbContext.Entry(obj).Property(x => x.Status).IsModified = false;

                            _unitOfWork.Repository<EmployeeInformation>().Update(obj);
                        }
                        else
                        {
                            //2. Insert

                            obj.CreatedBy = requestMessage.UserID;
                            obj.CreatedDate = DateTime.Now;
                            obj.UpdatedBy = requestMessage.UserID;
                            obj.UpdatedDate = DateTime.Now;
                            obj.Status = (int)Enums.Status.Active;

                            await _unitOfWork.Repository<EmployeeInformation>().InsertAsync(obj);
                        }

                        if (await _unitOfWork.SaveChangesAsync() > 0)
                        {
                            // 2. Employee -> System User
                            bool systemUserCreated = await _systemUserService.CreateUpdateSystemUserForEmployee(obj, requestMessage.UserID, isUpdate);

                            // 3. Employee vs Company
                            if (systemUserCreated)
                            {
                                bool result = await mapEmployeeVsCompany(obj.EmployeeInformationID, obj.ListCompany);
                                if (result)
                                {
                                    return new ResponseMessage
                                    {
                                        ResponseCode = (int)Enums.ResponseCode.Success,
                                        Message = $"Employee Information saved successfully.",
                                        ResponseObj = obj
                                    };
                                }
                            }
                        }

                        return new ResponseMessage
                        {
                            ResponseCode = (int)Enums.ResponseCode.Failed,
                            Message = $"Sorry, Employee Information save failed."
                        };

                        //Log write
                        // LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveRole");

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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveRole");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.InternalServerError;
            }

            return responseMessage;
        }

        private async Task<bool> mapEmployeeVsCompany(int employeeID, List<Company> listCompany)
        {
            try
            {
                // 1. Remove all previous
                string sql = "delete from CompanyEmployeeMapping where EmployeeID = '" + employeeID + "'";
                _unitOfWork.RawSqlQuery<CompanyEmployeeMapping>(sql);

                // 2. insert all new
                List<CompanyEmployeeMapping> list = new List<CompanyEmployeeMapping>();
                foreach (Company company in listCompany)
                {
                    CompanyEmployeeMapping employeeMapping = new CompanyEmployeeMapping();
                    employeeMapping.CompanyID = company.CompanyId;
                    employeeMapping.EmployeeID = employeeID;

                    list.Add(employeeMapping);
                }

                await _unitOfWork.Repository<CompanyEmployeeMapping>().InsertRangeAsync(list);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ResponseMessage> EmployeeStatusUpdate(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            string message = string.Empty;
            string messageError = string.Empty;

            try
            {
                int employeeID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());
                Expression<Func<EmployeeInformation, bool>> expression = x => x.EmployeeInformationID == employeeID;

                EmployeeInformation obj = await _unitOfWork.Repository<EmployeeInformation>().GetFirstOrDefaultAsync(predicate: expression);

                if (obj == null)
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Warning,
                        Message = $"Sorry, EmployeeInformation doesn't exist.",
                        ResponseObj = null
                    };
                }

                obj.UpdatedDate = DateTime.Now;
                obj.UpdatedBy = requestMessage.UserID;
                obj.Status = obj.Status == 1 ? (int)Enums.Status.Inactive : (int)Enums.Status.Active;
                message = obj.Status == 1 ? "Activated" : "Inactivated";
                messageError = obj.Status == 1 ? "Activate" : "Inactivate";
                //obj.Status = (int)Enums.Status.Active;

                _unitOfWork.Repository<EmployeeInformation>().Update(obj);

                _unitOfWork.DbContext.Entry(obj).Property(x => x.CreatedBy).IsModified = false;
                _unitOfWork.DbContext.Entry(obj).Property(x => x.CreatedDate).IsModified = false;

                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Success,
                        Message = $"Employee " + message + " successfully",
                        ResponseObj = obj.EmployeeInformationID
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Failed,
                        Message = $"Sorry. Employee " + messageError + " failed."
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseMessage
                {
                    ResponseCode = (int)Enums.ResponseCode.InternalServerError,
                    Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Update, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SoftDelete")
                };
            }
        }

        public async Task<ResponseMessage> DeleteSoft(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            try
            {
                int employeeID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());
                Expression<Func<EmployeeInformation, bool>> expression = x => x.EmployeeInformationID == employeeID && x.Status != (int)Enums.Status.Delete;
                EmployeeInformation obj = await _unitOfWork.Repository<EmployeeInformation>().GetFirstOrDefaultAsync(predicate: expression);

                if (obj == null)
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Warning,
                        Message = $"Sorry, EmployeeInformation doesn't exist.",
                        ResponseObj = null
                    };
                }

                obj.UpdatedDate = DateTime.Now;
                obj.UpdatedBy = requestMessage.UserID;
                obj.Status = (int)Enums.Status.Delete;

                _unitOfWork.Repository<EmployeeInformation>().Update(obj);

                _unitOfWork.DbContext.Entry(obj).Property(x => x.CreatedBy).IsModified = false;
                _unitOfWork.DbContext.Entry(obj).Property(x => x.CreatedDate).IsModified = false;

                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    //2. Delete System User
                    if (true)
                    {
                        Expression<Func<SystemUser, bool>> expression2 = x => x.ReferenceTypeID == (int)Enums.UserReferenceType.Employee &&
                                                        x.ReferenceID == employeeID && x.Status != (int)Enums.Status.Delete;
                        SystemUser existingSystemUser = await _unitOfWork.Repository<SystemUser>().GetFirstOrDefaultAsync(predicate: expression2);
                        if (existingSystemUser?.SystemUserID > 0)
                        {
                            existingSystemUser.Status = (int)Enums.Status.Delete;
                            existingSystemUser.UpdatedBy = requestMessage.UserID;
                            existingSystemUser.UpdatedDate = DateTime.Now;

                            _unitOfWork.Repository<SystemUser>().Update(existingSystemUser);
                            _unitOfWork.DbContext.Entry(existingSystemUser).Property(x => x.CreatedBy).IsModified = false;
                            _unitOfWork.DbContext.Entry(existingSystemUser).Property(x => x.CreatedDate).IsModified = false;
                        }

                        if (await _unitOfWork.SaveChangesAsync() > 0)
                        {
                            return new ResponseMessage
                            {
                                ResponseCode = (int)Enums.ResponseCode.Success,
                                Message = $"EmployeeInformation deleted successfully",
                                ResponseObj = obj.EmployeeInformationID
                            };
                        }
                        else
                        {
                            return new ResponseMessage
                            {
                                ResponseCode = (int)Enums.ResponseCode.Failed,
                                Message = $"Sorry. EmployeeInformation remove failed."
                            };
                        }
                    }
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Success,
                        Message = $"EmployeeInformation deleted successfully",
                        ResponseObj = obj.EmployeeInformationID
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Failed,
                        Message = $"Sorry. EmployeeInformation remove failed."
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseMessage
                {
                    ResponseCode = (int)Enums.ResponseCode.InternalServerError,
                    Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SoftDelete")
                };
            }
        }
        public async Task<ResponseMessage> Delete(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            try
            {
                int TransportRequisitionID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());
                Expression<Func<EmployeeInformation, bool>> expression = x => x.EmployeeInformationID == TransportRequisitionID && x.Status == (int)Enums.Status.Active;

                EmployeeInformation obj = await _unitOfWork.Repository<EmployeeInformation>().GetFirstOrDefaultAsync(predicate: expression);

                if (obj == null)
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Warning,
                        Message = $"Sorry, EmployeeInformation doesn't exist.",
                        ResponseObj = null
                    };
                }

                _unitOfWork.Repository<EmployeeInformation>().Delete(obj);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Success,
                        Message = $"EmployeeInformation deleted successfully",
                        ResponseObj = obj.EmployeeInformationID
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Failed,
                        Message = $"Sorry. EmployeeInformation remove failed."
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseMessage
                {
                    ResponseCode = (int)Enums.ResponseCode.InternalServerError,
                    Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SoftDelete")
                };
            }
        }

        #region private
        private bool CheckedValidation(EmployeeInformation obj, ResponseMessage responseMessage)
        {
            if (string.IsNullOrEmpty(obj.FirstName))
            {
                responseMessage.Message = ValidationMessage.FirstName;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.ValidationError;
                return false;
            }

            if (string.IsNullOrEmpty(obj.Alias))
            {
                responseMessage.Message = ValidationMessage.Alias;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.ValidationError;
                return false;
            }

            if (string.IsNullOrEmpty(obj.Phone))
            {
                responseMessage.Message = ValidationMessage.Phone;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.ValidationError;
                return false;
            }

            if (string.IsNullOrEmpty(obj.Email))
            {
                responseMessage.Message = ValidationMessage.Email;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.ValidationError;
                return false;
            }

            if (obj.RoleId <= 0)
            {
                responseMessage.Message = ValidationMessage.Role;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.ValidationError;
                return false;
            }

            if (obj.ListCompany == null || obj.ListCompany.Count <= 0)
            {
                responseMessage.Message = ValidationMessage.Company;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.ValidationError;
                return false;
            }

            // *1. Check is exist "First Name"
            EmployeeInformation existingObj = _sbDbContext.EmployeeInformation .Where(x => x.FirstName == obj.FirstName
                                && x.Status != (int)Enums.Status.Delete).AsNoTracking().FirstOrDefault();
            if (existingObj != null)
            {
                if (existingObj.EmployeeInformationID != obj.EmployeeInformationID)
                {
                    responseMessage.Message = ValidationMessage.FirstNameExist;
                    //responseMessage.ResponseCode = (int)Enums.ResponseCode.ValidationError;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                    return false;
                }
            }

            // *2. Check is exist "Alias"
            EmployeeInformation existingAliasObj = _sbDbContext.EmployeeInformation.Where(x => x.Alias == obj.Alias
                                && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefault();
            if (existingAliasObj != null)
            {
                if (existingAliasObj.EmployeeInformationID != obj.EmployeeInformationID)
                {
                    responseMessage.Message = ValidationMessage.AliasExist;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.ValidationError;
                    return false;
                }
            }

            // *3. Check is exist "PHone"
            EmployeeInformation existingPHoneObj = _sbDbContext.EmployeeInformation.Where(x => x.Phone == obj.Phone
                                && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefault();
            if (existingPHoneObj != null)
            {
                if (existingPHoneObj.EmployeeInformationID != obj.EmployeeInformationID)
                {
                    responseMessage.Message = ValidationMessage.PhoneExist;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.ValidationError;
                    return false;
                }
            }

            // *4. Check is exist "Email"
            EmployeeInformation existingEmailObj = _sbDbContext.EmployeeInformation.Where(x => x.Email == obj.Email
                                && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefault();
            if (existingEmailObj != null)
            {
                if (existingEmailObj.EmployeeInformationID != obj.EmployeeInformationID)
                {
                    responseMessage.Message = ValidationMessage.EmailExist;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.ValidationError;
                    return false;
                }
            }

            return true;
        }

        #endregion

        private bool _disposed = false;

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _unitOfWork.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public interface IEmployeeServices : IDisposable
    {
        Task<ResponseMessage> Search(RequestMessage requestMessage);
        Task<ResponseMessage> GetAll(RequestMessage requestMessage);
        Task<ResponseMessage> GetById(RequestMessage requestMessage);
        Task<ResponseMessage> Save(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteSoft(RequestMessage requestMessage);
        Task<ResponseMessage> Delete(RequestMessage requestMessage);
        Task<ResponseMessage> EmployeeStatusUpdate(RequestMessage requestMessage);
    }
}
