using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using LiRACore.Models.RawData;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LiRACore.Helpers
{


    //public static class Serialize
    //{
    //    public static string ToJson(this Trip self) => JsonConvert.SerializeObject(self, Converter.Settings);
    //}


    public static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            };
        }

        internal class ParseStringConverter : JsonConverter
        {
            public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                long l;
                if (Int64.TryParse(value, out l))
                {
                    return l;
                }
                throw new Exception("Cannot unmarshal type long");
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (long)untypedValue;
                serializer.Serialize(writer, value.ToString());
                return;
            }

            public static readonly ParseStringConverter Singleton = new ParseStringConverter();




        }

        public static partial class JsonExtensions
        {
            public static void ToNewlineDelimitedJson<T>(Stream stream, IEnumerable<T> items)
            {
                // Let caller dispose the underlying stream 
                using (var textWriter = new StreamWriter(stream, new UTF8Encoding(false, true), 1024, true))
                {
                    ToNewlineDelimitedJson(textWriter, items);
                }
            }

            public static void ToNewlineDelimitedJson<T>(TextWriter textWriter, IEnumerable<T> items)
            {
                var serializer = JsonSerializer.CreateDefault();

                foreach (var item in items)
                {
                    // Formatting.None is the default; I set it here for clarity.
                    using (var writer = new JsonTextWriter(textWriter) { Formatting = Formatting.None, CloseOutput = false })
                    {
                        serializer.Serialize(writer, item);
                    }
                    // https://web.archive.org/web/20180513150745/http://specs.okfnlabs.org/ndjson/
                    // Each JSON text MUST conform to the [RFC7159] standard and MUST be written to the stream followed by the newline character \n (0x0A). 
                    // The newline charater MAY be preceeded by a carriage return \r (0x0D). The JSON texts MUST NOT contain newlines or carriage returns.
                    textWriter.Write("\n");
                }
            }
        }
    }

