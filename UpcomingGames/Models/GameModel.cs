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
        public List<string> short_images = new List<string>();
        public List<string> platforms = new List<string>();


        public int game_id { get; set; }
        public string name { get; set; } = null;
        public string released { get; set; } = null;
        public int release_date_int { get; set; }
        public string rating { get; set; }
        public string bg_image { get; set; }
        public string bg_blur_image { get; set; }
        public string YT_trailer { get; set; } = null;

        public GameModel(string json, int count = 0)
        {
            JObject jObject = JObject.Parse(json);
            JToken jUser = jObject["results"][count];
            game_id = Int32.Parse((string)jUser["id"]);
            name = jUser["name"].ToString();
            released = (string)jUser["released"];
            rating = (string)jUser["rating"];
            bg_image = (string)jUser["background_image"];
            release_date_int = Int32.Parse(Regex.Replace(released, "[^A-Za-z0-9 ]", ""));

            try
            {
                if (short_images.ElementAt(0) == null)
                {
                    bg_image = "~/Images/default.jpg";
                }
                else
                {
                    bg_image = short_images.ElementAt(0);
                }
            }
            catch (Exception e)
            {
            }

            try
            {
                for (int i = 0; i < 6; i++)
                {
                    platforms.Add((string)jUser["parent_platforms"][i]["platform"]["name"]);
                }
            }
            catch (Exception e)
            {
            }


            try
            {
                for (int i = 0; i < 6; i++)
                {
                    short_images.Add((string)jUser["short_screenshots"][i]["image"]);
                }
            }
            catch (Exception e)
            {
            }

            try
            {
                YT_trailer = "https://www.youtube.com/embed/" + (string)jUser["clip"]["video"];
            }
            catch (Exception e)
            {
                YT_trailer = null;
            }


        }

        public string GetReadableDate(string original_date)
        {
            string date = DateTime.Parse(original_date).ToLongDateString();
            return date;
        }
    }
}