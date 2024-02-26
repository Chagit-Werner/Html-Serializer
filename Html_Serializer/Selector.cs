using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Serializer
{
    internal class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Child{ get; set; }
        public Selector Parent { get; set; }
        public static Selector ParseQuery(string queryString)
        {
            // Split the query string into parts based on spaces
            string[] parts = queryString.Split(' ');

            Selector rootSelector = new Selector();
            Selector currentSelector = rootSelector;

            foreach (string part in parts)
            {
                // Break the string into parts according to the separators # and .
                string[] subParts = part.Split(new[] { '#', '.' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string subPart in subParts)
                {
                    // Update the current selector properties based on the parts of the string
                    if (subPart.StartsWith("#"))
                    {
                        // If the part starts with #, it is an id
                        currentSelector.Id = subPart.Substring(1);
                    }
                    else if (subPart.StartsWith("."))
                    {
                        // If the part starts with ., it is a class
                        currentSelector.Classes ??= new List<string>();
                        currentSelector.Classes.Add(subPart.Substring(1));
                    }
                    else
                    {
                        // If the part does not start with # or ., check if it is a valid HTML tag name
                        // and only if so, put it in the TagName property
                        if (IsValidTagName(subPart))
                        {
                            currentSelector.TagName = subPart;
                        }
                        else
                        {
                            // Handle invalid tag names if needed
                            Console.WriteLine($"Invalid tag name: {subPart}");
                        }
                    }
                }

                // Create a new Selector object, add it as a child of the current selector, and update the current selector
                Selector newSelector = new Selector { Parent = currentSelector };
                currentSelector.Child = newSelector;
                currentSelector = newSelector;
            }

            return rootSelector;
        }
        private static bool IsValidTagName(string tagName)
        {
            return HtmlHelper.Instance.AllTags.Contains(tagName) && !HtmlHelper.Instance.NoClosingTags.Contains(tagName);
        }
    }
}
