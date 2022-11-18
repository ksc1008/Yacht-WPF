using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

    public class MainViewModel:BaseViewModel
    {
        DiceRoller mainWindow;

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
        public PhongMaterial BlackMaterial { get; private set; }
        public PhongMaterial GrayMaterial { get; private set; }
        public PhongMaterial WhiteMaterial { get; private set; }
        public PhongMaterial BlueMaterial { get; private set; }
        public PhongMaterial YellowMaterial { get; private set; }
        public PhongMaterial GreenMaterial { get; private set; }
        private CompositionTargetEx compositeHelper = new CompositionTargetEx();


        public Media3D.Transform3D PlaneTransform { get; private set; }
        public Media3D.Transform3D Bar1Transform { get; private set; }
        public MeshGeometry3D Plane { get; private set; }
        public MeshGeometry3D Bar1 { get; private set; }
        public MeshGeometry3D Bar2 { get; private set; }
        public MeshGeometry3D Bar3 { get; private set; }
        public ProjectionCamera Camera1 { private set; get; }

        public bool IsAnimationPlaying { get; private set; } = false;

        public DiceAnimator[] animators = new DiceAnimator[5];

        public Vector3D UpDirection { set; get; } = new Vector3D(0, 1, 0);


        TextureModel DiceTexture;
        TextureModel DiceNormalTexture;

        public MainViewModel(DiceRoller window)
        {
            /*
             *  X축 방향 : 3
             *  Y축 방향 : 6
             *  Z축 방향 : 2
             */

            DiceTexture = TextureModel.Create(new System.Uri(@"./Textures/1.png",UriKind.RelativeOrAbsolute).ToString());
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
                Position = new Point3D(0, 12, 10),
                LookDirection = new Vector3D(0, -8, -4),
                UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 5000,
                NearPlaneDistance = .1f,
                FieldOfView = 60
            };
            DirectionalLightColor = Colors.White;
            AmbientLightColor = Colors.White;
            DirectionalLightDirection = new Vector3D(-2, -5, -2);

            this.ShadowMapResolution = new Size(2048, 2048);

            var reader = new ObjReader();

            RedMaterial = PhongMaterials.Red;
            GreenMaterial = PhongMaterials.Green;
            BlueMaterial = PhongMaterials.Blue;
            WhiteMaterial = PhongMaterials.White;
            YellowMaterial = PhongMaterials.Yellow;
            RedMaterial.RenderShadowMap = true;
            SpotLightColor = Color.FromArgb(200,100,100,100);

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

            animators[0] = new DiceAnimator(Model,90);
            animators[1] = new DiceAnimator(Model2, 90);
            animators[2] = new DiceAnimator(Model3, 90);
            animators[3] = new DiceAnimator(Model4, 90);
            animators[4] = new DiceAnimator(Model5, 90);



            var b2 = new MeshBuilder();
            b2.AddBox(new Vector3(0, 0, 0), 20, 0, 50, BoxFaces.PositiveY);
            Plane = b2.ToMeshGeometry3D();
            PlaneTransform = new Media3D.TranslateTransform3D(-0, 0.5, 10);
            GrayMaterial = PhongMaterials.Indigo;
            GrayMaterial.DiffuseColor = new Color4(.3f, 0, 0, 1);

            var b3 = new MeshBuilder();
            b3.AddBox(new Vector3(0, .5f, 0), 15, 1, 1);
            Bar1 = b3.ToMeshGeometry3D();
            BlackMaterial = PhongMaterials.Black;
            Bar1Transform = new Media3D.TranslateTransform3D(0, 0, 3);

            GrayMaterial.RenderShadowMap = true;
            WhiteMaterial.RenderShadowMap = true;
            GreenMaterial.RenderShadowMap = true;
            BlueMaterial.RenderShadowMap = true;
            YellowMaterial.RenderShadowMap = true;

            Camera1 = new PerspectiveCamera
            {
                Position = new Point3D(0, 20, 6),
                LookDirection = new Vector3D(0, -1, 0.05),
                UpDirection = new Vector3D(1, 0, 0),
                FarPlaneDistance = 5000,
                NearPlaneDistance = 1,
                FieldOfView = 45
            };
        }

        public void StartAnim() { 
            compositeHelper.Rendering += Render;
            IsAnimationPlaying = true;
        }

        public void EndAnim()
        {
            compositeHelper.Rendering -= Render;
            IsAnimationPlaying = false;
        }

        private void Render(object? sender, System.Windows.Media.RenderingEventArgs e) { 

            long ts = Stopwatch.GetTimestamp();
            long fq = Stopwatch.Frequency;
            bool end = false;
            foreach(var anim in animators)
            {
                if (!anim.Update(ts, fq))
                {
                    end = true;
                }
            }

            if (end)
                EndAnim();

        }

    }
}
