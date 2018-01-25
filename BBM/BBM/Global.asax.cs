using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BBM.Business.Infractstructure.Security;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using BBM.Business.Infractstructure;

namespace BBM
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MappersBusiness.Boot();
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {

                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                CustomPrincipalSerializeModel serializeModel = JsonConvert.DeserializeObject<CustomPrincipalSerializeModel>(authTicket.UserData);
                CustomPrincipal newUser = new CustomPrincipal(authTicket.Name);

                newUser.UserId = serializeModel.UserId;
                newUser.UserName = serializeModel.UserName;
                newUser.Email = serializeModel.Email;
                newUser.ChannelId = serializeModel.ChannelId;
                newUser.BranchesId = serializeModel.BranchesId;
                newUser.role_branches = serializeModel.role_branches;
                newUser.IsPrimary = serializeModel.IsPrimary;
                HttpContext.Current.User = newUser;
            }

        }
    }
}
