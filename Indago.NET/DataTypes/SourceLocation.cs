using Com.Cadence.Indago.Scripting.Generated;

namespace Indago.DataTypes;

/// <summary>
/// Represents the source location of an object.
/// </summary>
/// <param name="blSourceLocation"></param>
public class SourceLocation(BusinessLogicSourceLocation blSourceLocation)
{
    /// <summary>
    /// The full file system path to the file that declares this object.
    /// </summary>
    /// <example>
    /// <code>
    /// var fileName = signal.Declaration.Source.FileName;
    /// </code>
    /// </example>
    public string FileName => blSourceLocation.File;

    /// <summary>
    /// The start line within the file upon which this object declaration/instantiation/exists.
    /// (Start with 1)
    /// </summary>
    public int StartLine => (int)blSourceLocation.StartLine;
    
    /// <summary>
    /// The end line within the file upon which this object declaration/instantiation/exists.
    /// (Start with 1)
    /// </summary>
    public int EndLine => (int)blSourceLocation.EndLine;

    /// <summary>
    /// The fetchable range of lines within the file upon which this object declaration/instantiation/exists.
    /// (Start with 0)
    /// </summary>
    public Range LineRange => (StartLine - 1)..EndLine;
    
    /// <summary>
    /// The start column within the file upon which this object declaration/instantiation/exists.
    /// (Start with 1)
    /// </summary>
    public int StartColumn => (int)blSourceLocation.StartColumn;
    
    /// <summary>
    /// The end column within the file upon which this object declaration/instantiation/exists.
    /// (Start with 1)
    /// </summary>
    public int EndColumn => (int)blSourceLocation.EndColumn;
    
    /// <summary>
    /// The fetchable range of columns within the file upon which this object declaration/instantiation/exists.
    /// (Start with 0)
    /// </summary>
    public Range ColumnRange => (StartColumn - 1)..EndColumn;

    /// <summary>
    /// Get the source code of the file that declares this object
    /// by open the file and read it
    /// </summary>
    /// <returns>Code snippet of the object declaration</returns>
    public string GetSourceCode()
    {
        string[] lines = File.ReadAllLines(FileName);
        string[] usableLines = lines[LineRange];

        if (StartColumn == EndColumn)
        {
            return string.Join('\n', usableLines);
        }
        
        if (usableLines.Length == 1)
        {
            return usableLines[0][ColumnRange];
        }

        usableLines[0] = usableLines[0][ColumnRange.Start..];
        usableLines[^1] = usableLines[^1][..ColumnRange.End];
        return string.Join('\n', usableLines);
    }

    private Range LineRangeActual => StartLine..EndLine;
    private Range ColumnRangeActual => StartColumn..EndColumn;
    
    public override string ToString()
        => $"{FileName}[{LineRangeActual}, {ColumnRangeActual}]";
}