using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVistaTicketNotepad
{
    class RootObject
    {
        private string content;

        public RootObject(string content)
        {
            content = content.Replace("Comments (Action)", "Comments");        //  MAYBE?
            content = content.Replace("Requesting Person", "RequestingPerson");
            content = content.Replace("Current Status", "CurrentStatus");
            content = content.Replace("SLA Target", "SLATarget");
            content = content.Replace("Support Person", "SupportPerson");
            content = content.Replace("Ticket Description", "TicketDescription");
            content = content.Replace("Action Type", "ActionType");
            Console.WriteLine(content);

            JObject jObject = JObject.Parse(content);
            //Description description = new Description();
            HREF = (string)jObject["HREF"];
            //object test = ( (string)jObject["records"]);
            records = jObject["records"].ToObject<List<Record>>();

            //COMMENT = jObject["COMMENT"].ToObject<Comment>();
        }


        public string HREF { get; set; }
        public int recordcount { get; set; }
        public int previouspage { get; set; }
        public int nextpage { get; set; }
        public List<Record> records { get; set; }
    
    }
}
