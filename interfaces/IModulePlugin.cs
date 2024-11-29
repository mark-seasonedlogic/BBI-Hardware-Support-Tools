namespace BBIHardwareSupport
{
public interface IModulePlugin
{
    string Name { get; }
    void Initialize();

}
}
