namespace WebVella.Tefter.Database.Dbo;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal class DboTypeConverterAttribute : Attribute
{
    public IDboPropertyValueConverter Converter { get; set; } = null;

    private DboTypeConverterAttribute()
    {
    }

    public DboTypeConverterAttribute(Type converterType)
    {
        if (converterType == null)
            throw new Exception("DboTypeConverterAttribute: Converter type provided is null");


        if (!typeof(IDboPropertyValueConverter).IsAssignableFrom(converterType))
            throw new Exception("DboTypeConverterAttribute: Converter type provided does not inherite IDboPropertyValueConverter");

        Converter = (IDboPropertyValueConverter)Activator.CreateInstance(converterType);
    }
}
