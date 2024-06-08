namespace WebApps.UI.Models.Commons;

public class BaseViewModel
{
    protected BaseViewModel()
    {
        MainClass = "";
        ShowSiteBottom = true;
    }

    public string MainClass { get; set; }

    public bool ShowSiteBottom { get; set; }
}