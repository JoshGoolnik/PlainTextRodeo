using Rodeo.PlainText.Core.Entities;

namespace Rodeo.PlainText.Core.Intefaces
{
    internal interface IPlainTextDataParser
    {
        DataNodeResult Parse(string input);
    }
}
