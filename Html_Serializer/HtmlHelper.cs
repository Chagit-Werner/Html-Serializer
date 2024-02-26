using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Html_Serializer
{
    internal class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();


        //just like a Getter method:
        public static HtmlHelper Instance => _instance;
        public string[] AllTags { get;  set; }
        public string[] NoClosingTags { get;  set; }

        //In order to enable only singletone - we define the ctor in 'private' protection level.
        private HtmlHelper()
        {
            // Load data from JSON files into arrays
            AllTags = LoadTags("HtmlTags.json");
            NoClosingTags = LoadTags("HtmlVoidTags.json");
        }

        private string[] LoadTags(string filePath)
        {
            try
            {
                //קורא מהקבץ ומכניס למשתנה
                string jsonContent = File.ReadAllText(filePath);
                //מפרק את תוכן הקבץ לאוביקטים של C#
                string[] tagsArray = JsonSerializer.Deserialize<string[]>(jsonContent);
                return tagsArray;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tags from {filePath}: {ex.Message}");
                return null;
            }

        }
      
    }
}
