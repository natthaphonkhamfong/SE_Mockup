﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO.Ports;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace MockupV1
{
    public partial class Form2 : Form
    {
        private static ConnectDatabase databasecmd;
        private bool isStart = false;
        private bool isReset = false;
        private int readCount = 0;
        private int addDataCount = 0;
        private int addServerCount = 0;
        private int addFileCount = 0;
        private SerialPort iSerialPort;
        private int m_nType = -1;




        byte[] m_btAryBuffer = new byte[4096];

        int m_nLenth = 0;

        public Form2()
        {
            InitializeComponent();
            databasecmd = new ConnectDatabase();
        }

        public void updateDataGridView()
        {
            if(databasecmd.connection.State == ConnectionState.Closed)
            {
                databasecmd.connectDB();

            }
            try
            {
                //using (MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT epc,time,ant_id FROM checkpoint", databasecmd.connection))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT epc,time,ant_id FROM checkpoint",databasecmd.connection))
                {
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    dataGridView1.DataSource = ds.Tables[0];
                }

                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (databasecmd.connection.State == ConnectionState.Open)
                    {
                        databasecmd.connection.Clone();
                    }
                }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            WindowState = FormWindowState.Maximized;

            ant1.ForeColor = Color.Gray;
            ant2.ForeColor = Color.Gray;
            ant3.ForeColor = Color.Gray;
            ant4.ForeColor = Color.Gray;
            ant5.ForeColor = Color.Gray;
            ant6.ForeColor = Color.Gray;
            ant7.ForeColor = Color.Gray;
            ant8.ForeColor = Color.Gray;
            ant1.Text = "ant1 : 00:00:00:00";
            ant2.Text = "ant2 : 00:00:00:00";
            ant3.Text = "ant3 : 00:00:00:00";
            ant4.Text = "ant4 : 00:00:00:00";
            ant5.Text = "ant1 : 00:00:00:00";
            ant6.Text = "ant2 : 00:00:00:00";
            ant7.Text = "ant3 : 00:00:00:00";
            ant8.Text = "ant4 : 00:00:00:00";


            comPort.SelectedIndex = 2;
            comBaudrate.SelectedIndex = 1;

            iSerialPort = new SerialPort();

            iSerialPort.DataReceived += new SerialDataReceivedEventHandler(ReceivedComData);
        }

        private void resetAnt()
        {
            ant1.Text = "ant1 : 00:00:00:00";
            ant2.Text = "ant2 : 00:00:00:00";
            ant3.Text = "ant3 : 00:00:00:00";
            ant4.Text = "ant4 : 00:00:00:00";
            ant5.Text = "ant1 : 00:00:00:00";
            ant6.Text = "ant2 : 00:00:00:00";
            ant7.Text = "ant3 : 00:00:00:00";
            ant8.Text = "ant4 : 00:00:00:00";
        }

        private void connectLAN_Click(object sender, EventArgs e)
        {

            string strException = string.Empty;
            string strComPort = comPort.Text;
            int nBaudrate = Convert.ToInt32(comBaudrate.Text);

            int nRet = OpenCom(strComPort, nBaudrate, out strException);

            if (nRet != 0)
            {
                string strLog = "Connection failed, failure cause: " + strException;
               
                MessageBox.Show(strLog);
              

                return;
            }
            else
            {
                string strLog = "Connect" + strComPort + "@" + nBaudrate.ToString();
             
                MessageBox.Show(strLog);
                start.Enabled = true;
                
                comBaudrate.Enabled = false;
                comPort.Enabled = false;
                connectLAN.BackColor = Color.Green;
                  
                connectLAN.Enabled = false;

                return;
            }

            if (!port4.Checked && !port8.Checked)
            {
                MessageBox.Show("Please Select Port Enable");
            }
            else if (port4.Checked)
            {
                //sent signal enable Hardware
                //need function to check port status
                ant1.ForeColor = Color.Green;
                ant2.ForeColor = Color.Green;
                ant3.ForeColor = Color.Green;
                ant4.ForeColor = Color.Green;
                port4.Enabled = false;
                port8.Enabled = false;
                connectLAN.Enabled = false;
                pointList.Enabled = false;
                start.Enabled = true;
                status1.ForeColor = Color.LimeGreen;
                status1.Text = "Connected";
            }
            else
            {
                //sent signal enable Hardware
                //need function to check port status
                ant1.ForeColor = Color.Green;
                ant2.ForeColor = Color.Green;
                ant3.ForeColor = Color.Green;
                ant4.ForeColor = Color.Green;
                ant5.ForeColor = Color.Green;
                ant6.ForeColor = Color.Green;
                ant7.ForeColor = Color.Green;
                ant8.ForeColor = Color.Green;
                port4.Enabled = false;
                port8.Enabled = false;
                connectLAN.Enabled = false;
                pointList.Enabled = false;
                start.Enabled = true;
                status1.ForeColor = Color.LimeGreen;
                status1.Text = "Connected";
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
  
        }

        private void start_Click(object sender, EventArgs e)
        {
            isStart = !isStart;
            if(isStart)
            {
                reset.Enabled = false;
                start.BackColor = Color.Red;
                start.ForeColor = Color.White;
                start.Text = "Stop";
                label6.Enabled = true;
                //updateDataGridView();
                //databasecmd.cmd.CommandText = "SELECT MAX(TIME(time)) as time FROM checkpoint WHERE ant_id = 1";
                //using (MySqlDataReader reader = databasecmd.cmd.ExecuteReader())
                //{
                //    if (reader.Read())
                //    {
                //        ant1.Text = "ant1 : " + reader.GetString(0);
                //    }
                //}
                //databasecmd.cmd.CommandText = "SELECT MAX(TIME(time)) as time FROM checkpoint WHERE ant_id = 2";
                //using (MySqlDataReader reader = databasecmd.cmd.ExecuteReader())
                //{
                //    if (reader.Read())
                //    {
                //        ant2.Text = "ant2 : " + reader.GetString(0);
                //    }
                //}
                //databasecmd.cmd.CommandText = "SELECT MAX(TIME(time)) as time FROM checkpoint WHERE ant_id = 3";
                //using (MySqlDataReader reader = databasecmd.cmd.ExecuteReader())
                //{
                //    if (reader.Read())
                //    {
                //        ant3.Text = "ant3 : " + reader.GetString(0);
                //    }
                //}
                //databasecmd.cmd.CommandText = "SELECT MAX(TIME(time)) as time FROM checkpoint WHERE ant_id = 4";
                //using (MySqlDataReader reader = databasecmd.cmd.ExecuteReader())
                //{
                //    if (reader.Read())
                //    {
                //        ant4.Text = "ant4 : " + reader.GetString(0);
                //    }
                //}
            }
            else
            {
                label6.Enabled = false;
                reset.Enabled = true;
                start.BackColor = Color.White;
                start.ForeColor = Color.Black;
                start.Text = "Start";
            }
        }

        private void reset_Click(object sender, EventArgs e)
        {
            isReset = !isReset;
            if (isReset)
            {
                reset.Enabled = false;
                dataGridView1.DataSource = new DataGridView();
                resetAnt();
            }
        }

        private void optionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ReceivedComData(object sender, SerialDataReceivedEventArgs e)
        {
            
            try
            {
                int nCount = iSerialPort.BytesToRead;
                label6.ForeColor = System.Drawing.Color.Green;
                if (nCount == 0)
                {
                    return;
                }

                byte[] btAryBuffer = new byte[nCount];
                iSerialPort.Read(btAryBuffer, 0, nCount);
                
                Console.Write(ByteArrayToString(btAryBuffer, 0, btAryBuffer.Length));
                Console.WriteLine("");
                //RunReceiveDataCallback(btAryBuffer);
                label6.ForeColor = System.Drawing.Color.Black;
            }
            catch (System.Exception ex)

            {

            }
        }
        public static string ByteArrayToString(byte[] btAryHex, int nIndex, int nLen)
        {
            if (nIndex + nLen > btAryHex.Length)
            {
                nLen = btAryHex.Length - nIndex;
            }

            string strResult = string.Empty;

            for (int nloop = nIndex + 7; nloop < nIndex + nLen - 2; nloop++)
            {
                string strTemp = string.Format(" {0:X2}", btAryHex[nloop]);

                strResult += strTemp;
            }

            return strResult;
        }

        public int OpenCom(string strPort, int nBaudrate, out string strException)
        {
            strException = string.Empty;

            if (iSerialPort.IsOpen)
            {
                iSerialPort.Close();
            }

            try
            {
                iSerialPort.PortName = strPort;
                iSerialPort.BaudRate = nBaudrate;
                iSerialPort.ReadTimeout = 200;
                iSerialPort.Open();
            }
            catch (System.Exception ex)
            {
                strException = ex.Message;
                return -1;
            }

            m_nType = 0;
            return 0;
        }

        
    }

}
