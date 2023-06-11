namespace coreconsole.enums;

public enum ExitCode: int
{
    Success = 0,
    BadBase64 = 1,
    Base64NotPokemon = 2,
    UnknownErrorDuringBase64ToPokemon = 3,
    EnvNotConfigured = 4
}