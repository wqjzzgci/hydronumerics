using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace HelixToolkit
{
    public class XamlHelper
    {
        public static string GetXaml(Viewport3D view)
        {
            var sb = new StringBuilder();
            var tw = new StringWriter(sb);
            var xw = new XmlTextWriter(tw) { Formatting = Formatting.Indented };
            XamlWriter.Save(view, xw);
            xw.Close();
            string xaml = sb.ToString();

            xaml = xaml.Replace(string.Format(
                "<Viewport3D Height=\"{0}\" Width=\"{1}\" ",
                view.ActualHeight, view.ActualWidth),
                                "<Viewport3D ");

            return xaml;
        }

        public static string GetXaml(object obj)
        {
            var sb = new StringBuilder();
            var tw = new StringWriter(sb);
            var xw = new XmlTextWriter(tw) { Formatting = Formatting.Indented };
            XamlWriter.Save(obj, xw);
            xw.Close();
            return sb.ToString();
        }
    }
}