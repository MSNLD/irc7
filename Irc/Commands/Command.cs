using Irc.Enumerations;

namespace Irc.Commands;

public class Command : ICommand
{
    protected int _requiredMaximumParameters;
    protected int _requiredMinimumParameters;

    public Command(int requiredMinimumParameters = 0, int requiredMaximumParameters = 0)
    {
        _requiredMinimumParameters = requiredMinimumParameters;
        _requiredMaximumParameters = requiredMaximumParameters;
    }

    public string GetName()
    {
        return GetType().Name;
    }

    public EnumCommandDataType GetDataType()
    {
        throw new NotImplementedException();
    }

    public void Execute(ChatFrame chatFrame)
    {
        throw new NotImplementedException();
    }

    public bool CheckParameters(ChatFrame chatFrame)
    {
        if (_requiredMinimumParameters < 0) return true;

        if (chatFrame.Message.Parameters.Count >= _requiredMinimumParameters) return true;

        return false;
    }
}