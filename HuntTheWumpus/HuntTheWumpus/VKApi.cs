using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace HuntTheWumpus {
	class VKApi {
		private WebBrowser wb = null;
		private string token = "";
		private string user = "";
		private string json = "";

		public void OauthAutorize() {
			wb = new WebBrowser();
			wb.Show();
		}

		private string ParseJsonFormat(string JsonString, string field) {
			string luck = "";
			for (int i = JsonString.IndexOf(field) + field.Length; i < JsonString.IndexOf(',', JsonString.IndexOf(field) + field.Length); ++i) {
				if (JsonString[i] != '"' && JsonString[i] != '\\' && JsonString[i] != ',') {
					luck += JsonString[i];
				}
			}
			return luck;
		}

		public string SendVKApi(string send) {
			MessageBox.Show(send);
			var WebRgetURL = WebRequest.Create(send);
			var objectStream = WebRgetURL.GetResponse().GetResponseStream();
			var objReader = new StreamReader(objectStream);
			json = objReader.ReadLine();
			MessageBox.Show(json);
			return json;
		}

		public void Access_authorize() {
			if (wb != null) {
				if (wb.access_token != "" && wb.user_id != "") {
					token = wb.access_token;
					user = wb.user_id;
					wb.Close();
					wb = null;
					//PublicPhoto("data/Achievements/Bottle.png");
				}
			}
		}

		public void PublicPhoto(string PhotoAdress) {
			string upload_url = SendVKApi("https://api.vk.com/method/photos.getWallUploadServer?user_id=" + user + "&access_token=" + token);
			MessageBox.Show(upload_url);
			string luck_url = ParseJsonFormat(upload_url, "\"upload_url\":");
			MessageBox.Show(luck_url);
			WebClient client = new WebClient();
			var bytes = new NameValueCollection();
			//HttpWebRequest rq = (HttpWebRequest)WebRequest.Create(luck_url);
			/*using (Stream s = rq.GetRequestStream()) {
				s.Write()
			}
			bytes.Add("photo", );*/
			//var responce = client.UploadValues(luck_url, );
		}
	}
}
