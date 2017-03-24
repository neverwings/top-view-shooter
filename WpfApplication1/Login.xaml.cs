using System.Windows;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System;

namespace WpfApplication1
{
	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login: Window
	{
		public Login()
		{
			InitializeComponent();
		}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Window1 window1 = new Window1(UsernameTxtbox.Text);
            string URL = "localhost:8000";
            string ifSuccess = HTTP.GETfromServer("type=player&subtype=add&username=" + UsernameTxtbox.Text + "&ipaddress="+URL);//+HTTP.getIP()+":8080");
            if (ifSuccess == "ok")
            {
                new Window1(UsernameTxtbox.Text, URL);
                //window1.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("An error has occured while logging in.");
            }
        }


	}
}
