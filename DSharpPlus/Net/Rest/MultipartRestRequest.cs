using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

using DSharpPlus.Entities;

namespace DSharpPlus.Net;

/// <summary>
/// Represents a multipart HTTP request.
/// </summary>
internal sealed record MultipartRestRequest : IRestRequest
{
    /// <inheritdoc/>
    public required string Url { get; init; }

    /// <summary>
    /// The method for this request.
    /// </summary>
    public required HttpMethod Method { get; init; }

    /// <inheritdoc/>
    public required string Route { get; init; }

    /// <inheritdoc/>
    public bool IsExemptFromGlobalLimit { get; init; }

    /// <summary>
    /// The headers for this request.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Headers { get; init; }

    /// <summary>
    /// Gets the dictionary of values attached to this request.
    /// </summary>
    public required IReadOnlyDictionary<string, string> Values { get; init; }

    /// <summary>
    /// Gets the dictionary of files attached to this request.
    /// </summary>
    public IReadOnlyList<DiscordMessageFile>? Files { get; init; }

    public HttpRequestMessage Build()
    {
        HttpRequestMessage request = new()
        {
            Method = this.Method,
            RequestUri = new($"{Endpoints.BASE_URI}/{this.Url}")
        };

        if (this.Headers is not null)
        {
            foreach (KeyValuePair<string, string> header in this.Headers)
            {
                request.Headers.Add(header.Key, Uri.EscapeDataString(header.Value));
            }
        }

        request.Headers.Add("Connection", "keep-alive");
        request.Headers.Add("Keep-Alive", "600");

        string boundary = "---------------------------" + DateTimeOffset.UtcNow.Ticks.ToString("x");

        MultipartFormDataContent content = new(boundary);

        if (this.Values is not null)
        {
            foreach (KeyValuePair<string, string> element in this.Values)
            {
                content.Add(new StringContent(element.Value), element.Key);
            }
        }

        if (this.Files is not null)
        {
            for (int i = 0; i < this.Files.Count; i++)
            {
                DiscordMessageFile current = this.Files[i];

                StreamContent file = new(current.Stream);

                if (current.ContentType is not null)
                {
                    file.Headers.ContentType = MediaTypeHeaderValue.Parse(current.ContentType);
                }

                string filename = current.FileType is null
                    ? current.FileName
                    : $"{current.FileName}.{current.FileType}";

                // do we actually need this distinction? it's been made since the beginning of time,
                // but it doesn't seem very necessary
                if (this.Files.Count > 1)
                {
                    content.Add(file, $"file{i + 1}", filename);
                }
                else
                {
                    content.Add(file, "file", filename);
                }
            }
        }

        request.Content = content;

        return request;
    }
}
