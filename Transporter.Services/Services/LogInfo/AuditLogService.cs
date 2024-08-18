using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transporter.Common.Constants;
using Transporter.Common.Helper.AuditLog;
using Transporter.Common.Models;
using Transporter.DataAccess;
using Transporter.Repository;

namespace Transporter.Services.Services.Log
{
    public class AuditLogService : IAuditLogService
    {
        private IUnitOfWork<SbDbContext> _unitOfWork;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHttpContextAccessor _httpContext;
        public AuditLogService(IUnitOfWork<SbDbContext> unitOfWork, IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _serviceScopeFactory = serviceScopeFactory;
            _httpContext = httpContext;
        }


        public async Task SaveAuditLogs(int moduleId, string FormName, string CalledFunction, int actionId, int userRightId, int userTypeId, object logMessage, string logRefMessage, bool IsObject, int logTypeId)
        {

            try
            {
                var token = _httpContext.HttpContext.Request.Headers[ HttpHeaders.Token];

                var handler = new JwtSecurityTokenHandler();
                var JWTToken = handler.ReadToken(token)
                    as JwtSecurityToken;




                AuditLog log = new AuditLog();
                log.UserID = (JWTToken is null) ? "" : GetTokenUser(JWTToken);
                log.ModuleID = moduleId;
                log.FormName = FormName;
                log.CalledFunction = CalledFunction;
                log.ActionID = actionId;
                log.UserRightID = userRightId;
                log.UserTypeID = userTypeId;
                log.LogMessage = System.Text.Json.JsonSerializer.Serialize(logMessage);
                log.LogRefMessage = logRefMessage;
                log.IsObj = IsObject;
                log.LogTime = DateTime.Now;
                log.LogTypeID = logTypeId;
                log.SessionID = (JWTToken is null) ? 0 : Getsession(JWTToken); ;
                log.CompanyID = (JWTToken is null) ? 0 : GetTokenCompanyID(JWTToken);
                //for create new theard
                Thread thread = new Thread(() => saveLogs(log));
                thread.Start();

            }
            catch (Exception ex)
            {
                //TO DO:Later will write txt file 
            }
        }


        public async Task saveLogs(AuditLog log)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    //TODO: jafar ulla - do it.

                    //var db = scope.ServiceProvider.GetService<SbDbContext>();
                    //await db.AuditLog.AddAsync(log);
                    //await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {


            }

        }


        private bool _disposed = false;

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _unitOfWork.SaveChanges();
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

        private string GetTokenUser(JwtSecurityToken token)
        {
            return token.Claims.First(claim => claim.Type == JwtClaims.UserId).Value;
        }

        private int GetTokenCompanyID(JwtSecurityToken token)
        {
            return Convert.ToInt32(token.Claims.Where(claim => claim.Type == JwtClaims.CompanyId).Select(p => p.Value).FirstOrDefault());
        }

        private int Getsession(JwtSecurityToken token)
        {
            return Convert.ToInt32(token.Claims.Where(claim => claim.Type == JwtClaims.SessionId).Select(p => p.Value).FirstOrDefault());
        }

    }
    public interface IAuditLogService
    {
        Task SaveAuditLogs(int moduleId, string FormName, string CalledFunction, int actionId, int userRightId, int userTypeId, object logMessage, string logRefMessage, bool IsObject, int logTypeId);
    }
}
