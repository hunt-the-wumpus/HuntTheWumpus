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
using System;
//using System.Web.Helpers;

namespace HuntTheWumpus {

	class VKresult {
		public string server { get; set; }
		public string photo { get; set; }
		public string hash { get; set; }
	}

	class Photo {
		public int id { get; set; }
		public int album_id { get; set; }
		public int owner_id { get; set; }
		public int user_id { get; set; }
		public string text { get; set; }
		public int date { get; set; }
		public int width { get; set; }
		public int height { get; set; }
	}

	class VKApi {
		private WebBrowser wb = null;
		private string token = "";
		private string user = "";
		private string json = "";
		private const string uri = "http://alexsytsev2014.wix.com/huntthewumpus#!oauth/iu2or";
		private bool published = false;

		public void OauthAuthorize() {
			wb = new WebBrowser("https://oauth.vk.com/authorize?client_id=5407281&display=page&redirect_uri=http://oauth.vk.com/blank.html&scope=wall,photos&response_type=token&v=5.0&revoke=1");
			wb.Show();
		}

		private string ParseJsonFormat(string JsonString, string field, char ignored) {
			string luck = "";
			for (int i = JsonString.IndexOf(field) + field.Length; i < JsonString.IndexOf(',', JsonString.IndexOf(field) + field.Length); ++i) {
				if (JsonString[i] != '"' && JsonString[i] != '\\' && JsonString[i] != ',' && JsonString[i] != ignored) {
					luck += JsonString[i];
				}
			}
			return luck;
		}

		private string ParseJsonFormat(string JsonString, string field, string next_field) {
			string luck = "";
			for (int i = JsonString.IndexOf("\":\"", JsonString.IndexOf(field)) + ("\":\"").Length; i < JsonString.IndexOf(next_field, JsonString.IndexOf(field)); ++i) {
					luck += JsonString[i];
			}
			return luck;
		}

		public string SendVKApi(string send) {
			var WebRgetURL = WebRequest.Create(send);
			var objectStream = WebRgetURL.GetResponse().GetResponseStream();
			var objReader = new StreamReader(objectStream);
			json = objReader.ReadLine();
			return json;
		}

		public void Access_authorize() {
			if (wb != null && !published) {
				if (wb.access_token != "" && wb.user_id != "") {
					token = wb.access_token;
					user = wb.user_id;
					wb.Navigate(uri);
					PublicPhoto();
				}
			}
		}

		private string HttpUploadFile(string url, string file) {
			System.Net.ServicePointManager.Expect100Continue = false;
			WebClient uploadclient = new WebClient();
			string res = Encoding.ASCII.GetString(uploadclient.UploadFile(url, "POST", file));
			return res;
		}

		public void PublicPhoto() {
			string upload_url = SendVKApi("https://api.vk.com/method/photos.getWallUploadServer?user_id=" + user + "&access_token=" + token);
			string luck_url = ParseJsonFormat(upload_url, "\"upload_url\":", '~');
			WebClient client = new WebClient();
			string result = HttpUploadFile(luck_url, @"data/Share.jpg");
			VKresult res = JsonConvert.DeserializeObject<VKresult>(result);
			string photo = SendVKApi("https://api.vk.com/method/photos.saveWallPhoto?photo=" + res.photo + "&hash=" + res.hash + "&user_id=" + user + "&server=" + res.server + "&access_token=" + token + "&v=5.0");
			Photo p = JsonConvert.DeserializeObject<Photo>(photo);
			p.id = int.Parse(ParseJsonFormat(photo, "\"id\"", ':'));
			SendVKApi("https://api.vk.com/method/wall.post?owner_id=" + user + "&friends_only=0&attachments=photo" + user + "_" + p.id.ToString() + "&access_token=" + token);
			SendVKApi("https://api.vk.com/method/stats.trackVisitor");
			published = true;
			MessageBox.Show("Achievement public at your account!");
		}
	}
}
