using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.IO;
using System.Security.Cryptography;

namespace TCPChatClientServerGUI
{
    public partial class FrmClient : Form
    {
        public FrmClient()
        {
            InitializeComponent();
        }

        Socket sckClient;
        private void butKetnoi_Click(object sender, EventArgs e)
        {
            //tao socket
            sckClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //connect
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(txtServerIP.Text), (int)numServerPort.Value);
            sckClient.BeginConnect(ep, new AsyncCallback(xulyketnoi),null);
        }
        void xulyketnoi(IAsyncResult result)
        {
            sckClient.EndConnect(result);
            //Cap nhat trang thai, va bat dau gui nhan du lieu
            //cap nhat trang thai
            lbTrangThai.Invoke(new CapNhatGiaoDien(CapNhatTrangThai), new object[] { "Ket noi thanh cong." });
            //Bat dau nhan du lieu
            sckClient.BeginReceive(data, 0, 1024, SocketFlags.None, new AsyncCallback(xulydulieunhanduoc), null);
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
            String translatethongdiep = TranslateText(giaimahoa);

            //Chen thong diep vao textbox noidungchat
            txtNoidungChat.Invoke(new CapNhatGiaoDien(CapNhatNoiDungChat), new object[] { "Server: " + translatethongdiep  });
            //Cho nhan tiep
            sckClient.BeginReceive(data, 0, 1024, SocketFlags.None, new AsyncCallback(xulydulieunhanduoc), null);
        }
        public string TranslateText(string input)
        {
            string url = String.Format
            ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
             "en", "vi", Uri.EscapeUriString(input));
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
        delegate void CapNhatGiaoDien(string s);
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

            CapNhatNoiDungChat("Client: " + originalMessage);
            txtThongdiep.Text = "";
        }

        private void txtThongdiep_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
            {
                //goi lai ham gui
                butGui_Click(null, null);
            }
        }

        private void txtNoidungChat_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
public class EncryptionHelper
{
    private const string EncryptionKey = "ThisIsASecretKey123"; // Key for encryption, you can change it

    public static string Encrypt(string clearText)
    {
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        return clearText;
    }

    public static string Decrypt(string cipherText)
    {
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;

    }
    public static string EncryptMessage(string clearText)
    {
        return Encrypt(clearText);
    }

    public static string DecryptMessage(string cipherText)
    {
        return Decrypt(cipherText);
    }
}
