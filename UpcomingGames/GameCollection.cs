using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Web;
using UpcomingGames.Models;

namespace UpcomingGames
{
    public class GameCollection
    {

        public List<GameModel> listofgames = new List<GameModel>();
        public int offset = 0;

        public async Task<List<GameModel>> GameList()
        {
            API_Handler getRequest = new API_Handler();
            listofgames = await getRequest.GetThisMonthGames();
            SortGameList();
            return listofgames;
        }


        /// <summary>
        /// Simple bubble sort used to sort on the release date of each game in the list, and sort them in ascending order.
        /// </summary>
        public void SortGameList()
        {
            GameModel t;
            for (int i = 0; i < listofgames.Count ; i++)
            {
                for (int j = 0; j < listofgames.Count - 1; j++)
                {
                    if (Int32.Parse(listofgames[j].newest_release_date) > Int32.Parse(listofgames[j + 1].newest_release_date))
                    {
                        t = listofgames[j + 1];
                        listofgames[j + 1] = listofgames[j];
                        listofgames[j] = t;
                    }
                }
            }
        }

        public async Task<List<GameModel>> GetGameList(int offset = 0)
        {
            await GameList();
            SortGameList();
            List<GameModel> templist = new List<GameModel>();
            try
            {
                for (int i = offset; templist.Count() < 5; i++)
                {
                    templist.Add(listofgames[i]);
                }
            }
            catch (Exception e)
            {

            }
            return templist;
        }

        public List<GameModel> InfiniteScrollList(int offset = 0)
        {
            List<GameModel> templist = new List<GameModel>();
            try
            {
                for (int i = offset; templist.Count() < 5; i++)
                {
                    templist.Add(listofgames[i]);
                }
            }
            catch (Exception e)
            {

            }
            return templist;
        }
    }
}