using System;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace HelixToolkit
{
    // todo: this is under construction...
    [Obsolete]
    public class AnimatedModelVisual3D : ModelVisual3D, IDisposable
    {
        #region Animation event/timer

        private readonly Stopwatch _animationWatch = new Stopwatch();
        private long _lastTick;

        private DispatcherTimer _updateTimer = new DispatcherTimer();

        public double AnimationTime
        {
            get { return _animationWatch.ElapsedMilliseconds*0.001; }
        }

        public void Dispose()
        {
            StopAnimationTimer();
        }

        public void ResetAnimation()
        {
            _animationWatch.Reset();
            _animationWatch.Start();
            _lastTick = _animationWatch.ElapsedTicks;
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
            long ticks = _animationWatch.ElapsedTicks;
            double time = 1.0*(ticks - _lastTick)/Stopwatch.Frequency;
            OnAnimationTick(time);
            _lastTick = ticks;
        }


        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            OnRendering();
        }

        #endregion

        public AnimatedModelVisual3D()
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