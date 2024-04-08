// See https://aka.ms/new-console-template for more information
using System.ComponentModel;
using System.Xml;

internal class Program
{
    private static void Main(string[] args)
    {
        if(args.Length <= 0)
        {
            Console.WriteLine("Please provide a file name without extension");
            return;
        }

        var fileName = args[0];


        // Load the XML file
        XmlDocument sourceXML = new XmlDocument();
        sourceXML.Load($"bibles//{fileName}.xml");

        // Get the root element and print it out
        XmlElement sourceRoot = sourceXML.DocumentElement;
        Console.WriteLine(sourceRoot.Name);


        // create a new xml and store it to a file
        XmlDocument newXmlDoc = new XmlDocument();

        //add defualt xml declaration
        XmlDeclaration xmlDeclaration = newXmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
        newXmlDoc.AppendChild(xmlDeclaration);

        XmlElement xmlbible = newXmlDoc.CreateElement("XMLBIBLE");

        //add a child element to xmlbible called INFORMATION
        XmlElement information = newXmlDoc.CreateElement("INFORMATION");

        //add a child element to INFORMATION called title
        XmlElement title = newXmlDoc.CreateElement("title");

        //set the title to filename 
        title.InnerText = fileName;
        information.AppendChild(title);

        //add a child element to INFORMATION called identifier
        XmlElement identifier = newXmlDoc.CreateElement("identifier");
        //set the identifier to filename
        identifier.InnerText = fileName;
        information.AppendChild(identifier);

        //add a child element to INFORMATION called language
        XmlElement language = newXmlDoc.CreateElement("language");
        //set the language to ENG
        language.InnerText = "ENG";
        information.AppendChild(language);

        xmlbible.AppendChild(information);

        //loop thorough all the child nodes of sourceRoot called b and create a child not of xmlbible called BIBLEBOOK
        int i = 1;
        foreach (XmlNode book in sourceRoot.ChildNodes)
        {
            XmlElement bibleBook = newXmlDoc.CreateElement("BIBLEBOOK");
            bibleBook.SetAttribute("bnumber", i.ToString());
            bibleBook.SetAttribute("bname", book.Attributes["n"].Value);

            //loop through all the child nodes of b called c and create a child node of bibleBook called CHAPTER
            foreach (XmlNode chapter in book.ChildNodes)
            {
                XmlElement bibleChapter = newXmlDoc.CreateElement("CHAPTER");
                bibleChapter.SetAttribute("cnumber", chapter.Attributes["n"].Value);

                //loop through all the child nodes of c called v and create a child node of bibleChapter called V
                foreach (XmlNode verse in chapter.ChildNodes)
                {
                    XmlElement bibleVerse = newXmlDoc.CreateElement("VERS");
                    bibleVerse.SetAttribute("vnumber", verse.Attributes["n"].Value);
                    bibleVerse.InnerText = verse.InnerText;
                    bibleChapter.AppendChild(bibleVerse);
                }
                bibleBook.AppendChild(bibleChapter);
            }
            xmlbible.AppendChild(bibleBook);
        }

        



        newXmlDoc.AppendChild(xmlbible);

        //XMLBIBLE xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="zef2005.xsd" version="2.0.1.18" status="v" biblename="King James Version" type="x-bible" revision="0">
        xmlbible.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
        xmlbible.SetAttribute("xsi:noNamespaceSchemaLocation", "zef2005.xsd");
        xmlbible.SetAttribute("version", "2.0.1.18");
        xmlbible.SetAttribute("status", "v");
        xmlbible.SetAttribute("biblename", $"{fileName}");
        xmlbible.SetAttribute("type", "x-bible");
        xmlbible.SetAttribute("revision", "0");

        // save it to a file called new.xml
        newXmlDoc.Save($"{fileName}_zefania.xml");


        Console.WriteLine($"{fileName}_zefania.xml -- Successfully Generated");
    }
}