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
using System.Web.Helpers;

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
		/*public string photo_75 { get; set; }
		public string photo_130 { get; set; }
		public string photo_604 { get; set; }
		public string photo_807 { get; set; }
		public string photo_1280 { get; set; }
		public string photo_2560 { get; set; }*/
	}

	class VKApi {
		private WebBrowser wb = null;
		private string token = "";
		private string user = "";
		private string json = "";

		public void OauthAutorize() {
			wb = new WebBrowser();
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
			if (wb != null) {
				if (wb.access_token != "" && wb.user_id != "") {
					token = wb.access_token;
					user = wb.user_id;
					wb.Close();
					wb = null;
					PublicPhoto("");
				}
			}
		}

		private string HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc) {
			string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
			byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

			HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
			wr.ContentType = "multipart/form-data; boundary=" + boundary;
			wr.Method = "POST";
			wr.KeepAlive = true;
			wr.Credentials = CredentialCache.DefaultCredentials;

			Stream rs = wr.GetRequestStream();

			string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
			foreach (string key in nvc.Keys) {
				rs.Write(boundarybytes, 0, boundarybytes.Length);
				string formitem = string.Format(formdataTemplate, key, nvc[key]);
				byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
				rs.Write(formitembytes, 0, formitembytes.Length);
			}
			rs.Write(boundarybytes, 0, boundarybytes.Length);

			string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
			string header = string.Format(headerTemplate, paramName, file, contentType);
			byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
			rs.Write(headerbytes, 0, headerbytes.Length);

			FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
			byte[] buffer = new byte[4096];
			int bytesRead = 0;
			while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0) {
				rs.Write(buffer, 0, bytesRead);
			}
			fileStream.Close();

			byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
			rs.Write(trailer, 0, trailer.Length);
			rs.Close();

			WebResponse wresp = null;
			try {
				wresp = wr.GetResponse();
				Stream stream2 = wresp.GetResponseStream();
				StreamReader reader2 = new StreamReader(stream2);
				string responce = reader2.ReadToEnd();
				return responce;
			}
			catch (Exception ex) {
				MessageBox.Show("Error uploading file " + ex.Message);
				if (wresp != null) {
					wresp.Close();
					wresp = null;
				}
				return "Exception " + ex.Message;
			}
			finally {
				wr = null;
			}
		}

		public void PublicPhoto(string PhotoAdress) {
			string upload_url = SendVKApi("https://api.vk.com/method/photos.getWallUploadServer?user_id=" + user + "&access_token=" + token);
			string luck_url = ParseJsonFormat(upload_url, "\"upload_url\":", '~');
			WebClient client = new WebClient();
			var bytes = new NameValueCollection();
			bytes.Add("Photo", "upload");
			string result = HttpUploadFile(luck_url, @"data/achievements/dich.jpg", "file", "image/jpeg", bytes);
			VKresult res = JsonConvert.DeserializeObject<VKresult>(result);
			string photo = SendVKApi("https://api.vk.com/method/photos.saveWallPhoto?photo=" + res.photo + "&hash=" + res.hash + "&user_id=" + user + "&server=" + res.server + "&access_token=" + token + "&v=5.0");
			Photo p = JsonConvert.DeserializeObject<Photo>(photo);
			p.id = int.Parse(ParseJsonFormat(photo, "\"id\"", ':'));
			SendVKApi("https://api.vk.com/method/wall.post?owner_id=" + user + "&friends_only=0&attachments=photo" + user + "_" + p.id.ToString() + "&access_token=" + token);
			MessageBox.Show("Photo public at your account");
		}
	}
}
