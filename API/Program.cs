using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Data.SqlClient;
using System.Web;

public class Coord
{
    public double lon { get; set; }
    public double lat { get; set; }
}

public class Sys
{
    public int population { get; set; }
}

public class City
{
    public int id { get; set; }
    public string name { get; set; }
    public Coord coord { get; set; }
    public string country { get; set; }
    public int population { get; set; }
    public Sys sys { get; set; }
}

public class Main
{
    public double temp { get; set; }
    public double temp_min { get; set; }
    public double temp_max { get; set; }
    public double pressure { get; set; }
    public double sea_level { get; set; }
    public double grnd_level { get; set; }
    public int humidity { get; set; }
    public double temp_kf { get; set; }
}

public class Weather
{
    public int id { get; set; }
    public string main { get; set; }
    public string description { get; set; }
    public string icon { get; set; }
}

public class Clouds
{
    public int all { get; set; }
}

public class Wind
{
    public double speed { get; set; }
    public double deg { get; set; }
}

public class Snow
{
    public double __invalid_name__3h { get; set; }
}

public class Sys2
{
    public string pod { get; set; }
}

public class List
{
    public int dt { get; set; }
    public Main main { get; set; }
    public List<Weather> weather { get; set; }
    public Clouds clouds { get; set; }
    public Wind wind { get; set; }
    public Snow snow { get; set; }
    public Sys2 sys { get; set; }
    public string dt_txt { get; set; }
}

public class RootObject
{
    public City city { get; set; }
    public string cod { get; set; }
    public double message { get; set; }
    public int cnt { get; set; }
    public List<List> list { get; set; }
}



namespace API
{

    class Program

    {

        static void Main(string[] args)
        {
            Console.WriteLine("Tell me your name.");
            string userName = Console.ReadLine();
            Console.WriteLine("Please enter your zipcode:");
            string userZip = Console.ReadLine();

            string url = $"http://api.openweathermap.org/data/2.5/weather?zip={userZip},us&APPID=0d0118486a5bdd114c4e6bb076ae6bed";
            var request = WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            var rawJson = String.Empty;
            var response = request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                rawJson = reader.ReadToEnd();
            }

            var weather = JsonConvert.DeserializeObject<List>(rawJson);
            var city = JsonConvert.DeserializeObject<City>(rawJson);
            DateTime rightnow = DateTime.Now;

            Console.WriteLine("---------------------------------");
            Console.WriteLine($"In {city.name},{city.country} it is {weather.main.temp} degrees Kelvin with a humidity of {weather.main.humidity}. It is {rightnow}.");
            Console.WriteLine($"Goodbye, {userName}");
            Console.ReadLine();

            string temperatureData = weather.main.temp.ToString();
            string conditionData = weather.weather.ElementAt(0).description;

            var connectionStrings = @"Server=DESKTOP-577TSME\SQLEXPRESS;Database=WeatherApp;Trusted_Connection=True;";
            using (var connection = new SqlConnection(connectionStrings))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = @"INSERT INTO WeatherInquiries VALUES (@UserName, @Temperature, @Conditions, @DateTime)";
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@Temperature", temperatureData);
                    cmd.Parameters.AddWithValue("@Conditions", conditionData);
                    cmd.Parameters.AddWithValue("@DateTime", rightnow);

                    connection.Open();
                    var reader = cmd.ExecuteReader();
                    connection.Close();

                }
            }
        }
    }
}
