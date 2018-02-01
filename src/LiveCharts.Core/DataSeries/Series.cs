﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using LiveCharts.Core.Abstractions;
using LiveCharts.Core.Abstractions.DataSeries;
using LiveCharts.Core.Charts;
using LiveCharts.Core.Collections;
using LiveCharts.Core.Data;
using LiveCharts.Core.DefaultSettings;
using LiveCharts.Core.Drawing;
using LiveCharts.Core.Drawing.Svg;

namespace LiveCharts.Core.DataSeries
{
    /// <summary>
    /// The series class, represents a series to plot in a chart.
    /// </summary>
    /// <seealso cref="IResource" />
    public abstract class Series : IResource, ISeries
    {
        private readonly List<ChartModel> _usedBy = new List<ChartModel>();
        private bool _isVisible;
        private int _zIndex;
        private int[] _scalesAt;
        private bool _dataLabels;
        private string _title;
        private Color _stroke;
        private double _strokeThickness;
        private Color _fill;
        private Font _font;
        private double _defaultFillOpacity;
        private Geometry _geometry;
        private DataLabelsPosition _dataLabelsPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="Series"/> class.
        /// </summary>
        protected Series()
        {
            IsVisible = true;
            LiveChartsSettings.Build<ISeries>(this);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [data labels].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [data labels]; otherwise, <c>false</c>.
        /// </value>
        public bool DataLabels
        {
            get => _dataLabels;
            set
            {
                _dataLabels = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the index of the z.
        /// </summary>
        /// <value>
        /// The index of the z.
        /// </value>
        public int ZIndex
        {
            get => _zIndex;
            set
            {
                _zIndex = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the scales at array.
        /// </summary>
        /// <value>
        /// The scales at.
        /// </value>
        public int[] ScalesAt
        {
            get => _scalesAt;
            protected set
            {
                _scalesAt = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        /// <value>
        /// The stroke.
        /// </value>
        public Color Stroke
        {
            get => _stroke;
            set
            {
                _stroke = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        /// <value>
        /// The stroke thickness.
        /// </value>
        public double StrokeThickness
        {
            get => _strokeThickness;
            set
            {
                _strokeThickness = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the fill.
        /// </summary>
        /// <value>
        /// The fill.
        /// </value>
        public Color Fill
        {
            get => _fill;
            set
            {
                _fill = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public Font Font
        {
            get => _font;
            set
            {
                _font = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the default fill opacity.
        /// </summary>
        /// <value>
        /// The default fill opacity.
        /// </value>
        public double DefaultFillOpacity
        {
            get => _defaultFillOpacity;
            set
            {
                _defaultFillOpacity = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the default geometry.
        /// </summary>
        /// <value>
        /// The default geometry.
        /// </value>
        public Geometry Geometry
        {
            get => _geometry;
            set
            {
                _geometry = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the data labels position.
        /// </summary>
        /// <value>
        /// The data labels position.
        /// </value>
        public DataLabelsPosition DataLabelsPosition
        {
            get => _dataLabelsPosition;
            set
            {
                _dataLabelsPosition = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the default width of the point.
        /// </summary>
        /// <value>
        /// The default width of the point.
        /// </value>
        public abstract Point DefaultPointWidth { get; }

        /// <summary>
        /// Updates the view.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public abstract void UpdateView(ChartModel chart);

        /// <summary>
        /// Fetches the data for the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public abstract void Fetch(ChartModel chart);

        /// <summary>
        /// Gets the points that  its hover area contains the given n dimensions.
        /// </summary>
        /// <param name="selectionMode">The selection mode.</param>
        /// <param name="dimensions">The dimensions.</param>
        /// <returns></returns>
        public abstract IEnumerable<PackedPoint> SelectPointsByDimension(TooltipSelectionMode selectionMode, params double[] dimensions);

        /// <summary>
        /// Evaluates the data label.
        /// </summary>
        /// <param name="pointLocation">The point location.</param>
        /// <param name="pointMargin">The point margin.</param>
        /// <param name="betweenBottomLimit">The series bottom limit.</param>
        /// <param name="labelModel">The label model.</param>
        /// <param name="labelsPosition">The labels position.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// horizontal - null
        /// or
        /// vertical - null
        /// </exception>
        protected Point GetLabelPosition(
            Point pointLocation,
            Margin pointMargin,
            double betweenBottomLimit,
            Size labelModel,
            DataLabelsPosition labelsPosition)
        {
            const double toRadians = Math.PI / 180;
            var rotationAngle = DataLabelsPosition.Rotation;

            var xw =
                Math.Abs(Math.Cos(rotationAngle * toRadians) * labelModel.Width); // width's    horizontal    component
            var yw =
                Math.Abs(Math.Sin(rotationAngle * toRadians) * labelModel.Width); // width's    vertical      component
            var xh =
                Math.Abs(Math.Sin(rotationAngle * toRadians) * labelModel.Height); // height's   horizontal    component
            var yh =
                Math.Abs(Math.Cos(rotationAngle * toRadians) * labelModel.Height); // height's   vertical      component

            var width = xw + xh;
            var height = yh + yw;

            double left, top;

            switch (DataLabelsPosition.HorizontalAlignment)
            {
                case HorizontalAlingment.Centered:
                    left = pointLocation.X - .5 * width;
                    break;
                case HorizontalAlingment.Left:
                    left = pointLocation.X - pointMargin.Left - width;
                    break;
                case HorizontalAlingment.Right:
                    left = pointLocation.X + pointMargin.Right;
                    break;
                case HorizontalAlingment.Between:
                    left = ((pointLocation.X + betweenBottomLimit) / 2) - .5 * width;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(DataLabelsPosition.HorizontalAlignment), DataLabelsPosition.HorizontalAlignment,
                        null);
            }

            switch (DataLabelsPosition.VerticalAlignment)
            {
                case VerticalLabelPosition.Centered:
                    top = pointLocation.Y - .5 * height;
                    break;
                case VerticalLabelPosition.Top:
                    top = pointLocation.Y - pointMargin.Top - height;
                    break;
                case VerticalLabelPosition.Bottom:
                    top = pointLocation.Y + pointMargin.Bottom;
                    break;
                case VerticalLabelPosition.Between:
                    top = ((pointLocation.Y + betweenBottomLimit) / 2) - .5 * height;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(DataLabelsPosition.VerticalAlignment), DataLabelsPosition.VerticalAlignment, null);
            }

            return new Point(left, top);
        }

        internal void AddChart(ChartModel chart)
        {
            if (_usedBy.Contains(chart)) return;
            _usedBy.Add(chart);
        }

        #region IResource implementation

        public event DisposingResourceHandler Disposed;

        object IResource.UpdateId { get; set; }

        void IResource.Dispose(IChartView view)
        {
            OnDispose(view);
            Disposed?.Invoke(view);
        }

        protected virtual void OnDispose(IChartView view)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INPC implementation

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when a property changes.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    /// The series class with a defined plot model, represents a series to plot in a chart.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TCoordinate">The type of the coordinate.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TPoint">The type of the point.</typeparam>
    /// <seealso cref="IResource" />
    public abstract class Series<TModel, TCoordinate, TViewModel, TPoint> 
        : Series, IEnumerable<TModel>
        where TPoint : Point<TModel, TCoordinate, TViewModel>, new()
        where TCoordinate : ICoordinate
    {
        private IEnumerable<TModel> _itemsSource;
        private ModelToPointMapper<TModel, TCoordinate> _mapper;
        private Func<IPointView<TModel, Point<TModel, TCoordinate, TViewModel>, TCoordinate, TViewModel>>
            _pointViewProvider;
        private object _chartPointsUpdateId;
        private IList<TModel> _list;
        private INotifyRangeChanged<TModel> _rangedList;

        /// <summary>
        /// Initializes a new instance of the <see cref="Series{TModel, TCoordinate, TViewModel, TPoint}"/> class.
        /// </summary>
        protected Series()
        {
            ByValTracker = new List<TPoint>();
            _itemsSource = new PlotableCollection<TModel>();
            OnItemsIntancechanged();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Series{TModel, TCoordinate, TViewModel, TPoint}"/> class.
        /// </summary>
        /// <param name="itemsSource">The values.</param>
        protected Series(IEnumerable<TModel> itemsSource)
        {
            _itemsSource = itemsSource;
            OnItemsIntancechanged();
            ByValTracker = new List<TPoint>();
        }

        /// <summary>
        /// Gets or sets the <see cref="TModel"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="TModel"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public TModel this[int index]
        {
            get
            {
                EnsureIListImplementation();
                return _list[index];
            }
            set
            {
                EnsureIListImplementation();
                _list[index] = value;
            }
        }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public IEnumerable<TModel> ItemsSource
        {
            get => _itemsSource;
            set
            {
                _itemsSource = value;
                OnItemsIntancechanged();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the mapper.
        /// </summary>
        /// <value>
        /// The mapper.
        /// </value>
        public ModelToPointMapper<TModel, TCoordinate> Mapper
        {
            get => _mapper ?? LiveChartsSettings.GetCurrentMapperFor<TModel, TCoordinate>();
            set
            {
                _mapper = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the by value tracker.
        /// </summary>
        /// <value>
        /// The by value tracker.
        /// </value>
        public IList<TPoint> ByValTracker { get; }

        /// <summary>
        /// Gets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public IEnumerable<TPoint> Points { get; private set; }

        /// <summary>
        /// Gets or sets the point builder.
        /// </summary>
        /// <value>
        /// The point builder.
        /// </value>
        public Func<TModel, TViewModel> PointBuilder { get; set; }

        /// <summary>
        /// Gets or sets the point view provider.
        /// </summary>
        /// <value>
        /// The point view provider.
        /// </value>
        public Func<IPointView<TModel, Point<TModel, TCoordinate, TViewModel>, TCoordinate, TViewModel>>
            PointViewProvider
        {
            get => _pointViewProvider ?? DefaultPointViewProvider;
            set
            {
                _pointViewProvider = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the data range.
        /// </summary>
        /// <value>
        /// The data range.
        /// </value>
        public DimensionRange DataRange { get; } = new DimensionRange(double.PositiveInfinity, double.NegativeInfinity);

        /// <summary>
        /// Defaults the point view provider.
        /// </summary>
        /// <returns></returns>
        protected abstract IPointView<TModel, Point<TModel, TCoordinate, TViewModel>, TCoordinate, TViewModel>
            DefaultPointViewProvider();

        /// <inheritdoc />
        public override void Fetch(ChartModel model)
        {
            // returned cached points if this method was called from the same updateId.
            if (_chartPointsUpdateId == model.UpdateId) return;
            _chartPointsUpdateId = model.UpdateId;

            // Assign a color if the user did not set it.
            if (Stroke == Color.Empty || Fill == Color.Empty)
            {
                var nextColor = model.GetNextColor();
                if (Stroke == Color.Empty)
                {
                    Stroke = nextColor;
                }

                if (Fill == Color.Empty)
                {
                    Fill = nextColor.SetOpacity(DefaultFillOpacity);
                }
            }

            // call the factory to fetch our data.
            // Fetch() has 2 main tasks.
            // 1. Calculate each ChartPoint required by the series.
            // 2. Evaluate every dimension to get Max and Min limits.
            Points = LiveChartsSettings.Current.DataFactory
                .FetchData(
                    new DataFactoryArgs<TModel, TCoordinate, TViewModel, TPoint>
                    {
                        Series = this,
                        Chart = model,
                        Collection = ItemsSource.ToArray() // create a copy of the current points.
                    })
                .ToArray();
        }

        /// <inheritdoc />
        public override IEnumerable<PackedPoint> SelectPointsByDimension(TooltipSelectionMode selectionMode, params double[] dimensions)
        {
            return Points.Where(point => point.HoverArea.Contains(selectionMode, dimensions))
                .Select(point => new PackedPoint
                {
                    Key = point.Key,
                    Model = point.Model,
                    Chart = point.Chart,
                    Coordinate = point.Coordinate,
                    Series = point.Series,
                    View = point.View,
                    ViewModel = point.ViewModel
                });
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="LiveChartsException">Items - 200</exception>
        public void Add(TModel item)
        {
           EnsureIListImplementation();
            _list.Add(item);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(TModel item)
        {
            EnsureIListImplementation();
            _list.Remove(item);
        }

        /// <summary>
        /// Removes at a specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            EnsureIListImplementation();
            _list.RemoveAt(index);
        }

        /// <summary>
        /// Adds the given range of items.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(IEnumerable<TModel> items)
        {
            EnsureINotifyRangeImplementation();
            _rangedList.AddRange(items);
        }

        /// <summary>
        /// Removes the given range of items.
        /// </summary>
        /// <param name="items">The items.</param>
        public void RemoveRange(IEnumerable<TModel> items)
        {
            EnsureINotifyRangeImplementation();
            _rangedList.RemoveRange(items);
        }

        private void EnsureIListImplementation([CallerMemberName] string method = null)
        {
            if (_list == null)
            {
                throw new LiveChartsException(
                    $"{nameof(ItemsSource)} property, does not implement {nameof(IList<TModel>)}, " +
                    $"thus the method {method} is not supported.",
                    200);
            }
        }

        private void EnsureINotifyRangeImplementation([CallerMemberName] string method = null)
        {
            if (_list == null)
            {
                throw new LiveChartsException(
                    $"{nameof(ItemsSource)} property, does not implement {nameof(INotifyRangeChanged<TModel>)}, " +
                    $"thus the method {method} is not supported.",
                    200);
            }
        }

        private void OnItemsIntancechanged()
        {
            _list = _itemsSource as IList<TModel>;
            _rangedList = ItemsSource as INotifyRangeChanged<TModel>;
        }

        public IEnumerator<TModel> GetEnumerator()
        {
            return ItemsSource.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
