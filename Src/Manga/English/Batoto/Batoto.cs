using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using Extensions.Templates.Visual.Shared;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using AngleSharp.Html.Dom;
using AngleSharp.Io.Network;

namespace Extensions.Src.Manga.Batoto
{
	public class Batoto : IVisualExtension
	{
		public static string baseurl = "https://bato.to/";

		public static HttpClient client = new HttpClient();

		public int version { get => 1; }

		static Batoto()
		{
			var useragent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.54 Mobile Safari/537.36 Edg/101.0.1210.39";
			client.DefaultRequestHeaders.Add("User-Agent", useragent);
		}

		public async Task<IVisual[]> GetVisuals()
		{
			List<BatotoManga> manga = new List<BatotoManga>();

			var config = Configuration.Default
			.With(new HttpClientRequester())
			.WithDefaultLoader()
			.WithJs();

			var context = BrowsingContext.New(config);
			IDocument document = await context.OpenAsync(baseurl + "browse?langs=en,en_us").WaitUntilAvailable();
			var lists = document.All.Where(e => (e.Id?.Contains("series") ?? false) || (e.Id?.Contains("list") ?? false));
			var list = lists.First();

			foreach (var visual in list.Children)
			{
				var cover = visual.GetNodes<IHtmlImageElement>().FirstOrDefault()?.Source;
				var title = visual.GetNodes<IHtmlElement>(true, x => x.ClassName.Contains("title") && !string.IsNullOrEmpty(x.TextContent)).FirstOrDefault().TextContent;
				var id = visual.GetNodes<IHtmlAnchorElement>().FirstOrDefault()?.Href;
				manga.Add(new BatotoManga() { Cover = new Uri(cover), Name = title, Id = id });

				// Also possible to get:
				// Latest episode
				// Genres
				// CurrentPage, LastPage, NextPage

				Console.WriteLine(visual);
			}


			return manga.ToArray();
		}


	}

	public class BatotoManga : IVisual
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public Uri Cover { get; set; }
	}


}
