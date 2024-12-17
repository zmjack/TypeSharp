namespace Microsoft.AspNetCore.Mvc;

internal class RouteAttribute(string template) : Attribute
{
    public string Template { get; set; } = template;
}
