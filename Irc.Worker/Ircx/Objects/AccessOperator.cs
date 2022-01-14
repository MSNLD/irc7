namespace Irc.Worker.Ircx.Objects;

public class AccessOperator
{
    public EnumAccessOperator Operator;
    public string OperatorText;

    public AccessOperator(EnumAccessOperator Operator, string OperatorText)
    {
        this.Operator = Operator;
        this.OperatorText = OperatorText;
    }
}