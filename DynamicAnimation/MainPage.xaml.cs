using System;
using System.Diagnostics;
using System.Numerics;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace DynamicAnimation
{
    public sealed partial class MainPage
    {
        private CompositionPropertySet _propertySet;
        private readonly Inclinometer _inclinometer;
        public MainPage()
        {
            InitializeComponent();

            _inclinometer = Inclinometer.GetDefault();
        }

        private void TheGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            var bounds = Window.Current.Bounds;

            var hostVisual = ElementCompositionPreview.GetElementVisual(TheGrid);
            var visual = hostVisual.Compositor.CreateSpriteVisual();
            visual.Brush = hostVisual.Compositor.CreateColorBrush(Colors.Red);
            visual.Size = new Vector2(100, 100);
            visual.Offset = new Vector3(((float)TheGrid.ActualWidth - visual.Size.X) / 2.0f, (float)(TheGrid.ActualHeight - visual.Size.Y) / 2.0f, 0);
            ElementCompositionPreview.SetElementChildVisual(TheGrid, visual);


            var anim = visual.Compositor.CreateExpressionAnimation("Vector3(This.CurrentValue.X >= shared.step ? (This.CurrentValue.X <= (shared.width - shared.step) ? (This.CurrentValue.X + (shared.pitch == 0 ? 0 : shared.pitch > 0 ? shared.step : -shared.step) ) : shared.pitch >= 0 ? shared.width : shared.width - shared.step) : shared.pitch <= 0 ? 0 : shared.step,This.CurrentValue.Y >= shared.step ? (This.CurrentValue.Y <= (shared.height - shared.step) ? (This.CurrentValue.Y + (shared.roll == 0 ? 0 : shared.roll > 0 ? shared.step : -shared.step) ) : shared.roll >= 0 ? shared.height : shared.height - shared.step) : shared.roll <= 0 ? 0 : shared.step,This.CurrentValue.Z)");
            _propertySet = visual.Compositor.CreatePropertySet();
            _propertySet.InsertScalar("roll", 0);
            _propertySet.InsertScalar("pitch", 0);
            _propertySet.InsertScalar("step", 3);
            _propertySet.InsertScalar("width", (float)Math.Floor(bounds.Width) - visual.Size.X);
            _propertySet.InsertScalar("height", (float)Math.Floor(bounds.Height) - visual.Size.Y);

            anim.SetReferenceParameter("shared", _propertySet);
            visual.StartAnimation(nameof(visual.Offset), anim);
            _inclinometer.ReadingTransform = DisplayOrientations.LandscapeFlipped;
            _inclinometer.ReadingChanged += InclinometerOnReadingChanged;
        }

        private void InclinometerOnReadingChanged(Inclinometer sender, InclinometerReadingChangedEventArgs args)
        {
            _propertySet.InsertScalar("roll", args.Reading.RollDegrees);
            _propertySet.InsertScalar("pitch", args.Reading.PitchDegrees);
            Debug.WriteLine($"ROLL : {args.Reading.RollDegrees} PITCH : {args.Reading.PitchDegrees}");
        }
    }
}
