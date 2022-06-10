using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using Extensions.Templates.Visual.Shared;


namespace Extensions.Src.Novel.WlnUpdates
{
	public class WlnUpdates : IVisualExtension
	{
		public static HttpClient client = new HttpClient();

		public static string apiUrl = "https://www.wlnupdates.com/api";

		public int version { get => 1; }

		static WlnUpdates()
		{
			client.DefaultRequestHeaders.Host = "www.wlnupdates.com";
		}

		public Task<IVisual[]> SearchVisuals(string name)
		{
			throw new NotImplementedException();
		}

		public async Task<IVisual[]> GetVisuals()
		{
			List<WlnUpdatesNovel> novels = new List<WlnUpdatesNovel>();
			// Create request
			string request = "{\"mode\":\"search-advanced\",\"series-type\":{\"Translated\":\"included\"},\"include-results\":[\"covers\"]}";

			// Request content
			var resp = await Fetch(request);

			// Parse response JSON
			try
			{
				/*
					return await JsonSerializer.DeserializeAsync<User>(
										resp,
										new System.Text.Json.JsonSerializerOptions 
										{ 
											DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
											PropertyNameCaseInsensitive = true 
										});*/
				// {"Topic":"Json Serialization Part 1","Part":1,"Author":"Marc","Co-Author":"Helen","Keywords":["json","netcore","parsing"]}
				var json = JsonDocument.Parse(resp);
				var root = json.RootElement;
				var rootproperties = root.EnumerateObject(); // data, error, message
				var data = rootproperties.FirstOrDefault((p) => p.Name == "data");

				if (data.Value.ValueKind != JsonValueKind.Undefined)
				{

					foreach (var item in data.Value.EnumerateArray())
					{

						var properties = item.EnumerateObject();

						/*
						"chapter": 2939.0,
						"fragment": 0.0,
						"postfix": "",
						"published": "Fri, 03 Jun 2022 10:00:32 GMT",
						"series": {
							"id": 51731,
							"name": "Invincible Conqueror"
						},
						"srcurl": "https://www.wuxiaworld.com/novel/invincible/inv-chapter-2939",
						"tlgroup": {
							"id": 19,
							"name": "Wuxiaworld"
						},
						"volume": null
						 */
						string id = properties.FirstOrDefault(p => p.Name == "id" && p.Value.ValueKind != JsonValueKind.Undefined).Value.ToString();
						string title = properties.FirstOrDefault(p => p.Name == "title" && p.Value.ValueKind != JsonValueKind.Undefined).Value.ToString();
						string coverurl = "";

						var covers = properties.FirstOrDefault(p => p.Name == "covers").Value.EnumerateArray();
						if (covers.Count() > 0)
						{
							var cover = covers.FirstOrDefault().EnumerateObject();
							coverurl = cover
							.FirstOrDefault
							(
								p => p.Name == "url"
							).Value.ToString();
						}

						novels.Add(new WlnUpdatesNovel { Id = id, Name = title, Cover = coverurl == "" ? default(Uri) : new Uri(coverurl) });
					}


				}

				/*
				// Find all authors, returns enumerable with "Marc", "Helen"
				var authors = blogPost.RootElement.EnumerateObject()
								   .Where(it => it.Name.Contains("Author") && it.Value.ValueKind == JsonValueKind.String);

				// Find all keywords, returns enumerable with "json", "netcore", "parsing"
				var keywords = blogPost.RootElement.EnumerateObject()
								  .Where(it => it.Value.ValueKind == JsonValueKind.Array && it.Name == "Keywords")
								  .SelectMany(it => it.Value.EnumerateArray().Select(that => that.GetString()));*/
			}
			catch (JsonException) // Invalid JSON
			{
				Console.WriteLine("Invalid JSON.");
			}

			return novels.ToArray();
		}

		static async Task<Stream> Fetch(string jsonparam)
		{
			try
			{
				var resp = await client.PostAsync(apiUrl, new StringContent(content: jsonparam, Encoding.UTF8, "application/json"));
				return await resp.Content.ReadAsStreamAsync();

			}
			catch (Exception err)
			{
				return default(Stream);
			}


		}
	}

	public class WlnUpdatesNovel : IVisual
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public Uri Cover { get; set; }
	}


}
