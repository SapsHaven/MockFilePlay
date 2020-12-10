using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Xml;

[assembly: InternalsVisibleToAttribute("MockFilePlayTests")]  // Work on this later...

namespace MockFilePlay
{
    class Program
    {
        static void Main()
        {
            var mc = new MyComponent();

            mc.Validate();

            Console.WriteLine(mc.GetMyFile());

            Console.WriteLine(mc.ProcessXML());

            string[] files = new string[] { "a.txt", "b.xml" };

            mc.CallBuildManifest(files, "MAC123123", @"config");
        }
    }

    public class MyComponent    // sut
    {
        readonly IFileSystem fileSystem;
        //private readonly INowProvider _nowProvider;

        // <summary>Create MyComponent with the given fileSystem implementation</summary>
        public MyComponent(IFileSystem fileSystem) //, INowProvider _nowProvider)
        {
            this.fileSystem = fileSystem;
            //this._nowProvider = _nowProvider;
        }

        /// <summary>Create MyComponent</summary>
        public MyComponent() : this(
            fileSystem: new FileSystem() //use default implementation which calls System.IO
                                         //_nowProvider: new NowProvider()
        )
        {
            //_nowProvider = new INowProvider();
        }

        public void Validate()
        {
            foreach (var textFile in fileSystem.Directory.GetFiles(@"d:\", "*.txt", SearchOption.TopDirectoryOnly))
            {
                var text = fileSystem.File.ReadAllText(textFile);
                if (text != "Hello Mike!")
                    throw new NotSupportedException("We can't go on together. It's not me, it's you.");
            }
        }

        public string GetMyFile()
        {
            var text = fileSystem.File.ReadAllText(@"Demo\MyFile.txt");

            return text;
        }

        public string ProcessXML()
        {
            //string spec = System.Environment.CurrentDirectory + @"\Config\Data.xml";
            var text = fileSystem.File.ReadAllText(@"Config\Data.xml");

            // Load xml from string.
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);

            // Load xml from stream.
            Stream fileStream = fileSystem.File.OpenRead(@"Config\Data.xml");
            XmlDocument doc2 = ConvertStreamToXmlDoc(fileStream);

            return text;
        }

        internal static XmlDocument ConvertStreamToXmlDoc(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            XmlDocument doc = new XmlDocument();
            doc.Load(stream);

            return doc;
        }

        public string CallBuildManifest(string[] uploadFileList, string serialNumber, string subDir)
        {
            return BuildManifest(uploadFileList, serialNumber, subDir);
        }

        private string BuildManifest(string[] uploadFileList, string serialNumber, string subDir)
        {
            string filePathName = Path.Combine(subDir, $@"{serialNumber}eAgentManifest.txt");

            try
            {
                // using (StreamWriter sw = new StreamWriter(filePathName))
                var file = fileSystem.FileInfo.FromFileName(filePathName);
                using (StreamWriter sw = file.CreateText())
                {
                    int x = 1;
                    sw.WriteLine($@"File Manifest created on {DateTime.Now:o}");
                    foreach (string li in uploadFileList)
                    {
                        sw.WriteLine($@"{x++}) File:{li} ");
                    }
                    sw.WriteLine("End Of File");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                //_Log?.Error($@"Error building manifest: {ex.Message}", ex);
                Console.WriteLine(ex);
                filePathName = null;
            }

            return filePathName;
        }

        private int Sum(int num1, int num2)
        {
            return num1 + num2;
        }

    }

    public class NowProvider : INowProvider
    {
        public DateTime GetNow()
        {
            return DateTime.Now;
        }
    }
}
