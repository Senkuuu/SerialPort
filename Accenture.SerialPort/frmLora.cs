using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Wima.Log;
using Wima.Lora;
using Wima.Lora.Model;
using Wima.Lora.Entities;

namespace Accenture.SerialPort
{
    public partial class frmLora : Form
    {
        private ClientServer CS = null;
        private string devaddr = null;
        public frmLora()
        {
            InitializeComponent();
        }

        private void TsbStart_Click(object sender, EventArgs e)
        {
            CS = new ClientServer();
            CS.Start();

            tsbStart.Enabled = false;
            tsbStop.Enabled = true;
            //mainTimer.Enabled = true;

            MessageBox.Show("CS服务器启动成功！", "服务器启动", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
