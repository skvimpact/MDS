using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XmlCreator
{
    public class XmlCreatorSrv
    {
        private static string xml =
@"<?xml version = ""1.0"" ?>
<library>
    <books>
        <book checkedout = ""no"">
        <title> To Kill a Mockingbird</title>
        <author>Harper Lee</author>
        </book>
        <book checkedout = ""no"">
        <title>A</title>
        <author>Harper Lee</author>
        </book>
        <book checkedout = ""no"">
        <title>B</title>
        <author>Harper Lee2</author>
        </book>
        <book checkedout = ""no"">
        <title>C</title>
        <author>Harper Lee3</author>
        </book>


            </books>
            <movies>
                <movie checkedout = ""no"">
                <title> King Kong</title>
                <year>1933</year>
                    <book checkedout = ""no"">
                    <title>AAA</title>
                    <author>Harper Lee</author>
                    </book>
                </movie>
                <movie checkedout = ""yes"">
                <title> King Kong</title>
                <year>2005</year>
                    <book checkedout = ""no"">
                    <title>BBB</title>
                    <author>Harper Lee</author>
                    </book>
                </movie>
                <movie checkedout = ""yes"">
                <title> To Kill A Mockingbird</title>
                <year>1962</year>
                </movie>
                <movie checkedout = ""no"">
                <title> The Green Mile</title>
                <year>1999</year>
                </movie>
            </movies>
        </library>";
                

        public static void Do()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            //XmlNodeList books = doc.SelectNodes("library/books/book");
            //XmlNodeList books = doc.SelectNodes("library/books/book[title='A']");
            XmlNodeList books = doc.SelectNodes("//book[author='Harper Lee']");
            foreach (XmlNode book in books)
            {
                Console.WriteLine(book.OuterXml);
            }

            //XmlNode book = doc.SelectSingleNode("//book[title='To Kill a Mockingbird']");
            //book.Attributes["checkedout"].Value = "yes";
        }

        public static void DD()
        {
            XmlDocument document = new XmlDocument();
            /*
            XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = document.DocumentElement;
            document.InsertBefore(xmlDeclaration, root);
            */
            /*
            XmlNode book = document.CreateElement("book");
            XmlAttribute checkedOut = document.CreateAttribute("checkedout");
            XmlAttribute damaged = document.CreateAttribute("damaged");
            checkedOut.Value = "no";
            damaged.Value = "no";
            book.Attributes.Append(checkedOut);


            book.Attributes.Append(damaged);
            document.AppendChild(book);
            */
            XmlElement element1 = document.CreateElement(string.Empty, "Request", string.Empty);
            document.AppendChild(element1);

            XmlElement id = document.CreateElement(string.Empty, "ID", string.Empty);
            //XmlText idText = document.CreateTextNode("aaaooff");
            //id.AppendChild(idText);
            id.InnerText = "always first";
            element1.AppendChild(id);

            XmlElement dt = document.CreateElement(string.Empty, "DT", string.Empty);
            XmlText dtText = document.CreateTextNode("04.04.1974");
            dt.AppendChild(dtText);
            element1.AppendChild(dt);

            XmlElement element2 = document.CreateElement(string.Empty, "Params", string.Empty);
            element1.AppendChild(element2);

            XmlAttribute mandatory = document.CreateAttribute("mandatory");
            mandatory.Value = "no";


            element2.Attributes.Append(mandatory);
            //dt.Attributes.Append(mandatory);

            var s = document.OuterXml;
        }
    }
}
