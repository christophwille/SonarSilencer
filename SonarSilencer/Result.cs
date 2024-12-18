namespace SonarSilencer
{
    /*
        https://andrewlock.net/working-with-the-result-pattern-part-1-replacing-exceptions-as-control-flow/
        https://andrewlock.net/working-with-the-result-pattern-part-2-safety-and-simplicity-with-linq/
        https://andrewlock.net/working-with-the-result-pattern-part-3-adding-more-extensions/
        https://andrewlock.net/working-with-the-result-pattern-part-4-is-the-result-pattern-worth-it/
     */

    public enum ResultStatus
    {
        Success,
        NotFound,
        Error,
    }

    public record ResultResponse<T>(ResultStatus Status, T? Result, string Message);
}
