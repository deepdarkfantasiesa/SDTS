using System;
using System.Collections.Generic;
using System.Text;

namespace SDTS
{
    public class Constants
    {
        private static string http = "http://";
        private static string host = "10.0.2.2:5000";
        private static string SignController = "/api/sign/";
        public static string SigninString = http+host+SignController+"signin";
        public static string SignupString = http + host + SignController + "signup";
        //public static string SigninString = "http://10.0.2.2:5000/api/sign/signin";


    }
}
