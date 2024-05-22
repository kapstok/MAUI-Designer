using Microsoft.Maui.Layouts;

namespace MauiDesigner;

public class Dock
{
    private VerticalStackLayout terminal;
    private AbsoluteLayout screen;
    private bool initialHideDock = true;

    public Dock()
    {
        Button button = new Button();
        button.Text = "Close";
        button.Clicked += (sender, args) => terminal.IsVisible = false;
        
        terminal = new VerticalStackLayout();
        terminal.BackgroundColor = Color.FromRgba(81, 43, 212, 150);
        terminal.IsVisible = false;
        terminal.Children.Add(button);
    }
    
    public ContentPage Draw(ContentPage contentPage)
    {
        ImageButton compileIcon = new ImageButton
        {
            Source = "compile.png",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End
        };
        compileIcon.Clicked += (sender, args) =>
        {
            terminal.IsVisible = true;
            terminal.WidthRequest = screen.Width;
            terminal.HeightRequest = screen.Height - 150;
        };

        screen = new AbsoluteLayout();
        screen.SetLayoutFlags(compileIcon, AbsoluteLayoutFlags.All);
        screen.SetLayoutBounds(compileIcon, new Rect(0, 0, 1, 1));
        screen.Children.Add(terminal);
        screen.Children.Add(compileIcon);

        if (initialHideDock)
        {
            compileIcon.TranslateTo(0, 100, 0);
        }
        
        PointerGestureRecognizer gestures = new PointerGestureRecognizer();
        gestures.PointerEntered += (sender, args) =>
        {
            compileIcon.TranslateTo(0, 0);
            initialHideDock = false;
        };
        gestures.PointerExited += (sender, args) =>
        {
            compileIcon.TranslateTo(0, 100);
            initialHideDock = true;
        }; 
        screen.GestureRecognizers.Add(gestures);

        Grid grid = new Grid();
        grid.Children.Add(contentPage.Content);
        grid.Children.Add(screen);
        contentPage.Content = grid;

        return contentPage;
    }
}