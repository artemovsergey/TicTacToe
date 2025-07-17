using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TicTacToeApp.API.Data.Converters;

public class DateTimeToCharConverter : ValueConverter<DateTime, string>
{
    public DateTimeToCharConverter() : base(
        dateTime => dateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
        stringValue => DateTime.ParseExact(stringValue, "yyyyMMdd", CultureInfo.InvariantCulture)
    )
    {
    }
}