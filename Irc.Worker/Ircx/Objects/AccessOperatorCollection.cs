using System.Collections.Generic;
using Irc.Constants;

namespace Irc.Worker.Ircx.Objects;

public class AccessOperatorCollection
{
    public List<AccessOperator> Operators;

    public AccessOperatorCollection()
    {
        Operators = new List<AccessOperator>();
        Operators.Add(new AccessOperator(EnumAccessOperator.ADD, Resources.AccessEntryOperatorAdd));
        Operators.Add(new AccessOperator(EnumAccessOperator.DELETE, Resources.AccessEntryOperatorDelete));
        Operators.Add(new AccessOperator(EnumAccessOperator.LIST, Resources.AccessEntryOperatorList));
        Operators.Add(new AccessOperator(EnumAccessOperator.CLEAR, Resources.AccessEntryOperatorClear));
    }
}