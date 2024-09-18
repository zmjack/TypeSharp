namespace TypeSharp;

public interface IEncodable
{
    string Encode(Indent indent, string ownerPrefix);
}
