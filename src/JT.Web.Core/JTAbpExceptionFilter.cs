using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.ExceptionHandling;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Runtime.Validation;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace JT.Web
{
    public class JTAbpExceptionFilter : AbpExceptionFilter
    {
        public JTAbpExceptionFilter(IErrorInfoBuilder errorInfoBuilder, IAbpAspNetCoreConfiguration configuration)
            : base(errorInfoBuilder, configuration)
        {
        }

        protected override int GetStatusCode(ExceptionContext context, bool wrapOnError)
        {
            var exp = context.Exception;
            if (exp is AbpAuthorizationException abpAuthExp)
            {
                return context.HttpContext.User.Identity.IsAuthenticated
                    ? (int)HttpStatusCode.Forbidden
                    : (int)HttpStatusCode.Unauthorized;
            }
            if (exp is AbpValidationException abpValidExp)
            {
                return (int)HttpStatusCode.BadRequest;
            }
            if (exp is EntityNotFoundException entNFoundExp)
            {
                return (int)HttpStatusCode.NotFound;
            }
            if (exp is UserFriendlyException userFriExp)
            {
                var code = userFriExp.Code;
                if (code > 0)
                {
                    return code;
                }
            }
            if (wrapOnError)
            {
                return (int)HttpStatusCode.InternalServerError;
            }
            return context.HttpContext.Response.StatusCode;
        }
    }
}
