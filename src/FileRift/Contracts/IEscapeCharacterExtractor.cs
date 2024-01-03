namespace FileRift.Contracts;

public interface IEscapeCharacterExtractor
{
    char? GetEscapeCharacter(string[] rows);
}