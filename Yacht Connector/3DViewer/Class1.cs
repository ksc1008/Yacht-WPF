using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Controls;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D9;
using Yacht_Connector._3DViewer;

namespace Yacht_Connector
{
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using MeshGeometry3D = HelixToolkit.SharpDX.Core.MeshGeometry3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using Color = System.Windows.Media.Color;
    using Plane = SharpDX.Plane;
    using Vector3 = SharpDX.Vector3;
    using Colors = System.Windows.Media.Colors;
    using Color4 = SharpDX.Color4;
    using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
    using ProjectionCamera = HelixToolkit.Wpf.SharpDX.ProjectionCamera;
    using Material = HelixToolkit.Wpf.SharpDX.Material;
    public partial class MainViewModel : BaseViewModel
    {
        public MeshGeometryModel3D Model { get; private set; }
        public MeshGeometryModel3D Model2 { get; private set; }
        public MeshGeometryModel3D Model3 { get; private set; }
        public MeshGeometryModel3D Model4 { get; private set; }
        public MeshGeometryModel3D Model5 { get; private set; }
        public Size ShadowMapResolution { get; private set; }
        public PhongMaterial RedMaterial { get; private set; }
        public Vector3D DirectionalLightDirection { get; private set; }
        public Color DirectionalLightColor { get; private set; }
        public Color SpotLightColor { get; private set; }
        public Color AmbientLightColor { get; private set; }
        public PhongMaterial BarMaterial { get; private set; }
        public PhongMaterial PlaneMaterial { get; private set; }
        protected CompositionTargetEx compositeHelper = new CompositionTargetEx();

        public Media3D.Transform3D PlaneTransform { get; private set; }
        public Media3D.Transform3D Bar1Transform { get; private set; }
        public MeshGeometry3D Plane { get; private set; }
        public MeshGeometry3D Bar1 { get; private set; }
        public ProjectionCamera Camera1 { private set; get; }
        
        public MeshGeometryModel3D[] Models = new MeshGeometryModel3D[5];
        public DiceRollingAnimator[] animators = new DiceRollingAnimator[5];
        public DiceMoveAnimator[] moveAnimators = new DiceMoveAnimator[5];

        bool firstRun = true;

        public MainViewModel(DiceRoller window)
        {
            /*
             *  X축 방향 : 3
             *  Y축 방향 : 6
             *  Z축 방향 : 2
             */


            DiceTexture = TextureModel.Create(new System.Uri(@"./Textures/1.png", UriKind.RelativeOrAbsolute).ToString());
            DiceNormalTexture = TextureModel.Create(new System.Uri(@"./Textures/1.png_normal.png", UriKind.RelativeOrAbsolute).ToString());

            var ModelMaterial = new PhongMaterial
            {
                DiffuseColor = Colors.White.ToColor4(),
                SpecularColor = Colors.White.ToColor4(),
                SpecularShininess = 50f,
                DiffuseMap = DiceTexture,
                NormalMap = DiceNormalTexture,
            };
            mainWindow = window;

            EffectsManager = new DefaultEffectsManager();

            Camera = new PerspectiveCamera()
            {
                Position = new Point3D(0, 10, 5.5),
                LookDirection = new Vector3D(0, -8, -1),
                UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 5000,
                NearPlaneDistance = .1f,
                FieldOfView = 75
            };
            DirectionalLightColor = Colors.White;
            AmbientLightColor = Colors.White;
            DirectionalLightDirection = new Vector3D(-2, -5, -2);

            this.ShadowMapResolution = new Size(2048, 2048);

            var reader = new ObjReader();
            SpotLightColor = Color.FromArgb(200, 100, 100, 100);

            var t = reader.Read(@"./Models/dice.obj");

            Model = new MeshGeometryModel3D
            {
                Geometry = t[0].Geometry,

                Material = ModelMaterial
            };
            Model2 = new MeshGeometryModel3D
            {
                Geometry = t[0].Geometry,

                Material = ModelMaterial
            };
            Model3 = new MeshGeometryModel3D
            {
                Geometry = t[0].Geometry,

                Material = ModelMaterial
            };
            Model4 = new MeshGeometryModel3D
            {
                Geometry = t[0].Geometry,

                Material = ModelMaterial

            };
            Model5 = new MeshGeometryModel3D
            {
                Geometry = t[0].Geometry,

                Material = ModelMaterial
            };


            int fps = 90;

            animators[0] = new DiceRollingAnimator(Model, fps);
            animators[1] = new DiceRollingAnimator(Model2, fps);
            animators[2] = new DiceRollingAnimator(Model3, fps);
            animators[3] = new DiceRollingAnimator(Model4, fps);
            animators[4] = new DiceRollingAnimator(Model5, fps);

            Models[0] = Model;
            Models[1] = Model2;
            Models[2] = Model3;
            Models[3] = Model4;
            Models[4] = Model5;

            moveAnimators[0] = new DiceMoveAnimator(Model, 2);
            moveAnimators[1] = new DiceMoveAnimator(Model2, 2);
            moveAnimators[2] = new DiceMoveAnimator(Model3, 2);
            moveAnimators[3] = new DiceMoveAnimator(Model4, 2);
            moveAnimators[4] = new DiceMoveAnimator(Model5, 2);

            for (int i = 0; i < 5; i++)
            {
                var c = new RotateTransform3D(new AxisAngleRotation3D()).ToMatrix().ToMatrix3D();
                c.Translate(new Vector3D(0, 1.5, -40));
                Models[i].Transform = new MatrixTransform3D(c);
            }



            var b2 = new MeshBuilder();
            b2.AddBox(new Vector3(0, 0, -3), 8, 0, 8, BoxFaces.PositiveY);
            Plane = b2.ToMeshGeometry3D();
            PlaneTransform = new Media3D.TranslateTransform3D(-0, 0.5, 10);
            PlaneMaterial = PhongMaterials.Indigo;
            PlaneMaterial.DiffuseColor = new Color4(.3f, 0, 0, 1);

            var b3 = new MeshBuilder();
            b3.AddBox(new Vector3(0, 1.5f, 11), 8, 3, .5);
            b3.AddBox(new Vector3(4, 1.5f, 7), .5, 3, 8.5);
            b3.AddBox(new Vector3(-4, 1.5f, 7), .5, 3, 8.5);
            b3.AddBox(new Vector3(0, 1.5f, 3), 8, 3, .5);
            Bar1 = b3.ToMeshGeometry3D();           
            BarMaterial = PhongMaterials.Black;
            BarMaterial.DiffuseColor = new Color4(.12f,.1f,.1f,1);
            BarMaterial.AmbientColor = new Color4(0.21f, 0.21f, 0.21f,1);

            PlaneMaterial.RenderShadowMap = true;
            BarMaterial.RenderShadowMap = true;

            Camera1 = new PerspectiveCamera
            {
                Position = new Point3D(0, 20, 6),
                LookDirection = new Vector3D(0, -1, 0.05),
                UpDirection = new Vector3D(1, 0, 0),
                FarPlaneDistance = 5000,
                NearPlaneDistance = 1,
                FieldOfView = 45
            };
            compositeHelper.Rendering += RenderMove;
        }

    }
}
