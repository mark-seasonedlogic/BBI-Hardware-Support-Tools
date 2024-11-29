using System.Threading.Tasks;

namespace BBIHardwareSupport
{
    public interface ITextModulePlugin : IModulePlugin
    {
        Task<string> GetTextDataAsync(string parameter);
    }
}
