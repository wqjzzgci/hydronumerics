using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.CoordinateTransforms;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes
{
    public abstract partial class AxisControl<T> : ContentControl where T : IComparable
    {
        private StackCanvas layoutStackCanvas = new StackCanvas();
        private Size availableSize;

        private Range<T> range;
        public Range<T> Range
        {
            get
            {
                return range;
            }
            set
            {
                range = value;
                CheckSizeConstraints();
                UpdateUIRepresentation();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.availableSize = availableSize;
            Size realResult = base.MeasureOverride(availableSize);
            Size res = new Size(double.IsInfinity(availableSize.Width) ? (realResult.Width) : (availableSize.Width), double.IsInfinity(availableSize.Height) ? (realResult.Height) : (availableSize.Height));
            return res;
        }

        public AxisControl()
        {
            Content = layoutStackCanvas;
            layoutStackCanvas.Placement = Placement;
            UpdateSizeGetters();
            UpdateUILayout();
            loadedPartsChanged += new EventHandler(AxisControl_loadedPartsChanged);
            //SizeChanged += new SizeChangedEventHandler(AxisControl_SizeChanged);
        }

        void AxisControl_loadedPartsChanged(object sender, EventArgs e)
        {
            isMinorTicksSyned = false;
            CheckPartsTicksCounts();
            CheckPartsMinorTicksCount();
        }

        private void CheckSizeConstraints()
        {
            if (Placement == AxisPlacement.Left || Placement == AxisPlacement.Right)
            {
                if (availableSize.Height != Height) UpdateUILayout();
            }
            else
            {
                if (availableSize.Width != Width) UpdateUILayout();
            }
        }

        #region Providers
        private LabelProviderBase<T> mayorLabelProvider;
        /// <summary>
        /// Gets or sets the mayor label provider, which creates labels for mayor ticks.
        /// If null, mayor labels will not be shown.
        /// </summary>
        /// <value>The mayor label provider.</value>
        public LabelProviderBase<T> MayorLabelProvider
        {
            get { return mayorLabelProvider; }
            set
            {
                if (mayorLabelProvider != value)
                {
                    DetachMayorLabelProvider();

                    mayorLabelProvider = value;

                    AttachMayorLabelProvider();

                    UpdateUIRepresentation();
                }
            }
        }

        private bool syncPartsTicks = false;

        /// <summary>
        /// Whether to sync ticks counts between different axisParts or not. Default is false;
        /// </summary>
        public bool SyncPartsTicks
        {
            get
            {
                return syncPartsTicks;
            }
            set
            {
                syncPartsTicks = value;
            }
        }

        private LabelProviderBase<T> labelProvider;
        /// <summary>
        /// Gets or sets the label provider, which generates labels for axis ticks.
        /// Should not be null.
        /// </summary>
        /// <value>The label provider.</value>
        public LabelProviderBase<T> LabelProvider
        {
            get { return labelProvider; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (labelProvider != value)
                {
                    DetachLabelProvider();

                    labelProvider = value;

                    AttachLabelProvider();

                    UpdateUIRepresentation();
                }
            }
        }

        private ITicksProvider<T> ticksProvider;
        public ITicksProvider<T> TicksProvider
        {
            get { return ticksProvider; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (ticksProvider != value)
                {
                    ticksProvider = value;
                    UpdateUIRepresentation();
                }
            }
        }

        private IPartsProvider<T> partsProvider;
        public IPartsProvider<T> PartsProvider
        {
            get
            {
                return partsProvider;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (partsProvider != value)
                {
                    partsProvider = value;
                    partsProvider.ParentAxis = this;
                    UpdateUIRepresentation();
                }
            }
        }

        public Func<T, double> ConvertToDouble;
        private Func<Point, double> GetCoordinate = p => p.X;
        private Func<double, Point> CreateDataPoint = d => new Point(d, 0);


        public event EventHandler loadedPartsChanged;
        private void RaiseLoadedPartsChanged()
        {
            if (loadedPartsChanged != null)
                loadedPartsChanged(this, null);
        }



        private CoordinateTransform transform = CoordinateTransform.CreateDefault();

        public CoordinateTransform Transform
        {
            get { return transform; }
            set
            {
                if (transform != value)
                {
                    transform = value;
                    UpdateUIRepresentation();
                }
            }
        }

        #endregion

        #region View Options
        private AxisPlacement placement = AxisPlacement.Bottom;
        public AxisPlacement Placement
        {
            get { return placement; }
            set
            {
                if (placement != value)
                {
                    placement = value;
                    layoutStackCanvas.Placement = Placement;
                    UpdateSizeGetters();
                    UpdateUILayout();
                }
            }
        }

        #endregion

        #region Helpful methods
        private void AttachLabelProvider()
        {
            if (labelProvider != null)
            {
                labelProvider.Changed += labelProvider_Changed;
            }
        }

        private void DetachLabelProvider()
        {
            if (labelProvider != null)
            {
                labelProvider.Changed -= labelProvider_Changed;
            }
        }

        private void labelProvider_Changed(object sender, EventArgs e)
        {
            UpdateUIRepresentation();
        }

        private void DetachMayorLabelProvider()
        {
            if (mayorLabelProvider != null)
            {
                mayorLabelProvider.Changed -= mayorLabelProvider_Changed;
            }
        }

        private void AttachMayorLabelProvider()
        {
            if (mayorLabelProvider != null)
            {
                mayorLabelProvider.Changed += mayorLabelProvider_Changed;
            }
        }

        private void mayorLabelProvider_Changed(object sender, EventArgs e)
        {
            UpdateUIRepresentation();
        }

        private void InitTransform(Size newRenderSize)
        {
            Rect dataRect = CreateDataRect();

            transform = transform.WithRects(dataRect, new Rect(new Point(0, 0), newRenderSize));
        }

        private void UpdateSizeGetters()
        {
            switch (placement)
            {
                case AxisPlacement.Left:
                case AxisPlacement.Right:
                    GetCoordinate = p => p.Y;
                    break;
                case AxisPlacement.Top:
                case AxisPlacement.Bottom:
                    GetCoordinate = p => p.X;
                    break;
                default:
                    break;
            }

            switch (placement)
            {
                case AxisPlacement.Left:
                    CreateDataPoint = d => new Point(0, d);
                    break;
                case AxisPlacement.Right:
                    CreateDataPoint = d => new Point(1, d);
                    break;
                case AxisPlacement.Top:
                    CreateDataPoint = d => new Point(d, 1);
                    break;
                case AxisPlacement.Bottom:
                    CreateDataPoint = d => new Point(d, 0);
                    break;
                default:
                    break;
            }
        }

        private Rect CreateDataRect()
        {
            double min = ConvertToDouble(Range.Min);
            double max = ConvertToDouble(Range.Max);

            Rect dataRect;
            switch (placement)
            {
                case AxisPlacement.Left:
                case AxisPlacement.Right:
                    dataRect = new Rect(new Point(min, min), new Point(max, max));
                    break;
                case AxisPlacement.Top:
                case AxisPlacement.Bottom:
                    dataRect = new Rect(new Point(min, min), new Point(max, max));
                    break;
                default:
                    throw new NotSupportedException();
            }
            return dataRect;
        }

        #endregion

#if DEBUG
        public string DebugString { get; set; }
#endif
        private void UpdateUILayout()
        {
            if (availableSize.Height == 0 || availableSize.Width == 0) return;
            switch (Placement)
            {
                case AxisPlacement.Left:
                case AxisPlacement.Right:
                    Height = availableSize.Height;
                    //HorizontalAlignment = HorizontalAlignment.Stretch;
                    break;
                case AxisPlacement.Top:
                case AxisPlacement.Bottom:
                    Width = availableSize.Width;
                    break;
                default: throw new NotSupportedException("Placement");
            }
            CleanPartsCach();
            UpdateUIRepresentation();
        }

        bool updateUIRfrozen;
        /// <summary>
        /// Freeze automatic UI Represntation updates
        /// </summary>
        public void FreezeUirUpdates()
        {
            updateUIRfrozen = true;
        }

        /// <summary>
        /// Unfreeze automatic UI Represntation updates
        /// </summary>
        public void UnfreezeUirUpdates()
        {
            updateUIRfrozen = false;
        }

        private void CleanPartsCach()
        {
            partsProvider.CleanCach();
            foreach (AxisPartControl<T> p in partsPresentOnScreen)
            {
                p.SizeChanged -= part_SizeChanged;
            }
            partsPresentOnScreen.Clear();
            layoutStackCanvas.Children.Clear();
            ticksConstraint = -1;
        }

        private List<double> screenTicks = new List<double>();
        private List<double> tmpList = new List<double>();
        public double[] ScreenTicks
        {
            get
            {
                screenTicks.Clear();
                foreach (AxisPartControl<T> part in partsPresentOnScreen)
                {
                    tmpList.Clear();
                    tmpList.AddRange(part.ScreenTicks);
                    for (int i = 0; i < part.ScreenTicks.Length; i++)
                    {
                        tmpList[i] += StackCanvas.GetCoordinate(part);
                    }
                    screenTicks.AddRange(tmpList);
                }
                screenTicks.Sort();
                return screenTicks.ToArray();
            }
        }

        private List<MinorTickInfo<double>> minorScreenTicks = new List<MinorTickInfo<double>>();
        public MinorTickInfo<double>[] MinorScreenTicks
        {
            get
            {
                minorScreenTicks.Clear();
                foreach (AxisPartControl<T> part in partsPresentOnScreen)
                {
                    minorScreenTicks.AddRange(part.MinorScreenTicks);
                }
                return minorScreenTicks.ToArray();
            }
        }
        private int ticksConstraint = -1;
        private int minorTicksConstaint = -1;

        private bool isMinorTicksSyned = false;
        private void CheckPartsMinorTicksCount()
        {
            if (!SyncPartsTicks) return;
            minorTicksConstaint = -1;
            bool done;
            do
            {
                done = true;
                foreach (AxisPartControl<T> part in partsPresentOnScreen)
                {
                    if (part.MinorTicksCount != 0)
                    {
                        if (minorTicksConstaint == -1)
                        {
                            minorTicksConstaint = part.MinorTicksCount;
                            continue;
                        }

                        if (part.MinorTicksCount < minorTicksConstaint)
                        {
                            done = false;
                            minorTicksConstaint = part.MinorTicksCount;
                            break;
                        }
                        if (part.MinorTicksCount > minorTicksConstaint)
                        {
                            part.MinorTicksUpperConstraint = minorTicksConstaint;
                        }
                    }
                    else return;
                }

            } while (!done);
            isMinorTicksSyned = true;
        }

        private void CheckPartsTicksCounts()
        {
            if(!SyncPartsTicks) return;
            ticksConstraint = -1;
            bool done;
            do
            {
                done = true;
                foreach (AxisPartControl<T> part in partsPresentOnScreen)
                {
                    if (ticksConstraint == -1)
                    {
                        ticksConstraint = part.TicksCount;
                        continue;
                    }
                    if (part.TicksCount < ticksConstraint)
                    {
                        done = false;
                        ticksConstraint = part.TicksCount;
                        break;
                    }
                    if (part.TicksCount > ticksConstraint)
                    {
                        part.TicksUpperConstraint = ticksConstraint;
                    }
                }
            } while (!done);
        }

        private List<AxisPartControl<T>> partsPresentOnScreen = new List<AxisPartControl<T>>();
        private double activeScale = 0;
        StringBuilder builder = new StringBuilder();
        public void UpdateUIRepresentation()
        {
            if (updateUIRfrozen || ConvertToDouble == null || Transform == null || PartsProvider == null || LabelProvider == null || TicksProvider == null) return;
            if (RenderSize.Height == 0 && RenderSize.Width == 0)
                return;
//            InitTransform(RenderSize);

            //has the scale changed?
            if (Math.Abs(activeScale - (ConvertToDouble(Range.Max) - ConvertToDouble(Range.Min))) > (activeScale / 100))
            {
                activeScale = ConvertToDouble(Range.Max) - ConvertToDouble(Range.Min);
                CleanPartsCach();
            }

            Range<T>[] partsSizes = PartsProvider.GetPartsSizes(Range);

            builder = new StringBuilder();

            //Let's calculate what parts do we need, and position them
            foreach (Range<T> r in partsSizes)
            {
                double left = GetCoordinate(CreateDataPoint(ConvertToDouble(r.Min)).DataToScreen(Transform));
                double right = GetCoordinate(CreateDataPoint(ConvertToDouble(r.Max)).DataToScreen(Transform));
                AxisPartControl<T> part = PartsProvider.GetPart(r);
                //shall we add the part
                if (!partsPresentOnScreen.Contains(part))
                {
                    layoutStackCanvas.Children.Add(part);
                    switch (Placement)
                    {
                        case AxisPlacement.Left:
                        case AxisPlacement.Right:
                            part.Height = Math.Abs(right - left);
                            part.Clip = new RectangleGeometry() { Rect = new Rect(new Point(), new Size(100, part.Height)) };
                            break;
                        case AxisPlacement.Top:
                        case AxisPlacement.Bottom:
                            part.Width = Math.Abs(right - left);
                            part.Clip = new RectangleGeometry() { Rect = new Rect(new Point(), new Size(part.Width, 100)) };
                            break;
                    }
                    partsPresentOnScreen.Add(part);
                    part.SizeChanged += new SizeChangedEventHandler(part_SizeChanged);
                    RaiseLoadedPartsChanged();
                }

                //just move the part
                switch (Placement)
                {
                    case AxisPlacement.Left:
                    case AxisPlacement.Right:
                        StackCanvas.SetCoordinate(part, right);
                        break;
                    case AxisPlacement.Bottom:
                    case AxisPlacement.Top:
                        StackCanvas.SetCoordinate(part, left);
                        break;

                }

                builder.AppendLine(Placement + ":" + r.ToString());// + " converted: (" + left + "," + right + ")");
                //" act size: ("+part.ActualWidth+", "+part.ActualHeight+") "+
                //"rand size: ("+part.RenderSize+")");
            }

            //Parts to be removed from the screen
            List<AxisPartControl<T>> toRemove = new List<AxisPartControl<T>>();

            //if we have unnessesery parts, remove them            
            foreach (AxisPartControl<T> part in partsPresentOnScreen)
            {
                if (partsProvider.ShouldRemoveFromScreen(part,Range))
                    toRemove.Add(part);
            }

            foreach (AxisPartControl<T> p in toRemove)
            {
                partsPresentOnScreen.Remove(p);
                layoutStackCanvas.Children.Remove(p);
                p.SizeChanged -= part_SizeChanged;
            }
            if (toRemove.Count > 0) RaiseLoadedPartsChanged();

            if (!isMinorTicksSyned) CheckPartsMinorTicksCount();

#if DEBUG
            DebugString = "Axis rander size: " + RenderSize +
                "\nAvl size: " + availableSize +
                "\n"
                + builder.ToString();
#endif
        }

        void part_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RaiseLoadedPartsChanged();
        }
    }
}