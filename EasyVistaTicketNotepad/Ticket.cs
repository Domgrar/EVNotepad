﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVistaTicketNotepad
{
    class Ticket
    {
        public string recipient { get; set; }
        public string requestor { get; set; }
        public string Comment { get; set; }
        public string Description { get; set; }
        public string Support_Person { get; set; }
        public string SLA_Target { get; set; }
        public string Number { get; set; }
        public string Current_Status { get; set; }
        public string Short_Description { get; set; }
        public string Designated_Queue { get; set; }
        public string DaysLeftForSLA { get; set; }
        public string ActionType { get; set; }

        public Ticket(Record rec)
        {
            recipient = rec.Recipient;
            requestor = rec.RequestingPerson;
            Comment = rec.Comments;
            Description = getInnerHTMLOfDescription( rec.TicketDescription);
            Support_Person = rec.SupportPerson;
            SLA_Target = rec.SLATarget;
            Number = rec.Number;
            Current_Status = rec.CurrentStatus;
            Short_Description = getShortString(this.Description);
            Designated_Queue = CheckIfTicketExistsTextFile(Number, "Assignment");
            DaysLeftForSLA = getDaysLeftForSLATarget(SLA_Target);
            ActionType = CheckIfTicketExistsTextFile(Number, "WorkOrder");
        }

        public static string getInnerHTMLOfDescription(string recDescription)
        {
            string innerHTML = string.Empty;

            HtmlDocument doc = new HtmlDocument();
            
            doc.LoadHtml(recDescription);
            
            foreach (HtmlNode pTag in doc.DocumentNode.SelectNodes("//p"))
            {
                 innerHTML += pTag.InnerText;
                // do whatever with text
            }
            innerHTML = innerHTML.Replace("&nbsp;", "");
            
            Console.WriteLine(innerHTML);
            return innerHTML;
        }

        public static string getShortString(string description)
        {
            string shortDescription = string.Empty;

            if (description.Length < 60)
            {
                shortDescription = description.Substring(0, description.Length / 2);
            }
            else
            {
                shortDescription = description.Substring(0, 60);
            }

            return shortDescription;
        }

        public static string CheckIfTicketExistsTextFile(string ticketNumber, string attributeSearch)
        {
            string currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string textFilePath = currentDirectory + "\\TicketQueueInfo.txt";
            string textFileText = string.Empty;
            string assignment = string.Empty;

            if (File.Exists(textFilePath))
            {
                textFileText = System.IO.File.ReadAllText(textFilePath);

                if (textFileText.Contains(ticketNumber))
                {
                    if (attributeSearch == "Assignment")
                    {
                        assignment = getViewAssignment(textFileText, ticketNumber);
                    }else if(attributeSearch == "WorkOrder")
                    {
                        assignment = GetWorkOrderStatus(ticketNumber, textFileText);
                    }
                }
                else
                {
                    writeTicketToTextFile(ticketNumber, textFilePath);
                }


            }
            else
            {
                File.Create(currentDirectory + "\\TicketQueueInfo.txt");

            }
            return assignment;
        }

        public static string getViewAssignment(string fileText, string ticketNumber)
        {
            List<string> textFileLines = new List<string>();
            List<string> fileNumberList = new List<string>();
            string ticketQueue = string.Empty;
            int i = 0;

            textFileLines = fileText.Split('\n').ToList<string>();

            foreach(string currentLine in textFileLines)
            {
                string currentNumber = currentLine.Split(' ')[0];

                if(currentNumber == ticketNumber)
                {
                    ticketQueue = currentLine.Split(' ')[1];                 
                }

                i++;
            }

            return ticketQueue;
        }

        public static string GetWorkOrderStatus(string ticketNumber, string fileText)
        {
            string workOrderStatus = string.Empty;

            List<string> textFileLines = new List<string>();
            List<string> fileNumberList = new List<string>();
            
            int i = 0;

            textFileLines = fileText.Split('\n').ToList<string>();

            foreach (string currentLine in textFileLines)
            {
                string currentNumber = currentLine.Split(' ')[0];

                if (currentNumber == ticketNumber)
                {
                   workOrderStatus = currentLine.Split(' ')[2];
                }

                i++;
            }

            return workOrderStatus;
        }

        public static void writeTicketToTextFile(string ticketNumber, string textFilePath)
        {
            using (StreamWriter sw = File.AppendText(textFilePath))
            {
                sw.WriteLine(ticketNumber + " " + "Uncategorized No");
            }
        }

        public string getDaysLeftForSLATarget(string SLATarget)
        {
            string daysLeft = string.Empty;

            DateTime SLADate = DateTime.Parse(SLATarget);
            DateTime todayDate = DateTime.Now;

            daysLeft =  (((todayDate - SLADate).Days) * -1).ToString();

            Console.WriteLine(daysLeft);

            return daysLeft;
        }







    }
}
