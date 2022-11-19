using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Yacht_Connector._3DViewer
{
    public static class VectorCalculator
    {
        public enum Face
        {
            Up, Down, Left, Right, Front, Back
        };

        public static readonly Face[] FacePerDieNum = { Face.Up,Face.Down, Face.Front, Face.Right, Face.Left, Face.Back, Face.Up };

        public static readonly Vector3[] FaceToVec3 = { Vector3.Up, Vector3.Down, Vector3.Left, Vector3.Right, Vector3.ForwardLH, Vector3.BackwardLH };


        public static Face GetUpperFace(Vector3 upVec)
        {
            float f1 = MathF.Abs(upVec.X);
            float f2 = MathF.Abs(upVec.Y);
            float f3 = MathF.Abs(upVec.Z);

            if (f1 > f2)
            {
                if(f3 > f1)
                {
                    if (upVec.Z > 0)
                        return Face.Front;
                    else
                        return Face.Back;
                }

                if (upVec.X > 0)
                {
                    return Face.Right;
                }
                else
                {
                    return Face.Left;
                }

            }
            else if (f2 > f3)
            {
                if(upVec.Y > 0)
                {
                    return Face.Up;
                }
                else
                {
                    return Face.Down;
                }
            }
            else
            {
                if (upVec.Z > 0)
                {
                    return Face.Front;
                }
                else
                {
                    return Face.Back;
                }
            }
        }

        public static AxisAngleRotation3D ManipulateFace(Face Origin, Face Dest)
        {
            Vector3 a;

            if (Origin == Dest)
            {
                return new AxisAngleRotation3D();
            }

            Vector3 oV = FaceToVec3[(int)Origin];
            Vector3 dV = FaceToVec3[(int)Dest];
            Vector3 v = FaceToVec3[(int)Origin] - FaceToVec3[(int)Dest];

            if (Math.Abs(v.X) == 2)
            {
                return new AxisAngleRotation3D(new Vector3D(0,1,0), 180);
            }
            if (Math.Abs(v.Y) == 2)
            {
                return new AxisAngleRotation3D(new Vector3D(0, 0, 1), 180);
            }
            if (Math.Abs(v.Z) == 2)
            {
                return new AxisAngleRotation3D(new Vector3D(0, 1, 0), 180);
            }


            return new AxisAngleRotation3D(Vector3D.CrossProduct(oV.ToVector3D(), dV.ToVector3D()),-90);
        }
    }
}
