using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Serializer
{
    internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; }=new Dictionary<string, string>();
        public List<string> Classes { get; set; }=new List<string>();
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }

        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();



       
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            if (Name != null)
            {
                stringBuilder.Append($"Tag: {Name}");
                if (!string.IsNullOrEmpty(Id))
                {
                    stringBuilder.Append($" | Id: {Id}");
                }

                if (Classes.Count > 0)
                {
                    stringBuilder.Append(" | Classes: ");
                    stringBuilder.Append(string.Join(", ", Classes));
                }
            }

            return stringBuilder.ToString();
        }
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement currentParent = Parent;

            while (currentParent != null)
            {
                yield return currentParent;
                currentParent = currentParent.Parent;
            }
        }
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                HtmlElement currentElement = queue.Dequeue();
                yield return currentElement;

                if (currentElement.Children != null)
                {
                    foreach (var child in currentElement.Children)
                    {
                        queue.Enqueue(child);
                    }
                }
            }
        }
        public IEnumerable<HtmlElement> FindElements(Selector selector)
        {
            HashSet<HtmlElement> result = new HashSet<HtmlElement>();

            foreach (var child in Descendants())
                child.FindElementsRecursively(selector, result);
            return result;
        }

        private void FindElementsRecursively(Selector selector, HashSet<HtmlElement> result)
        {
            if (!IsMatch(selector))
                return;

            if (selector.Child == null)
                result.Add(this);
            else
                foreach (var child in Descendants())
                    child.FindElementsRecursively(selector.Child, result);
        }

        private bool IsMatch(Selector selector)
        {
            return ((selector.TagName == null || Name.Equals(selector.TagName))
                && (selector.Id == null || selector.Id.Equals(Id))
                && (selector.Classes.Intersect(Classes).Count() == selector.Classes.Count));
        }

    }
}
