using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;

namespace Dobby.ApiControl
{
    public class ApiController
    {
        private string _apiKey { get; set; }
        public ApiController(string apiKey)
        {
            _apiKey = apiKey;
        }
        public async Task<string> Search(string part, string q)
        {
            var service = new YouTubeService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                ApiKey = _apiKey,
                ApplicationName = "Dobby"
            });

            var searchListRequest = service.Search.List(part);
            searchListRequest.MaxResults = 1;
            searchListRequest.Type = "video";
            searchListRequest.Q = q;

            var searchListResponse = await searchListRequest.ExecuteAsync();
            string video = "";

            var searchResult = searchListResponse.Items[0];
            if (searchResult.Id.Kind.Equals("youtube#video"))
            { 
                video = $"{searchResult.Snippet.Title} ({searchResult.Id.VideoId})";
            }

            return video;
        }

    }

}
