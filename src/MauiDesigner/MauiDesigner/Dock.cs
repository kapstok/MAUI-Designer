using Microsoft.Maui.Layouts;

namespace MauiDesigner;

public class Dock
{
    private AbsoluteLayout screen;
    private bool initialHideDock = true;
    private ImageButton compileIcon;
    private CompilePage compilePage;

    public Dock()
    {
        compileIcon = new ImageButton
        {
            Source = "compile.png",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End
        };

        compilePage = new CompilePage();
        compileIcon.Clicked += (sender, args) => compilePage.CompileTarget();
    }

    public void OnCompile(Action action)
    {
        compileIcon.Clicked += (sender, args) => action();
    }

    public void OnGoBack(Action action)
    {
        compilePage.Disappearing += (sender, args) => action();
    }
    
    public ContentPage Draw(ContentPage contentPage)
    {
        compileIcon.Clicked += (sender, args) => Compile();

        screen = new AbsoluteLayout();
        screen.SetLayoutFlags(compileIcon, AbsoluteLayoutFlags.All);
        screen.SetLayoutBounds(compileIcon, new Rect(0, 0, 1, 1));
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

    private void Compile()
    {
        Shell.Current.CurrentPage.Navigation.PushAsync(compilePage);
    }
}