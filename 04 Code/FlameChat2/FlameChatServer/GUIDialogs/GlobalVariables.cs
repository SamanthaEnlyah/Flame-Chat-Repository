using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FlameChatAdmin.GUIDialogs
{
    public class GlobalVariables
    {
        public static IPAddress ServerIP { get; set; }
        public static string SQLServerExpressInstanceID { get; set; } = "SQLEXPRESS"; // Default value, can be changed by user input
    }
}
