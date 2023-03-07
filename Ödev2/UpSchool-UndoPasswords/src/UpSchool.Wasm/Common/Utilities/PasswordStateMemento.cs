namespace UpSchool.Wasm.Common.Utilities;

public class PasswordStateMemento
{
    public string Value { get; set; }

    public PasswordStateMemento(PasswordState state)
    {
        Value = state.Password;
    }
}