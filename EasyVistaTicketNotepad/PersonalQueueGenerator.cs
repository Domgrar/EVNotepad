using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EasyVistaTicketNotepad
{
    class PersonalQueueGenerator
    {
        public static List<Ticket> getJeremyPersonalQueue()
        {
            //List<string> jeremyPersonalTicketsList = new List<string>();
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
            Console.WriteLine("************************PERSONAL QUEUE***********************");
            Console.WriteLine(ro.records.ToString());
            foreach (Record rec in ro.records)
            {
                Console.WriteLine(rec.TicketDescription);
                jeremyPersonalTicketsList.Add(new Ticket(rec));
            }

            return jeremyPersonalTicketsList;
        }

        public void ChangeQueueDesignation(string ticketNumber, List<Ticket> personalQueue)
        {

        }
    }
}
