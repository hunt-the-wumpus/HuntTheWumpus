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

		public WebBrowser() {
			InitializeComponent();
		}

		private void WebBrowser_Load(object sender, EventArgs e) {
			access_token = "";
			user_id = "";
			webBrowser1.Navigate("https://oauth.vk.com/authorize?client_id=5407281&display=page&redirect_uri=http://oauth.vk.com/blank.html&scope=wall,photos&response_type=token&v=5.45&revoke=1");
		}

		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
			if (e.Url.ToString().IndexOf("access_token") != -1) {
				string url = e.Url.ToString();
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
				MessageBox.Show("Авторизация пройдена! Можете закрыть окно браузера");
				access_token = access_token_luck;
				user_id = luck_user_id;
			}
		}
	}
}
