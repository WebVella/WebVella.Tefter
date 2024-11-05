namespace WebVella.Tefter.Database.Dbo;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal class TfDboTypeConverterAttribute : Attribute
{
    public ITfDboPropertyValueConverter Converter { get; set; } = null;

    private TfDboTypeConverterAttribute()
    {
    }

    public TfDboTypeConverterAttribute(Type converterType)
    {
        if (converterType == null)
            throw new Exception("DboTypeConverterAttribute: Converter type provided is null");


        if (!typeof(ITfDboPropertyValueConverter).IsAssignableFrom(converterType))
            throw new Exception("DboTypeConverterAttribute: Converter type provided does not inherite IDboPropertyValueConverter");

        Converter = (ITfDboPropertyValueConverter)Activator.CreateInstance(converterType);
    }
}
