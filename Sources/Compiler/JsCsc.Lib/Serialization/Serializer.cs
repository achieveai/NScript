//-----------------------------------------------------------------------
// <copyright file="Serializer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib.Serialization
{
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Definition for Serializer
    /// </summary>
    public class Serializer
    {
        public enum SerializationKind
        {
            NetSerializer,
            ProtoBuf,
            Json
        }

        private static NetSerializer.Serializer s_netSerializer;
        private static JsonSerializer s_jsonSerializer;

        private static NetSerializer.Serializer NetSerializer
        {
            get
            {
                if (s_netSerializer == null)
                {
                    s_netSerializer = new NetSerializer.Serializer(
                        typeof(Serializer).Assembly.GetTypes()
                            .Where((t) =>
                                t.Namespace == "JsCsc.Lib.Serialization"
                                && t.CustomAttributes
                                    .Any(att =>
                                        att.AttributeType.Name == "SerializableAttribute")));
                }

                return s_netSerializer;
            }
        }

        private static JsonSerializer JsonSerializer
        {
            get
            {
                if (s_jsonSerializer == null)
                {
                    s_jsonSerializer = JsonSerializer
                        .Create(new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        });
                }

                return s_jsonSerializer;
            }
        }

        public static void Serialize(
            Stream stream,
            FullAst fullAst,
            SerializationKind kind = SerializationKind.NetSerializer)
        {
            switch (kind)
            {
                case SerializationKind.NetSerializer:
                    NetSerializer.SerializeDirect(
                        stream,
                        fullAst);
                    break;
                case SerializationKind.ProtoBuf:
                    ProtoBuf.Serializer.Serialize(
                        stream,
                        fullAst);
                    break;
                case SerializationKind.Json:
                    using (var writer = new JsonTextWriter(
                        new StreamWriter(
                            stream,
                            System.Text.Encoding.UTF8,
                            16 * 1024,
                            true)))
                    {
                        JsonSerializer.Serialize(
                            writer,
                            fullAst);
                    }

                    break;
            }
        }

        public static FullAst Deserialize(
            Stream stream,
            SerializationKind kind = SerializationKind.NetSerializer)
        {
            switch (kind)
            {
                case SerializationKind.NetSerializer:
                    NetSerializer.DeserializeDirect<FullAst>(
                        stream,
                        out var fullAst);
                    return fullAst;
                case SerializationKind.ProtoBuf:
                    return ProtoBuf.Serializer.Deserialize<FullAst>(
                        stream);
                case SerializationKind.Json:
                    var reader = new JsonTextReader(new StreamReader(stream));
                    return JsonSerializer.Deserialize<FullAst>(reader);
            }

            return null;
        }
    }
}
