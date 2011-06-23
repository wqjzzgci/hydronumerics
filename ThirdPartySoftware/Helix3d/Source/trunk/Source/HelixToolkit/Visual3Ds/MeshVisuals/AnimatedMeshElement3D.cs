using System;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;

namespace HelixToolkit
{
    // todo: this is under construction...
    public abstract class AnimatedMeshElement3D : MeshElement3D, IDisposable
    {
        #region Animation event/timer

        private readonly Stopwatch animationWatch = new Stopwatch();
        private long lastTick;

        private DispatcherTimer _updateTimer = new DispatcherTimer();

        public double AnimationTime
        {
            get { return animationWatch.ElapsedMilliseconds*0.001; }
        }

        public void Dispose()
        {
            StopAnimationTimer();
        }

        public void ResetAnimation()
        {
            animationWatch.Reset();
            animationWatch.Start();
            lastTick = animationWatch.ElapsedTicks;
        }

        public void StartAnimationTimer(double interval)
        {
            ResetAnimation();
            _updateTimer.Interval = TimeSpan.FromSeconds(interval);
            _updateTimer.IsEnabled = true;
        }

        public void StopAnimationTimer()
        {
            _updateTimer.IsEnabled = false;
            _updateTimer = null;
        }

        private void UpdateTimerTick(object sender, EventArgs e)
        {
            long ticks = animationWatch.ElapsedTicks;
            double time = 1.0*(ticks - lastTick)/Stopwatch.Frequency;
            OnAnimationTick(time);
            lastTick = ticks;
        }


        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            OnRendering();
        }

        #endregion

        protected AnimatedMeshElement3D()
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            _updateTimer.Tick += UpdateTimerTick;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt">Delta time since last call of OnAnimationTick (seconds)</param>
        protected virtual void OnAnimationTick(double dt)
        {
        }

        protected virtual void OnRendering()
        {
        }
    }
}