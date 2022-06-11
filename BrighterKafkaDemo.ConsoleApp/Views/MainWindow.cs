using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui;
using Unix.Terminal;

namespace BrighterKafkaDemo.ConsoleApp.Views;

public class MainWindow : Window
{
    private readonly IServiceProvider _serviceProvider;
    
    public MainWindow(IServiceProvider serviceProvider ) : base("BrighterKafkaDemo.ConsoleApp")
    {
        _serviceProvider = serviceProvider;
    }

    public void InitStyles()
    {
        X = 0;
        Y = 1;
    }

    public void InitControls()
    {
        var menu = new MenuBar(new []
        {
            new MenuBarItem("_File", new[]
            {
                new MenuItem("_Quit", "", () => Application.RequestStop())
            })
        });
        
        Add(menu);

        var messagesListView = new ListView()
        {
            X = 0,
            Y = 2,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        
        Add(messagesListView);
        
        var messageContainer = _serviceProvider.GetService<MessageContainer>();
        messageContainer.RegisterReceiver(messagesListView);
    }
}