using System.ComponentModel;

public record DocumentResponse(ValidationResult Success, string Issue);

public enum ValidationResult
{
    WIN, MISTAKE, FAIL
}