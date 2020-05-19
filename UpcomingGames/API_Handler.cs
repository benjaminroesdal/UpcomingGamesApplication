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
        public async Task<List<UpcomingGames.Models.GameModel>> GetThisMonthGames(string endpoint)
        {
            List<UpcomingGames.Models.GameModel> games = new List<Models.GameModel>();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "UpcomingGames-School-Project");
                string url = $"https://api.rawg.io/api/{endpoint}";
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    using (HttpContent content = response.Content)
                    {
                        string mycontent = await content.ReadAsStringAsync();
                        bool stringcontent = true;
                        int count = 0;
                        while (stringcontent)
                        {
                            try
                            {
                                if (mycontent.Length < 100)
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
                    }
                }
            }
            return games;
        }

        /// <summary>
        /// This method awaits the GetThisMonthGames method, when it's done it puts the list from gamesgames into gamerequest list.
        /// </summary>
        /// <returns></returns>
        public async Task<List<GameModel>> ThisYearsReleases(int pageindex,int pagesize = 5)
        {
            gamerequest.Clear();
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            List<GameModel> templist = new List<GameModel>();
            try
            {
                var gamesgames = await GetThisMonthGames($"games?dates={date},2020-12-31&developers=16257,1612,10,109,131691,16261,6,18664,405,425,8933,13071,4132,3678,3232,774,14020,26425,313,120,8230,5342,3801,14819,5366,124,1971,4,4207,343,14641,13207,14278,18495,13972,3715,9023,18487,13972,4037,4303,6763&ordering=released&page_size=5&page={pageindex}");
                    foreach (var item in gamesgames)
                    {
                        templist.Add(item);
                    }
            }
            catch (Exception e)
            {

            }
            return templist;
        }

        public async Task<List<GameModel>> SpecificGetRequest(string date1, string date2, int pageindex = 1)
        {
            var gamesgames = await GetThisMonthGames($"games?dates={date1+","+date2}&developers=16257,1612,10,109,131691,16261,6,18664,405,425,8933,13071,4132,3678,3232,774,14020,26425,313,120,8230,5342,3801,14819,5366,124,1971,4,4207,343,14641,13207,14278,18495,13972,3715,9023,18487,13972,4037,4303,6763&ordering=released&page_size=5&page={pageindex}");
            gamerequest = gamesgames;
            return gamesgames;
        }


        public API_Handler()
        {
        }
    }
}