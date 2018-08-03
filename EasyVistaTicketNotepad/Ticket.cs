using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyVistaTicketNotepad
{

    public class Ticket
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
        public string IsWorkOrder { get; set; }
        public ListView currentListView { get; set; }
        public string textFilePath { get; set; }

        
        
        
        public Ticket() { }

        public Ticket(Record rec)
        {
            
            recipient = rec.Recipient;
            requestor = rec.RequestingPerson;
            Comment = rec.Comments;
            Description = rec.TicketDescription;
            Support_Person = rec.SupportPerson;
            SLA_Target = rec.SLATarget;
            Number = rec.Number;
            Current_Status = rec.CurrentStatus;
            //Short_Description = getShortString(this.Description);
            Designated_Queue = CheckIfTicketExistsTextFile(Number, "Assignment");
            //DaysLeftForSLA = getDaysLeftForSLATarget(SLA_Target);
            ActionType = CheckIfTicketExistsTextFile(Number, "WorkOrder");
            textFilePath = System.IO.Directory.GetCurrentDirectory() + "\\TicketQueueInfo.txt";
            this.FixDescription();
            this.SetShortDescription();
            this.SetDaysLeftForSLATarget();
            
        }

        public Ticket(ListViewItem ticketItem)
        {
            this.Number = ticketItem.SubItems[0].Text;
            this.recipient = ticketItem.SubItems[1].Text;
            this.SLA_Target = ticketItem.SubItems[2].Text;
            this.Description = ticketItem.SubItems[3].Text;
            this.Comment = ticketItem.SubItems[4].Text;
            this.Designated_Queue = ticketItem.SubItems[5].Text;
            this.IsWorkOrder = ticketItem.SubItems[6].Text;

        }

        public void SetShortDescription()
        {
            if (this.Description.Length > 60)
            {
                this.Short_Description = this.Description.Substring(0, 60);
            }
            else
            {
                this.Short_Description = this.Description;
            }
        }

        public  void FixDescription()
        {
            string innerHTML = string.Empty;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            
            doc.LoadHtml(this.Description);
            
            foreach (HtmlNode pTag in doc.DocumentNode.SelectNodes("//p"))
            {
                 innerHTML += pTag.InnerText;
                // do whatever with text
            }
            innerHTML = innerHTML.Replace("&nbsp;", "");
            
            Console.WriteLine(innerHTML);
            this.Description = innerHTML;
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
                    return "Uncategorized";
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

        public void SetDaysLeftForSLATarget()
        {
            string daysLeft = string.Empty;

            DateTime SLADate = DateTime.Parse(this.SLA_Target);
            DateTime todayDate = DateTime.Now;

            daysLeft = (((todayDate - SLADate).Days) * -1).ToString();

            Console.WriteLine(daysLeft);

            this.DaysLeftForSLA = daysLeft;
        }

        /*
        public string getDaysLeftForSLATarget(string SLATarget)
        {
            string daysLeft = string.Empty;

            DateTime SLADate = DateTime.Parse(SLATarget);
            DateTime todayDate = DateTime.Now;

            daysLeft =  (((todayDate - SLADate).Days) * -1).ToString();

            Console.WriteLine(daysLeft);

            return daysLeft;
        }
        */





        #region ticketFieldUpdates

        public static void updateListViewAssignment(List<ListViewItem> currentListViewItems, Ticket ticketToUpdate, ListView currentListView)
        {
            // #1 need to update the group in the listView
            Ticket currentTicket = new Ticket();
            currentTicket.Number = currentListViewItems[0].Text;
           // currentTicket.Designated_Queue = 

            // #2 need to update the text file to reflect the new listView group
        }





        #endregion

        //Work order status will come out of the listview item
        public string GetWorkOrderStatus()
        {
            if (this.Number == "")
            {
                return "No";
            }
            else
            {
                return this.IsWorkOrder;
            }
        }

        public void GetGroupName()
        {
            string groupName = string.Empty;

            if(this.Number == "")
            {
                this.Designated_Queue = "Uncategorized";
            }
            else
            {
                if(this.currentListView.Name == "listView1")
                {
                    this.Designated_Queue = "Uncategorized";
                }
            else if (this.currentListView.Name == "listView2")
            {
                    this.Designated_Queue = "Group";
            }
            else if (this.currentListView.Name == "listView3")
            {
                    this.Designated_Queue = "Procurment";
            }
            
        }
        }


        public static void ChangeWorkOrderStatus(Ticket ticketToChange)
        {
            string textFilePath = System.IO.Directory.GetCurrentDirectory() + "\\TicketQueueInfo.txt";

            string textFromFile = File.ReadAllText(textFilePath);
            string newTextForFile = string.Empty;

            List<string> allLines = textFromFile.Split('\n').ToList<string>();

            foreach(string line in allLines)
            {
                if(line.Split(' ').Contains(ticketToChange.Number))
                {
                    newTextForFile += ticketToChange.Number + " " + ticketToChange.Designated_Queue + " " + ticketToChange.IsWorkOrder + '\r' + '\n';
                }
                else
                {
                    newTextForFile += line + '\r' + '\n';
                }
            }

            File.WriteAllText(textFilePath, newTextForFile);
        }




    }
}
