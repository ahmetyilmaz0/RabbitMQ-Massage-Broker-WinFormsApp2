using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Thread t = new Thread(new ThreadStart(ChangeText));
            t.Start();
        }
        private void ChangeText()
        {
            while (true)
            {
                _consumer = new Consumer(_queueName);
                SetText(_consumer.GetMessage());
            }
        }
        private delegate void SetTextDelegate(string number);
        private void SetText(string number)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new SetTextDelegate(SetText), new object[] { number });
                return;
            }
            
            if (number != null)
            {
                listBox1.Items.Add("KMS Customer Client: " + number);
            }       
        }
        //it doesnt find queue first running because there is no queue in name 
        private static readonly string _queueName = "KMSCustomerClient";
        private static readonly string _queueNameBack = "KMSBaseClient";
        private static Publisher _publisher;
        private static Consumer _consumer;

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (sender is TextBox)
                {
                    TextBox txb = (TextBox)sender;
                    _publisher = new Publisher(_queueNameBack, txb.Text);
                    listBox1.Items.Add("KMS Base Client " + txb.Text);
                    textBox1.Text = "";
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "KMSBaseClient";
        }
    }
}
