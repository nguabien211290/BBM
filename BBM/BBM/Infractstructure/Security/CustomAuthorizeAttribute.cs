using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Models.Enum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace BBM.Infractstructure.Security
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public string UsersConfigKey { get; set; }
        public string RolesConfigKey { get; set; }
        public RolesEnum[] RolesEnums { get; set; }

        protected virtual CustomPrincipal CurrentUser
        {
            get { return HttpContext.Current.User as CustomPrincipal; }
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                var authorizedUsers = ConfigurationManager.AppSettings[UsersConfigKey];
                //var authorizedRoles = ConfigurationManager.AppSettings[RolesConfigKey];
                var authorizedRoles = Helpers.RolesAuthor(RolesEnums);

                Users = String.IsNullOrEmpty(Users) ? authorizedUsers : Users;
                Roles = authorizedRoles;// String.IsNullOrEmpty(Roles) ? authorizedRoles : Roles;

                if (!String.IsNullOrEmpty(Roles))
                {
                    
                    if (!CurrentUser.IsInRole(Roles))
                    {
                        filterContext.Result = new RedirectToRouteResult(new
                     RouteValueDictionary(new { controller = "Auth", action = "Access_Denied" }));

                        // base.OnAuthorization(filterContext); //returns to login url
                    }
                }

                //if (!String.IsNullOrEmpty(Users))
                //{
                //    if (!Users.Contains(CurrentUser.UserId.ToString()))
                //    {
                //        filterContext.Result = new RedirectToRouteResult(new
                //     RouteValueDictionary(new { controller = "Auth", action = "Access_Denied" }));

                //        // base.OnAuthorization(filterContext); //returns to login url
                //    }
                //}
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                   RouteValueDictionary(new { controller = "Auth", action = "Access_Denied" }));
            }
        }
    }
}