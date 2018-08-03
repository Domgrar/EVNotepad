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
        const string WORK_ORDER_TEXT = "Flip Work Order";
        const string MOVE_TO_QUEUE = "Move to a new queue";
        const string MOVE_TO_RESPONDED = "Move to responded";
        const string UPDATE_DESCRIPTION = "Update Description";
        List<Ticket> jeremyPersonalQueue = new List<Ticket>();

       //Dictionary<int, string> listViewFieldDictionary = new Dictionary<int, string>();
        


        public Form1()
        {
            InitializeComponent();

            jeremyPersonalQueue = SQLiteOperationFactory.GetAllQueueTickets();

            const int ROW_HEIGHT = 75;
            const int VIEW_WIDTH = 750;

            addColumnHeaders(listView1);
            addColumnHeaders(listView2);
            //addColumnHeaders(listView3);

            setRowHeight(listView1, ROW_HEIGHT);
            setRowHeight(listView2, ROW_HEIGHT);
           // setRowHeight(listView3, ROW_HEIGHT);

           // listView1.Width = VIEW_WIDTH;
          //  listView2.Width = VIEW_WIDTH;

            //DisplayListViewRightClickMenu(listView1);
            contextMenuStrip1.Items.Add(WORK_ORDER_TEXT);
            contextMenuStrip1.Items.Add(UPDATE_DESCRIPTION);
            contextMenuStrip1.Items.Add(MOVE_TO_QUEUE);



            (contextMenuStrip1.Items[0] as ToolStripMenuItem).DropDownItems.Add("Uncategorized");
            (contextMenuStrip1.Items[0] as ToolStripMenuItem).DropDownItems.Add("Group");
            (contextMenuStrip1.Items[0] as ToolStripMenuItem).DropDownItems.Add("Procurment");
            listView1.ContextMenuStrip = contextMenuStrip1;

            listView1.ShowItemToolTips = true;
            
            //this.listView1.Columns

            

            foreach (Ticket currentTicket in jeremyPersonalQueue)
            {
               

               

                if (currentTicket.Designated_Queue.Contains( "Uncategorized"))
                {
                    var item1 = new ListViewItem(new[] { currentTicket.Number, currentTicket.recipient, currentTicket.DaysLeftForSLA + " Days left", currentTicket.Short_Description, currentTicket.Comment, currentTicket.Designated_Queue, currentTicket.IsWorkOrder });
                    item1.ToolTipText = currentTicket.Description;
                    
                    listView1.Items.Add(item1);
                }
                else if(currentTicket.Designated_Queue.Contains("Response"))
                {
                    var item1 = new ListViewItem(new[] { currentTicket.Number, currentTicket.recipient, currentTicket.DaysLeftForSLA + " Days left", currentTicket.Short_Description, currentTicket.Comment, currentTicket.Designated_Queue, currentTicket.IsWorkOrder });
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
                var listViewItem = (List<ListViewItem>)e.Data.GetData(typeof(List<ListViewItem>));

                foreach(var item in listViewItem)
                {
                    Ticket currentTicket = new Ticket();
                    currentTicket.Number = item.SubItems[0].Text;
                    currentTicket.currentListView = (ListView)sender;
                    currentTicket.Designated_Queue = getGroupName((ListView)sender);
                    currentTicket.IsWorkOrder = item.SubItems[6].Text;

                    item.SubItems[5].Text = currentTicket.Designated_Queue;

                    item.ListView.Items.Remove(item);
                    listView1.Items.Add(item);

                    SQLiteOperationFactory.UpdateDesignatedQueueInDB(currentTicket);
                   // UpdateTextFile(currentTicket);
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
                
                var listViewItem = (List<ListViewItem>)e.Data.GetData(typeof (List<ListViewItem>));

                foreach (var item in listViewItem)
                {
                    Ticket currentTicket = new Ticket();
                    currentTicket.Number = item.SubItems[0].Text;
                    currentTicket.currentListView = (ListView)sender;
                    currentTicket.Designated_Queue = getGroupName((ListView)sender);
                    currentTicket.IsWorkOrder = item.SubItems[6].Text;


                    item.SubItems[5].Text = currentTicket.Designated_Queue;

                    item.ListView.Items.Remove(item);
                    listView2.Items.Add(item);

                    SQLiteOperationFactory.UpdateDesignatedQueueInDB(currentTicket);
                   // UpdateTextFile(currentTicket);
                    
                }


                // move to dest LV

                
                
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


       


        private string getWorkOrderStatus(ListView sender)
        {
            //Get work order
            string isWorkOrder = string.Empty;

            string selectedItem = sender.SelectedItems.ToString();

            return isWorkOrder;

            
        }

        public string getGroupName(ListView obj)
        {
            string groupName = string.Empty;
            if(obj.Name == "listView1")
            {
                groupName = "Uncategorized";
            }else if(obj.Name == "listView2")
            {
                groupName = "Response";
            }else if(obj.Name == "listView3")
            {
                groupName = "Procurment";
            }
            return groupName;
        }


        public void UpdateTextFile(Ticket currentTicket)
        {
            
            string currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string textFilePath = currentDirectory + "\\TicketQueueInfo.txt";
            string textFromFile = System.IO.File.ReadAllText(textFilePath);
            List<string> textLines = textFromFile.Split('\n').ToList<string>();
            string newText = string.Empty;

            //ONLY UPDATES THE GROUPNAME IN THE LINE RIGHT NOW
            foreach(string line in textLines)
            {
                if (line.Contains(currentTicket.Number))
                {
                    newText += currentTicket.Number + " " + currentTicket.Designated_Queue + " " + (string)currentTicket.IsWorkOrder + '\r' + '\n';           // NEED  the work order status out of this
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
             
            ListView.SelectedListViewItemCollection rightClickedItem = listView1.SelectedItems;

            switch (clickedItem.Text)
            {
                case MOVE_TO_QUEUE:
                    UpdateForm.Show();
                    break;
                case WORK_ORDER_TEXT:
                    
                    foreach (ListViewItem item in rightClickedItem)
                    {
                        //Set the work order element to yes and then update the text file 
                        Ticket currentTicket = new Ticket();
                        currentTicket.Number = item.SubItems[0].Text;
                        if (item.SubItems[6].Text.Contains("No"))
                        {
                            currentTicket.IsWorkOrder = "Yes";
                            item.SubItems[6].Text = "Yes";
                        }
                        else
                        {
                            currentTicket.IsWorkOrder = "No";
                            item.SubItems[6].Text = "No";
                        }
                        currentTicket.Designated_Queue = item.SubItems[5].Text;

                        SQLiteOperationFactory.UpdateWorkOrderStatusInDB(currentTicket);

                    }
                    break;
                case UPDATE_DESCRIPTION:
                    UpdateDescriptionOnForm(rightClickedItem);
                   // MessageBox.Show("Update Description clicked");
                    break;

                case "Group":
                    MessageBox.Show("Group clicked");
                    break;
                case "Uncategorized":
                    MessageBox.Show("Uncategorized clicked");
                    break;
                case "Procurment":
                    MessageBox.Show("Procurment Clicked");
                    break;


            }
            
           
        }

       

        public static void UpdateDescriptionOnForm(ListView.SelectedListViewItemCollection itemToUpdateCollection)
        {
            Ticket ticketToUpdate = null;
            UpdateDescriptionForm updateForm;
            if(itemToUpdateCollection.Count > 1)
            {
                throw new Exception("Too many items to update");
            }
            foreach(ListViewItem item in  itemToUpdateCollection)
            {
                ticketToUpdate = new Ticket((ListViewItem)item);
               // UpdateDescriptionForm.updateTicket = ticketToUpdate;
            }
            
            updateForm = new UpdateDescriptionForm(ticketToUpdate);
            updateForm.Show();
           // MessageBox.Show(ticketToUpdate.Number);
            //Launch form to get new description
        }

    }

    

}
