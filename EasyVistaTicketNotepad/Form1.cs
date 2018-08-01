using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyVistaTicketNotepad
{
    public partial class Form1 : Form
    {
        const string WORK_ORDER_TEXT = "Work Order";
        const string MOVE_TO_QUEUE = "Move to a new queue";
        List<Ticket> jeremyPersonalQueue = new List<Ticket>();

        public Form1()
        {
            InitializeComponent();

            const int ROW_HEIGHT = 75;
            const int VIEW_WIDTH = 750;

            addColumnHeaders(listView1);
            addColumnHeaders(listView2);
            addColumnHeaders(listView3);

            setRowHeight(listView1, ROW_HEIGHT);
            setRowHeight(listView2, ROW_HEIGHT);
            setRowHeight(listView3, ROW_HEIGHT);

            listView1.Width = VIEW_WIDTH;
            listView2.Width = VIEW_WIDTH;

            //DisplayListViewRightClickMenu(listView1);
            contextMenuStrip1.Items.Add(WORK_ORDER_TEXT);
            contextMenuStrip1.Items.Add("Update Description");
            contextMenuStrip1.Items.Add(MOVE_TO_QUEUE);
            listView1.ContextMenuStrip = contextMenuStrip1;


            jeremyPersonalQueue = PersonalQueueGenerator.getJeremyPersonalQueue();
            //this.listView1.Columns

            

            foreach (Ticket currentTicket in jeremyPersonalQueue)
            {
               

               

                if (currentTicket.Designated_Queue.Contains( "Uncategorized"))
                {
                    var item1 = new ListViewItem(new[] { currentTicket.Number, currentTicket.recipient, currentTicket.DaysLeftForSLA + " Days left", currentTicket.Short_Description, currentTicket.Comment, currentTicket.Designated_Queue, currentTicket.ActionType });
                    listView1.Items.Add(item1);
                }
                else if(currentTicket.Designated_Queue.Contains("Group"))
                {
                    var item1 = new ListViewItem(new[] { currentTicket.Number, currentTicket.recipient, currentTicket.DaysLeftForSLA + " Days left", currentTicket.Short_Description, currentTicket.Comment, currentTicket.Designated_Queue, currentTicket.ActionType });
                    listView2.Items.Add(item1);
                }
               
            }

          

            
           

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        public static void addColumnHeaders(ListView view)
        {
            ColumnHeader numberHeader, recipientHeader, SLAHeader, TicketDescription, Comments, DesignatedQueue, IsWorkOrder;
            numberHeader = new ColumnHeader();
            recipientHeader = new ColumnHeader();
            SLAHeader = new ColumnHeader();
            TicketDescription = new ColumnHeader();
            Comments = new ColumnHeader();
            DesignatedQueue = new ColumnHeader();
            IsWorkOrder = new ColumnHeader();

            IsWorkOrder.Text = "Is In Work Order?";
            IsWorkOrder.TextAlign = HorizontalAlignment.Left;
            IsWorkOrder.Width = 100;

            DesignatedQueue.Text = "Queue";
            DesignatedQueue.TextAlign = HorizontalAlignment.Left;
            DesignatedQueue.Width = 100;

            TicketDescription.Text = "Description";
            TicketDescription.TextAlign = HorizontalAlignment.Left;
            TicketDescription.Width = 200;

            Comments.Text = "Comments";
            Comments.TextAlign = HorizontalAlignment.Left;
            Comments.Width = 100;

            numberHeader.Text = "Incident Number";
            numberHeader.TextAlign = HorizontalAlignment.Left;
            numberHeader.Width = 100;
            

            recipientHeader.Text = "Recipient Name";
            recipientHeader.TextAlign = HorizontalAlignment.Left;
            recipientHeader.Width = 100;

            SLAHeader.Text = "SLA Date";
            SLAHeader.TextAlign = HorizontalAlignment.Left;
            SLAHeader.Width = 70;

            view.Columns.Add(numberHeader);
            view.Columns.Add(recipientHeader);
            view.Columns.Add(SLAHeader);
            view.Columns.Add(TicketDescription);
            view.Columns.Add(Comments);
            view.Columns.Add(DesignatedQueue);
            view.Columns.Add(IsWorkOrder);
        }


        public static void setRowHeight(ListView view, int height)
        {
            ImageList HeightControlImageList = new System.Windows.Forms.ImageList();
            HeightControlImageList.ImageSize = new System.Drawing.Size(1, height);
            HeightControlImageList.TransparentColor = System.Drawing.Color.Transparent;
            view.SmallImageList = HeightControlImageList;
        }


        #region listView1EventsRegions
        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // create array or collection for all selected items
            var items = new List<ListViewItem>();
            // add dragged one first
            items.Add((ListViewItem)e.Item);
            // optionally add the other selected ones
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                if (!items.Contains(lvi))
                {
                    items.Add(lvi);
                }
            }
            // pass the items to move...
            listView1.DoDragDrop(items, DragDropEffects.Move);
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(List<ListViewItem>)))
            {
                var items = (List<ListViewItem>)e.Data.GetData(typeof(List<ListViewItem>));
                string ticketNumber = items[0].Text;
                string groupName = getGroupName((ListView)sender);


                // move to dest LV
                foreach (ListViewItem lvi in items)
                {

                    lvi.SubItems[5].Text = groupName;

                    lvi.ListView.Items.Remove(lvi);
                    listView1.Items.Add(lvi);

                    updateTextFile(ticketNumber, groupName, "No");

                }
            }
        }

        private void listView1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(List<ListViewItem>)))
            {
                e.Effect = DragDropEffects.Move;
                // textBox1.AppendText("List View 2 Drag Over hit");
            }
        }

        #endregion



        #region listView2Events
        private void listView2_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(List<ListViewItem>)))
            {
                var items = (List<ListViewItem>)e.Data.GetData(typeof(List<ListViewItem>));
                string ticketNumber =  items[0].Text;
                string groupName = getGroupName((ListView)sender);
                
                
                // move to dest LV
                foreach (ListViewItem lvi in items)
                {
                    
                    lvi.SubItems[5].Text = groupName;

                    lvi.ListView.Items.Remove(lvi);
                    listView2.Items.Add(lvi);

                    updateTextFile(ticketNumber, groupName, "No");
                 
                }
            }
        }

        private void listView2_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(List<ListViewItem>)))
            {
                e.Effect = DragDropEffects.Move;
               // textBox1.AppendText("List View 2 Drag Over hit");
            }
        }

        private void listView2_ItemDrag(object sender, ItemDragEventArgs e)
        {
            
            // create array or collection for all selected items
            var items = new List<ListViewItem>();
            // add dragged one first
            items.Add((ListViewItem)e.Item);
            // optionally add the other selected ones
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                if (!items.Contains(lvi))
                {
                    items.Add(lvi);
                }
            }
            // pass the items to move...
            listView2.DoDragDrop(items, DragDropEffects.Move);
        }

        #endregion


        #region listView3Region
        private void listView3_ItemDrag(object sender, ItemDragEventArgs e)
        {

        }


        #endregion




        public string getGroupName(ListView obj)
        {
            string groupName = string.Empty;
            if(obj.Name == "listView1")
            {
                groupName = "Uncategorized";
            }else if(obj.Name == "listView2")
            {
                groupName = "Group";
            }else if(obj.Name == "listView3")
            {
                groupName = "Procurment";
            }
            return groupName;
        }

        public void updateTextFile(string ticketNumber, string groupName, string isWorkOrder)
        {
            string currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string textFilePath = currentDirectory + "\\TicketQueueInfo.txt";
            string textFromFile = System.IO.File.ReadAllText(textFilePath);
            List<string> textLines = textFromFile.Split('\n').ToList<string>();
            string newText = string.Empty;

            //ONLY UPDATES THE GROUPNAME IN THE LINE RIGHT NOW
            foreach(string line in textLines)
            {
                if (line.Contains(ticketNumber))
                {
                    newText += ticketNumber + " " + groupName + " " +  + '\r' + '\n';           // NEED  the work order status out of this
                }
                else
                {
                    newText += line + '\r' + '\n';
                }
            }

            Console.WriteLine(newText);
            System.IO.File.WriteAllText(textFilePath, newText);

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
          
        }

       

        void contextMenu_ItemClicked()
        {
            
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var clickedItem = e.ClickedItem;
            Form UpdateForm = new UpdateGroupAssignmentForm();

            if(clickedItem.Text == MOVE_TO_QUEUE)
            {
                UpdateForm.Show();
            }
        }

        
    }
}
