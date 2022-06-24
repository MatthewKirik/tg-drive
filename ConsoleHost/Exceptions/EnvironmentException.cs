namespace ConsoleHost.Exceptions;

public class EnvironmentException : Exception
{
    public EnvironmentException(string variablePurpose, string variableName)
        : base(
            $"You should provide {variablePurpose} in {variableName} environment variable")
    {
    }
}
