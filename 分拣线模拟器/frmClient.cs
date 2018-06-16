using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HPSocketCS;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using OMH.WCS.Service;
using System.Text.RegularExpressions;

namespace OMH.WCS.simulator
{
    public enum AppState
    {
        Starting, Started, Stoping, Stoped, Error
    }

    public enum StudentType
    {
        None, Array, List, Single,
    }

    public partial class frmClient : Form
    {
        private AppState appState = AppState.Stoped;

        private delegate void ConnectUpdateUiDelegate();
        private delegate void SetAppStateDelegate(AppState state);
        private delegate void ShowMsg(string msg);
        private delegate void UpdataCountRec(int  count);
        private delegate void UpdataCountSend(int count);
        private ShowMsg AddMsgDelegate;
        HPSocketCS.TcpClient client = new HPSocketCS.TcpClient();
        private UpdataCountRec UpdataCountRecDelegate;
        private UpdataCountSend UpdataCountSendDelegate;

        public frmClient()
        {
            InitializeComponent();
        }

        private void frmClient_Load(object sender, EventArgs e)
        {
            OMH.WCS.Helper.ServiceBaiShi.ServiceStart();
            try
            {
                // 加个委托显示msg,因为on系列都是在工作线程中调用的,ui不允许直接操作
                AddMsgDelegate = new ShowMsg(AddMsg);
                UpdataCountRecDelegate = new UpdataCountRec(UpdataCountReclbl);
                // 设置client事件
                client.OnPrepareConnect += new TcpClientEvent.OnPrepareConnectEventHandler(OnPrepareConnect);
                client.OnConnect += new TcpClientEvent.OnConnectEventHandler(OnConnect);
                client.OnSend += new TcpClientEvent.OnSendEventHandler(OnSend);
                client.OnReceive += new TcpClientEvent.OnReceiveEventHandler(OnReceive);
                client.OnClose += new TcpClientEvent.OnCloseEventHandler(OnClose);

                //// 设置包头标识,与对端设置保证一致性
                //client.PackHeaderFlag = 0x3a;
                //// 设置最大封包大小
                //client.MaxPackSize = 0x1000;

                SetAppState(AppState.Stoped);
            }
            catch (Exception ex)
            {
                SetAppState(AppState.Error);
                AddMsg(ex.Message);
            }
            btnStart_Click(null,null);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                String ip = this.txtIpAddress.Text.Trim();
                ushort port = ushort.Parse(this.txtPort.Text.Trim());

                // 写在这个位置是上面可能会异常
                SetAppState(AppState.Starting);

                AddMsg(string.Format("$客户端链接中 ... -> ({0}:{1})", ip, port));

                if (client.Connect(ip, port, this.cbxAsyncConn.Checked))
                {
                    if (cbxAsyncConn.Checked == false)
                    {
                        SetAppState(AppState.Started);
                    }

                    AddMsg(string.Format("$客户端链接成功 -> ({0}:{1})", ip, port));
                }
                else
                {
                    SetAppState(AppState.Stoped);
                    throw new Exception(string.Format("$客户端链接失败 -> {0}({1})", client.ErrorMessage, client.ErrorCode));
                }
            }
            catch (Exception ex)
            {
                AddMsg(ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            
            // 停止服务
            AddMsg("$开始停止服务");
            if (client.Stop())
            {
                SetAppState(AppState.Stoped);
            }
            else
            {
                AddMsg(string.Format("$停止服务错误 -> {0}({1})", client.ErrorMessage, client.ErrorCode));
            }
        }

        byte[] Gbytes = new byte[25] ;
        int xiaocheCount = 0;
        int quanshuCount = 0;
        int gongbaotaiCount = 0;
        int saomiaoyiCount = 0;
        string billcodepart1 = "100000000";
        int billcodepart2 = 0;
        string billcode = "";
        string tip = "";  //基础参数汇报
        int sendCount = 0;
        int recCount = 0;
        private void btnSend_Click(object sender, EventArgs e)
        {
            sendCount++;
            Gbytes = new byte[25];
            xiaocheCount++;
            if (xiaocheCount > 256) xiaocheCount = 1;

            quanshuCount++;
            if (quanshuCount > 3) quanshuCount = 1;

            gongbaotaiCount++;
            if (gongbaotaiCount > 9) gongbaotaiCount = 1;

            saomiaoyiCount++;
            if (saomiaoyiCount > 2) saomiaoyiCount = 1;

            billcodepart2++;
            if (billcodepart2 > 99999) billcodepart2 = 1;
            billcode = billcodepart1 + billcodepart2.ToString("00000");

            tip = "-"+"小车:" + xiaocheCount.ToString() + "-" + "圈数:" + quanshuCount.ToString() + "-" + "供包台:" + gongbaotaiCount.ToString() + "-" + "顶扫ID:" + saomiaoyiCount.ToString();

            

            try
            {
                Gbytes[0] = 0x00;    //小车高位
                Gbytes[1] = (byte)Convert.ToInt16(xiaocheCount);  //循环  小车低位
                Gbytes[2] = (byte)Convert.ToInt16(quanshuCount);  //1-3  圈数
                Gbytes[3] = (byte)Convert.ToInt16(gongbaotaiCount);    //1-9  供包台
                Gbytes[4] = (byte)Convert.ToInt16(saomiaoyiCount);   //扫描仪 1-2
                Gbytes[5] = 0x00;   //重量
                Gbytes[6] = 0x00;   //重量
                Gbytes[7] = 0x00;   //重量
                Gbytes[8] = 0x00;   //重量
                Gbytes[9] = 0x00;   //重量
                Gbytes[10] = 0x7c;   //分隔符
                //11-24  单号
                byte[] bytes1 = Encoding.Default.GetBytes(billcode);
                for (int i = 0; i <= 13; i++)
                {
                    //Gbytes[11 + i] = (byte)Convert.ToInt16( bytes1[i].ToString("x"));
                    //Gbytes[11 + i] = (byte)Convert.ToInt16(Convert.ToString(bytes1[i], 16));
                    Gbytes[11 + i] = bytes1[i];
                }

                IntPtr connId = client.ConnectionId;
                Gbytes = CommonService.MakePLCMessage(Gbytes);
                // 发送
                if (client.Send(Gbytes, Gbytes.Length))
                {
                    AddMsg(string.Format("$ ({0}) 发送成功 --> {1}", connId, billcode + tip+ "-" + DateTime.Now.ToShortTimeString()));

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < Gbytes.Length; i++)
                    {
                        sb.Append(" " + Gbytes[i].ToString("x").PadLeft(2, '0'));
                    }
                    AddMsg(sb.ToString());
                    int temp = Regex.Matches(sb.ToString(), @"0d 0a").Count;
                    lblSend.Text = sendCount.ToString();
                }
                else
                {
                    AddMsg(string.Format("$ ({0}) 发送失败 --> {1} ({2})", connId, billcode, Gbytes.Length));
                }

            }
            catch (Exception ex)
            {
                AddMsg(string.Format("$ 发送失败 -->  错误信息 ({0})", ex.Message));
            }

        }

        private void lbxMsg_KeyPress(object sender, KeyPressEventArgs e)
        {

            // 清理listbox
            if (e.KeyChar == 'c' || e.KeyChar == 'C')
            {
                this.lbxMsg.Items.Clear();
                Gbytes = new byte[25];
                 xiaocheCount = 0;
                 quanshuCount = 0;
                 gongbaotaiCount = 0;
                 saomiaoyiCount = 0;
                 billcodepart1 = "100000000";
                 billcodepart2 = 0;
                 billcode = "";
                 tip = "";  //基础参数汇报
                 sendCount = 0;
                 recCount = 0;
                lblRec.Text = "0";
                lblSend.Text = "0";
            }
        }

        void ConnectUpdateUi()
        {
            if (this.cbxAsyncConn.Checked == true)
            {
                SetAppState(AppState.Started);
            }
        }

        HandleResult OnPrepareConnect(TcpClient sender, IntPtr socket)
        {
            return HandleResult.Ok;
        }

        HandleResult OnConnect(TcpClient sender)
        {
            // 已连接 到达一次

            // 如果是异步联接,更新界面状态
            this.Invoke(new ConnectUpdateUiDelegate(ConnectUpdateUi));

            AddMsg(string.Format(" > [{0},链接成功]", sender.ConnectionId));

            return HandleResult.Ok;
        }

        HandleResult OnSend(TcpClient sender, byte[] bytes)
        {
            // 客户端发数据了
            AddMsg(string.Format(" > [{0},发送数据] -> ({1} bytes)", sender.ConnectionId, bytes.Length));

            return HandleResult.Ok;
        }

        HandleResult OnReceive(TcpClient sender, byte[] bytes)
        {
            // 数据到达了
            //recCount++;
            AddMsg(string.Format(" > [{0},接收数据] -> ({1} bytes)", sender.ConnectionId, bytes.Length));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(" " + bytes[i].ToString("x").PadLeft(2, '0'));
            }
            AddMsg(sb.ToString());

            int temp = Regex.Matches(sb.ToString(), @"0d 0a").Count;
            recCount = recCount + temp;

            UpdataCountReclbl(recCount);
            //lblRec.Text = recCount.ToString();
            return HandleResult.Ok;
        }




        HandleResult OnClose(TcpClient sender, SocketOperation enOperation, int errorCode)
        {
            if (errorCode == 0)
                // 连接关闭了
                AddMsg(string.Format(" > [{0},关闭链接]", sender.ConnectionId));
            else
                // 出错了
                AddMsg(string.Format(" > [{0},错误信息] -> OP:{1},错误编号:{2}", sender.ConnectionId, enOperation, errorCode));

            // 通知界面,只处理了连接错误,也没进行是不是连接错误的判断,所以有错误就会设置界面
            // 生产环境请自己控制
            this.Invoke(new SetAppStateDelegate(SetAppState), AppState.Stoped);

            return HandleResult.Ok;
        }

        /// <summary>
        /// 设置程序状态
        /// </summary>
        /// <param name="state"></param>
        void SetAppState(AppState state)
        {
            appState = state;
            this.btnStart.Enabled = (appState == AppState.Stoped);
            this.btnStop.Enabled = (appState == AppState.Started);
            this.txtIpAddress.Enabled = (appState == AppState.Stoped);
            this.txtPort.Enabled = (appState == AppState.Stoped);
            this.cbxAsyncConn.Enabled = (appState == AppState.Stoped);
            this.btnSend.Enabled = (appState == AppState.Started);
        }

        /// <summary>
        /// 往listbox加一条项目
        /// </summary>
        /// <param name="msg"></param>
        void AddMsg(string msg)
        {
            if (this.lbxMsg.InvokeRequired)
            {
                // 很帅的调自己
                this.lbxMsg.Invoke(AddMsgDelegate, msg);
            }
            else
            {
                if (this.lbxMsg.Items.Count > 100)
                {
                    this.lbxMsg.Items.RemoveAt(0);
                }
                this.lbxMsg.Items.Add(msg);
                this.lbxMsg.TopIndex = this.lbxMsg.Items.Count - (int)(this.lbxMsg.Height / this.lbxMsg.ItemHeight);
            }
        }

        /// <summary>
        /// 往listbox加一条项目
        /// </summary>
        /// <param name="msg"></param>
        void UpdataCountReclbl(int count)
        {
            if (this.lblRec.InvokeRequired)
            {
                // 很帅的调自己
                this.lbxMsg.Invoke(UpdataCountRecDelegate, count);
            }
            else
            {
                this.lblRec.Text = count.ToString();
            }
        }

        /// <summary>
        /// 往listbox加一条项目
        /// </summary>
        /// <param name="msg"></param>
        void UpdataCountSendlbl(int count)
        {
            if (this.lblSend.InvokeRequired)
            {
                // 很帅的调自己
                this.lbxMsg.Invoke(UpdataCountSendDelegate, count);
            }
            else
            {
                this.lblRec.Text = count.ToString();
            }
        }



        private void frmClient_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (client != null)
            {
                client.Destroy();
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            int temp = 0;
            if (!int.TryParse(txtTimerInterval.Text, out temp))
            {
                MessageBox.Show("timer时间错误!");
            }
            timerTest.Interval = temp;
            timerTest.Start();


        }

        private void timerTest_Tick(object sender, EventArgs e)
        {
            btnSend_Click(null, null);


            //try
            //{
            //    string send = this.txtSend.Text;
            //    if (send.Length == 0)
            //    {
            //        return;
            //    }

            //    byte[] bytes = Encoding.Default.GetBytes(send);

            //    bytes = CommonService.MakePLCMessage(bytes);

            //    IntPtr connId = client.ConnectionId;

            //    // 发送
            //    if (client.Send(bytes, bytes.Length))
            //    {
            //        AddMsg(string.Format("$ ({0}) 发送成功 --> {1}", connId, send));
            //    }
            //    else
            //    {
            //        AddMsg(string.Format("$ ({0}) 发送失败 --> {1} ({2})", connId, send, bytes.Length));
            //    }

            //}
            //catch (Exception ex)
            //{
            //    AddMsg(string.Format("$ Send Fail -->  msg ({0})", ex.Message));
            //}
        }

        private void btnTimerStop_Click(object sender, EventArgs e)
        {
            if (timerTest.Enabled) timerTest.Stop();
        }
    }
}


//3a 1d 11 76 01 00 a3 00 08 02 30 30 30 30 30 7c 37 30 37 33 32 33 38 39 37 31 34 35 30 30 79 0d 0a
