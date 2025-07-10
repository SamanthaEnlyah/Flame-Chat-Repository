
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using SharedModel.Models;
using System.Security.Principal;


namespace FlameChatDatabaseLibrary.DB
{
    public class DBContext
    {
        static FlameChatContext db;
        public static string SQLServerExpressInstanceID = "";


        public static FlameChatContext FlameDBContext
        {
            get
            {
                if (db == null)
                {
                    string computerName = Environment.MachineName;

                    var contextOptions = new DbContextOptionsBuilder<FlameChatContext>()
                        .UseSqlServer(@"Data Source=" + computerName + "\\" + SQLServerExpressInstanceID + ";Initial Catalog=FlameChat;Integrated Security=True;Encrypt=False;Trust Server Certificate=True") // Corrected spelling of 'flamechat' to 'FlameChat'
                        .Options;
                    db = new FlameChatContext(contextOptions);
                }
                return db;
            }
        }
    }
}
