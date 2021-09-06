Imports System.Windows
Imports System.Data
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
	Partial Public Class Window1
		Inherits Window

		Private racers As BindingList(Of Racer)
		Private animationElements As New Dictionary(Of Integer, AnimationElement)()
		Private timer As New DispatcherTimer()
		Public Sub New()
			InitializeComponent()
			InitData()
			grid.ItemsSource = racers
			CommandBindings.Add(New CommandBinding(AnimationElement.Accelerate, AddressOf OnAccelerate, AddressOf OnCanAccelerate))
			CommandBindings.Add(New CommandBinding(AnimationElement.Decelerate, AddressOf OnDecelerate, AddressOf OnCanDecelerate))
			AddHandler Me.Loaded, AddressOf Window1_Loaded
			AddHandler timer.Tick, AddressOf timer_Tick
			timer.Interval = TimeSpan.FromMilliseconds(100)
			timer.IsEnabled = True
		End Sub

		Private Sub timer_Tick(ByVal sender As Object, ByVal e As EventArgs)
			For Each racer As Racer In racers
				racer.Distance += GetAnimationElement(racer).CurrentSpeed * timer.Interval.TotalHours
			Next racer
			Dim sortedRacers As New List(Of Racer)(racers)
			Dim comparison As New Comparison(Of Racer)(Function(x As Racer, y As Racer)
				Return Comparer.Default.Compare(x.Distance, y.Distance)
			End Function)
			sortedRacers.Sort(comparison)
			For Each racer As Racer In racers
				racer.Rank = racers.Count - sortedRacers.IndexOf(racer)
			Next racer
			CommandManager.InvalidateRequerySuggested()
		End Sub

		Private Sub Window1_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			For Each racer As Racer In racers
				StartAnimation(racer, Colors.LightPink)
			Next racer
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
			racers.Add(New Racer() With {
				.Name = "Michael Schumacher",
				.Speed = 0
			})
		End Sub

		Private Sub grid_CustomUnboundColumnData(ByVal sender As Object, ByVal e As DevExpress.Xpf.Grid.GridColumnDataEventArgs)
			If e.Column Is Nothing OrElse Not e.IsGetData Then
				Return
			End If
			If e.Column.FieldName = "AnimationElement" Then
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
			Dim speedAnimation As New DoubleAnimation() With {
				.To = racer.Speed,
				.AccelerationRatio = 0.5,
				.DecelerationRatio = 0.5,
				.Duration = New Duration(TimeSpan.FromSeconds(5))
			}
			Dim colorAnimation As New ColorAnimation() With {
				.From = color,
				.To = Colors.Transparent,
				.Duration = New Duration(TimeSpan.FromSeconds(5))
			}
			element.BeginAnimation(AnimationElement.CurrentSpeedProperty, speedAnimation)
			element.BeginAnimation(AnimationElement.SpeedColorProperty, colorAnimation)
		End Sub
		Private Sub OnCanAccelerate(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
			Dim racer As Racer = GetRacer(e.Parameter)
			e.CanExecute = racer IsNot Nothing AndAlso racer.Speed < 300 AndAlso racer.Name <> "Michael Schumacher"
		End Sub
		Private Sub OnCanDecelerate(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
			Dim racer As Racer = GetRacer(e.Parameter)
			e.CanExecute = racer IsNot Nothing AndAlso racer.Speed > 0 AndAlso racer.Name <> "Michael Schumacher"
		End Sub
		Private Function GetRacer(ByVal commandParameter As Object) As Racer
			If commandParameter Is Nothing Then
				Return Nothing
			End If
			Dim rowHandle As Integer = DirectCast(commandParameter, RowData).RowHandle.Value
			Return CType(grid.GetRow(rowHandle), Racer)
		End Function
		Private Function GetAnimationElement(ByVal racer As Racer) As AnimationElement
			Return GetAnimationElement(racers.IndexOf(racer))
		End Function
		Private Function GetAnimationElement(ByVal listIndex As Integer) As AnimationElement
			Dim element As AnimationElement = Nothing
			If Not animationElements.TryGetValue(listIndex, element) Then
				element = New AnimationElement()
				animationElements(listIndex) = element
			End If
			Return element
		End Function

	End Class
	Public Class Racer
		Implements INotifyPropertyChanged

		Private Shared rnd As New Random()
'INSTANT VB NOTE: The field speed was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private speed_Conflict As Double = 100
'INSTANT VB NOTE: The field distance was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private distance_Conflict As Double
'INSTANT VB NOTE: The field rank was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private rank_Conflict As Integer
		Public Property Name() As String
		Public Sub New()
			speed_Conflict += rnd.NextDouble() * 20
		End Sub
		Public Property Rank() As Integer
			Get
				Return rank_Conflict
			End Get
			Set(ByVal value As Integer)
				If Rank = value Then
					Return
				End If
				rank_Conflict = value
				OnPropertyChanged("Rank")
			End Set
		End Property
		Public Property Speed() As Double
			Get
				Return speed_Conflict
			End Get
			Set(ByVal value As Double)
				If Speed = value Then
					Return
				End If
				speed_Conflict = value
				OnPropertyChanged("Speed")
			End Set
		End Property
		Public Property Distance() As Double
			Get
				Return distance_Conflict
			End Get
			Set(ByVal value As Double)
				If Distance = value Then
					Return
				End If
				distance_Conflict = value
				OnPropertyChanged("Distance")
			End Set
		End Property
		Public Event PropertyChanged As PropertyChangedEventHandler
		Private Sub OnPropertyChanged(ByVal propertyName As String)
			RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))

		End Sub
	End Class
	Public Class AnimationElement
		Inherits FrameworkContentElement

		Public Shared ReadOnly Accelerate As New RoutedUICommand("Accelerate", "Accelerate", GetType(AnimationElement))
		Public Shared ReadOnly Decelerate As New RoutedUICommand("Decelerate", "Decelerate", GetType(AnimationElement))
		Public Shared ReadOnly CurrentSpeedProperty As DependencyProperty = DependencyProperty.Register("CurrentSpeed", GetType(Double), GetType(AnimationElement), New PropertyMetadata(0R))
		Public Shared ReadOnly SpeedColorProperty As DependencyProperty = DependencyProperty.Register("SpeedColor", GetType(Color), GetType(AnimationElement), New PropertyMetadata(Colors.Transparent))
		Public Property CurrentSpeed() As Double
			Get
				Return DirectCast(GetValue(CurrentSpeedProperty), Double)
			End Get
			Set(ByVal value As Double)
				SetValue(CurrentSpeedProperty, value)
			End Set
		End Property
		Public Property SpeedColor() As Color
			Get
				Return DirectCast(GetValue(SpeedColorProperty), Color)
			End Get
			Set(ByVal value As Color)
				SetValue(SpeedColorProperty, value)
			End Set
		End Property
	End Class
End Namespace
