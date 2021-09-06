<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128650236/21.1.5%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E1532)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [Window1.xaml](./CS/AnimateCells/Window1.xaml) (VB: [Window1.xaml](./VB/AnimateCells/Window1.xaml))
* [Window1.xaml.cs](./CS/AnimateCells/Window1.xaml.cs) (VB: [Window1.xaml](./VB/AnimateCells/Window1.xaml))
<!-- default file list end -->
# How to embed animation into grid cells


<p>Starting with version 17.1, our controls provide the built-in way to show the current <a href="https://documentation.devexpress.com/WPF/17130/Controls-and-Libraries/Data-Grid/Conditional-Formatting">Conditional Formatting</a> with an animated effect. All you need is to apply the required conditional formatting and then set the <a href="https://documentation.devexpress.com/WPF/DevExpress.Xpf.Grid.TableView.AnimateConditionalFormattingTransition.property">TableView.AnimateConditionalFormattingTransition</a> property to True. If your task is to use your own custom animation that cannot be implemented using the <a href="https://documentation.devexpress.com/WPF/17130/Controls-and-Libraries/Data-Grid/Conditional-Formatting">Conditional Formatting</a> feature, use the approach from the current example. It illustrates how to implement an animated element, and display it within a grid cell. This functionality is based on using cell templates.</p>
<p><strong>See Also:</strong><br> <a href="https://www.devexpress.com/Support/Center/p/E841">How to highlight modified rows</a></p>

<br/>


