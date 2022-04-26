using System.Text;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.AspNetCore;
using CloudNative.CloudEvents.Core;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace CloudEvents.Utilities;

public class CloudEventInputFormatter: TextInputFormatter
{
    private readonly CloudEventFormatter _formatter;

    /// <summary>
    /// Constructs a new instance that uses the given formatter for deserialization.
    /// </summary>
    /// <param name="formatter"></param>
    public CloudEventInputFormatter(CloudEventFormatter formatter)
    {
        _formatter = Validation.CheckNotNull(formatter, nameof(formatter));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/cloudevents+json"));

        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    /// <inheritdoc />
    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        Validation.CheckNotNull(context, nameof(context));
        Validation.CheckNotNull(encoding, nameof(encoding));

        var request = context.HttpContext.Request;

        try
        {
            var cloudEvent = await request.ToCloudEventAsync(_formatter);
            return await InputFormatterResult.SuccessAsync(cloudEvent);
        }
        catch (Exception)
        {
            return await InputFormatterResult.FailureAsync();
        }
    }

    /// <inheritdoc />
    protected override bool CanReadType(Type type)
        => type == typeof(CloudEvent) && base.CanReadType(type);
}