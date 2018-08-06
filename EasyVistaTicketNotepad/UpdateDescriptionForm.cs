﻿using System;
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
    public partial class UpdateDescriptionForm : Form
    {

        public  Ticket mToUpdate;
        public ListViewItem mSelectedItem;

        public UpdateDescriptionForm(Ticket toUpdate, ListViewItem selectedItem)
        {
            InitializeComponent();
            mToUpdate = toUpdate;
            mSelectedItem = selectedItem;
            richTextBox1.Text = mToUpdate.Description;
           // richTextBox1.Text = updateTicket.Description;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // save changes to SQL database and pass the changes back via the updateTicket
            MessageBox.Show(mToUpdate.Number);

            mToUpdate.Description = richTextBox1.Text;
            //Get new description from textbox

            SQLiteOperationFactory.UpdateDescriptionInDB(mToUpdate);
            //Save to db

            HideAndReturnTicket(mToUpdate);
            //return new ticket info
            
        }

        public void HideAndReturnTicket(Ticket returnTicket)
        {
            returnTicket = mToUpdate;
            this.Close();
             
        }

        private void UpdateDescriptionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.GetNewTicket(mToUpdate, mSelectedItem);
        }
    }
}
