using HelixToolkit.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using WPFCADViewer.Models;

namespace WPFCADViewer
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Private Varibles
        private HelixViewport3D _hvp;
        #endregion

        public MainWindowViewModel()
        {
            this.LoadObjFileCommand = new RelayCommand(OnLoadObjFileExecuted);
        }
        
        #region Commands
        public ICommand LoadObjFileCommand { get; set; }
        #endregion

        #region Properties  
        public HelixViewport3D Hvp
        {
            get { return _hvp; }
            set 
            {
                _hvp = value;
                this.NotifyPropertyChanged();
            }
        }

        #endregion

        #region Private Methods
        #region Command Handlers
        private void OnLoadObjFileExecuted(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "OBJ files (*.obj)|*.obj|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3DModels");
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    ObjReader objReader = new ObjReader();
                    var model3dGroup = objReader.Read(openFileDialog.FileName);
                    ModelVisual3D modelVisual3D = new ModelVisual3D { Content = model3dGroup };

                    this.Hvp.Children.Clear();
                    this.Hvp.Children.Add(new SunLight());
                    this.Hvp.Children.Add(modelVisual3D);
                    this.SetView(new Vector3D(0, 0, -1), new Vector3D(0, 1, 0));
                }
                catch { }
            }
        }

        #endregion

        private void SetView(Vector3D lookDir, Vector3D upDir, double distance = 1000)
        {
            var center = new Point3D(0, 0, 0);
            if (lookDir.LengthSquared < 1e-12) lookDir = new Vector3D(0, 0, -1);
            lookDir.Normalize();
            if (upDir.LengthSquared < 1e-12) upDir = new Vector3D(0, 1, 0);
            upDir.Normalize();

            var position = center - lookDir * distance;

            if (this.Hvp.Camera is ProjectionCamera pc)
            {
                pc.Position = position;
                pc.LookDirection = lookDir * distance;
                pc.UpDirection = upDir;
            }
            this.Hvp.ZoomExtents();
        }
        #endregion
    }
}
