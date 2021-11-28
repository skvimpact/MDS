using MDS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MDS
{
    public enum ActionType
    {
        Add,
        Remove
    }

    public enum Multiplicity 
    {
        One,       // 1
        None_One,  // 0..1
        One_Many,  // 1..N
        None_Many  // 0..N        
    }

    public class MdsSchema
    {
        private readonly SchemaItem[] items;
        private static readonly string Wrap = "Wrap";
        private readonly XDocument xmlDocument;
        private readonly XDocument xsdDocument;
        public string XmlDocument => $"{xmlDocument}";
        public string XsdDocument => $"{xsdDocument}";
        //readonly Dictionary<int, XElement> xElements;
        //readonly Dictionary<int, XAttribute> xAttributes;

        public MdsSchema(ISchemaRepo repo)
        {
            items = repo.SchemaItems.ToArray();
            (int rootId, string rootName) = items
                .Where(i => i.ParentElementID == 0)
                .Select(i => (i.IntMessageLineID, i.ElementName))
                .FirstOrDefault();

            xmlDocument = new XDocument(
                new XDeclaration("1.0", "utf-8", "no"),
                new XElement(rootName));

            Dictionary<int, XElement> xElements = new Dictionary<int, XElement> { { rootId, xmlDocument.Root } };
            Dictionary<int, XAttribute> xAttributes = new Dictionary<int, XAttribute>();

            foreach (var item in items.Where(i => i.ParentElementID != 0))
            {
                switch (item.LineType)
                {
                    case 0:
                    case 1:
                        xElements[item.IntMessageLineID] =
                            new XElement(item.ElementName, string.IsNullOrWhiteSpace(item.Value) && item.LineType == 1 ? item.ElementName : item.Value);
                        break;
                    case 2:
                        xAttributes[item.IntMessageLineID] =
                            new XAttribute(item.ElementName, item.Value);
                        break;
                    default:
                        throw new InvalidOperationException("unknown item type");
                }
            }

            foreach (var item in items.Where(i => i.ParentElementID != 0))
            {
                switch (item.LineType)
                {
                    case 0:
                    case 1:
                        xElements[item.ParentElementID]
                            .Add(xElements[item.IntMessageLineID]);
                        break;
                    case 2:
                        xElements[item.ParentElementID]
                            .Add(xAttributes[item.IntMessageLineID]);
                        break;
                    default:
                        throw new InvalidOperationException("unknown item type");
                }
                item.Indentation = Indentation(item.IntMessageLineID);
                item.XPath = XPath(item.IntMessageLineID);
            }

            xsdDocument = ExcludeWrap(XsdInferer.Infer(All()));

            Dictionary<string, XElement> elementXPath = new Dictionary<string, XElement>();
            xsdDocument
                .Descendants()
                .Where(x => x.HasAttributes && x.Attribute("name") != null)
                .ToList()
                .ForEach(element => elementXPath[XPath(element)] = element);

            items.ToList().ForEach(
                i => {
                    var element = elementXPath[XPath(i.IntMessageLineID)];
                    if (element.Attribute("type") != null)
                        element.Attribute("type").Value = $"{i.IntMessageLineID}";});
        }

        override public string ToString() => xmlDocument.ToString();

        public IEnumerable<string> Paths(int id)
        {
            var item = items.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault();
            if (!item.ParentElementID.Equals(0))
                foreach(var itemName  in Paths(item.ParentElementID))                
                    yield return itemName;                
            yield return item.ElementName;            
        }

        public string XPath(int id) =>
            string.Join("/", Paths(id));

        public int Indentation(int id)
        {
            //SchemaItems.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault().ParentElementID.Equals(0) ?
            //0 :
            //1 + Indentation(SchemaItems.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault().ParentElementID);
            var parent = items.Where(i => i.IntMessageLineID.Equals(id)).First().ParentElementID;
            return parent.Equals(0) ? 0 : 1 + Indentation(parent);
        }

        public string DocumentVar(int id, ActionType actionType)
        {
            XDocument doc = XDocument.Load(new StringReader(xmlDocument.ToString()));
            var paths = Paths(id);

            if (paths.Count() == 1)
                return $"{doc}";

            XElement parent = doc.Element(paths.First());
            foreach (var name in paths.Skip(1).SkipLast(1))
                parent = parent.Element(name);

            XElement element = parent.Element(paths.Last());
            XAttribute attribute = parent.Attribute(paths.Last());

            switch (actionType)
            {
                case ActionType.Add :
                    for (int i = 0; i < 5; i++)
                        parent.Add(element);
                    break;
                case ActionType.Remove:                    
                    element?.Remove();
                    attribute?.Remove();
                    break;
            }
            return doc.ToString();
        }

        public IEnumerable<(int id, string name, ActionType actionType)> Deviations()
        {            
            foreach (var i in items)
                switch ((Multiplicity)i.Multiplicity)
                {
                    case Multiplicity.None_Many:
                        yield return (i.IntMessageLineID, i.ElementName, ActionType.Add);
                        yield return (i.IntMessageLineID, i.ElementName, ActionType.Remove);
                        break;
                    case Multiplicity.None_One:
                        yield return (i.IntMessageLineID, i.ElementName, ActionType.Remove);
                        break;
                    case Multiplicity.One_Many:
                        yield return (i.IntMessageLineID, i.ElementName, ActionType.Add);
                        break;
                }
        }

        public IEnumerable<string> AllVariants()
        {
            yield return $"<!-- Normal -->\n{xmlDocument}";
            foreach (var deviation in Deviations())
                yield return $"<!-- {deviation.id}/{deviation.name}/{deviation.actionType} -->\n" +
                    $"{DocumentVar(deviation.id, deviation.actionType)}";
        }

        public string All()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<{Wrap}>");
            foreach (var variant in AllVariants())
            {
                sb.AppendLine(variant);
            }
            sb.AppendLine($"</{Wrap}>");
            return sb.ToString();
        }

        public XDocument ExcludeWrap(string xsdWrapped)
        {
            var xsdMarkup = XDocument.Load(new StringReader(xsdWrapped));

            XElement extracted = xsdMarkup
                .Descendants()
                .Where(i => i.Attribute("name")?.Value.Equals(xmlDocument.Root.Name.LocalName) ?? false)
                .FirstOrDefault();

            extracted?
                .Attribute("maxOccurs")?
                .Remove();

            xsdMarkup
                .Descendants()
                .Where(i => i.Attribute("name")?.Value.Equals(Wrap) ?? false)
                .FirstOrDefault()?
                .Remove();

            xsdMarkup
                .Root
                .Add(extracted);

            return xsdMarkup;
        }

        public IEnumerable<string> ValidateAll(string xsdMarkup)
        {
            string message;
            foreach(var variant in AllVariants())
            {
                message = XsdValidator.Validate(xsdMarkup, variant);
                if (!string.IsNullOrEmpty(message))
                    yield return message;
            }
        }

        public string XPath(XElement element)
        {
            if (element.Parent == null)
                return null;
            var parentXPath = XPath(element.Parent);
            var path = element.Attribute("name")?.Value;
            var prefix = path == null || parentXPath == null ? null : "/";
            return $"{parentXPath}{prefix}{path}";
        }
    }
}
