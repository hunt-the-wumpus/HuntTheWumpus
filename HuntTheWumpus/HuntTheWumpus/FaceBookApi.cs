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
using System.Configuration;

namespace HuntTheWumpus {
	class FaceBookApi {

		private WebBrowser wb;
		private string code = "";
		private string access_token = "";
		private const string client_id = "249609735389185";
		private const string client_secret = "c15de867a49ab5f238eed9a2b2f31826";
		private const string uri = "http://alexsytsev2014.wix.com/huntthewumpus";

		public void OauthAuthorize() {
			//wb = new WebBrowser("https://www.facebook.com/dialog/oauth?client_id=" + client_id + "&redirect_uri=" + uri + "&response_type=code");
			wb = new WebBrowser("https://www.facebook.com/dialog/oauth?client_id=" + client_id + "&client_secret=" + client_secret + "&redirect_uri=" + uri + "&responce_type=token");
			wb.Show();
		}

		private string RequestResponse(string pUrl) {
			HttpWebRequest webRequest = System.Net.WebRequest.Create(pUrl) as HttpWebRequest;
			webRequest.Method = "GET";
			webRequest.ServicePoint.Expect100Continue = false;
			webRequest.Timeout = 20000;

			Stream responseStream = null;
			StreamReader responseReader = null;
			string responseData = "";
			try {
				WebResponse webResponse = webRequest.GetResponse();
				responseStream = webResponse.GetResponseStream();
				responseReader = new StreamReader(responseStream);
				responseData = responseReader.ReadToEnd();
			}
			catch (Exception exc) {
				MessageBox.Show("ERROR : " + exc.Message);
			}
			finally {
				if (responseStream != null) {
					responseStream.Close();
					responseReader.Close();
				}
			}

			return responseData;
		}

		private string ParseValue(string mainurl, string field) {
			string url = mainurl;
			url = url.Replace('#', '=');
			url = url.Replace('?', '=');
			url = url.Replace('/', '=');
			url = url.Replace('&', '=');
			string[] parts = url.Split('=');
			for (int i = 0; i < parts.Length; ++i) {
				if (parts[i] == field) {
					return parts[i + 1];
				}
			}
			return "";
		}

		public void Access_code() {
			/*if (code == "") {
				if (wb.code != "") {
					code = wb.code;
					MessageBox.Show("Your code is " + code);
					var responseData = RequestResponse("https://graph.facebook.com/oauth/access_token?debug=all&client_id=" + client_id + "&client_secret=" + client_secret + "&redirect_uri=" + uri + "/&code=" + code + "&responce_type=id");
					if (responseData == "") {
						return;
					}
					MessageBox.Show("result getted is " + responseData);
					access_token = ParseValue(responseData, "access_token");
					wb = null;
					MessageBox.Show("Your token is " + access_token);
					MessageBox.Show(SendFaceBook("https://graph.facebook.com/me/feed?debug=all&message=123&access_token=" + access_token));
				}
			}*/
			if (access_token == "") {
				if (wb.access_token != "") {
					access_token = wb.access_token;
					MessageBox.Show("Your token is " + access_token);
					HttpPost("https://graph.facebook.com/me/feed?access_token=" + access_token, "message=123&link=google.ru");
					//MessageBox.Show(SendFaceBook("https://graph.facebook.com/me/feed?debug=all&message=123&access_token=" + access_token));
				}
			}
		}

		private string SendFaceBook(string send) {
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(send);
			request.Method = "GET";
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			StreamReader reader = new StreamReader(response.GetResponseStream());
			string result = reader.ReadToEnd();
			response.Close();
			return result;
		}

		private string HttpPost(string pUrl, string pPostData) {
			HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(pUrl);
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.Method = "POST";
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(pPostData);
			Stream requestWriter = webRequest.GetRequestStream(); //GetRequestStream
			requestWriter.Write(bytes, 0, bytes.Length);
			requestWriter.Close();

			Stream responseStream = null;
			StreamReader responseReader = null;
			string responseData = "";
			try {
				WebResponse webResponse = webRequest.GetResponse();
				responseStream = webResponse.GetResponseStream();
				responseReader = new StreamReader(responseStream);
				responseData = responseReader.ReadToEnd();
			}
			catch (Exception exc) {
				throw new Exception("could not post : " + exc.Message);
			}
			finally {
				if (responseStream != null) {
					responseStream.Close();
					responseReader.Close();
				}
			}

			return responseData;
		}
	}
}
