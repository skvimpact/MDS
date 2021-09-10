using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace MDS
{
    public class XsdInferer
    {
        /*
        private static string xml =
@"<?xml version = ""1.0"" ?>
<library>

    <books>
        <book>
        <title> To Kill a Mockingbird</title>        
        </book>
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


    <books>
        <book>
        <title> To Kill a Mockingbird</title>        
        </book>
        <m1 a=""a"">m1</m1>
    </books>
    <books>
        <book>
        <title> To Kill a Mockingbird</title>        
        </book>
        <m1 a=""a"">m1</m1>
    </books>
    <books>
        <m1 a=""a"">m1</m1>
     </books>


            <!--<movies>
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
            </movies>-->
        </library>";*/
        public static string Infer(string xml)
        {
            string result = null;
            MemoryStream memStream = new MemoryStream(Encoding.ASCII.GetBytes(xml));
            //
            XmlReader reader = XmlReader.Create(memStream);
            //XmlSchemaSet schemaSet = new XmlSchemaSet();
            //XmlSchemaInference schema = new XmlSchemaInference();
            XmlSchemaSet schemaSet = new XmlSchemaInference().InferSchema(reader);

            foreach (XmlSchema s in schemaSet.Schemas())
            {
                using (var stringWriter = new StringWriter())
                {
                    using (var writer = XmlWriter.Create(stringWriter))
                    {
                        s.Write(writer);
                    }

                    result = stringWriter.ToString();
                }
            }


//            XDocument doc1 = new XDocument(
//    new XElement("Root",
//        new XElement("Child1", "content1"),
//        new XElement("Child2", "content1")
//    )
//);
//            // doc1.v
//            XmlSchemaSet schemas = new XmlSchemaSet();
//            XmlReader r = XmlReader.Create("books.xml");

//            XmlReader reader5 = XmlReader.Create(new StringReader(markup))
//            schemas.Add("", "CustomersOrders.xsd");
            return result;
        }
    }
}
