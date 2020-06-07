using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;

namespace UpcomingGames.Models
{
    public class GameModel
    {
        public List<string> images = new List<string>();
        public List<int> platforms = new List<int>();
        public List<string> release_dates = new List<string>();
        public List<int> newest_platform_releases = new List<int>();
        public List<string> genres = new List<string>();
        public List<string> publishers = new List<string>();
        public List<string> developers = new List<string>();

        public bool duplicate { get; set; }
        public int game_id { get; set; }
        public string name { get; set; } = null;
        public string released { get; set; } = null;
        public string newest_release_date { get; set; } = null;
        public int newest_platform_release { get; set; }
        public string YT_trailer { get; set; } = null;
        public string cover_image { get; set; } = null;
        public string summary { get; set; }
        public string official_site { get; set; } = null;
        public string steam_site { get; set; } = null;
        public string epicgames_site { get; set; } = null;
        public string gog_site { get; set; } = null;


        public GameModel(string json, int count = 0, bool gameinfo = false) {
            JArray jarray = JArray.Parse(json);
            JToken jUser = jarray[count];
            game_id = Int32.Parse((string)jUser["id"]);
            name = jUser["name"].ToString();
            summary = (string)jUser["summary"];

            try
            {
                if (jUser.Children().Count() == 11)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if ((bool)jUser["involved_companies"][i]["developer"])
                        {
                            developers.Add((string)jUser["involved_companies"][i]["company"]["name"]);
                        }
                        else if ((bool)jUser["involved_companies"][i]["publisher"])
                        {
                            publishers.Add((string)jUser["involved_companies"][i]["company"]["name"]);
                        }
                        else
                        {
                        }
                    }
                }
                else
                {
                }
            }
            catch (Exception e)
            {
            }

            try
            {
                if (jUser.Children().Count() == 11)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        genres.Add((string)jUser["genres"][i]["name"]);
                    }
                }
                else
                {
                }
            }
            catch (Exception e)
            {

            }

            try
            {
                if (jUser.Children().Count() == 11)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        images.Add($"//images.igdb.com/igdb/image/upload/t_screenshot_big/{(string)jUser["screenshots"][i]["image_id"]}.jpg");
                    }
                }
                else
                {
                }
            }
            catch (Exception e)
            {

            }

            try
            {
                if (jUser.Children().Count() == 11)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        string tempname = (string)jUser["videos"][i]["name"];
                        if (tempname == "Trailer")
                        {
                            YT_trailer = "https://www.youtube.com/embed/" + (string)jUser["videos"][i]["video_id"];
                        }
                        else
                        {

                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            try
            {
                ReleasesAndPlatforms(jUser);
            }
            catch (Exception e)
            {

            }

            try
            {
                if (jUser.Children().Count() == 11)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int tempcat = Int32.Parse((string)jUser["websites"][i]["category"]);
                        if (tempcat == 1)
                        {
                            official_site = (string)jUser["websites"][i]["url"];
                        }
                        else if (tempcat == 13)
                        {
                            steam_site = (string)jUser["websites"][i]["url"];
                        }
                        else if (tempcat == 16)
                        {
                            epicgames_site = (string)jUser["websites"][i]["url"];
                        }
                        else if (tempcat == 17)
                        {
                            gog_site = (string)jUser["websites"][i]["url"];
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            SetClosestReleaseDate();
        }

        public GameModel(string json, int count = 0)
        {
            JArray jarray = JArray.Parse(json);
            JToken jUser = jarray[count];
            game_id = Int32.Parse((string)jUser["id"]);
            name = jUser["name"].ToString();
            released = UnixTimeStampToDateTime(double.Parse((string)jUser["first_release_date"]));
            cover_image = "//images.igdb.com/igdb/image/upload/t_cover_small_2x/" + (string)jUser["cover"]["image_id"] + ".jpg";



            //The code below gets a tempdate, and a tempplatform from the jUser, it then iterates on the int (tempplat)
            //to see if the platforms list already contains an integer matching the one just found, if it does, it will check which integer is lowest, and keep the lowest one for that platform.
            //(The reason i keep the lowest is because, usually a game will release globally at the same time except a few smaller regions, but i only care about the bigger portion of the world),
            //and the lowest release date on a certain platform seems to be the global release date (in my testing atleast).
            try
            {
                ReleasesAndPlatforms(jUser);
            }
            catch (Exception e)
            {

            }
            SetClosestReleaseDate();
        }




        public string GetReadableDate(string original_date)
        {
            string date = DateTime.Parse(original_date).ToLongDateString();
            return date;
        }

        /// <summary>
        /// takes a double (unixTimeStamp) and converts it into a ToLongDateString. (Example - "Tuesday, July 14, 2020".
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public string UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            string date = dtDateTime.ToLongDateString();
            return date;
        }

        public void ReleasesAndPlatforms(JToken jUser)
        {
            //The code below gets a tempdate, and a tempplatform from the jUser, it then iterates on the int (tempplat)
            //to see if the platforms list already contains an integer matching the one just found, if it does, it will check which integer is lowest, and keep the lowest one for that platform.
            //(The reason i keep the lowest is because, usually a game will release globally at the same time except a few smaller regions, but i only care about the bigger portion of the world),
            //and the lowest release date on a certain platform seems to be the global release date (in my testing atleast).
            try
            {
                //Loops 10 times in case a game has all 2020 platforms.
                for (int i = 0; i < 10; i++)
                {
                    string tempdate = (string)jUser["release_dates"][i]["date"];
                    int tempplat = Int32.Parse((string)jUser["release_dates"][i]["platform"]);
                    //Sometimes a game will have a release date, without an actual date, (very confusing) so here we just skip if that's the case.
                    if (tempdate == null)
                    {
                    }
                    else
                    {
                        //If there aren't any items in the list, there's no problem adding one.
                        if (platforms.Count == 0 && release_dates.Count == 0)
                        {
                            platforms.Add(tempplat);
                            release_dates.Add(tempdate);
                        }
                        else
                        {
                            for (int j = 0; j < platforms.Count; j++)
                            {
                                //If the platform at (j) index is equal to the tempplat integer, it will proceed into the next if statement.
                                if (platforms.ElementAt(j) == tempplat)
                                {
                                    //If the tempdate is lower than the current date at the (j) index, it will remove the higher int, and replace it with the lower version (tempdate)
                                    if (Int32.Parse(tempdate) < Int32.Parse(release_dates.ElementAt(j)) && Int32.Parse(tempdate) < (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)
                                    {
                                        release_dates.RemoveAt(j);
                                        platforms.RemoveAt(j);
                                        release_dates.Add(tempdate);
                                        platforms.Add(tempplat);
                                        j--;
                                    }
                                    //Else it breaks out of the loop since the one matching is obviously higher, so we won't need it.
                                    else
                                    {
                                        break;
                                    }
                                }
                                //If it manages to loop through the entire list without finding a duplicate, it will assume that the list doesn't contain a matching platform, and therefor add it.
                                else if (j + 1 == platforms.Count)
                                {
                                    release_dates.Add(tempdate);
                                    platforms.Add(tempplat);
                                }
                                else
                                {
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }


        public void SetClosestReleaseDate()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int tempint = Int32.Parse(release_dates.Last()) - unixTimestamp;
            int tempindex = 0;
            for (int i = 0; i < release_dates.Count; i++)
            {
                if (unixTimestamp < Int32.Parse(release_dates.ElementAt(i)))
                {
                    int tempintdifference;
                    tempintdifference = Int32.Parse(release_dates.ElementAt(i)) - unixTimestamp;
                    if (tempintdifference <= tempint)
                    {
                        tempint = tempintdifference;
                        tempindex = i;
                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }
            newest_release_date = release_dates[tempindex];
            //Takes the last release date, and puts it into a string named newest_release_date
            for (int i = 0; i < release_dates.Count; i++)
            {
                if (release_dates.ElementAt(i) == newest_release_date)
                {
                    newest_platform_releases.Add(platforms.ElementAt(i));
                }
                else
                {

                }
            }
        }
    }
}