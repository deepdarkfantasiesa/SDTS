﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SDTS
{
    public class Constants
    {
        private static string http = "http://";
        //private static string host = "10.0.2.2:5000";//用项目名启动后端用这个
        //private static string host = "192.168.50.113";
        private static string host = "192.168.50.113:24082";

        private static string SignController = "/api/sign/";
        private static string WardsController = "/api/managewards/";
        private static string AreaController = "/api/securearea/";
        private static string UserController = "/api/user/";

        public static string SigninString = http+host+SignController+"signin";
        public static string SignupString = http + host + SignController + "signup";
        public static string ManageWardsString = http + host + WardsController + "getwards";
        public static string GetWardDetailString = http + host + WardsController + "getdetail";

        public static string PutSecureAreaString = http + host + AreaController + "ctreatearea";
        public static string PostSecureAreaString = http + host + AreaController + "alterarea";
        public static string DeleteSecureAreaString = http + host + AreaController + "deletearea";
        public static string GetSecureAreasString = http + host + AreaController + "getareas";

        public static string GetUserInfoString = http + host + UserController + "getuserinfo";


        //public static string SigninString = "http://10.0.2.2:5000/api/sign/signin";


    }
}
