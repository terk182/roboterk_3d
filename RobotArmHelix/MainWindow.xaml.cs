﻿#define IRB6700

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;
using System.IO;


/**
 * Author: Gabriele Marini (Gabryxx7)
 * This class load the 3d models of all the parts of the robotic arms and add them to the viewport
 * It also defines the relations among the joints of the robotic arms in order to reflect the movement of the robot in the real world
 * **/
namespace RobotArmHelix
{
    class Joint
    {
        public Model3D model = null;
        public double angle = 0;
        public double angleMin = -180;
        public double angleMax = 180;
        public int rotPointX = 0;
        public int rotPointY = 0;
        public int rotPointZ = 0;
        public int rotAxisX = 0;
        public int rotAxisY = 0;
        public int rotAxisZ = 0;

        public Joint(Model3D pModel)
        {
            model = pModel;
        }
    }

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainWindow : Window
   { 
        //provides functionality to 3d models
        Model3DGroup RA = new Model3DGroup(); //RoboticArm 3d group
        Model3D geom = null; //Debug sphere to check in which point the joint is rotatin

        List<Joint> joints = null;

        bool switchingJoint = false;
        bool isAnimating = false;

        Color oldColor = Colors.White;
        GeometryModel3D oldSelectedModel = null;
        string basePath = "";
        ModelVisual3D visual;
        double LearningRate = 0.01;
        double SamplingDistance = 0.15;
        double DistanceThreshold = 20;
        //provides render to model3d objects
        ModelVisual3D RoboticArm = new ModelVisual3D();
        Transform3DGroup F1;
        Transform3DGroup F2;
        Transform3DGroup F3;
        Transform3DGroup F3_1;
        Transform3DGroup F4;
        Transform3DGroup F5;
        Transform3DGroup F6;

        Transform3DGroup F7;
        Transform3DGroup F8;
        Transform3DGroup F9;
        Transform3DGroup F10;   
        RotateTransform3D R;
        TranslateTransform3D T;
        Vector3D reachingPoint;
        int movements = 10;
        System.Windows.Forms.Timer timer1;

#if IRB6700
        //directroy of all stl files
        private const string MODEL_PATH1 = "Base.stl";
        private const string MODEL_PATH2 = "L1_link.stl";
        private const string MODEL_PATH3 = "L2_link.stl";
        private const string MODEL_PATH4 = "base_griper.stl";
        private const string MODEL_PATH5 = "L1_link.stl";
        private const string MODEL_PATH6 = "L1_link.stl";
        private const string MODEL_PATH7 = "3Link V2.stl";
        private const string MODEL_PATH8 = "sub_link1.stl";
        private const string MODEL_PATH9 = "Sub_Link2 V2.stl";
        private const string MODEL_PATH10 = "IRB6700-MH3_245-300_IRC5_rev00_LINK02m_CABLE.stl";
        private const string MODEL_PATH11 = "IRB6700-MH3_245-300_IRC5_rev00_LINK03a_CABLE.stl";
        private const string MODEL_PATH12 = "IRB6700-MH3_245-300_IRC5_rev00_LINK03b_CABLE.stl";
        private const string MODEL_PATH13 = "IRB6700-MH3_245-300_IRC5_rev02_LINK03m_CABLE.stl";
        private const string MODEL_PATH14 = "IRB6700-MH3_245-300_IRC5_rev01_LINK04_CABLE.stl";
        private const string MODEL_PATH15 = "IRB6700-MH3_245-300_IRC5_rev00_ROD_CAD.stl";
        private const string MODEL_PATH16 = "IRB6700-MH3_245-300_IRC5_rev00_LOGO1_CAD.stl";
        private const string MODEL_PATH17 = "IRB6700-MH3_245-300_IRC5_rev00_LOGO2_CAD.stl";
        private const string MODEL_PATH18 = "subport_l1.stl";
        private const string MODEL_PATH19 = "Stand.stl";
        private const string MODEL_PATH20 = "MoveBaseV2.stl";
#else

        private const string MODEL_PATH1 = "IRB4600_20kg-250_LINK1_CAD_rev04.stl";
        private const string MODEL_PATH2 = "IRB4600_20kg-250_LINK2_CAD_rev04.stl";
        private const string MODEL_PATH3 = "IRB4600_20kg-250_LINK3_CAD_rev005.stl";
        private const string MODEL_PATH4 = "IRB4600_20kg-250_LINK4_CAD_rev04.stl";
        private const string MODEL_PATH5 = "IRB4600_20kg-250_LINK5_CAD_rev04.stl";
        private const string MODEL_PATH6 = "IRB4600_20kg-250_LINK6_CAD_rev04.stl";
        private const string MODEL_PATH7 = "IRB4600_20kg-250_LINK3_CAD_rev04.stl";
        private const string MODEL_PATH8 = "IRB4600_20kg-250_CABLES_LINK1_rev03.stl";
        private const string MODEL_PATH9 = "IRB4600_20kg-250_CABLES_LINK2_rev03.stl";
        private const string MODEL_PATH10 = "IRB4600_20kg-250_CABLES_LINK3_rev03.stl";
        private const string MODEL_PATH11 = "IRB4600_20kg-250_BASE_CAD_rev04.stl";
#endif


        public MainWindow()
        {
            InitializeComponent();
            basePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\3D_Models\\";
            List<string> modelsNames = new List<string>();
            modelsNames.Add(MODEL_PATH1);
            modelsNames.Add(MODEL_PATH2);
            modelsNames.Add(MODEL_PATH3);
            modelsNames.Add(MODEL_PATH4);
            modelsNames.Add(MODEL_PATH5);
            modelsNames.Add(MODEL_PATH6);
            modelsNames.Add(MODEL_PATH7);
            modelsNames.Add(MODEL_PATH8);
            modelsNames.Add(MODEL_PATH9);
            modelsNames.Add(MODEL_PATH10);
            modelsNames.Add(MODEL_PATH11);//Until here for the 4600
#if IRB6700
            modelsNames.Add(MODEL_PATH12);
            modelsNames.Add(MODEL_PATH13);
            modelsNames.Add(MODEL_PATH14);
            modelsNames.Add(MODEL_PATH15);
            modelsNames.Add(MODEL_PATH16);
            modelsNames.Add(MODEL_PATH17);
            modelsNames.Add(MODEL_PATH18);
            modelsNames.Add(MODEL_PATH19);
            modelsNames.Add(MODEL_PATH20);
#endif
            RoboticArm.Content = Initialize_Environment(modelsNames);

            /** Debug sphere to check in which point the joint is rotating**/
            var builder = new MeshBuilder(true, true);
            var position = new Point3D(0, 0, 0);
            builder.AddSphere(position, 10, 10, 10);
            geom = new GeometryModel3D(builder.ToMesh(), Materials.Brown);
            visual = new ModelVisual3D();
            visual.Content = geom;

            viewPort3d.RotateGesture = new MouseGesture(MouseAction.RightClick);
            viewPort3d.PanGesture = new MouseGesture(MouseAction.LeftClick);
            viewPort3d.Children.Add(visual);
            viewPort3d.Children.Add(RoboticArm);
            viewPort3d.Camera.LookDirection = new Vector3D(1843, -305, -788);
            viewPort3d.Camera.UpDirection = new Vector3D(-0.468, 0.078, 0.880);
            viewPort3d.Camera.Position = new Point3D(-1620, 440, 800);

            double[] angles = { joints[0].angle, joints[1].angle, joints[2].angle, joints[3].angle, joints[4].angle, joints[5].angle };
            ForwardKinematics(angles);

            changeSelectedJoint();

            timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 5;
            timer1.Tick += new System.EventHandler(timer1_Tick);
        }

        private Model3DGroup Initialize_Environment(List<string> modelsNames)
        {
            try
            {
                ModelImporter import = new ModelImporter();
                joints = new List<Joint>();
                var f = new Model_SetPosition();
                foreach (string modelName in modelsNames)
                {
                    var materialGroup = new MaterialGroup();
                    Color mainColor = Colors.White;
                    EmissiveMaterial emissMat = new EmissiveMaterial(new SolidColorBrush(mainColor));
                    DiffuseMaterial diffMat = new DiffuseMaterial(new SolidColorBrush(mainColor));
                    SpecularMaterial specMat = new SpecularMaterial(new SolidColorBrush(mainColor), 200);
                    materialGroup.Children.Add(emissMat);
                    materialGroup.Children.Add(diffMat);
                    materialGroup.Children.Add(specMat);

                    var link = import.Load(basePath + modelName);
                    GeometryModel3D model = link.Children[0] as GeometryModel3D;
                    model.Material = materialGroup;
                    model.BackMaterial = materialGroup;
                    if(modelName == "Stand.stl")
                    {
                        model = f.SetPositionData(model, Colors.Red, -60, 0, -60);
                        model = f.SetRotaion(model, 1, 0, 0, 90);
                    }
                    else if (modelName == "Base.stl")
                    {
                        model = f.SetPositionData(model, Colors.White, -80, 90, -35);
                        model = f.SetRotaion(model, 1, 0, 0, 90);
                        model = f.SetRotaion(model, 0, 0, 1, 180);

                    }
                    else if (modelName == "MoveBaseV2.stl")
                    {
                        model = f.SetPositionData(model, Colors.White, -88, -90, -45);
                        model = f.SetRotaion(model, 1, 0, 0, 90);
                        model = f.SetRotaion(model, 1, 0, 0, 180);
                        model = f.SetRotaion(model, 0, 0, 1, 180);

                    }
                    else if (modelName == "subport_l1.stl")
                    {
                        model = f.SetPositionData(model, Colors.White, 25, 90, -38);
                        model = f.SetRotaion(model, 1, 0, 0, 90);
                        model = f.SetRotaion(model, 0, 0, 1, 270);
                        //model = f.SetRotaion(model, 0, 0, 1, 180);

                    }
                    else if (modelName == "3Link V2.stl")
                    {
                        model = f.SetPositionData(model, Colors.White, -62, 277, -27);
                        model = f.SetRotaion(model, 1, 0, 0, 90);
                        model = f.SetRotaion(model, 0, 0, 1, 270);
                        model = f.SetRotaion(model, 1, 0, 0, 3);

                    }
                    else if (modelName == "sub_link1.stl")
                    {
                        model = f.SetPositionData(model, Colors.White, -62, 163, 28);
                        model = f.SetRotaion(model, 1, 0, 0, 90);
                        model = f.SetRotaion(model, 0, 0, 1, 90);
                     //   model = f.SetRotaion(model, 1, 0, 0, -2);

                    }
                    else if (modelName == "L1_link.stl")
                    {
                        model = f.SetPositionData(model, Colors.Green, -13.5, 136,  -15.5);
                        model = f.SetRotaion(model, 0, 1, 0, 90);
                        model = f.SetRotaion(model, 1, 0, 0, 90);
                    }
                    else if (modelName == "base_griper.stl")
                    {
                        model = f.SetPositionData(model, Colors.Green, -185, 207, -27);
                        model = f.SetRotaion(model, 1, 0, 0, 90);
                        model = f.SetRotaion(model, 0, 0, 1, -90);
                        //model = f.SetRotaion(model, 1, 0, 0, -30);

                    }
                    else if (modelName == "Sub_Link2 V2.stl")
                    {
                        model = f.SetPositionData(model, Colors.Green, 0, 262, -35);
                        model = f.SetRotaion(model, 1, 0, 0, 90);
                        model = f.SetRotaion(model, 0, 0, 1, -90);
                        model = f.SetRotaion(model, 1, 0, 0, -36);

                    }
                    else if (modelName == "L2_link.stl")
                    {
                        model = f.SetPositionData(model, Colors.Green, -16.8, 239, -10);
                        model = f.SetRotaion(model, 1, 0, 0, 90);
                        model = f.SetRotaion(model, 0, 0, 1, -90);
                        model = f.SetRotaion(model, 1, 0, 0, -30);

                    }
                    else
                    {
                        model = f.SetRotaion(model, 1, 0, 0, 90);
                    }
                    joints.Add(new Joint(link));
                }

                RA.Children.Add(joints[0].model);
                RA.Children.Add(joints[1].model);
                RA.Children.Add(joints[2].model);
                RA.Children.Add(joints[3].model);
                //RA.Children.Add(joints[4].model);
                //RA.Children.Add(joints[5].model);
                //RA.Children.Add(joints[6].model);
                //RA.Children.Add(joints[7].model);
                //RA.Children.Add(joints[8].model);
                //RA.Children.Add(joints[9].model);
                //RA.Children.Add(joints[10].model);
#if IRB6700
                //RA.Children.Add(joints[11].model);
                //RA.Children.Add(joints[12].model);
                //RA.Children.Add(joints[13].model);
                //RA.Children.Add(joints[14].model);
                //RA.Children.Add(joints[15].model);
                //RA.Children.Add(joints[16].model);
                RA.Children.Add(joints[17].model);
                RA.Children.Add(joints[18].model);
                RA.Children.Add(joints[19].model);
#endif

#if IRB6700
                Color cableColor = Colors.DarkSlateGray;
                changeModelColor(joints[6], cableColor);
                changeModelColor(joints[7], cableColor);
                changeModelColor(joints[8], cableColor);
                changeModelColor(joints[9], cableColor);
                changeModelColor(joints[10], cableColor);
                changeModelColor(joints[11], cableColor);
                changeModelColor(joints[12], cableColor);
                changeModelColor(joints[13], cableColor);

                changeModelColor(joints[14], Colors.Gray);

                changeModelColor(joints[15], Colors.Red);
                changeModelColor(joints[16], Colors.Red);
                changeModelColor(joints[17], Colors.Red);

                changeModelColor(joints[18], Colors.Gray);
                changeModelColor(joints[19], Colors.Gray);

                joints[0].angleMin = -180;
                joints[0].angleMax = 180;
                joints[0].rotAxisX = 0;
                joints[0].rotAxisY = 0;
                joints[0].rotAxisZ = 1;
                joints[0].rotPointX = 0;
                joints[0].rotPointY = 0;
                joints[0].rotPointZ = 0;

                joints[1].angleMin = 0;
                joints[1].angleMax = 0;
                joints[1].rotAxisX = 1;
                joints[1].rotAxisY = 0;
                joints[1].rotAxisZ = 0;
                joints[1].rotPointX = 0;
                joints[1].rotPointY = 0;
                joints[1].rotPointZ = 150;

                joints[2].angleMin = 0;
                joints[2].angleMax = 0;
                joints[2].rotAxisX = 1;
                joints[2].rotAxisY = 0;
                joints[2].rotAxisZ = 0;
                joints[2].rotPointX = 0;
                joints[2].rotPointY = 2;
                joints[2].rotPointZ = 287;

                joints[3].angleMin = -180;
                joints[3].angleMax = 180;
                joints[3].rotAxisX = 1;
                joints[3].rotAxisY = 0;
                joints[3].rotAxisZ = 0;
                joints[3].rotPointX = -8;
                joints[3].rotPointY = 131;
                joints[3].rotPointZ = 213;

                joints[4].angleMin = -115;
                joints[4].angleMax = 115;
                joints[4].rotAxisX = 0;
                joints[4].rotAxisY = 1;
                joints[4].rotAxisZ = 0;
                joints[4].rotPointX = 0;
                joints[4].rotPointY = 0;
                joints[4].rotPointZ = 0;

                joints[5].angleMin = -180;
                joints[5].angleMax = 180;
                joints[5].rotAxisX = 1;
                joints[5].rotAxisY = 0;
                joints[5].rotAxisZ = 0;
                joints[5].rotPointX = 0;
                joints[5].rotPointY = 0;
                joints[5].rotPointZ = 0;


                joints[17].angleMin = -180;
                joints[17].angleMax = 180;
                joints[17].rotAxisX = 1;
                joints[17].rotAxisY = 0;
                joints[17].rotAxisZ = 0;
                joints[17].rotPointX = 29;
                joints[17].rotPointY = -33;
                joints[17].rotPointZ = 173;
#else
                changeModelColor(joints[6], Colors.Red);
                changeModelColor(joints[7], Colors.Black);
                changeModelColor(joints[8], Colors.Black);
                changeModelColor(joints[9], Colors.Black);
                changeModelColor(joints[10], Colors.Gray);

                RA.Children.Add(joints[0].model);
                RA.Children.Add(joints[1].model);
                RA.Children.Add(joints[2].model);
                RA.Children.Add(joints[3].model);
                RA.Children.Add(joints[4].model);
                RA.Children.Add(joints[5].model);
                RA.Children.Add(joints[6].model);
                RA.Children.Add(joints[7].model);
                RA.Children.Add(joints[8].model);
                RA.Children.Add(joints[9].model);
                RA.Children.Add(joints[10].model);
                
                joints[0].angleMin = -180;
                joints[0].angleMax = 180;
                joints[0].rotAxisX = 0;
                joints[0].rotAxisY = 0;
                joints[0].rotAxisZ = 1;
                joints[0].rotPointX = 0;
                joints[0].rotPointY = 0;
                joints[0].rotPointZ = 0;

                joints[1].angleMin = -100;
                joints[1].angleMax = 60;
                joints[1].rotAxisX = 0;
                joints[1].rotAxisY = 1;
                joints[1].rotAxisZ = 0;
                joints[1].rotPointX = 175; 
                joints[1].rotPointY = -200;
                joints[1].rotPointZ = 500;

                joints[2].angleMin = -90;
                joints[2].angleMax = 90;
                joints[2].rotAxisX = 0;
                joints[2].rotAxisY = 1;
                joints[2].rotAxisZ = 0;
                joints[2].rotPointX = 190;
                joints[2].rotPointY = -700;
                joints[2].rotPointZ = 1595;

                joints[3].angleMin = -180;
                joints[3].angleMax = 180;
                joints[3].rotAxisX = 1;
                joints[3].rotAxisY = 0;
                joints[3].rotAxisZ = 0;
                joints[3].rotPointX = 400;
                joints[3].rotPointY = 0;
                joints[3].rotPointZ = 1765;

                joints[4].angleMin = -115;
                joints[4].angleMax = 115;
                joints[4].rotAxisX = 0;
                joints[4].rotAxisY = 1;
                joints[4].rotAxisZ = 0;
                joints[4].rotPointX = 1405;
                joints[4].rotPointY = 50;
                joints[4].rotPointZ = 1765;

                joints[5].angleMin = -180;
                joints[5].angleMax = 180;
                joints[5].rotAxisX = 1;
                joints[5].rotAxisY = 0;
                joints[5].rotAxisZ = 0;
                joints[5].rotPointX = 1405;
                joints[5].rotPointY = 0;
                joints[5].rotPointZ = 1765;
#endif
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception Error:" + e.StackTrace);
            }
            return RA;
        }

        public static T Clamp<T>(T value, T min, T max)
            where T : System.IComparable<T>
                {
                    T result = value;
                    if (value.CompareTo(max) > 0)
                        result = max;
                    if (value.CompareTo(min) < 0)
                        result = min;
                    return result;
                }

        private void ReachingPoint_TextChanged(object sender, TextChangedEventArgs e)
        {
            //try
            //{
            //    reachingPoint = new Vector3D(Double.Parse(TbX.Text), Double.Parse(TbY.Text), Double.Parse(TbZ.Text));
            //    geom.Transform = new TranslateTransform3D(reachingPoint);
            //}
            //catch (Exception exc)
            //{

            //}
        }

        private void jointSelector_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            changeSelectedJoint();
        }

        private void changeSelectedJoint()
        {
            if (joints == null)
                return;

            int sel = ((int)jointSelector.Value) - 1;
            switchingJoint = true;
            unselectModel();
            if(sel < 0)
            {
                jointX.IsEnabled = false;
                jointY.IsEnabled = false;
                jointZ.IsEnabled = false;
                jointXAxis.IsEnabled = false;
                jointYAxis.IsEnabled = false;
                jointZAxis.IsEnabled = false;
            }
            else
            {
                if (!jointX.IsEnabled)
                {
                    jointX.IsEnabled = true;
                    jointY.IsEnabled = true;
                    jointZ.IsEnabled = true;
                    jointXAxis.IsEnabled = true;
                    jointYAxis.IsEnabled = true;
                    jointZAxis.IsEnabled = true;
                }
                jointX.Value = joints[sel].rotPointX;
                jointY.Value = joints[sel].rotPointY;
                jointZ.Value = joints[sel].rotPointZ;
                jointXAxis.IsChecked = joints[sel].rotAxisX == 1 ? true : false;
                jointYAxis.IsChecked = joints[sel].rotAxisY == 1 ? true : false;
                jointZAxis.IsChecked = joints[sel].rotAxisZ == 1 ? true : false;
                selectModel(joints[sel].model);
                updateSpherePosition();
            }
            switchingJoint = false;
        }

        private void rotationPointChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (switchingJoint)
                return;

            int sel = ((int)jointSelector.Value) - 1;
            joints[sel].rotPointX = (int)jointX.Value;
            joints[sel].rotPointY = (int)jointY.Value;
            joints[sel].rotPointZ = (int)jointZ.Value;
            updateSpherePosition();
        }

        private void updateSpherePosition()
        {
            int sel = ((int)jointSelector.Value) - 1;
            if (sel < 0)
                return;

            Transform3DGroup F = new Transform3DGroup();
            F.Children.Add(new TranslateTransform3D(joints[sel].rotPointX, joints[sel].rotPointY, joints[sel].rotPointZ));
            F.Children.Add(joints[sel].model.Transform);
            geom.Transform = F;
        }

        private void joint_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isAnimating)
                return;

            joints[0].angle = joint1.Value;
            joints[1].angle = joint2.Value;
            joints[2].angle = joint3.Value;
            joints[3].angle = joint4.Value;
            joints[4].angle = joint5.Value;
            joints[7].angle = joint6.Value;
            execute_fk();
        }


        private void CheckBox_StateChanged(object sender, RoutedEventArgs e)
        {
            if (switchingJoint)
                return;

            int sel = ((int)jointSelector.Value) - 1;
            joints[sel].rotAxisX = jointXAxis.IsChecked.Value ? 1 : 0;
            joints[sel].rotAxisY = jointYAxis.IsChecked.Value ? 1 : 0;
            joints[sel].rotAxisZ = jointZAxis.IsChecked.Value ? 1 : 0;
        }


        /**
         * This methodes execute the FK (Forward Kinematics). It starts from the first joint, the base.
         * */
        private void execute_fk()
        {
            /** Debug sphere, it takes the x,y,z of the textBoxes and update its position
             * This is useful when using x,y,z in the "new Point3D(x,y,z)* when defining a new RotateTransform3D() to check where the joints is actually  rotating */
            double[] angles = { joints[0].angle, joints[1].angle, joints[2].angle, joints[3].angle, joints[4].angle, joints[7].angle };
            ForwardKinematics(angles);
            updateSpherePosition();
        }

        private Color changeModelColor(Joint pJoint, Color newColor)
        {
            Model3DGroup models = ((Model3DGroup)pJoint.model);
            return changeModelColor(models.Children[0] as GeometryModel3D, newColor);
        }

        private Color changeModelColor(GeometryModel3D pModel, Color newColor)
        {
            if (pModel == null)
                return oldColor;

            Color previousColor = Colors.Black;

            MaterialGroup mg = (MaterialGroup)pModel.Material;
            if (mg.Children.Count > 0)
            {
                try
                {
                    previousColor = ((EmissiveMaterial)mg.Children[0]).Color;
                    ((EmissiveMaterial)mg.Children[0]).Color = newColor;
                    ((DiffuseMaterial)mg.Children[1]).Color = newColor;
                }
                catch (Exception exc)
                {
                    previousColor = oldColor;
                }
            }

            return previousColor;
        }


        private void selectModel(Model3D pModel)
        {
            try
            {
                Model3DGroup models = ((Model3DGroup) pModel);
                oldSelectedModel = models.Children[0] as GeometryModel3D;
            }
            catch (Exception exc)
            {
                oldSelectedModel = (GeometryModel3D) pModel;
            }
            oldColor = changeModelColor(oldSelectedModel, ColorHelper.HexToColor("#ff3333"));
        }

        private void unselectModel()
        {
            changeModelColor(oldSelectedModel, oldColor);
        }

        private void ViewPort3D_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           Point mousePos = e.GetPosition(viewPort3d);
           PointHitTestParameters hitParams = new PointHitTestParameters(mousePos);
           VisualTreeHelper.HitTest(viewPort3d, null, ResultCallback, hitParams);
        }

        private void ViewPort3D_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Perform the hit test on the mouse's position relative to the viewport.
            HitTestResult result = VisualTreeHelper.HitTest(viewPort3d, e.GetPosition(viewPort3d));
            RayMeshGeometry3DHitTestResult mesh_result = result as RayMeshGeometry3DHitTestResult;

            if (oldSelectedModel != null)
                unselectModel();

            if (mesh_result != null)
            {
                selectModel(mesh_result.ModelHit);
            }
        }

        public HitTestResultBehavior ResultCallback(HitTestResult result)
        {
            // Did we hit 3D?
            RayHitTestResult rayResult = result as RayHitTestResult;
            if (rayResult != null)
            {
                // Did we hit a MeshGeometry3D?
                RayMeshGeometry3DHitTestResult rayMeshResult = rayResult as RayMeshGeometry3DHitTestResult;
                geom.Transform = new TranslateTransform3D(new Vector3D(rayResult.PointHit.X, rayResult.PointHit.Y, rayResult.PointHit.Z));

                if (rayMeshResult != null)
                {
                    // Yes we did!
                }
            }

            return HitTestResultBehavior.Continue;
        }

        public void StartInverseKinematics(object sender, RoutedEventArgs e)
        {
            if (timer1.Enabled)
            {
                button.Content = "Go to position";
                isAnimating = false;
                timer1.Stop();
                movements = 0;
            }
            else
            {
                geom.Transform = new TranslateTransform3D(reachingPoint);
                movements = 5000;
                button.Content = "STOP";
                isAnimating = true;
                timer1.Start();
            }
        }

        public void timer1_Tick(object sender, EventArgs e)
        {
            double[] angles = {joints[0].angle, joints[1].angle, joints[2].angle, joints[3].angle, joints[4].angle, joints[7].angle};
            angles = InverseKinematics(reachingPoint, angles);
            joint1.Value = joints[0].angle = angles[0];
            joint2.Value = joints[1].angle = angles[1];
            joint3.Value = joints[2].angle = angles[2];
            joint4.Value = joints[3].angle = angles[3];
            joint5.Value = joints[4].angle = angles[4];
            joint6.Value = joints[7].angle = angles[7];

            if ((--movements) <= 0)
            {
                button.Content = "Go to position";
                isAnimating = false;
                timer1.Stop();
            }
        }
       
        public double[] InverseKinematics(Vector3D target, double[] angles)
        {
            if (DistanceFromTarget(target, angles) < DistanceThreshold)
            {
                movements = 0;
                return angles;
            }

            double[] oldAngles = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            angles.CopyTo(oldAngles, 0);
            for (int i = 0; i <= 5; i++)
            {
                // Gradient descent
                // Update : Solution -= LearningRate * Gradient
                double gradient = PartialGradient(target, angles, i);
                angles[i] -= LearningRate * gradient;

                // Clamp
                angles[i] = Clamp(angles[i], joints[i].angleMin, joints[i].angleMax);

                // Early termination
                if (DistanceFromTarget(target, angles) < DistanceThreshold || checkAngles(oldAngles, angles))
                {
                    movements = 0;
                    return angles;
                }
            }

            return angles;
        }

        public bool checkAngles(double[] oldAngles, double[] angles)
        {
            for(int i = 0; i <= 5; i++)
            {
                if (oldAngles[i] != angles[i])
                    return false;
            }

            return true;
        }

        public double PartialGradient(Vector3D target, double[] angles, int i)
        {
            // Saves the angle,
            // it will be restored later
            double angle = angles[i];

            // Gradient : [F(x+SamplingDistance) - F(x)] / h
            double f_x = DistanceFromTarget(target, angles);

            angles[i] += SamplingDistance;
            double f_x_plus_d = DistanceFromTarget(target, angles);

            double gradient = (f_x_plus_d - f_x) / SamplingDistance;

            // Restores
            angles[i] = angle;

            return gradient;
        }


        public double DistanceFromTarget(Vector3D target, double[] angles)
        {
            Vector3D point = ForwardKinematics (angles);      
            return Math.Sqrt(Math.Pow((point.X - target.X), 2.0) + Math.Pow((point.Y - target.Y), 2.0) + Math.Pow((point.Z - target.Z), 2.0));
        }
        

        public Vector3D ForwardKinematics(double [] angles)
        {            
            //The base only has rotation and is always at the origin, so the only transform in the transformGroup is the rotation R
            F1 = new Transform3DGroup();
            R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[0].rotAxisX, joints[0].rotAxisY, joints[0].rotAxisZ), angles[0]), new Point3D(joints[0].rotPointX, joints[0].rotPointY, joints[0].rotPointZ));
            F1.Children.Add(R);

            //This moves the first joint attached to the base, it may translate and rotate. Since the joint are already in the right position (the .stl model also store the joints position
            //in the virtual world when they were first created, so if you load all the .stl models of the joint they will be automatically positioned in the right locations)
            //so in all of these cases the first translation is always 0, I just left it for future purposes if something need to be moved
            //After that, the joint needs to rotate of a certain amount (given by the value in the slider), and the rotation must be executed on a specific point
            //After some testing it looks like the point 175, -200, 500 is the sweet spot to achieve the rotation intended for the joint
            //finally we also need to apply the transformation applied to the base 
            F2 = new Transform3DGroup();
            T = new TranslateTransform3D(0,0,0);
            R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[1].rotAxisX, joints[1].rotAxisY, joints[1].rotAxisZ), angles[1]), new Point3D(joints[1].rotPointX, joints[1].rotPointY, joints[1].rotPointZ));
            F2.Children.Add(T);
            F2.Children.Add(R);
            F2.Children.Add(F1);



       









            //The second joint is attached to the first one. As before I found the sweet spot after testing, and looks like is rotating just fine. No pre-translation as before
            //and again the previous transformation needs to be applied
            F3 = new Transform3DGroup();
            T = new TranslateTransform3D(0, 0, 0);
            R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[2].rotAxisX, joints[2].rotAxisY, joints[2].rotAxisZ), angles[2]), new Point3D(joints[2].rotPointX, joints[2].rotPointY, joints[2].rotPointZ));
            F3.Children.Add(T);
            F3.Children.Add(R);
            F3.Children.Add(F2);


            F7 = new Transform3DGroup();
            T = new TranslateTransform3D(0, 0, 0);
            R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[7].rotAxisX, joints[17].rotAxisY, joints[7].rotAxisZ), angles[2]), new Point3D(joints[7].rotPointX, joints[17].rotPointY, joints[7].rotPointZ));
            F7.Children.Add(T);
            F7.Children.Add(R);
            
            F7.Children.Add(F3);








            //as before
            F4 = new Transform3DGroup();
            T = new TranslateTransform3D(0,0,0); //1500, 650, 1650
            R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[3].rotAxisX, joints[3].rotAxisY, joints[3].rotAxisZ), angles[3]), new Point3D(joints[3].rotPointX, joints[3].rotPointY, joints[3].rotPointZ));
            F4.Children.Add(T);
            F4.Children.Add(R);

            F4.Children.Add(F3);

            //as before
            F5 = new Transform3DGroup();
            T = new TranslateTransform3D(0, 0, 0);
            R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[4].rotAxisX, joints[4].rotAxisY, joints[4].rotAxisZ), angles[4]), new Point3D(joints[4].rotPointX, joints[4].rotPointY, joints[4].rotPointZ));
            F5.Children.Add(T);
            F5.Children.Add(R);
            F5.Children.Add(F4);

            //NB: I was having a nightmare trying to understand why it was always rotating in a weird way... SO I realized that the order in which
            //you add the Children is actually VERY IMPORTANT in fact before I was applyting F and then T and R, but the previous transformation
            //Should always be applied as last (FORWARD Kinematics)
            F6 = new Transform3DGroup();
            T = new TranslateTransform3D(0, 0, 0);
            R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[5].rotAxisX, joints[5].rotAxisY, joints[5].rotAxisZ), angles[5]), new Point3D(joints[5].rotPointX, joints[5].rotPointY, joints[5].rotPointZ));
            F6.Children.Add(T);
            F6.Children.Add(R);
            F6.Children.Add(F5);









            F6 = new Transform3DGroup();
            T = new TranslateTransform3D(0, 0, 0);
            R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[6].rotAxisX, joints[6].rotAxisY, joints[6].rotAxisZ), angles[2]), new Point3D(joints[6].rotPointX, joints[6].rotPointY, joints[6].rotPointZ));
            F6.Children.Add(T);
            F6.Children.Add(R);
            F6.Children.Add(F5);



            F8 = new Transform3DGroup();
            T = new TranslateTransform3D(0, 0, 0);
            R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[1].rotAxisX, joints[1].rotAxisY, joints[1].rotAxisZ), angles[2]), new Point3D(joints[1].rotPointX, joints[1].rotPointY, joints[1].rotPointZ));
            F8.Children.Add(T);
            F8.Children.Add(F1);
          
            F8.Children.Add(R);



            joints[0].model.Transform = F1; //First joint
            joints[1].model.Transform = F2; //Second joint (the "biceps")
            joints[2].model.Transform = F3; //third joint (the "knee" or "elbow")
            joints[3].model.Transform = F4; //the "forearm"
            joints[4].model.Transform = F5; //the tool plate
            joints[5].model.Transform = F6; //the tool
            
            Tx.Content = joints[5].model.Bounds.Location.X;
            Ty.Content = joints[5].model.Bounds.Location.Y;
            Tz.Content = joints[5].model.Bounds.Location.Z;
            Tx_Copy.Content = geom.Bounds.Location.X;
            Ty_Copy.Content = geom.Bounds.Location.Y;
            Tz_Copy.Content = geom.Bounds.Location.Z;

#if IRB6700
            joints[6].model.Transform = F6;
            joints[7].model.Transform = F2;
            joints[19].model.Transform = F1;
            joints[14].model.Transform = F1;

            joints[8].model.Transform = F2;
            joints[9].model.Transform = F2;

            joints[10].model.Transform = F3;
            joints[11].model.Transform = F3;
            joints[12].model.Transform = F3;
            joints[16].model.Transform = F3;

            joints[13].model.Transform = F4;
            joints[17].model.Transform = F1;
#else
            joints[7].model.Transform = F1; //Cables

            joints[8].model.Transform = F2; //Cables

            joints[6].model.Transform = F3; //The ABB writing
            joints[9].model.Transform = F3; //Cables
#endif

            return new Vector3D(joints[5].model.Bounds.Location.X, joints[5].model.Bounds.Location.Y, joints[5].model.Bounds.Location.Z);
        }

    }

}
