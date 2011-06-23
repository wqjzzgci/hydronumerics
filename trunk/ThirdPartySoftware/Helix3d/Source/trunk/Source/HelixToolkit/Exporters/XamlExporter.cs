using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using System.Xml;

namespace HelixToolkit
{
    /// <summary>
    /// Export Viewport or model to XAML.
    /// </summary>
    public class XamlExporter : IExporter, IDisposable
    {
        private XmlTextWriter xw;
        public bool CreateResourceDictionary { get; set; }

        public XamlExporter(string path)
        {
            CreateResourceDictionary = true;
            xw = new XmlTextWriter(path, Encoding.UTF8) { Formatting = Formatting.Indented };
        }

        public void Close()
        {
            xw.Close();
        }

        public void Export(Viewport3D viewport)
        {
            object obj = viewport;
            if (CreateResourceDictionary)
                obj = WrapInResourceDictionary(obj);

            XamlWriter.Save(obj, xw);
        }

        public void Export(Visual3D visual)
        {
            object obj = visual;
            if (CreateResourceDictionary)
                obj = WrapInResourceDictionary(obj);
            XamlWriter.Save(obj, xw);
        }

        public void Export(Model3D model)
        {
            object obj = model;
            if (CreateResourceDictionary)
                obj = WrapInResourceDictionary(obj);
            XamlWriter.Save(obj, xw);
        }

        public static ResourceDictionary WrapInResourceDictionary(object obj)
        {
            var rd = new ResourceDictionary();
            var list = obj as IEnumerable;
            if (list != null)
            {
                int i = 1;
                foreach (var o in list)
                {
                    rd.Add("Model" + i, o);
                    i++;
                }
            }
            else
            {
                rd.Add("Model", obj);
            }
            return rd;
        }


        public void Dispose()
        {
            Close();
        }
    }
}