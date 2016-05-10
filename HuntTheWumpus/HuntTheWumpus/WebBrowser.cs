using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HuntTheWumpus {
	public partial class WebBrowser : Form {

		public string access_token { get; private set; }
		public string user_id { get; private set; }
		private string address = "";
		public string code { get; private set; }

		public WebBrowser(string load_address) {
			access_token = "";
			address = load_address;
			InitializeComponent();
		}

		public void Navigate(string url) {
			webBrowser1.Navigate(url);
		}

		private void WebBrowser_Load(object sender, EventArgs e) {
			access_token = "";
			user_id = "";
			code = "";
			webBrowser1.Navigate(address);
		}

		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
			#region GetVK;
			if (e.Url.ToString().IndexOf("access_token") != -1) {
				string url = e.Url.ToString();
				if (url.IndexOf("facebook") != -1) {
					return;
				}
				url = url.Replace('#', '=');
				url = url.Replace('&', '=');
				url =  url.Replace('?', '=');
				string[] array = url.Split('=').ToArray<string>();
				string access_token_bad = "";
				string bad_user_id = "";
				for (int i = 0; i < array.Length; ++i) {
					if (array[i] == "access_token" || array[i] == "&access_token" || array[i] == "?access_token" || array[i] == "#access_token") {
						access_token_bad = array[i + 1];
					}
					if (array[i] == "user_id" || array[i] == "&user_id" || array[i] == "?user_id" || array[i] == "#user_id") {
						bad_user_id = array[i + 1];
					}
				}
				string access_token_luck = "";
				for (int i = 0; i < access_token_bad.Length; ++i) {
					char sumbol = access_token_bad[i];
					if (sumbol != '?' && sumbol != ',' && sumbol != '&') {
						access_token_luck += sumbol;
					} else {
						//break;
					}
				}
				string luck_user_id = "";
				for (int i = 0; i < bad_user_id.Length; ++i) {
					char sumbol = bad_user_id[i];
					if (sumbol != '?' && sumbol != ',' && sumbol != '&') {
						luck_user_id += sumbol;
					} else {
						//break;
					}
				}
				access_token = access_token_luck;
				user_id = luck_user_id;
				MessageBox.Show("Authorization success!");
			}
			#endregion
			#region GetFaceBook
			if (e.Url.ToString().IndexOf("code") != -1) {
				string urlbase = e.Url.ToString();
				if (urlbase.IndexOf("facebook") != -1) {
					return;
				}
				string url = e.Url.Query;
				string outed = "";
				bool state = false;
				for (int i = 0; i < url.Length; ++i) {
					if (state) {
						outed += url[i];
					} else {
						if (url[i] == '=') {
							state = true;
						}
					}
				}
				code = outed;
				MessageBox.Show("Authorization success!");
			}
			#endregion
		}
	}
}
