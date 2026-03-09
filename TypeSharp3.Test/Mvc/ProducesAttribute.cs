using System.Collections.ObjectModel;

namespace Microsoft.AspNetCore.Mvc;

public class ProducesAttribute(string contentType, params string[] additionalContentTypes) : Attribute
{
    public Collection<string> ContentTypes { get; set; } = [contentType, .. additionalContentTypes];
}
