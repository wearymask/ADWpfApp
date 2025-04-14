using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.DirectoryServices.Protocols;
using System.Net;

namespace ADWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;
            string server = txtServer.Text.Trim();
            int port = int.TryParse(txtPort.Text.Trim(), out int p) ? p : (chkLDAPS.IsChecked == true ? 636 : 389);
            bool useLDAPS = chkLDAPS.IsChecked == true;

            txtResult.Text = "🔄 正在驗證中...";

            try
            {
                var identifier = new LdapDirectoryIdentifier(server, port);
                var credential = new NetworkCredential(username, password);
                using (var connection = new LdapConnection(identifier, credential))
                {
                    connection.AuthType = AuthType.Negotiate;

                    if (useLDAPS)
                    {
                        connection.SessionOptions.SecureSocketLayer = true;
                        connection.SessionOptions.VerifyServerCertificate += (conn, cert) => true; // 如需跳過憑證驗證
                    }

                    connection.Bind();
                    txtResult.Text = "✅ 驗證成功";
                }
            }
            catch (LdapException ex)
            {
                txtResult.Text = $"❌ 驗證失敗：{ex.Message}\n\n【詳細錯誤】\n{ex.ServerErrorMessage}";
            }
            catch (Exception ex)
            {
                txtResult.Text = $"❌ 發生未知錯誤：{ex.Message}";
            }
        }

        private void chkLDAPS_Checked(object sender, RoutedEventArgs e)
        {
            txtPort.Text = "636";
        }

        private void chkLDAPS_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPort.Text = "389";
        }
    }
}