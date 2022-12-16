Imports System.Windows
Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System
Imports System.Windows.Input
Imports DevExpress.Xpf.Grid
Imports System.Windows.Media.Animation
Imports System.Windows.Threading
Imports System.Windows.Media

Namespace AnimateCells

    ''' <summary>
    ''' Interaction logic for Window1.xaml
    ''' </summary>
    Public Partial Class Window1
        Inherits Window

        Private racers As BindingList(Of Racer)

        Private animationElements As Dictionary(Of Integer, AnimationElement) = New Dictionary(Of Integer, AnimationElement)()

        Private timer As DispatcherTimer = New DispatcherTimer()

        Public Sub New()
            Me.InitializeComponent()
            InitData()
            Me.grid.ItemsSource = racers
            CommandBindings.Add(New CommandBinding(AnimationElement.Accelerate, AddressOf OnAccelerate, AddressOf OnCanAccelerate))
            CommandBindings.Add(New CommandBinding(AnimationElement.Decelerate, AddressOf OnDecelerate, AddressOf OnCanDecelerate))
            AddHandler Loaded, New RoutedEventHandler(AddressOf Window1_Loaded)
            AddHandler timer.Tick, New EventHandler(AddressOf timer_Tick)
            timer.Interval = TimeSpan.FromMilliseconds(100)
            timer.IsEnabled = True
        End Sub

        Private Sub timer_Tick(ByVal sender As Object, ByVal e As EventArgs)
            For Each racer As Racer In racers
                racer.Distance += GetAnimationElement(racer).CurrentSpeed * timer.Interval.TotalHours
            Next

            Dim sortedRacers As List(Of Racer) = New List(Of Racer)(racers)
            Dim comparison As Comparison(Of Racer) = New Comparison(Of Racer)(Function(ByVal x, ByVal y) Comparer.Default.Compare(x.Distance, y.Distance))
            sortedRacers.Sort(comparison)
            For Each racer As Racer In racers
                racer.Rank = racers.Count - sortedRacers.IndexOf(racer)
            Next

            Call CommandManager.InvalidateRequerySuggested()
        End Sub

        Private Sub Window1_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
            For Each racer As Racer In racers
                StartAnimation(racer, Colors.LightPink)
            Next
        End Sub

        Private Sub InitData()
            racers = New BindingList(Of Racer)()
            racers.Add(New Racer() With {.Name = "Lewis Hamilton"})
            racers.Add(New Racer() With {.Name = "Felipe Massa"})
            racers.Add(New Racer() With {.Name = "Kimi Räikkönen"})
            racers.Add(New Racer() With {.Name = "Robert Kubica"})
            racers.Add(New Racer() With {.Name = "Fernando Alonso"})
            racers.Add(New Racer() With {.Name = "Nick Heidfeld"})
            racers.Add(New Racer() With {.Name = "Heikki Kovalainen"})
            racers.Add(New Racer() With {.Name = "Michael Schumacher", .Speed = 0})
        End Sub

        Private Sub grid_CustomUnboundColumnData(ByVal sender As Object, ByVal e As GridColumnDataEventArgs)
            If e.Column Is Nothing OrElse Not e.IsGetData Then Return
            If Equals(e.Column.FieldName, "AnimationElement") Then
                e.Value = GetAnimationElement(e.ListSourceRowIndex)
            End If
        End Sub

        Private Sub OnAccelerate(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
            Dim racer As Racer = GetRacer(e.Parameter)
            racer.Speed = Math.Min(300, racer.Speed + 50)
            StartAnimation(racer, Colors.LightPink)
        End Sub

        Private Sub OnDecelerate(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
            Dim racer As Racer = GetRacer(e.Parameter)
            racer.Speed = Math.Max(0, racer.Speed - 50)
            StartAnimation(racer, Colors.LightBlue)
        End Sub

        Private Sub StartAnimation(ByVal racer As Racer, ByVal color As Color)
            Dim element As AnimationElement = GetAnimationElement(racer)
            Dim speedAnimation As DoubleAnimation = New DoubleAnimation() With {.[To] = racer.Speed, .AccelerationRatio = 0.5, .DecelerationRatio = 0.5, .Duration = New Duration(TimeSpan.FromSeconds(5))}
            Dim colorAnimation As ColorAnimation = New ColorAnimation() With {.From = color, .[To] = Colors.Transparent, .Duration = New Duration(TimeSpan.FromSeconds(5))}
            element.BeginAnimation(AnimationElement.CurrentSpeedProperty, speedAnimation)
            element.BeginAnimation(AnimationElement.SpeedColorProperty, colorAnimation)
        End Sub

        Private Sub OnCanAccelerate(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            Dim racer As Racer = GetRacer(e.Parameter)
            e.CanExecute = racer IsNot Nothing AndAlso racer.Speed < 300 AndAlso Not Equals(racer.Name, "Michael Schumacher")
        End Sub

        Private Sub OnCanDecelerate(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            Dim racer As Racer = GetRacer(e.Parameter)
            e.CanExecute = racer IsNot Nothing AndAlso racer.Speed > 0 AndAlso Not Equals(racer.Name, "Michael Schumacher")
        End Sub

        Private Function GetRacer(ByVal commandParameter As Object) As Racer
            If commandParameter Is Nothing Then Return Nothing
            Dim rowHandle As Integer = CType(commandParameter, RowData).RowHandle.Value
            Return CType(Me.grid.GetRow(rowHandle), Racer)
        End Function

        Private Function GetAnimationElement(ByVal racer As Racer) As AnimationElement
            Return GetAnimationElement(racers.IndexOf(racer))
        End Function

        Private Function GetAnimationElement(ByVal listIndex As Integer) As AnimationElement
            Dim element As AnimationElement
            If Not animationElements.TryGetValue(listIndex, element) Then
                element = New AnimationElement()
                animationElements(listIndex) = element
            End If

            Return element
        End Function
    End Class

    Public Class Racer
        Implements INotifyPropertyChanged

        Private Shared rnd As Random = New Random()

        Private speedField As Double = 100

        Private distanceField As Double

        Private rankField As Integer

        Public Property Name As String

        Public Sub New()
            speedField += rnd.NextDouble() * 20
        End Sub

        Public Property Rank As Integer
            Get
                Return rankField
            End Get

            Set(ByVal value As Integer)
                If Rank = value Then Return
                rankField = value
                OnPropertyChanged("Rank")
            End Set
        End Property

        Public Property Speed As Double
            Get
                Return speedField
            End Get

            Set(ByVal value As Double)
                If Speed = value Then Return
                speedField = value
                OnPropertyChanged("Speed")
            End Set
        End Property

        Public Property Distance As Double
            Get
                Return distanceField
            End Get

            Set(ByVal value As Double)
                If Distance = value Then Return
                distanceField = value
                OnPropertyChanged("Distance")
            End Set
        End Property

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Private Sub OnPropertyChanged(ByVal propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class

    Public Class AnimationElement
        Inherits FrameworkContentElement

        Public Shared ReadOnly Accelerate As RoutedUICommand = New RoutedUICommand("Accelerate", "Accelerate", GetType(AnimationElement))

        Public Shared ReadOnly Decelerate As RoutedUICommand = New RoutedUICommand("Decelerate", "Decelerate", GetType(AnimationElement))

        Public Shared ReadOnly CurrentSpeedProperty As DependencyProperty = DependencyProperty.Register("CurrentSpeed", GetType(Double), GetType(AnimationElement), New PropertyMetadata(0R))

        Public Shared ReadOnly SpeedColorProperty As DependencyProperty = DependencyProperty.Register("SpeedColor", GetType(Color), GetType(AnimationElement), New PropertyMetadata(Colors.Transparent))

        Public Property CurrentSpeed As Double
            Get
                Return CDbl(GetValue(CurrentSpeedProperty))
            End Get

            Set(ByVal value As Double)
                SetValue(CurrentSpeedProperty, value)
            End Set
        End Property

        Public Property SpeedColor As Color
            Get
                Return CType(GetValue(SpeedColorProperty), Color)
            End Get

            Set(ByVal value As Color)
                SetValue(SpeedColorProperty, value)
            End Set
        End Property
    End Class
End Namespace
