using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace RobotArmHelix
{
     public class Model_SetPosition
    {
        public GeometryModel3D GetStandPositionData(GeometryModel3D model, Color color)
        {

            ModelImporter import = new ModelImporter();
            var materialGroup = new MaterialGroup();
            Color mainColor = color;
            EmissiveMaterial emissMat = new EmissiveMaterial(new SolidColorBrush(mainColor));
            DiffuseMaterial diffMat = new DiffuseMaterial(new SolidColorBrush(mainColor));
            SpecularMaterial specMat = new SpecularMaterial(new SolidColorBrush(mainColor), 200);
            materialGroup.Children.Add(emissMat);
            materialGroup.Children.Add(diffMat);
            materialGroup.Children.Add(specMat);



            var _axis = new Vector3D(1, 0, 0);
            var matrix = model.Transform.Value;
            matrix.Rotate(new Quaternion(_axis, 90));
            model.Transform = new MatrixTransform3D(matrix);//(new Quaternion(_axis, 90)); 
            model.Material = materialGroup;
            model.BackMaterial = materialGroup;
            return model;

        }

        public GeometryModel3D SetPositionData(GeometryModel3D model, Color color, double x, double y, double z)
        {

            ModelImporter import = new ModelImporter();
            var materialGroup = new MaterialGroup();
            Color mainColor = color;
            EmissiveMaterial emissMat = new EmissiveMaterial(new SolidColorBrush(mainColor));
            DiffuseMaterial diffMat = new DiffuseMaterial(new SolidColorBrush(mainColor));
            SpecularMaterial specMat = new SpecularMaterial(new SolidColorBrush(mainColor), 200);
            materialGroup.Children.Add(emissMat);
            materialGroup.Children.Add(diffMat);
            materialGroup.Children.Add(specMat);

            var my_point = new Point3D(x, y, z);


            TranslateTransform3D myTranslate = new TranslateTransform3D(my_point.X, my_point.Y, my_point.Z);



            model.Transform = myTranslate;//(new Quaternion(_axis, 90)); 

            model.Material = materialGroup;
            model.BackMaterial = materialGroup;

            //model.Position = new Point3D(0, 0, 0);
            return model;

        }
        public GeometryModel3D SetPosition(GeometryModel3D model, double x, double y, double z)
        {



            var my_point = new Point3D(x, y, z);


            TranslateTransform3D myTranslate = new TranslateTransform3D(my_point.X, my_point.Y, my_point.Z);



            model.Transform = myTranslate;//(new Quaternion(_axis, 90)); 



            //model.Position = new Point3D(0, 0, 0);
            return model;

        }
        public GeometryModel3D SetRotaion(GeometryModel3D model, double x, double y, double z, double red)
        {





            var _axis = new Vector3D(x, y, z);
            var matrix = model.Transform.Value;

            matrix.Rotate(new Quaternion(_axis, red));

            model.Transform = new MatrixTransform3D(matrix);//(new Quaternion(_axis, 90)); 



            //model.Position = new Point3D(0, 0, 0);
            return model;

        }
    }
}
