// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
class Program
{
    static async Task Main(string[] args)
    {   
        Console.WriteLine(args[0]);
        //APIInterface = new APIInterface();

        string city = args[0];
        var apikeyHolder = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(),"privateVariables.json"))
            .Build();
        
        string apiKey = apikeyHolder["apiKey"];
        string apiUrl = "https://api.weatherapi.com/v1/current.json?key="+apiKey+"&q="+city+"&aqi=no";
        using (HttpClient client = new HttpClient())
        {
             HttpResponseMessage response = await client.GetAsync(apiUrl);
             if(response.IsSuccessStatusCode){
                string jsonData = await response.Content.ReadAsStringAsync();
                var parsedData = JObject.Parse(jsonData);
                //Console.WriteLine(parsedData);
                string city_name = parsedData["location"]["name"].ToString();
                string state = parsedData["location"]["region"].ToString();
                string date = parsedData["current"]["last_updated"].ToString();
                decimal temp_c = Convert.ToDecimal(parsedData["current"]["temp_c"]);
                decimal wind_kph = Convert.ToDecimal(parsedData["current"]["wind_kph"]);
                string wind_dir = parsedData["current"]["wind_dir"].ToString();
                decimal feelslike_c = Convert.ToDecimal(parsedData["current"]["feelslike_c"]);
                decimal vis_km = Convert.ToDecimal(parsedData["current"]["vis_km"]);
                decimal gust_kph = Convert.ToDecimal(parsedData["current"]["gust_kph"]);
                Console.WriteLine(date);
                MySqlHelper.InsertData(city_name,state,date,temp_c,wind_kph, wind_dir, feelslike_c, vis_km, gust_kph);
             } 
                
                
        }
    }
}

        


class MySqlHelper
{
    private const string connectionString = "Server=localhost; Database=weatherSaver; Uid=urisoltz;";
    public static void InsertData(string city_name, string state, string date, decimal temp_c, decimal wind_kph, string wind_dir, decimal feelslike_c, decimal vis_km, decimal gust_kph)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO weather_data (city_name, state, date, temp_c, wind_kph, wind_dir, feelslike_c, vis_km, gust_kph) VALUES (@city_name, @state, @date, @temp_c, @wind_kph, @wind_dir, @feelslike_c, @vis_km, @gust_kph)";
            using (MySqlCommand command = new MySqlCommand (query,connection))
            {
                command.Parameters.AddWithValue("@city_name", city_name);
                command.Parameters.AddWithValue("@state", state);
                command.Parameters.AddWithValue("@date", date); 
                command.Parameters.AddWithValue("@temp_c", temp_c);
                command.Parameters.AddWithValue("@wind_kph", wind_kph);
                command.Parameters.AddWithValue("@wind_dir", wind_dir);
                command.Parameters.AddWithValue("@feelslike_c", feelslike_c);
                command.Parameters.AddWithValue("@vis_km", vis_km);
                command.Parameters.AddWithValue("@gust_kph", gust_kph);
                command.ExecuteNonQuery(); 
            }
        }
        
    }
}