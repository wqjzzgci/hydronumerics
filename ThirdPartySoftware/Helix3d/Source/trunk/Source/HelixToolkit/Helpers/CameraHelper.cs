using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// Helper class for Media3D.Camera.
    /// </summary>
    public class CameraHelper
    {
        /// <summary>
        /// Creates a default perspective camera.
        /// </summary>
        /// <returns></returns>
        public static PerspectiveCamera CreateDefaultCamera()
        {
            var cam = new PerspectiveCamera();
            Reset(cam);
            return cam;
        }

        /// <summary>
        /// Resets the specified camera.
        /// </summary>
        /// <param name="camera">The camera.</param>
        public static void Reset(Camera camera)
        {
            var pcamera = camera as PerspectiveCamera;
            if (pcamera != null)
                Reset(pcamera);

            var ocamera = camera as OrthographicCamera;
            if (ocamera != null)
                Reset(ocamera);
        }

        /// <summary>
        /// Resets the specified perspective camera.
        /// </summary>
        /// <param name="camera">The camera.</param>
        public static void Reset(PerspectiveCamera camera)
        {
            if (camera == null)
                return;
            camera.Position = new Point3D(2, 16, 20);
            camera.LookDirection = new Vector3D(-2, -16, -20);
            camera.UpDirection = new Vector3D(0, 0, 1);
            camera.FieldOfView = 45;
            camera.NearPlaneDistance = 0.1;
            camera.FarPlaneDistance = 100000;
        }

        /// <summary>
        /// Resets the specified orthographic camera.
        /// </summary>
        /// <param name="camera">The camera.</param>
        public static void Reset(OrthographicCamera camera)
        {
            if (camera == null)
                return;
            camera.Position = new Point3D(2, 16, 20);
            camera.LookDirection = new Vector3D(-2, -16, -20);
            camera.UpDirection = new Vector3D(0, 0, 1);
            camera.Width = 40;
            camera.NearPlaneDistance = 0.1;
            camera.FarPlaneDistance = 100000;
        }

        /// <summary>
        /// Copies all members of the source <see cref="Camera"/>.
        /// </summary>
        /// <param name="source">Source camera.</param>
        /// <param name="dest">Destination camera.</param>
        public static void Copy(PerspectiveCamera source, PerspectiveCamera dest)
        {
            if (source == null || dest == null)
                return;

            dest.LookDirection = source.LookDirection;
            dest.Position = source.Position;
            dest.UpDirection = source.UpDirection;
            dest.FieldOfView = source.FieldOfView;
            dest.NearPlaneDistance = source.NearPlaneDistance;
            dest.FarPlaneDistance = source.FarPlaneDistance;
        }

        /// <summary>
        /// Copy the direction of the source <see cref="Camera"/> only. Used for the CoordinateSystem view.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="distance"></param>
        public static void CopyDirectionOnly(ProjectionCamera source, ProjectionCamera dest, double distance)
        {
            if (source == null || dest == null)
                return;

            Vector3D dir = source.LookDirection;
            dir.Normalize();
            dir *= distance;

            dest.LookDirection = dir;
            dest.Position = new Point3D(-dest.LookDirection.X, -dest.LookDirection.Y, -dest.LookDirection.Z);
            dest.UpDirection = source.UpDirection;
        }
        
        /// <summary>
        /// Animates the camera position and direction
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <param name="newPosition">The position to animate to</param>
        /// <param name="newDirection">The direction to animate to</param>
        /// <param name="newUpDirection">The up direction to animate to</param>
        /// <param name="animationTime">Animation time in milliseconds</param>
        public static void AnimateTo(ProjectionCamera camera, Point3D newPosition, Vector3D newDirection, Vector3D newUpDirection, double animationTime)
        {
            var fromPosition = camera.Position;
            var fromDirection = camera.LookDirection;
            var fromUpDirection = camera.UpDirection;

            camera.Position = newPosition;
            camera.LookDirection = newDirection;
            camera.UpDirection = newUpDirection;

            if (animationTime > 0)
            {
                var a1 = new Point3DAnimation(fromPosition, newPosition,
                    new Duration(TimeSpan.FromMilliseconds(animationTime))) { AccelerationRatio = 0.3, DecelerationRatio = 0.5, FillBehavior = FillBehavior.Stop };
                camera.BeginAnimation(ProjectionCamera.PositionProperty, a1);

                var a2 = new Vector3DAnimation(fromDirection, newDirection,
                                               new Duration(TimeSpan.FromMilliseconds(animationTime))) { AccelerationRatio = 0.3, DecelerationRatio = 0.5, FillBehavior = FillBehavior.Stop };
                camera.BeginAnimation(ProjectionCamera.LookDirectionProperty, a2);

                var a3 = new Vector3DAnimation(fromUpDirection, newUpDirection,
                                               new Duration(TimeSpan.FromMilliseconds(animationTime))) { AccelerationRatio = 0.3, DecelerationRatio = 0.5, FillBehavior = FillBehavior.Stop };
                camera.BeginAnimation(ProjectionCamera.UpDirectionProperty, a3);

            }
        }

        /// <summary>
        /// Animates the camera position and direction
        /// </summary>
        /// <param name="camera">An ortographic camera.</param>
        /// <param name="newWidth">The Width to animate to.</param>
        /// <param name="animationTime">Animation time in milliseconds</param>
        public static void AnimateWidth(OrthographicCamera camera, double newWidth, double animationTime)
        {
            double fromWidth  = camera.Width;

            camera.Width = newWidth;

            if (animationTime > 0)
            {
                var a1 = new DoubleAnimation(fromWidth, newWidth,
                    new Duration(TimeSpan.FromMilliseconds(animationTime))) { AccelerationRatio = 0.3, DecelerationRatio = 0.5, FillBehavior = FillBehavior.Stop };
                camera.BeginAnimation(OrthographicCamera.WidthProperty, a1);
            }
        }

    }
}
