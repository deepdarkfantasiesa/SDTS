﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class UserTypes
    {
        public static IList<string> types;
        static UserTypes()
        {
            types = new List<string>();
            types.Add("监护人");
            types.Add("被监护人");
            types.Add("志愿者");
        }
    }
}
