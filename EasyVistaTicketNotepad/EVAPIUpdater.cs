using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EasyVistaTicketNotepad
{
    class EVAPIUpdater
    {


        //Calls Comment below because EV's comment == description in service manger
        public static void UpdateTicketDescription(Ticket ticketToUpdate)
        {
            string uri = @"https://dhgllp.easyvista.com/api/v1/50004/requests/" + ticketToUpdate.Number.Replace(" ", "");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";
            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("administrator:TJd944bq14MQw64"));

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = " {   \"Comment\": \""+ticketToUpdate.Description+"\"} ";
                Console.WriteLine("JSON Post Response : " + json);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Console.WriteLine("JSON result! " + result);
            }


        }
    }
}
