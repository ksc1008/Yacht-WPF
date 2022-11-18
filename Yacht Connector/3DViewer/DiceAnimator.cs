using HelixToolkit.SharpDX.Core.Animations;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using GeometryModel3D = HelixToolkit.Wpf.SharpDX.GeometryModel3D;

namespace Yacht_Connector._3DViewer
{
    public class DiceAnimator
    {
        int frame = 0;
        List<Vector3D> positions = new();
        List<Vector3D> rotAxis = new();
        List<float> rotAngles = new();
        long startTime;
        Vector3D up = new Vector3D(0, 1, 0);
        MeshGeometryModel3D model;
        long lastiter = 0;
        int cnt = 0;

        private int _intervalMillisecond;
        public int Interval { get => _intervalMillisecond; set => _intervalMillisecond = value; }

        public DiceAnimator(MeshGeometryModel3D _model, int interval)
        {
            Interval = interval;
            model = _model;
        }

        public void SetAnimation(List<float> l, int diceCount, int diceIndex)
        {
            positions.Clear();
            rotAxis.Clear();
            rotAngles.Clear();
            int sz = 7 * diceCount;
            frame = l.Count / sz;
            int offset = diceIndex * 7;
            for (int i = 0; i < frame; i++)
            {
                positions.Add(new Vector3D(l[offset], l[offset + 1], l[offset + 2]));
                rotAxis.Add(new Vector3D(l[offset + 3], l[offset + 4], l[offset + 5]) * 180 / MathF.PI);
                rotAngles.Add(l[offset + 6] * 180 / MathF.PI);
                offset += sz;
            }
        }

        public void StartAnimation(long timeStamp)
        {
            startTime = timeStamp;
        }

        public bool Update(long timeStamp, long frequency)
        {
            cnt++;
            int iter = MathUtil.Clamp((int)(((float)(timeStamp - startTime) / frequency) * _intervalMillisecond),0,frame-1);
            if (iter == lastiter)
                return true;
            Debug.WriteLine("Updating.."+cnt);
            lastiter = iter;
            var d = GetGeometryData(iter);

            var mat = new RotateTransform3D(new AxisAngleRotation3D(d.Item2, d.Item3)).ToMatrix().ToMatrix3D();
            Debug.WriteLine(d.Item1.ToString());
            //mat.Scale(new Vector3D(.5, .5, .5));
            mat.Translate(d.Item1);
            model.Transform = new MatrixTransform3D(mat);

            if (iter == frame - 1)
                return false;
            return true;
        }

        (Vector3D, Vector3D, float) GetGeometryData(int iter)
        {
            return (positions[iter], rotAxis[iter], rotAngles[iter]);
        }
    }
}
