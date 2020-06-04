using Newtonsoft.Json.Linq;
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
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            List<UpcomingGames.Models.GameModel> games = new List<Models.GameModel>();
            string mycontent = await SendStringRequest("https://api-v3.igdb.com/games/", $"fields id,first_release_date,name,screenshots.*,release_dates.date,release_dates.platform,videos.name,videos.video_id, platforms,cover.image_id; where release_dates.date > {unixTimestamp} & hypes > 15; limit 100;");
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
                        if (game.duplicate != true)
                        {
                            games.Add(game);
                            count++;
                        }
                        else
                        {

                        }
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

        //public async Task GetGameCover()
        //{
        //    List<int> templist = new List<int>();
        //    List<int> gameidlist = new List<int>();
        //    List<string> querylist = new List<string>();
        //    GameCollection gameCollection = GameCollection.Instance;
        //    for (int i = 0; i < gameCollection.listofgames.Count; i++)
        //    {
        //        if (i + 1 == gameCollection.listofgames.Count)
        //        {
        //            querylist.Add(gameCollection.listofgames.ElementAt(i).game_id.ToString());
        //        }
        //        else
        //        {
        //            querylist.Add(gameCollection.listofgames.ElementAt(i).game_id.ToString() + ",");
        //        }
        //    }
        //    string testingstring = string.Join("", querylist);
        //    string mycontent = await SendStringRequest("https://api-v3.igdb.com/covers/", $"fields *; where game = ({testingstring}); limit 100;");
        //    string gameid;
        //    string image_id;
        //    for (int i = 0; i < 100; i++)
        //    {
        //        try
        //        {
        //            JArray jarray = JArray.Parse(mycontent);
        //            JToken jUser = jarray[i];
        //            gameid = (string)jUser["game"];
        //            image_id = (string)jUser["image_id"];
        //            if (templist.Contains(Int32.Parse(gameid)))
        //            {
        //            }
        //            else
        //            {
        //                foreach (var item in gameCollection.listofgames)
        //                {
        //                    if (item.game_id == Int32.Parse(gameid))
        //                    {
        //                        item.cover_image = $"https://images.igdb.com/igdb/image/upload/t_cover_small_2x/{image_id}.jpg";
        //                        templist.Add(Int32.Parse(gameid));
        //                    }
        //                    else
        //                    {
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //        }
        //    }
        //}



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