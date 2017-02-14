using System.Linq;
using System.Xml.Linq;

namespace NugetPackagerAssistant.Common
{
    public class XmlHelper
    {
        private readonly XDocument _xmlDocument;
        private readonly string _xmlFilename;

        public XmlHelper(string xmlFilename)
        {
            _xmlDocument = XDocument.Load(xmlFilename);
            _xmlFilename = xmlFilename;
        }

        public XElement GetElement(string path)
        {
            var nodes = path.Split('/');
            XElement element = _xmlDocument.Element(nodes.First());

            for (var i = 1; i < nodes.Length; i++)
            {
                if (element == null) return null;
                element = element.Element(nodes[i]);
            }

            return element;
        }

        public bool RemoveElement(string path)
        {
            var node = GetElement(path);
            if (node == null)
                return false;

            node.Remove();
            return true;
        }

        public bool AddElement(string path, XElement element, bool addAfterSelf = true)
        {
            var node = GetElement(path);
            if (node == null)
                return false;

            if (addAfterSelf)
                node.AddAfterSelf(element);
            else
                node.Add(element);
            return true;
        }

        public void Save()
        {
            _xmlDocument.Save(_xmlFilename);
        }
    }
}
