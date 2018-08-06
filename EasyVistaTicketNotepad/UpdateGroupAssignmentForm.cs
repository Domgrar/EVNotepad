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
    public partial class UpdateGroupAssignmentForm : Form
    {
        public UpdateGroupAssignmentForm()
        {
            InitializeComponent();
        }

        private void UpdateWorkOrderForm_Load(object sender, EventArgs e)
        {
           
            string[] groupsArray = { "Application Development", "Procurment", "To-Do" };
           
            for(int i = 0; i < groupsArray.Length; i++)
            {
                comboBox1.Items.Add(groupsArray[i]);
            }


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void UpdateGroupAssignmentForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
