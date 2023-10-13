using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.IO;
using System.Security.Cryptography;

namespace TCPChatClientServerGUI
{
    public partial class FrmServer : Form
    {
        public FrmServer()
        {
            InitializeComponent();
        }
        
        //Khai bao 2 sockets
        Socket sckServer, sckClient;
        //sckServer: cho ket noi den tu client
        //sckClient: truyen nhan du lieu voi client
        private void butKhoitao_Click(object sender, EventArgs e)
        {
            //Tao socket
            sckServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind, Listen
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, (int)numServerPort.Value);
            sckServer.Bind(ep);
            sckServer.Listen(5);
            //Accept bat dong bo
            sckServer.BeginAccept(new AsyncCallback(xulyketnoi), null);
            lbTrangThai.Text = "Dang cho ket noi ....";
        }
        void xulyketnoi(IAsyncResult result)
        {
            sckClient = sckServer.EndAccept(result);
            //cap nhat trang thai
            lbTrangThai.Invoke(new CapNhatGiaoDien(CapNhatTrangThai), new object[] { "Ket noi thanh cong." });
            //Bat dau nhan du lieu
            sckClient.BeginReceive(data,0,1024,SocketFlags.None, new AsyncCallback(xulydulieunhanduoc),null);
        }
        //Khai bao bo dem de nhan du lieu
        byte[] data = new byte[1024];
        void xulydulieunhanduoc(IAsyncResult result)
        {
            //Goi ham EndReceive
            int size = sckClient.EndReceive(result);
            //Xu ly du lieu nhan duoc trong data[]
            String thongdiep = Encoding.ASCII.GetString(data, 0, size);
            string uppercaseThongdiep = thongdiep.ToUpper();
            string mahoa = EncryptionHelper.Encrypt(thongdiep);
            string giaimahoa = EncryptionHelper.Decrypt(mahoa);
            string translatethongdiep = TranslateText(giaimahoa);


            //Chen thong diep vao textbox noidungchat
            txtNoidungChat.Invoke(new CapNhatGiaoDien(CapNhatNoiDungChat), new object[] { "Client: " +translatethongdiep });
            //Cho nhan tiep
            sckClient.BeginReceive(data, 0, 1024, SocketFlags.None, new AsyncCallback(xulydulieunhanduoc), null);
        }
        delegate void CapNhatGiaoDien(string s);
        public static string EncryptText(string input)
        {
            char[] chars = input.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }
        public static string DecryptText(string input)
        {
            // Đảo chuỗi một lần nữa để giải mã
            char[] chars = input.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }
        void CapNhatTrangThai(string s)
        {
            lbTrangThai.Text = s;
        }
        void CapNhatNoiDungChat(string s)
        {
            txtNoidungChat.Text += s + "\r\n";
        }

        private void butGui_Click(object sender, EventArgs e)
        {
            string originalMessage = txtThongdiep.Text;
            string encryptedMessage = EncryptionHelper.EncryptMessage(originalMessage);
            sckClient.Send(Encoding.ASCII.GetBytes(encryptedMessage));

            CapNhatNoiDungChat("Sever: " + originalMessage);
            txtThongdiep.Text = "";
        }

        private void txtNoidungChat_TextChanged(object sender, EventArgs e)
        {

        }
        public string TranslateText(string input)
        {
            string url = String.Format
            ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
             "vi", "en", Uri.EscapeUriString(input));
            HttpClient httpClient = new HttpClient();
            string result = httpClient.GetStringAsync(url).Result;
            var jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);
            var translationItems = jsonData[0];
            string translation = "";
            foreach (object item in translationItems)
            {
                IEnumerable translationLineObject = item as IEnumerable;
                IEnumerator translationLineString = translationLineObject.GetEnumerator();
                translationLineString.MoveNext();
                translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
            }
            if (translation.Length > 1) { translation = translation.Substring(1); };
            return translation;
        }
        private void txtThongdiep_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
            {
                //goi lai ham gui
                butGui_Click(null, null);
            }
        }
    }
    
}
