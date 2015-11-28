using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaser;

namespace Global
{
    public class OPTIONS
    {
        public static Portal Login;
        public static Portal Chat;

        public static int clientID;
        public static Link Connection;

        public static void init()
        {
            Login = new Portal();
            Chat = new Portal();
    }
    }
}
