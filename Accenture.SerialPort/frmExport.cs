using BXHSerialPort;
using System;
using System.Data;
using System.Windows.Forms;



namespace Accenture.SerialPort
{
    public partial class frmExport : Form
    {
        public frmExport()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "当天数据")
            {
                //sql语句中存入你需要的导出数据的语句
                string sql = "select  MACID , MACKEY, MCUID , QECODE, BATCHNO, CONVERT(varchar(100), CREATEDON, 120) 'CREATEDON', CONVERT(varchar(100), EndDate, 120)'EndDate'  from dbo.YiBao where DATEDIFF(D, CREATEDON,GETDATE())=0";

                DataTable dt = DBHelper.GetDataTable(sql);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel(*.xls)|*.xls";
                sfd.FileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = sfd.FileName;
                    PublicMethod.OutToExcelFromDataTable(fileName, dt, true);
                }
            }
            else if (comboBox1.Text == "前一天的数据")
            {
                //sql语句中存入你需要的导出数据的语句
                string sql = "select  MACID , MACKEY, MCUID , QECODE, BATCHNO, CONVERT(varchar(100), CREATEDON, 120) 'CREATEDON', CONVERT(varchar(100), EndDate, 120)'EndDate'  from dbo.YiBao where DATEDIFF(D, CREATEDON,GETDATE())=1";
                DataTable dt = DBHelper.GetDataTable(sql);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel(*.xls)|*.xls";
                sfd.FileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = sfd.FileName;
                    PublicMethod.OutToExcelFromDataTable(fileName, dt, true);
                }
            }

            else if (comboBox1.Text == "本周数据")
            {
                //sql语句中存入你需要的导出数据的语句
                string sql = @"select  MACID , MACKEY, MCUID , QECODE, BATCHNO, CONVERT(varchar(100), CREATEDON, 120) 'CREATEDON', CONVERT(varchar(100), EndDate, 120)'EndDate'  from dbo.YiBao where DATEDIFF(week, CREATEDON,GETDATE())=0; ";
                DataTable dt = DBHelper.GetDataTable(sql);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel(*.xls)|*.xls";
                sfd.FileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = sfd.FileName;
                    PublicMethod.OutToExcelFromDataTable(fileName, dt, true);

                    DialogResult = System.Windows.Forms.DialogResult.OK;
                }
            }
            else
                 if (comboBox1.Text == "当月数据")
            {
                //sql语句中存入你需要的导出数据的语句
                string sql = @"select  MACID , MACKEY, MCUID , QECODE, BATCHNO, CONVERT(varchar(100), CREATEDON, 120) 'CREATEDON', CONVERT(varchar(100), EndDate, 120)'EndDate'  from dbo.YiBao where DATEDIFF(M, CREATEDON,GETDATE())=0; ";
                DataTable dt = DBHelper.GetDataTable(sql);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel(*.xls)|*.xls";
                sfd.FileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = sfd.FileName;
                    PublicMethod.OutToExcelFromDataTable(fileName, dt, true);

                    DialogResult = System.Windows.Forms.DialogResult.OK;
                }
            }
            else if (comboBox1.Text == "上月数据")
            {
                //sql语句中存入你需要的导出数据的语句
                string sql =
                    @"select  MACID , MACKEY, MCUID , QECODE, BATCHNO, CONVERT(varchar(100), CREATEDON, 120) 'CREATEDON', CONVERT(varchar(100), EndDate, 120)'EndDate'   from dbo.YiBao where DATEDIFF(M, CREATEDON,GETDATE())=1 ";
                DataTable dt = DBHelper.GetDataTable(sql);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel(*.xls)|*.xls";
                sfd.FileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = sfd.FileName;
                    PublicMethod.OutToExcelFromDataTable(fileName, dt, true);

                    DialogResult = System.Windows.Forms.DialogResult.OK;
                }
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "cstr.1997")
            {
                this.btnOK.Enabled = true;
                this.comboBox1.Enabled = true;
                // todo:查询可以导出的数据过滤条件
            }
        }


    }
}
