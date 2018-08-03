using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EasyVistaTicketNotepad
{

    

    class SQLiteOperationFactory
    {

        public static SQLiteConnection jeremyConnection = new SQLiteConnection("Data Source=PersonalQueue.db;Version=3;");
        

        public static string ReadAllTicketsFromDB()
        {
            SQLiteConnection sqlite = SQLiteOperationFactory.jeremyConnection;
            string textReader = string.Empty;
            SQLiteCommand sqliteCMD = sqlite.CreateCommand();

            sqliteCMD.CommandText = "SELECT * FROM JeremyQueue";

            SQLiteDataReader dataReader = sqliteCMD.ExecuteReader();

            int i = 0;
            while (dataReader.Read())
            {
                object idReader = dataReader.GetValue(0);
                textReader += dataReader.GetString(i);
                i += 1;
               // OutputTextBox.Text += idReader + " '" + textReader + "' " + "\n";
            }

            sqlite.Close();

            return textReader;
            
        }


        public static List<Ticket> GetAllQueueTickets()
        {

            SQLiteConnection sqlite = SQLiteOperationFactory.jeremyConnection;
            string textReader = string.Empty;
            SQLiteCommand sqliteCMD = sqlite.CreateCommand();
            sqlite.Open();
            

            

            List<Ticket> jeremyPersonalTicketsList = new List<Ticket>();

            string apiResponse = string.Empty;
            WebRequest req = WebRequest.Create(@"https://dhgllp.easyvista.com/api/v1/50004/internalqueries?queryguid=9EF97ED0-335F-46B0-86A1-24651AAFEE9A&filterguid=0A37CD95-20F1-4C17-89A8-CC4DECAAC2E8&viewguid=C92F1011-AE34-475C-A2B1-6D621E8C4D9C");
            req.Method = "GET";
            req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("administrator:TJd944bq14MQw64"));
            //req.Credentials = new NetworkCredential("username", "password");
            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;



            using (var stream = resp.GetResponseStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    apiResponse = sr.ReadToEnd();
                }
            }



            RootObject ro = new RootObject(apiResponse);
            
            

            foreach (Record rec in ro.records)
            {
                bool isInDBFlag = false;

                Ticket newTicket = new Ticket();
                // Check if this already exists in the db 
                sqliteCMD.CommandText = "SELECT * FROM JeremyQueue WHERE TicketNumber LIKE '" + rec.Number + "'";
                SQLiteDataReader dataReader = sqliteCMD.ExecuteReader();
                while (dataReader.Read())
                {
                    if(dataReader.GetString(0) == rec.Number)
                    {

                        newTicket.Number = dataReader["TicketNumber"].ToString();
                        newTicket.recipient = dataReader["Recipient"].ToString();
                        newTicket.SLA_Target = dataReader["SLA"].ToString();
                        newTicket.Description = dataReader["Description"].ToString();
                        newTicket.Comment = dataReader["Comment"].ToString();
                        newTicket.Designated_Queue = dataReader["DesignatedQueue"].ToString();
                        newTicket.IsWorkOrder = dataReader["IsWorkOrder"].ToString();
                        newTicket.SetShortDescription();
                        newTicket.SetDaysLeftForSLATarget();

                        isInDBFlag = true;
                       // jeremyPersonalTicketsList.Add(newDBTicket);
                        // Build a new ticket from the db and add it into out jeremyPersonalTickets
                    }
                    
                }
                dataReader.Close();
                if (isInDBFlag)
                {
                    jeremyPersonalTicketsList.Add(newTicket);
                }
                else
                {
                    newTicket = new Ticket(rec);
                    //Add default fields
                    newTicket.Designated_Queue = "Uncategorized";
                    newTicket.IsWorkOrder = "No";
                    newTicket.SetShortDescription();

                    jeremyPersonalTicketsList.Add(newTicket);
                    WriteNewTicketToDB(newTicket);
                    //Add the ticket to the DB
                }
               
            }

            sqlite.Close();
            return jeremyPersonalTicketsList;
            
        }

        public static bool WriteNewTicketToDB(Ticket newTicket)
        {
            SQLiteConnection sqlite = SQLiteOperationFactory.jeremyConnection;
            string textReader = string.Empty;
            SQLiteCommand sqliteCMD = new SQLiteCommand("INSERT INTO JeremyQueue (TicketNumber, Recipient, SLA, Description, Comment, DesignatedQueue, IsWorkOrder) VALUES (?, ?, ?, ?, ?, ?, ?)", sqlite);
            //sqlite.Open();

            
            sqliteCMD.Parameters.AddWithValue("TicketNumber", newTicket.Number);
            sqliteCMD.Parameters.AddWithValue("Recipient", newTicket.recipient);
            sqliteCMD.Parameters.AddWithValue("SLA", newTicket.SLA_Target);
            sqliteCMD.Parameters.AddWithValue("Description", newTicket.Description);
            sqliteCMD.Parameters.AddWithValue("Comment", newTicket.Comment);
            sqliteCMD.Parameters.AddWithValue("DesignatedQueue", newTicket.Designated_Queue);
            sqliteCMD.Parameters.AddWithValue("IsWorkOrder", "No");

            try
            {
                sqliteCMD.ExecuteNonQuery();
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }

        public static void UpdateWorkOrderStatusInDB(Ticket updateTicket)
        {
            SQLiteConnection sqlite = SQLiteOperationFactory.jeremyConnection;
            string textReader = string.Empty;
            SQLiteCommand sqliteCMD = new SQLiteCommand("UPDATE JeremyQueue SET IsWorkOrder = '" + updateTicket.IsWorkOrder + "' WHERE TicketNumber = '" + updateTicket.Number +"';", sqlite);
            sqlite.Open();

            try
            {
                sqliteCMD.ExecuteNonQuery();
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                sqlite.Close();
            }
        }

        public static void UpdateDesignatedQueueInDB(Ticket updateTicket)
        {
            SQLiteConnection sqlite = SQLiteOperationFactory.jeremyConnection;
            string textReader = string.Empty;
            SQLiteCommand sqliteCMD = new SQLiteCommand("UPDATE JeremyQueue SET DesignatedQueue = '" + updateTicket.Designated_Queue + "' WHERE TicketNumber = '" + updateTicket.Number + "';", sqlite);
            sqlite.Open();

            try
            {
                sqliteCMD.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                sqlite.Close();
            }
        }


        //Delete tickets that are no longer in the personal queue from the db
        public static void DeleteNonExistantTickets(List<Ticket> apiTicketList)
        {

        }


    }
}
