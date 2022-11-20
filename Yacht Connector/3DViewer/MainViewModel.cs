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

    public class MainViewModel : BaseViewModel
    {
        int floatNum = 0;
        int[] diceResult = { -1, -1, -1, -1, -1 };
        int[] floatings = { -1, -1, -1, -1, -1};
        int[] keep = { 0, 1, 2, 3, 4 };

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
        public PhongMaterial BarMaterial { get; private set; }
        public PhongMaterial PlaneMaterial { get; private set; }
        private CompositionTargetEx compositeHelper = new CompositionTargetEx();


        public Media3D.Transform3D PlaneTransform { get; private set; }
        public Media3D.Transform3D Bar1Transform { get; private set; }
        public MeshGeometry3D Plane { get; private set; }
        public MeshGeometry3D Bar1 { get; private set; }
        public ProjectionCamera Camera1 { private set; get; }

        public bool IsAnimationPlaying { get; private set; } = false;

        public MeshGeometryModel3D[] Models = new MeshGeometryModel3D[5];
        public DiceRollingAnimator[] animators = new DiceRollingAnimator[5];
        public DiceMoveAnimator[] moveAnimators = new DiceMoveAnimator[5];

        public Vector3D UpDirection { set; get; } = new Vector3D(0, 1, 0);

        bool tryRolling = false;
        List<float> simulationResult;


        TextureModel DiceTexture;
        TextureModel DiceNormalTexture;

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

            for(int i = 0; i < 5; i++)
            {
                var c = new RotateTransform3D(new AxisAngleRotation3D()).ToMatrix().ToMatrix3D();
                c.Translate(GetKeepVector(i));
                Models[i].Transform = new MatrixTransform3D(c);
            }



            var b2 = new MeshBuilder();
            b2.AddBox(new Vector3(0, 0, 0), 20, 0, 50, BoxFaces.PositiveY);
            Plane = b2.ToMeshGeometry3D();
            PlaneTransform = new Media3D.TranslateTransform3D(-0, 0.5, 10);
            PlaneMaterial = PhongMaterials.Indigo;
            PlaneMaterial.DiffuseColor = new Color4(.3f, 0, 0, 1);

            var b3 = new MeshBuilder();
            b3.AddBox(new Vector3(0, 1.5f, 11), 8, 3, .5);
            b3.AddBox(new Vector3(4, 1.5f, 7), .5, 3, 8);
            b3.AddBox(new Vector3(-4, 1.5f, 7), .5, 3, 8);
            b3.AddBox(new Vector3(0, 1.5f, 3), 8, 3, .5);
            Bar1 = b3.ToMeshGeometry3D();
            BarMaterial = PhongMaterials.Black;

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
        }

        public void StartRolling()
        {
            compositeHelper.Rendering -= RenderMove;
            compositeHelper.Rendering += Render;
        }

        public void EndRolling()
        {
            compositeHelper.Rendering -= Render;
            IsAnimationPlaying = false;
        }

        private void Render(object? sender, System.Windows.Media.RenderingEventArgs e)
        {
            long ts = Stopwatch.GetTimestamp();
            long fq = Stopwatch.Frequency;
            bool end = false;
            compositeHelper.Rendering -= RenderMove;
            for (int i = 0; i < floatNum; i++)
            {
                if (!animators[floatings[i]].Update(ts, fq))
                    end = true;
            }

            if (end)
            {
                EndRolling();
                SetMove();
                compositeHelper.Rendering += RenderMove;
            }
        }

        public void Initialize(List<float> v)
        {
            IsAnimationPlaying = true;
            simulationResult = v;
            tryRolling = true;
            compositeHelper.Rendering += RenderMove;

            var ts = Stopwatch.GetTimestamp();
            var fq = Stopwatch.Frequency;
            for (int i = 0; i < 5; i++)
            {
                if (keep[i] == -1)
                    continue;
                moveAnimators[keep[i]].SetAnimation(GetKeepVector(i), new Vector3D(0, 1.5, -40), null, 1000);
                moveAnimators[keep[i]].StartAnimation(ts, fq, DiceMoveAnimator.AnimationPhase.BackToHand);
            }
            for(int i = 0; i < floatNum; i++)
            {
                moveAnimators[floatings[i]].SetAnimation(moveAnimators[floatings[i]].currentPos, new Vector3D(0, 1.5, -40), null, 1000);
                moveAnimators[floatings[i]].StartAnimation(ts, fq, DiceMoveAnimator.AnimationPhase.BackToHand);
            }
            for(int i = 0; i < 5; i++)
            {
                floatings[i] = i;
                keep[i] = -1;
            }
            floatNum = 5;
        }

        public bool DiceRollReady()
        {
            if (IsAnimationPlaying)
                return false;
            for (int i = 0; i < 5; i++)
            {
                if (moveAnimators[i].OnRunning)
                    return false;
            }
            return true;
        }

        public void AnimateDiceRoll(List<float> v)
        {
            IsAnimationPlaying = true;
            simulationResult = v;
            tryRolling = true;
            var ts = Stopwatch.GetTimestamp();
            var fq = Stopwatch.Frequency;
            for(int i = 0; i < floatNum; i++)
            {
                moveAnimators[floatings[i]].SetAnimation(moveAnimators[floatings[i]].currentPos, new Vector3D(0, 2, 40), null, 1000);
                moveAnimators[floatings[i]].StartAnimation(ts, fq, DiceMoveAnimator.AnimationPhase.BackToHand);
            }
        }

        void StartDiceRoll(List<float> v)
        {
            long t = Stopwatch.GetTimestamp();
            for (int i = 0; i < floatNum; i++)
            {
                animators[floatings[i]].SetAnimation(v, floatNum, i, diceResult[i]);
                animators[floatings[i]].StartAnimation(t);
            }
            StartRolling();
        }

        public int[] GetDiceValues()
        {
            int[] newArr = (int[])diceResult.Clone();
            return newArr;
        }

        public void SetDiceResult(List<int> dr, bool initRoll = false)
        {
            if (initRoll)
            {
                Debug.Assert(dr.Count == 5);
                for (int i = 0; i < dr.Count; i++)
                {
                    diceResult[i] = dr[i];
                }
            }
            else
            {
                Debug.Assert(dr.Count == floatNum);
                for (int i = 0; i < dr.Count; i++)
                {
                    diceResult[floatings[i]] = dr[i];
                }
            }
        }

        public bool ClickFloating(int floatIndex)
        {
            Debug.Assert(floatIndex >= 0 && floatIndex < 5);
            if (floatings[floatIndex] == -1 || IsAnimationPlaying)
                return false;
            if (moveAnimators[floatings[floatIndex]].currentPhase != DiceMoveAnimator.AnimationPhase.Float)
                return false;

            FloatingToKeep(floatIndex);
            return true;
        }

        public bool ClickKeep(int keepIndex)
        {
            Debug.Assert(keepIndex >= 0 && keepIndex < 5);
            if (keep[keepIndex] == -1|| IsAnimationPlaying)
                return false;
            if (moveAnimators[keep[keepIndex]].currentPhase != DiceMoveAnimator.AnimationPhase.Keep)
                return false;

            KeepToFloating(keepIndex);
            return true;
        }

        void FloatingToKeep(int floatIndex)
        {
            int keepIndex = -1;
            for (int i = 0; i < 5; i++)
            {
                if (keep[i] == -1)
                {
                    keepIndex = i;
                    break;
                }
            }

            Debug.Assert(keepIndex >= 0,"Keep is Full.");

            keep[keepIndex] = floatings[floatIndex];
            floatings[floatIndex] = -1;
            RearrangeFloatings();
            floatNum--;


            moveAnimators[keep[keepIndex]].SetAnimation(moveAnimators[keep[keepIndex]].currentPos, GetKeepVector(keepIndex));
            moveAnimators[keep[keepIndex]].StartAnimation(Stopwatch.GetTimestamp(), Stopwatch.Frequency, DiceMoveAnimator.AnimationPhase.Keep);
            ProcessArrangeAnimation();
        }

        void KeepToFloating(int keepIndex)
        {
            Debug.Assert(floatNum < 5, "Floating is Full");

            floatings[floatNum++] = keep[keepIndex];
            keep[keepIndex] = -1;

            moveAnimators[floatings[floatNum - 1]].SetAnimation(moveAnimators[floatings[floatNum - 1]].currentPos, GetFloatVector(floatNum, floatNum - 1));
            moveAnimators[floatings[floatNum - 1]].StartAnimation(Stopwatch.GetTimestamp(), Stopwatch.Frequency, DiceMoveAnimator.AnimationPhase.Float);
            ProcessArrangeAnimation();
        }

        void RearrangeFloatings()
        {
            int t;
            for(int i = 0; i < floatNum; i++)
            {
                if (floatings[i] == -1)
                {
                    for (int j = i + 1; j < floatNum; j++)
                    {
                        if (floatings[j] != -1)
                        {
                            floatings[i] = floatings[j];
                            floatings[j] = -1;
                            break;
                        }
                    }
                }
            }
        }

        void ProcessArrangeAnimation()
        {
            long ts = Stopwatch.GetTimestamp();
            long fq = Stopwatch.Frequency;
            for (int i = 0; i < floatNum; i++)
            {
                if (moveAnimators[floatings[i]].currentPhase == DiceMoveAnimator.AnimationPhase.Float)
                {
                    moveAnimators[floatings[i]].SetAnimation(moveAnimators[floatings[i]].currentPos, GetFloatVector(floatNum, i));
                    moveAnimators[floatings[i]].StartAnimation(ts, fq, DiceMoveAnimator.AnimationPhase.Float);
                }
                else if (moveAnimators[floatings[i]].currentPhase == DiceMoveAnimator.AnimationPhase.KeepToFloat)
                {
                    moveAnimators[floatings[i]].ChangeDestination(GetFloatVector(floatNum, i));
                }
            }
        }

        private Vector3D GetFloatVector(int floatNum, int floatIndex)
        {
            double span = 1.4 + (5 - floatNum) * 0.3;
            return new Vector3D(floatIndex * span - (float)(floatNum - 1) / 2 * span, 4, 5.5);

        }
        private Vector3D GetKeepVector(int keepIndex)
        {
            double span = 2; 
            return new Vector3D(keepIndex * span - 2 * span, 1.5, 0);
        }


        private void SetMove()
        {
            int[] order = new int[floatNum];
            for (int i = 0; i < floatNum; i++)
                order[i] = i;
            var sortedOrder = order.OrderBy(x => animators[floatings[x]].LastPos.X).ToArray();
            for (int i = 0; i < floatNum; i++)
                order[i] = floatings[sortedOrder[i]];
            for (int i = 0; i < floatNum; i++)
                floatings[i] = order[i];
            int index;

            double span = 1.4+ (5- floatNum) *0.3;

            for (int i = 0; i < floatNum; i++)
            {
                index = floatings[i];
                moveAnimators[index].SetAnimation(animators[index].LastPos, GetFloatVector(floatNum, i), animators[index].LastAngle, 600);
            }
            long ts = Stopwatch.GetTimestamp();
            long fq = Stopwatch.Frequency;

            for (int i = 0; i < floatNum; i++)
            {
                moveAnimators[floatings[i]].StartAnimation(ts, fq, DiceMoveAnimator.AnimationPhase.Float);
            }
        }

        private void RenderMove(object? sender, System.Windows.Media.RenderingEventArgs e)
        {
            long ts = Stopwatch.GetTimestamp();
            bool end = false;
            long fq = Stopwatch.Frequency;
            for (int i = 0; i < 5; i++)
            {
                if (moveAnimators[i].OnRunning) {
                    if(!moveAnimators[i].Update(ts, fq) && tryRolling)
                    {
                        end = true;
                    }
                }
            }

            if (end)
            {
                tryRolling = false;
                StartDiceRoll(simulationResult);
            }
        }

    }
}
