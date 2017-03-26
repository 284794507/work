using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SecurityTokens.Totp;

namespace SecurityTokens
{
    public partial class FrmAlterKey : Form
    {
        public FrmAlterKey()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            string key = txtNewKey.Text.Trim();
            if(string.IsNullOrEmpty(key))
            {
                MessageBox.Show("密钥不能为空，请重新输入！", "提示");
                return;
            }

            string time = txtTimeLen.Text.Trim();
            if (string.IsNullOrEmpty(key))
            {
                MessageBox.Show("有效时长不能为空，请重新输入！", "提示");
                return;
            }

            if(DialogResult.OK == MessageBox.Show("请问是否确定修改密钥，修改成功后相关验证可能会失效！", "提示", MessageBoxButtons.OKCancel))
            {
                Utility.CurKey = Base64.DecryptDES(key, Utility.PrivateKey);

                Utility.Interval = uint.Parse(time);
            }
           
        }

        private void txtTimeLen_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!(Char.IsNumber(e.KeyChar)) && e.KeyChar!=(char)8)
            {
                e.Handled = true;
            }
        }
    }
}
