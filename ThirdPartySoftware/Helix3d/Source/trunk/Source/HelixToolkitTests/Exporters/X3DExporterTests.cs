using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Xml.Schema;
using HelixToolkit;
using NUnit.Framework;
using NUnitHelpers;

namespace HelixToolkitTests
{
    [TestFixture]
    public class X3DExporterTests
    {

        [Test]
        public void Export_SimpleModel_ValidOutput()
        {
            string path = "temp.x3d";

            var runner = new CrossThreadTestRunner();
            runner.RunInSTA(
              delegate
              {
                  Console.WriteLine(Thread.CurrentThread.GetApartmentState());

                  var vp = new Viewport3D();
                  vp.Camera = CameraHelper.CreateDefaultCamera();
                  vp.Width = 1280;
                  vp.Height = 720;
                  vp.Children.Add(new DefaultLightsVisual3D());
                  var box = new BoxVisual3D();
                  box.UpdateModel();
                  vp.Children.Add(box);

                  using (var e = new X3DExporter(path))
                  {
                      e.Export(vp);
                  }
              });

            var result = Validate(path);
            Assert.IsNull(result, result);
        }

        // http://msdn.microsoft.com/en-us/library/as3tta56.aspx
        string Validate(string path)
        {
            var sc = new XmlSchemaSet();

            // Add the schema to the collection.
            string dir = @"..\..\..\..\Schemas\x3d\";
            sc.Add("http://www.web3d.org/specifications/x3d-3.1.xsd", dir + "x3d-3.1.xsd");
            // sc.Add("http://www.web3d.org/specifications/x3d-3.1-Web3dExtensionsPublic.xsd", dir + "x3d-3.1-Web3dExtensionsPublic.xsd");
            // sc.Add("http://www.web3d.org/specifications/x3d-3.1-Web3dExtensionsPrivate.xsd", dir + "x3d-3.1-Web3dExtensionsPrivate.xsd");
            var settings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Document,
                DtdProcessing = DtdProcessing.Parse,
                ValidationType = ValidationType.Schema,
                Schemas = sc,
                ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ProcessInlineSchema,
            };

            int warnings = 0;
            int errors = 0;

            settings.ValidationEventHandler += (sender, e) =>
            {
                Console.WriteLine(e.Message);
                if (e.Severity == XmlSeverityType.Warning)
                {
                    warnings++;
                }
                else
                {
                    errors++;
                }
            };

            using (var input = File.OpenRead(path))
            {
                using (var xvr = XmlReader.Create(input, settings))
                {
                    while (xvr.Read())
                    {
                        // do nothing
                    }

                    if (errors + warnings == 0)
                    {
                        return null;
                    }

                    return String.Format("Errors: {0}, Warnings: {1}", errors, warnings);
                    /*
                    catch (XmlSchemaException e)
                    {
                        Console.Error.WriteLine("Failed to read XML: {0}", e.Message);

                    }
                    catch (XmlException e)
                    {
                        Console.Error.WriteLine("XML Error: {0}", e.Message);

                    }
                    catch (IOException e)
                    {
                        Console.Error.WriteLine("IO error: {0}", e.Message);
                    }*/
                }
            }
        }
    }
}