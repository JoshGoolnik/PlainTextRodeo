using Rodeo.PlainText.Core.Entities;

namespace Rodeo.PlainText.Core.Interfaces
{
    public interface IPlainTextDataParser
    {
        DataNodeResult Parse(string input);
    }
}
