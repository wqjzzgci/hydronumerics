using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using HelixToolkit;
using Microsoft.Win32;

namespace StudioDemo
{
    public class MainViewModel : Observable
    {
        public ICommand FileOpenCommand { get; set; }
        public ICommand FileExportCommand { get; set; }
        public ICommand FileExitCommand { get; set; }
        public ICommand HelpAboutCommand { get; set; }
        public ICommand ViewZoomToFitCommand { get; set; }
        public ICommand EditCopyXamlCommand { get; set; }

        private const string OpenFileFilter = "3D model files (*.3ds;*.obj;*.lwo;*.stl)|*.3ds;*.obj;*.objz;*.lwo;*.stl";
        private const string TitleFormatString = "3D model viewer - {0}";

        private string _currentModelPath;
        public string CurrentModelPath
        {
            get { return _currentModelPath; }
            set { _currentModelPath = value; RaisePropertyChanged("CurrentModelPath"); }
        }

        private string _applicationTitle;
        public string ApplicationTitle
        {
            get { return _applicationTitle; }
            set { _applicationTitle = value; RaisePropertyChanged("ApplicationTitle"); }
        }

        private double expansion;

        public double Expansion
        {
            get { return expansion; }
            set
            {
                if (expansion != value)
                {
                    expansion = value;
                    RaisePropertyChanged("Expansion");
                }
            }
        }

        private IFileDialogService FileDialogService;
        public IHelixView3D HelixView { get; set; }

        private Model3D _currentModel;

        public Model3D CurrentModel
        {
            get { return _currentModel; }
            set { _currentModel = value; RaisePropertyChanged("CurrentModel"); }
        }

        public MainViewModel(IFileDialogService fds, IHelixView3D hv)
        {
            Expansion = 1;
            FileDialogService = fds;
            HelixView = hv;
            FileOpenCommand = new DelegateCommand(FileOpen);
            FileExportCommand = new DelegateCommand(FileExport);
            FileExitCommand = new DelegateCommand(FileExit);
            ViewZoomToFitCommand = new DelegateCommand(ViewZoomToFit);
            EditCopyXamlCommand = new DelegateCommand(CopyXaml);
            ApplicationTitle = "3D Model viewer";
        }

        private void FileExit()
        {
            App.Current.Shutdown();
        }

        private void FileExport()
        {
            var path = FileDialogService.SaveFileDialog(null, null, Exporters.Filter, ".png");
            if (path == null)
                return;
            HelixView.Export(path);
            /*
                        var ext = Path.GetExtension(path).ToLowerInvariant();
                        switch (ext)
                        {
                            case ".png":
                            case ".jpg":
                                HelixView.Export(path);
                                break;
                            case ".xaml":
                                {
                                    var e = new XamlExporter(path);
                                    e.Export(CurrentModel);
                                    e.Close();
                                    break;
                                }

                            case ".xml":
                                {
                                    var e = new KerkytheaExporter(path);
                                    e.Export(HelixView.Viewport);
                                    e.Close();
                                    break;
                                }
                            case ".obj":
                                {
                                    var e = new ObjExporter(path);
                                    e.Export(CurrentModel);
                                    e.Close();
                                    break;
                                }
                            case ".objz":
                                {
                                    var tmpPath = Path.ChangeExtension(path, ".obj");
                                     var e = new ObjExporter(tmpPath);
                                     e.Export(CurrentModel);
                                     e.Close();
                                    GZipHelper.Compress(tmpPath);
                                    break;
                                }
                            case ".x3d":
                                {
                                    var e = new X3DExporter(path);
                                    e.Export(CurrentModel);
                                    e.Close();
                                    break;
                                }
                        }*/
        }

        private void CopyXaml()
        {
            var rd = XamlExporter.WrapInResourceDictionary(CurrentModel);
            Clipboard.SetText(XamlHelper.GetXaml(rd));
        }

        private void ViewZoomToFit()
        {
            HelixView.ZoomToFit(500);
        }

        private void FileOpen()
        {
            CurrentModelPath = FileDialogService.OpenFileDialog("models", null, OpenFileFilter, ".3ds");
#if !DEBUG
            try
            {
#endif
                CurrentModel = ModelImporter.Load(CurrentModelPath);
                ApplicationTitle = String.Format(TitleFormatString, CurrentModelPath);
                HelixView.ZoomToFit(0);
#if !DEBUG
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
#endif
        }
    }

    public interface IFileDialogService
    {
        string OpenFileDialog(string initialDirectory, string defaultPath, string filter, string defaultExtension);
        string SaveFileDialog(string initialDirectory, string defaultPath, string filter, string defaultExtension);
    }

    public class FileDialogService : IFileDialogService
    {
        public string OpenFileDialog(string initialDirectory, string defaultPath, string filter, string defaultExtension)
        {
            var d = new OpenFileDialog();
            d.InitialDirectory = initialDirectory;
            d.FileName = defaultPath;
            d.Filter = filter;
            d.DefaultExt = defaultExtension;
            if (!d.ShowDialog().Value)
                return null;
            return d.FileName;
        }

        public string SaveFileDialog(string initialDirectory, string defaultPath, string filter, string defaultExtension)
        {
            var d = new SaveFileDialog();
            d.InitialDirectory = initialDirectory;
            d.FileName = defaultPath;
            d.Filter = filter;
            d.DefaultExt = defaultExtension;
            if (!d.ShowDialog().Value)
                return null;
            return d.FileName;
        }
    }
}