namespace Microsoft.AspNetCore.Mvc;

public class RouteAttribute(string template) : Attribute
{
    public string Template { get; set; } = template;
}
