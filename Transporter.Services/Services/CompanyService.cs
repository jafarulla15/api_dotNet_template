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
using Transporter.Services.Services.Log;
using Transporter.Services.Utils;
using static Transporter.Common.Enums.Enums;

namespace Transporter.Services
{
    public class CompanyServices : ICompanyServices
    {
        private readonly IUnitOfWork<SbDbContext> _unitOfWork;
        private readonly IExceptionLogService _exceptionLog;
        private readonly SbDbContext _sbDbContext;
        public CompanyServices(IUnitOfWork<SbDbContext> unitOfWork, IExceptionLogService exceptionLog, SbDbContext sbDbContext)
        {
            _unitOfWork = unitOfWork;
            _exceptionLog = exceptionLog;
            _sbDbContext = sbDbContext;
        }

        public async Task<ResponseMessage> GetAllData(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            try
            {
                Expression<Func<Company, bool>> expression = x => x.Status == (int)Enums.Status.Active;
                var lstCompany = (await _unitOfWork.Repository<Company>().GetAsync(predicate: expression));
                List<Company> lstComp = (List<Company>)lstCompany;
                lstComp = lstComp.OrderByDescending(x => x.CompanyName).ToList();

                return new ResponseMessage
                {
                    ResponseCode = (int)Enums.ResponseCode.Success,
                    Message = "Success",
                    ResponseObj = lstCompany
                };
            }
            catch (Exception ex)
            {
                await _exceptionLog.SaveExceptionLog((int)Enums.LogFixPriority.Medium, 0, ex.Message, ex.StackTrace.ToString(),
                                       "", "CompanyController", "GetById", (int)Enums.ActionType.View, "");
                return new ResponseMessage
                {
                    ResponseCode = (int)Enums.ResponseCode.InternalServerError,
                    Message = ex.Message
                };
            }
        }

        public async Task<ResponseMessage> GetAll(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            try
            {
                string searchtext = string.Empty;
                searchtext = requestMessage?.RequestObj?.ToString();

                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                IQueryable<Company> lst = (!string.IsNullOrEmpty(searchtext)) 
                ? _sbDbContext.Company.Where(x => x.Status == (int)Enums.Status.Active 
                && (x.CompanyName.ToLower().Contains(searchtext.ToLower())
                || x.CompanyCode.ToLower().Contains(searchtext.ToLower()) 
                || x.Email.ToLower().Contains(searchtext.ToLower()) 
                || x.ContactName.ToLower().Contains(searchtext.ToLower()) 
                || x.Alias.ToLower().Contains(searchtext.ToLower()) 
                || x.ContactName.ToLower().Contains(searchtext.ToLower()) 
                || x.Description.ToLower().Contains(searchtext.ToLower()))) 
                : _sbDbContext.Company.Where(x => x.Status == (int)Enums.Status.Active);

                var lstCompany = await lst.OrderBy(x => x.CompanyName).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lst.Count();

                responseMessage.ResponseObj = lstCompany;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                return responseMessage;

                //    Expression<Func<Company, bool>> expression = x => x.Status == (int)Enums.Status.Active;
                //    var lstCompany = (await _unitOfWork.Repository<Company>().GetAsync(predicate: expression));
                //    return new ResponseMessage
                //    {
                //        ResponseCode = (int)Enums.ResponseCode.Success,
                //        Message = "Success",
                //        ResponseObj = lstCompany
                //    };
            }
            catch (Exception ex)
            {
                await _exceptionLog.SaveExceptionLog((int)Enums.LogFixPriority.Medium, 0, ex.Message, ex.StackTrace.ToString(),
                                       "", "CompanyController", "GetById", (int)Enums.ActionType.View, "");
                return new ResponseMessage
                {
                    ResponseCode = (int)Enums.ResponseCode.InternalServerError,
                    Message = ex.Message
                };
            }
        }

        public async Task<ResponseMessage> GetAllMappingWithEmployee(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            try
            {
                string searchtext = string.Empty;
                searchtext = requestMessage?.RequestObj?.ToString();

                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                IQueryable<Company> lst = (!string.IsNullOrEmpty(searchtext))
                ? _sbDbContext.Company.Where(x => x.Status == (int)Enums.Status.Active
                && (x.CompanyName.ToLower().Contains(searchtext.ToLower())
                || x.CompanyCode.ToLower().Contains(searchtext.ToLower())
                || x.Email.ToLower().Contains(searchtext.ToLower())
                || x.ContactName.ToLower().Contains(searchtext.ToLower())
                || x.Alias.ToLower().Contains(searchtext.ToLower())
                || x.ContactName.ToLower().Contains(searchtext.ToLower())
                || x.Description.ToLower().Contains(searchtext.ToLower())))
                : _sbDbContext.Company.Where(x => x.Status == (int)Enums.Status.Active);

                var lstCompany = await lst.OrderBy(x => x.CompanyName).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lst.Count();

                responseMessage.ResponseObj = lstCompany;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                return responseMessage;

                //    Expression<Func<Company, bool>> expression = x => x.Status == (int)Enums.Status.Active;
                //    var lstCompany = (await _unitOfWork.Repository<Company>().GetAsync(predicate: expression));
                //    return new ResponseMessage
                //    {
                //        ResponseCode = (int)Enums.ResponseCode.Success,
                //        Message = "Success",
                //        ResponseObj = lstCompany
                //    };
            }
            catch (Exception ex)
            {
                await _exceptionLog.SaveExceptionLog((int)Enums.LogFixPriority.Medium, 0, ex.Message, ex.StackTrace.ToString(),
                                       "", "CompanyController", "GetById", (int)Enums.ActionType.View, "");
                return new ResponseMessage
                {
                    ResponseCode = (int)Enums.ResponseCode.InternalServerError,
                    Message = ex.Message
                };
            }
        }

        public async Task<ResponseMessage> Search(RequestMessage requestMessage)
        {
            try
            {
                CommonSearch search = JsonConvert.DeserializeObject<CommonSearch>(requestMessage?.RequestObj.ToString());
                string sql = "select * from Company where Status = " + (int)Enums.Status.Active;
                if (!string.IsNullOrEmpty(search.SearchText.Trim()))
                {
                    string searchText = search.SearchText.Trim();
                    sql += "   and (CompanyName like '\\" + searchText + "\\' or Alias like '\\" + searchText + "\\' or ContactName like '\\" + searchText + "\\' or ContactNo like '\\" + searchText + "\\' or Email like '\\" + searchText + "\\' or CompanyCode like '\\" + searchText + "\\' ) ";
                }

                sql = sql.Replace('\\', '%');

                List<Company> lstCompany = _unitOfWork.RawSqlQuery<Company>(sql);

                if (lstCompany != null)
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Success,
                        Message = "Success",
                        ResponseObj = lstCompany
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
                                       requestMessage?.RequestObj.ToString(), "CompanyController", "GetById", (int)Enums.ActionType.View, "");
                return new ResponseMessage
                {
                    ResponseCode = (int)Enums.ResponseCode.InternalServerError,
                    Message = ex.Message
                };
            }
        }

        public async Task<Company> GetCompanyById(int companyID)
        {
            try
            {
                Expression<Func<Company, bool>> expression = x => x.CompanyId == companyID && x.Status == (int)Enums.Status.Active;

                return await _unitOfWork.Repository<Company>().GetFirstOrDefaultAsync(predicate: expression);
            }
            catch (Exception ex)
            {
                await _exceptionLog.SaveExceptionLog((int)Enums.LogFixPriority.Medium, 0, ex.Message, ex.StackTrace.ToString(),
                                       companyID.ToString(), "CompanyController", "GetCompanyById", (int)Enums.ActionType.View, "");
            }

            return null;
        }

        public async Task<ResponseMessage> GetById(RequestMessage requestMessage)
        {
            try
            {
                int companyID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());
                Expression<Func<Company, bool>> expression = x => x.CompanyId == companyID && x.Status == (int)Enums.Status.Active;

                var details = await _unitOfWork.Repository<Company>().GetFirstOrDefaultAsync(predicate: expression);
                if (details != null)
                {
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
                                       requestMessage?.RequestObj.ToString(), "CompanyController", "GetById", (int)Enums.ActionType.View, "");
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
            try
            {
                Company obj = JsonConvert.DeserializeObject<Company>(requestMessage?.RequestObj.ToString());

                if (obj != null)
                {
                    if (CheckedValidation(obj, responseMessage))
                    {
                        if (obj.CompanyId > 0)
                        {
                            actionType = (int)Enums.ActionType.Update;
                            obj.UpdatedDate = DateTime.Now;
                            obj.UpdatedBy = requestMessage.UserID;

                            _unitOfWork.Repository<Company>().Update(obj);

                            _unitOfWork.DbContext.Entry(obj).Property(x => x.CreatedBy).IsModified = false;
                            _unitOfWork.DbContext.Entry(obj).Property(x => x.CreatedDate).IsModified = false;
                            _unitOfWork.DbContext.Entry(obj).Property(x => x.Status).IsModified = false;
                        }
                        else
                        {
                            obj.CreatedBy = requestMessage.UserID;
                            obj.CreatedDate = DateTime.Now;
                            obj.UpdatedBy = requestMessage.UserID;
                            obj.UpdatedDate = DateTime.Now;
                            obj.Status = (int)Enums.Status.Active;

                            await _unitOfWork.Repository<Company>().InsertAsync(obj);
                        }

                        if (await _unitOfWork.SaveChangesAsync() > 0)
                        {
                            return new ResponseMessage
                            {
                                ResponseCode = (int)Enums.ResponseCode.Success,
                                Message = $"Company saved successfully.",
                                ResponseObj = obj
                            };
                        }
                        else
                        {
                            return new ResponseMessage
                            {
                                ResponseCode = (int)Enums.ResponseCode.Failed,
                                Message = $"Sorry, Company save failed."
                            };
                        }

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
        public async Task<ResponseMessage> DeleteSoft(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                int TransportRequisitionID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                    // get company
                    Expression<Func<Company, bool>> expression = x => x.CompanyId == TransportRequisitionID && x.Status == (int)Enums.Status.Active;
                Company obj = await _unitOfWork.Repository<Company>().GetFirstOrDefaultAsync(predicate: expression);

                if (obj == null)
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Warning,
                        Message = $"Sorry, Company doesn't exist.",
                        ResponseObj = null
                    };
                }

                obj.UpdatedDate = DateTime.Now;
                obj.UpdatedBy = requestMessage.UserID;
                obj.Status = (int)Enums.Status.Delete;

                _unitOfWork.Repository<Company>().Update(obj);

                _unitOfWork.DbContext.Entry(obj).Property(x => x.CreatedBy).IsModified = false;
                _unitOfWork.DbContext.Entry(obj).Property(x => x.CreatedDate).IsModified = false;
                //_unitOfWork.DbContext.Entry(obj).Property(x => x.Status).IsModified = false;

                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Success,
                        Message = $"Company deleted successfully",
                        ResponseObj = obj.CompanyId
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Failed,
                        Message = $"Sorry. Company remove failed."
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

                Expression<Func<Company, bool>> expression = x => x.CompanyId == TransportRequisitionID && x.Status == (int)Enums.Status.Active;
                Company obj = await _unitOfWork.Repository<Company>().GetFirstOrDefaultAsync(predicate: expression);

                if (obj == null)
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Warning,
                        Message = $"Sorry, Company doesn't exist.",
                        ResponseObj = null
                    };
                }

                _unitOfWork.Repository<Company>().Delete(obj);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Success,
                        Message = $"Company deleted successfully",
                        ResponseObj = obj.CompanyId
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        ResponseCode = (int)Enums.ResponseCode.Failed,
                        Message = $"Sorry. Company remove failed."
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
        private bool CheckedValidation(Company obj, ResponseMessage responseMessage)
        {
            if (string.IsNullOrEmpty(obj.CompanyName))
            {
                responseMessage.Message = ValidationMessage.CompanyName;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.ValidationError;
                return false;
            }

            if (string.IsNullOrEmpty(obj.Alias))
            {
                responseMessage.Message = ValidationMessage.Alias;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.ValidationError;
                return false;
            }

            // 3. Check is exist "Company Name"
            Company existingObj = _sbDbContext.Company.Where(x => x.CompanyName == obj.CompanyName
                                && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefault();
            if (existingObj != null)
            {
                if (existingObj.CompanyId != obj.CompanyId) {
                    responseMessage.Message = ValidationMessage.CompanyNameExist;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.ValidationError;
                    return false;
                }
            }

            // 3. Check is exist "Alias"
            Company existingAliasObj = _sbDbContext.Company.Where(x => x.Alias == obj.Alias
                                && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefault();
            if (existingAliasObj != null)
            {
                if (existingAliasObj.CompanyId != obj.CompanyId)
                {
                    responseMessage.Message = ValidationMessage.AliasExist;
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

    public interface ICompanyServices : IDisposable
    {
        Task<ResponseMessage> Search(RequestMessage requestMessage);
        Task<ResponseMessage> GetAll(RequestMessage requestMessage);
        Task<ResponseMessage> GetById(RequestMessage requestMessage);
        Task<Company> GetCompanyById(int companyID);
        Task<ResponseMessage> Save(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteSoft(RequestMessage requestMessage);
        Task<ResponseMessage> Delete(RequestMessage requestMessage);

        Task<ResponseMessage> GetAllData(RequestMessage requestMessage);
    }
}
