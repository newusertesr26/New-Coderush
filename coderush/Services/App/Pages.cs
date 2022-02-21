using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Services.App
{
    //static class for app pages common information
    public static partial class Pages
    {
        public static class DataMaster
        {
            public const string ControllerName = "DataMaster";
            public const string RoleName = "DataMaster";
            public const string UrlDefault = "/DataMaster/DataMasters";
            public const string NavigationName = "DataMaster";
        }
        public static class Todo
        {
            public const string ControllerName = "Todo";
            public const string RoleName = "Todo";
            public const string UrlDefault = "/Todo/Index";
            public const string NavigationName = "Todo";
        }

        public static class Membership
        {
            public const string ControllerName = "Membership";
            public const string RoleName = "Membership";
            public const string UrlDefault = "/Membership/Index";
            public const string NavigationName = "Membership";
        }

        public static class Role
        {
            public const string ControllerName = "Role";
            public const string RoleName = "Role";
            public const string UrlDefault = "/Role/Index";
            public const string NavigationName = "Role";
        }
    }
}
