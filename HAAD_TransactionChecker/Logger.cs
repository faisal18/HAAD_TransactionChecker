using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAAD_TransactionChecker
{
    class Logger
    {
        public static string baseDir = System.Configuration.ConfigurationManager.AppSettings.Get("basedir");

        public static void Info(string data)
        {
            using (StreamWriter writer = File.AppendText(baseDir + @"\Log\Infolog.csv"))
            {
                writer.Write(System.DateTime.Now + " : " + data + "\n");
            }
        }

        public static void Error(Exception data)
        {
            using (StreamWriter writer = File.AppendText(baseDir + @"\Log\Exceptionlog.csv"))
            {
                writer.Write(System.DateTime.Now + " : " + data + "\n");
            }
        }

        public static void CreateResult()
        {
            if (!File.Exists(baseDir + @"\Output.csv"))
            {
                using (StreamWriter writer = File.AppendText(baseDir + @"\Output.csv"))
                {
                    writer.Write("DateTime,SenderID,ReceiverID,Filename,Present_On_PostOffice,file_State\n");
                }
            }
        }

        public static void CreateResult(string data)
        {
            if (File.Exists(baseDir + @"\Output.csv"))
            {
                using (StreamWriter writer = File.AppendText(baseDir + @"\Output.csv"))
                {
                    writer.Write(DateTime.Now + "," + data);
                }
            }
        }
    }
}
