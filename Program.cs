// See https://aka.ms/new-console-template for more information
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Xml;

namespace BibleConverter
{
    internal class Program
    {
        // create a dictionary of bible books with the number being the key as a string
        private static Dictionary<string, string> bibleBooks = new Dictionary<string, string>
        {
            {"1", "Genesis"},
            {"2", "Exodus"},
            {"3", "Leviticus"},
            {"4", "Numbers"},
            {"5", "Deuteronomy"},
            {"6", "Joshua"},
            {"7", "Judges"},
            {"8", "Ruth"},
            {"9", "1 Samuel"},
            {"10", "2 Samuel"},
            {"11", "1 Kings"},
            {"12", "2 Kings"},
            {"13", "1 Chronicles"},
            {"14", "2 Chronicles"},
            {"15", "Ezra"},
            {"16", "Nehemiah"},
            {"17", "Tobit"},
            {"18", "Judith"},
            {"19", "Esther"},
            {"20", "1 Maccabees"},
            {"21", "2 Maccabees"},
            {"22", "Job"},
            {"23", "Psalms"},
            {"24", "Proverbs"},
            {"25", "Ecclesiastes"},
            {"26", "Song of Songs"},
            {"27", "Wisdom"},
            {"28", "Sirach"},
            {"29", "Isaiah"},
            {"30", "Jeremiah"},
            {"31", "Lamentations"},
            {"32", "Baruch"},
            {"33", "Ezekiel"},
            {"34", "Daniel"},
            {"35", "Hosea"},
            {"36", "Joel"},
            {"37", "Amos"},
            {"38", "Obadiah"},
            {"39", "Jonah"},
            {"40", "Micah"},
            {"41", "Nahum"},
            {"42", "Habakkuk"},
            {"43", "Zephaniah"},
            {"44", "Haggai"},
            {"45", "Zechariah"},
            {"46", "Malachi"},
            {"47", "New Testament"},
            {"48", "Matthew"},
            {"49", "Mark"},
            {"50", "Luke"},
            {"51", "John"},
            {"52", "Acts"},
            {"53", "Romans"},
            {"54", "1 Corinthians"},
            {"55", "2 Corinthians"},
            {"56", "Galatians"},
            {"57", "Ephesians"},
            {"58", "Philippians"},
            {"59", "Colossians"},
            {"60", "1 Thessalonians"},
            {"61", "2 Thessalonians"},
            {"62", "1 Timothy"},
            {"63", "2 Timothy"},
            {"64", "Titus"},
            {"65", "Philemon"},
            {"66", "Hebrews"},
            {"67", "James"},
            {"68", "1 Peter"},
            {"69", "2 Peter"},
            {"70", "1 John"},
            {"71", "2 John"},
            {"72", "3 John"},
            {"73", "Jude"},
            {"74", "Revelation"}
        };


        private static List<BibleFormatType> bibleFormatTypes = new List<BibleFormatType>
                   {
                       new BibleFormatType
                       {
                                          Key = "bible",
                                          NameAttribute = "n",
                                          ChapterNumberAttribute = "n",
                                          VerseNumberAttribute = "n"
                                      },
                       new BibleFormatType
                       {
                                          Key = "bible2",
                                          NameAttribute = "number",
                                          BookNumberAttribute = "number",
                                          ChapterNumberAttribute = "number",
                                          VerseNumberAttribute = "number"
                                      }
        };

        private static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine("Please provide a file name");
                return;
            }

            if (args[0] == "--folder")
            {
                if(args.Length <= 1)
                {
                    Console.WriteLine("Please provide a folder name");
                    return;
                }

                var folderName = args[1];
                //check if folder exists    
                if (!Directory.Exists(folderName))
                {
                    Console.WriteLine($"Folder {folderName} does not exist");
                    return;
                }

                var files = Directory.GetFiles(folderName, "*.xml");
                foreach ( var file in files)
                {
                    ProcessFile(file);
                }
            }
            else
            {
                ProcessFile(args[0]);
            }
        }

        private static void ProcessFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File {fileName}  does not exist");
                return;
            }

            // Load the XML file
            XmlDocument sourceXML = new XmlDocument();
            sourceXML.Load(fileName);

            // Get the root element and print it out
            XmlElement sourceRoot = sourceXML.DocumentElement;

            var bibleFormatType = bibleFormatTypes.FirstOrDefault(x => x.Key == sourceRoot.Name);
            switch (sourceRoot.Name)
            {
                case "bible":
                    //check if there is an attribute called translation
                    if (sourceRoot.Attributes["translation"] == null)
                    {
                        ConvertBible(fileName);
                        break;
                    }
                    ConvertBible2(fileName);
                    break;
                default:
                    break;
            }
        }

        private static void ConvertBible(string fileName)
        {
            Console.WriteLine($"Converting {fileName} to Zefania XML format");

            var bibleVersion = Path.GetFileNameWithoutExtension(fileName);

            // Load the XML file
            XmlDocument sourceXML = new XmlDocument();
            sourceXML.Load(fileName);

            // Get the root element and print it out
            XmlElement sourceRoot = sourceXML.DocumentElement;
            Console.WriteLine(sourceRoot.Name);

            var bibleFormatType = bibleFormatTypes.FirstOrDefault(x => x.Key == sourceRoot.Name);

            if(bibleFormatType == null)
            {
                Console.WriteLine("Unknown source bible XML format");
                return;
            }

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
            title.InnerText = bibleVersion;
            information.AppendChild(title);

            //add a child element to INFORMATION called identifier
            XmlElement identifier = newXmlDoc.CreateElement("identifier");
            //set the identifier to filename
            identifier.InnerText = bibleVersion;
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
                i++;
                bibleBook.SetAttribute("bname", book.Attributes[bibleFormatType.NameAttribute].Value);

                //loop through all the child nodes of b called c and create a child node of bibleBook called CHAPTER
                foreach (XmlNode chapter in book.ChildNodes)
                {
                    XmlElement bibleChapter = newXmlDoc.CreateElement("CHAPTER");
                    bibleChapter.SetAttribute("cnumber", chapter.Attributes[bibleFormatType.ChapterNumberAttribute].Value);

                    //loop through all the child nodes of c called v and create a child node of bibleChapter called V
                    foreach (XmlNode verse in chapter.ChildNodes)
                    {
                        XmlElement bibleVerse = newXmlDoc.CreateElement("VERS");
                        bibleVerse.SetAttribute("vnumber", verse.Attributes[bibleFormatType.VerseNumberAttribute].Value);
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
            xmlbible.SetAttribute("biblename", $"{bibleVersion}");
            xmlbible.SetAttribute("type", "x-bible");
            xmlbible.SetAttribute("revision", "0");

            // save it to a file called new.xml
            //get directory of the file from fileName   
            var directory = Path.GetDirectoryName(fileName);
            var newFileName = $"{directory}//{bibleVersion}_zefania.xml";
            newXmlDoc.Save(newFileName);


            Console.WriteLine($"{newFileName} -- Successfully Generated");
        }

        private static void ConvertBible2(string fileName)
        {
            Console.WriteLine($"Converting {fileName} to Zefania XML format");

            var bibleVersion = Path.GetFileNameWithoutExtension(fileName);

            // Load the XML file
            XmlDocument sourceXML = new XmlDocument();
            sourceXML.Load(fileName);

            // Get the root element and print it out
            XmlElement sourceRoot = sourceXML.DocumentElement;

            var bibleFormatType = bibleFormatTypes.FirstOrDefault(x => x.Key == "bible2");

            if (bibleFormatType == null)
            {
                Console.WriteLine("Unknown source bible XML format");
                return;
            }

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
            title.InnerText = bibleVersion;
            information.AppendChild(title);

            //add a child element to INFORMATION called identifier
            XmlElement identifier = newXmlDoc.CreateElement("identifier");
            //set the identifier to filename
            identifier.InnerText = bibleVersion;
            information.AppendChild(identifier);

            //add a child element to INFORMATION called language
            XmlElement language = newXmlDoc.CreateElement("language");
            //set the language to ENG
            language.InnerText = "ENG";
            information.AppendChild(language);

            xmlbible.AppendChild(information);

            //loop thorough all the child nodes of sourceRoot called testament/book and create a child not of xmlbible called BIBLEBOOK
            int i = 1;
            foreach (XmlNode testament in sourceRoot.ChildNodes)
            {
                foreach (XmlNode book in testament.ChildNodes)
                {

                    XmlElement bibleBook = newXmlDoc.CreateElement("BIBLEBOOK");
                    bibleBook.SetAttribute("bnumber", book.Attributes[bibleFormatType.BookNumberAttribute].Value);
                    bibleBook.SetAttribute("bname", bibleBooks[book.Attributes[bibleFormatType.BookNumberAttribute].Value.ToString()]);

                    //loop through all the child nodes of b called c and create a child node of bibleBook called CHAPTER
                    foreach (XmlNode chapter in book.ChildNodes)
                    {
                        XmlElement bibleChapter = newXmlDoc.CreateElement("CHAPTER");
                        bibleChapter.SetAttribute("cnumber", chapter.Attributes[bibleFormatType.ChapterNumberAttribute].Value);

                        //loop through all the child nodes of c called v and create a child node of bibleChapter called V
                        foreach (XmlNode verse in chapter.ChildNodes)
                        {
                            XmlElement bibleVerse = newXmlDoc.CreateElement("VERS");
                            bibleVerse.SetAttribute("vnumber", verse.Attributes[bibleFormatType.VerseNumberAttribute].Value);
                            bibleVerse.InnerText = verse.InnerText;
                            bibleChapter.AppendChild(bibleVerse);
                        }
                        bibleBook.AppendChild(bibleChapter);
                    }
                    xmlbible.AppendChild(bibleBook);
                }
            }

            newXmlDoc.AppendChild(xmlbible);

            //XMLBIBLE xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="zef2005.xsd" version="2.0.1.18" status="v" biblename="King James Version" type="x-bible" revision="0">
            xmlbible.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xmlbible.SetAttribute("xsi:noNamespaceSchemaLocation", "zef2005.xsd");
            xmlbible.SetAttribute("version", "2.0.1.18");
            xmlbible.SetAttribute("status", "v");
            xmlbible.SetAttribute("biblename", $"{bibleVersion}");
            xmlbible.SetAttribute("type", "x-bible");
            xmlbible.SetAttribute("revision", "0");

            // save it to a file called new.xml
            //get directory of the file from fileName   
            var directory = Path.GetDirectoryName(fileName);
            var newFileName = $"{directory}//{bibleVersion}_zefania.xml";
            newXmlDoc.Save(newFileName);


            Console.WriteLine($"{newFileName} -- Successfully Generated");
        }

    }

    internal class BibleFormatType
    {
        public string Key { get; set; }

        public string NameAttribute { get; internal set; }

        public string BookNumberAttribute { get; internal set; }

        public string ChapterNumberAttribute { get; internal set; }

        public string VerseNumberAttribute { get; internal set; }
    }
}
