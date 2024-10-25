# Weather Report
This application displays the current weather conditions such as temperature, wind speed and direction for the selected city.
I created this small application to take a deeper dive when experimenting with several technologies.
- WPF with the MVVM pattern using the [Caliburn.Micro](https://github.com/Caliburn-Micro/Caliburn.Micro) framework.
- Boilerplate code for setting up DI (net8-style), in particular for Caliburn.Micro using the [Dapplo.Microsoft.Extensions.Hosting](https://github.com/dapplo/Dapplo.Microsoft.Extensions.Hosting#dapplomicrosoftextensionshostingcaliburnmicro) library.
- Unit tests using [AutoFixture](https://autofixture.github.io/docs/quick-start/), AutoMoq and xUnit for injecting the instances of SUTs and their dependencies into the test code.

## Program setup
The application gets away from the usual WPF basis with App.xaml as the entry point. Instead it uses the static [Program.Main] (WeatherReport.WinApp/Program.cs) method to create the instance of the generic host via HostBuilder which in turn is used to set up the DI container. It also uses a couple of convenient extension methods from Dapplo.Microsoft.Extensions.Hosting library, such as ConfigureWpf and ConfigureCaliburnMicro to set up Caliburn.Micro in the context of a generic host-based application.

## Caliburn.Micro and user controls
When creating UI components (user controls) I tried both view-first and viewmodel-first approach.
This [Gregory Reddick's blog post](https://blog.xoc.net/2018/06/using-usercontrols-with-caliburnmicro.html) helped me a lot with understanding the differences of the implementation in both cases.
The Caliburn.Micro documentation states that viewmodel-first is its preferred approach. This is especially true when a viewmodel relies on a DI container to inject the necessary dependencies.
However, when a UI control has some custom properties and its viewmodel does not need any dependencies (has default constructor) then the view-first implementation is more straightforward, see for example [ScrewView.xaml](WeatherReport.WinApp/Views/ScrewView.xaml), [ScrewView.xaml.cs](WeatherReport.WinApp/Views/ScrewView.xaml.cs) and [ScrewViewModel.cs](WeatherReport.WinApp/ViewModels/ScrewViewModel.cs).

## Unit testing with AutoFixture
AutoFixture with its implementation of SUT factory takes away a significant part of the burden of creating a SUT and its dependencies which improves maintainability of the tests. Together with the xUnit facilities and mocking framework extensions (Moq in this case) it makes tests more declarative by significantly reducing the Arrange part of the tests.