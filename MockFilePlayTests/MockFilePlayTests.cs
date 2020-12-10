using NUnit.Framework;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using MockFilePlay;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Reflection;

namespace MockFilePlayTests
{
    public class MockFilePlayTests // : MyComponent
    {
        IFileSystem theFileSystem;

        private static MockFileSystem BuildFileSystem()
        {
            string specXml = System.Environment.CurrentDirectory + @"\Config\Data.xml";

            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"d:\myfile.txt", new MockFileData("Hello Mike!!!  How you doing?") },
                { @"d:\demo\jQuery.js", new MockFileData("some js") },
                { @"d:\demo\image.gif", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                { @"demo\MyFile.txt", new MockFileData("All is relatively well...") },
                { @"Config\Data.xml", new MockFileData(XMLContents01()) }
            });

            //var n = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            //Console.WriteLine(n);

            return fileSystem;
        }

        private static string XMLContents01()
        {
            const string xml = @"<?xml version=""1.0""?>
                <XML id=""meter"">
                    <DATA>
                        <Email_Sch/>
                        <Email_Alarm/>
                        <SRL_NUM>111100409D4E9858</SRL_NUM>
                        <METER_ID>ACME MFG PLANT</METER_ID>
                        <MAC>00:04:D9:C4:E0:5F</MAC>
                        <METER_ADDR>100 Industrial Rd, Big Town, AR</METER_ADDR>
                        <METER_TIME>11/18/14 15:00 Tuesday</METER_TIME>
                        <IP>127.0.0.191</IP>
                        <TSF>1</TSF>
                        <MODEL>WEPM</MODEL>
                        <VER>112512_WEPM_ACE</VER>
                        <SB>
                            <TS>11/18/2014 15:00:00</TS>
                            <C0>100</C0>
                            <C1>0</C1>
                            <C2>0</C2>
                            <C3>0</C3>
                        </SB>

                    </DATA>
                </XML>
                ";
            return xml;
        }

        [SetUp]
        public void Setup()
        {
            theFileSystem = BuildFileSystem();
        }

        [Test]
        public void MyComponent_Validate_ShouldThrowNotSupportedExceptionIfTestingIsNotAwesome()
        {
            // Arrange
            var component = new MyComponent(theFileSystem);

            try
            {
                // Act
                component.Validate();
            }
            catch (NotSupportedException ex)
            {
                // Assert
                Assert.AreEqual("We can't go on together. It's not me, it's you.", ex.Message);
                return;
            }

            Assert.Fail("The expected exception was not thrown.");
        }

        [Test]
        public void MyComponent_GetMyFile()
        {
            // Arrange
            var component = new MyComponent(theFileSystem);

            // Act
            var result = component.GetMyFile();

            // Assert
            Assert.That(result, Is.EqualTo("All is relatively well..."));
        }

        [Test]
        public void MyComponent_PrivideXML()
        {
            // Arrange
            var component = new MyComponent(theFileSystem);
            theFileSystem.File.WriteAllText(@"d:\demo\Data.xml", XMLContents01());

            // Act
            var result = component.ProcessXML();

            // Assert
            Assert.That(result, Contains.Substring("<SRL_NUM>111100409D4E9858</SRL_NUM>"));
        }

        [Test]
        public void MyComponent_CallBuildManifest()
        {
            // Arrange
            var component = new MyComponent(theFileSystem);
            string[] files = new string[] { "a.txt", "b.xml" };

            // Act
            var result = component.CallBuildManifest(files, "MAC123123", @"config");

            var alltext = theFileSystem.File.ReadAllText(result);
            Console.WriteLine(alltext);

            Assert.That(alltext, Contains.Substring("2) File:b.xml"));
            Assert.That(result, Contains.Substring("MAC123123"));
        }


        [Test]
        public void MyComponent_BuildManifest()
        {
            // Arrange
            //var component = new MyComponent(theFileSystem);
            string[] files = new string[] { "a.txt", "b.xml" };

            MethodInfo buildManifest = typeof(MyComponent).GetMethod("BuildManifest", BindingFlags.NonPublic | BindingFlags.Instance);

            object[] args = new object[] { files, "MAC123123", @"config" };

            // Act
            //var result = component.CallBuildManifest(files, "MAC123123", @"config");
            var result = (string)buildManifest.Invoke(new MyComponent(theFileSystem), args);

            var alltext = theFileSystem.File.ReadAllText(result);
            Console.WriteLine(alltext);

            Assert.That(alltext, Contains.Substring("2) File:b.xml"));
            Assert.That(result, Contains.Substring("MAC123123"));
        }

        [Test]
        public void PrivateMethod_Sum()
        {
            MethodInfo sumPrivate = typeof(MyComponent).GetMethod("Sum", BindingFlags.NonPublic | BindingFlags.Instance);

            int sum = (int)sumPrivate.Invoke(new MyComponent(theFileSystem), new object[] { 6, 5 });

            Assert.That(sum, Is.EqualTo(11));
        }
    }
}
