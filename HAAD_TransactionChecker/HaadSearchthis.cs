using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HAAD_TransactionChecker
{
    class HaadSearchthis
    {

        public static string baseDir = System.Configuration.ConfigurationManager.AppSettings.Get("basedir");

        public static void execute()
        {

            string username = "Abu Dhabi National";
            string password = "vame4Pen";

            int direction = 1; //sent
            int downloadstatus = 1; //non downloaded
            int transactionType = 8;// RA


            try
            {
                username = System.Configuration.ConfigurationManager.AppSettings.Get("username");
                password = System.Configuration.ConfigurationManager.AppSettings.Get("password");
                direction = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("direction"));
                transactionType = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("transactionType"));


                List<Haad_Result> list_yo = new List<Haad_Result>();

                string[] files = File.ReadAllLines(baseDir + "Input.csv");
                
                Console.WriteLine("Input file parsed");
                Logger.CreateResult();
                for (int i = 1; i < files.Length; i++)
                {

                    Haad_Result yo = new Haad_Result();
                    string[] splitter = files[i].Split(',');

                    yo.ReceiverID = splitter[0].ToString();
                    yo.Filename = Path.GetFileNameWithoutExtension(splitter[1].ToString()) + ".xml";
                    bool found = false;


                    
                    if (CallHAADSearch(username, password, "", direction, downloadstatus, transactionType, "", yo) > 0)
                    {
                        Console.WriteLine("File found in Downloaded state");
                        Logger.Info("Found file in NON Downloaded state");
                        yo.Present_On_PostOffice = "Yes";
                        yo.file_State = "Downloaded";
                        list_yo.Add(yo);
                        found = true;
                    }
                    else if (CallHAADSearch(username, password, "", direction, 2, transactionType, "", yo) > 0)
                    {
                        Console.WriteLine("File found in Non Downloaded state");
                        Logger.Info("Found file in Downloaded state");
                        yo.Present_On_PostOffice = "Yes";
                        yo.file_State = "Not Downloaded";
                        list_yo.Add(yo);
                        found = true;

                    }
                    else
                    {
                        Console.WriteLine("File not found");
                        Logger.Info("File not found");
                        yo.Present_On_PostOffice = "NO";
                        yo.file_State = "N/A";
                        list_yo.Add(yo);
                    }

                }

                Logger.CreateResult();

                Console.WriteLine("Generating output");
                for (int i = 0; i < list_yo.Count; i++)
                {
                    string data = list_yo[i].SenderID + "," + list_yo[i].ReceiverID + "," + list_yo[i].Filename + "," +
                        list_yo[i].Present_On_PostOffice + "," + list_yo[i].file_State + "\n";


                    Logger.CreateResult(data);
                }

                Console.WriteLine("Program Complete");
                Console.Read();



            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private static int CallHAADSearch(string username, string password, string license, int direction, int downloadstatus, int TransactionType, string SearchDate, Haad_Result yo)
        {
            int i = 0;
            try
            {
                Logger.Info("Haad Search method called Started");
                string foundTransactions = string.Empty;
                string errorMessage = string.Empty;

                
                string SearchDateFrom = "";
                string SearchDateTo = "";
                string ePartner = "";

                HaaD.WebservicesSoapClient WS = new HaaD.WebservicesSoapClient();
                int result = WS.SearchTransactions(username, password, direction, license, ePartner, TransactionType, downloadstatus, yo.Filename, SearchDateFrom, SearchDateTo, -1, -1, out foundTransactions, out errorMessage);

                if (foundTransactions != "<Files></Files>")
                {
                    i = GetNumberofFiles(foundTransactions);


                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(foundTransactions);
                    string Filename = xdoc.SelectSingleNode("//Files/File").Attributes["FileName"].Value;
                    yo.file_ext = Path.GetExtension(Filename);

                    Logger.Info(foundTransactions);
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                i = -1;
            }

            return i;
        }

        private static int GetNumberofFiles(string files)
        {
            int i = 0;

            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(files);

                XmlNodeList nodelist = xdoc.SelectNodes("//File");
                i = nodelist.Count;

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return i;
        }

    }

    public class Haad_Result
    {
        public string SenderID { get; set; }
        public string ReceiverID { get; set; }
        public string Filename { get; set; }
        public string Present_On_PostOffice { get; set; }
        public string file_State { get; set; }
        public string file_ext { get; set; }

    }
}
