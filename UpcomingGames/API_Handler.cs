using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using UpcomingGames.Controllers;
using UpcomingGames.Models;

namespace UpcomingGames
{
    public class API_Handler
    {
        public List<GameModel> gamerequest = new List<GameModel>();
        public List<GameModel> GetGameList()
        {
            return gamerequest;
        }

        /// <summary>
        /// Sends a GET request to the specified URL, containing the endpoint specified in the parameter,
        /// then it runs a while loop where it creates instances of Gamemodel and sends data with them.
        /// </summary>
        /// <param name="endpoint">The desired endpoint to get data from</param>
        /// <returns>List task</returns>
        public async Task<List<UpcomingGames.Models.GameModel>> GetThisMonthGames()
        {
            List<UpcomingGames.Models.GameModel> games = new List<Models.GameModel>();
            string mycontent = await SendStringRequest("https://api-v3.igdb.com/games/", $"fields id,first_release_date,name,screenshots.*,release_dates.date,release_dates.platform,videos.name,videos.video_id, platforms; where release_dates.date > 1590926103  & hypes > 50; limit 100;");
            bool stringcontent = true;
            int count = 0;
            while (stringcontent)
            {
                try
                {
                    if (mycontent.Length < 50)
                    {
                        List<GameModel> emptylist = new List<GameModel>();
                        return emptylist;
                    }
                    else
                    {
                        //mycontent is json data from the GET request, and count is the game result
                        //so when it has run the loop once the count is 0, which means game 0 which Gamemodel then filters
                        //Then it counts up to 1, so GameModel know what data to fill into the properties of that instance
                        UpcomingGames.Models.GameModel game = new UpcomingGames.Models.GameModel(mycontent, count);
                        games.Add(game);
                        count++;
                    }
                }
                catch (Exception e)
                {
                    stringcontent = false;
                }
            }
            return games;
        }

        /// <summary>
        /// This method awaits the GetThisMonthGames method, when it's done it puts the list from gamesgames into gamerequest list.
        /// </summary>
        /// <returns></returns>




        private static HttpRequestMessage CreateRequest(string url, string query, string apiKey)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = new StringContent(query)
            };

            request.Headers.Add("user-key", apiKey);
            request.Headers.Add("Accept", "application/json");
            return request;
        }

        public static async Task<string> SendStringRequest(string url, string query)
        {
            HttpClient client = new HttpClient();
            var sharedRequest = CreateRequest(url, query, "2898c42117abf889ba9a88c9f391c51d");
            var sharedRespone = await client.SendAsync(sharedRequest);
            return await sharedRespone.Content.ReadAsStringAsync();
        }


        public API_Handler()
        {
        }
    }
}