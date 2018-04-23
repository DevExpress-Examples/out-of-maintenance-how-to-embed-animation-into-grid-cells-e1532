using System.Windows;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using System.Windows.Input;
using DevExpress.Xpf.Grid;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Media;

namespace AnimateCells {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window {
        BindingList<Racer> racers;
        Dictionary<int, AnimationElement> animationElements = new Dictionary<int, AnimationElement>();
        DispatcherTimer timer = new DispatcherTimer();
        public Window1() {
            InitializeComponent();
            InitData();
            grid.DataSource = racers;
            CommandBindings.Add(new CommandBinding(AnimationElement.Accelerate, OnAccelerate, OnCanAccelerate));
            CommandBindings.Add(new CommandBinding(AnimationElement.Decelerate, OnDecelerate, OnCanDecelerate));
            Loaded += new RoutedEventHandler(Window1_Loaded);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.IsEnabled = true;
        }

        void timer_Tick(object sender, EventArgs e) {
            foreach(Racer racer in racers) {
                racer.Distance += GetAnimationElement(racer).CurrentSpeed * timer.Interval.TotalHours;   
            }
            List<Racer> sortedRacers = new List<Racer>(racers);
            Comparison<Racer> comparison = new Comparison<Racer>(delegate(Racer x, Racer y) {
                return Comparer.Default.Compare(x.Distance, y.Distance);
            });
            sortedRacers.Sort(comparison);
            foreach(Racer racer in racers) {
                racer.Rank = racers.Count - sortedRacers.IndexOf(racer);
            }
            CommandManager.InvalidateRequerySuggested();
        }

        void Window1_Loaded(object sender, RoutedEventArgs e) {
            foreach(Racer racer in racers) {
                StartAnimation(racer, Colors.LightPink);
            }
        }
        void InitData() {
            racers = new BindingList<Racer>();
            racers.Add(new Racer() { Name = "Lewis Hamilton" });
            racers.Add(new Racer() { Name = "Felipe Massa" });
            racers.Add(new Racer() { Name = "Kimi Räikkönen" });
            racers.Add(new Racer() { Name = "Robert Kubica" });
            racers.Add(new Racer() { Name = "Fernando Alonso" });
            racers.Add(new Racer() { Name = "Nick Heidfeld" });
            racers.Add(new Racer() { Name = "Heikki Kovalainen" });
            racers.Add(new Racer() { Name = "Michael Schumacher", Speed = 0 });
        }

        private void grid_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e) {
            if(e.Column == null || !e.IsGetData)
                return;
            if(e.Column.FieldName == "AnimationElement") {
                e.Value = GetAnimationElement(e.ListSourceRowIndex);
            }
        }
        void OnAccelerate(object sender, ExecutedRoutedEventArgs e) {
            Racer racer = GetRacer(e.Parameter);
            racer.Speed = Math.Min(300, racer.Speed + 50);
            StartAnimation(racer, Colors.LightPink);
        }
        void OnDecelerate(object sender, ExecutedRoutedEventArgs e) {
            Racer racer = GetRacer(e.Parameter);
            racer.Speed = Math.Max(0, racer.Speed - 50);
            StartAnimation(racer, Colors.LightBlue);
        }
        void StartAnimation(Racer racer, Color color) { 
            AnimationElement element = GetAnimationElement(racer);
            DoubleAnimation speedAnimation = new DoubleAnimation() { 
                To = racer.Speed, 
                AccelerationRatio = 0.5, 
                DecelerationRatio = 0.5, 
                Duration = new Duration(TimeSpan.FromSeconds(5)) 
            };
            ColorAnimation colorAnimation = new ColorAnimation()
            {
                From = color,
                To = Colors.Transparent,
                Duration = new Duration(TimeSpan.FromSeconds(5))
            };
            element.BeginAnimation(AnimationElement.CurrentSpeedProperty, speedAnimation);
            element.BeginAnimation(AnimationElement.SpeedColorProperty, colorAnimation);
        }
        void OnCanAccelerate(object sender, CanExecuteRoutedEventArgs e) {
            Racer racer = GetRacer(e.Parameter);
            e.CanExecute = racer != null && racer.Speed < 300 && racer.Name != "Michael Schumacher";
        }
        void OnCanDecelerate(object sender, CanExecuteRoutedEventArgs e) {
            Racer racer = GetRacer(e.Parameter);
            e.CanExecute = racer != null && racer.Speed > 0 && racer.Name != "Michael Schumacher";
        }
        Racer GetRacer(object commandParameter) {
            if(commandParameter == null) return null;
            int rowHandle = ((RowData)commandParameter).RowHandle.Value;
            return (Racer)grid.GetRow(rowHandle);
        }
        AnimationElement GetAnimationElement(Racer racer) {
            return GetAnimationElement(racers.IndexOf(racer));
        }
        AnimationElement GetAnimationElement(int listIndex) {
            AnimationElement element;
            if(!animationElements.TryGetValue(listIndex, out element)) {
                element = new AnimationElement();
                animationElements[listIndex] = element;
            }
            return element;
        }

    }
    public class Racer : INotifyPropertyChanged {
        static Random rnd = new Random();
        double speed = 100;
        double distance;
        int rank;
        public string Name { get; set; }
        public Racer() {
            speed += rnd.NextDouble() * 20;
        }
        public int Rank {
            get { return rank; }
            set {
                if(Rank == value)
                    return;
                rank = value;
                OnPropertyChanged("Rank");
            }
        }
        public double Speed {
            get { return speed; }
            set {
                if(Speed == value)
                    return;
                speed = value;
                OnPropertyChanged("Speed");
            }
        }
        public double Distance {
            get { return distance; }
            set {
                if(Distance == value)
                    return;
                distance = value;
                OnPropertyChanged("Distance");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName) {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }
    }
    public class AnimationElement : FrameworkContentElement {
        public static readonly RoutedUICommand Accelerate = new RoutedUICommand("Accelerate", "Accelerate", typeof(AnimationElement));
        public static readonly RoutedUICommand Decelerate = new RoutedUICommand("Decelerate", "Decelerate", typeof(AnimationElement));
        public static readonly DependencyProperty CurrentSpeedProperty = DependencyProperty.Register("CurrentSpeed", typeof(double), typeof(AnimationElement), new PropertyMetadata(0d));
        public static readonly DependencyProperty SpeedColorProperty = DependencyProperty.Register("SpeedColor", typeof(Color), typeof(AnimationElement), new PropertyMetadata(Colors.Transparent));
        public double CurrentSpeed {
            get { return (double)GetValue(CurrentSpeedProperty); }
            set { SetValue(CurrentSpeedProperty, value); }
        }
        public Color SpeedColor {
            get { return (Color)GetValue(SpeedColorProperty); }
            set { SetValue(SpeedColorProperty, value); }
        }
    }
}
