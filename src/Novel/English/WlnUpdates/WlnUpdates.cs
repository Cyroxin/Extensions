using Extensions.template;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Extensions.src.Novel.WlnUpdates
{
	public class WlnUpdates : Extension
	{
		static HttpClient client = new HttpClient();

		static string apiUrl = "https://www.wlnupdates.com/api";

		public int version { get => 1; }

		public WlnUpdates()
		{
			client.DefaultRequestHeaders.Host = "www.wlnupdates.com";
			//this.getDetail(3);
			//this.search();
		}

		async void getDetail(object id)
		{
			var json = new
			{
				mode = "get-series-id",
				id = id,
			};

			var resp = await Fetch(json);
			if (resp != string.Empty)
			{
				Console.WriteLine(resp);
			}
		}

		async void search()
		{
			string jsonString = "{\"mode\":\"get-translated-releases\"}";


			var resp = await Fetch(jsonString);
			if (resp != string.Empty)
			{
				Console.WriteLine(resp);
			}
		}

		static async Task<string> Fetch<T>(T json)
		{
			if (json == null) return string.Empty;


			if(typeof(T) == typeof(string))
			{
				var resp = await client.PostAsync(apiUrl, new StringContent(content: (string)(object)json, Encoding.UTF8, "application/json"));
				return resp.Content.ReadAsStringAsync().Result;
			}
			else 
			{
				var resp = await client.PostAsync(apiUrl, JsonContent.Create(json));
				return resp.Content.ReadAsStringAsync().Result;
			}

			
		}
	}


}
