using Newtonsoft.Json.Linq;

public interface IParsable
{
    void Parse(JObject jObject);
}