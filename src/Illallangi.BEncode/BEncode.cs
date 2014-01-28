namespace Illallangi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public static class BEncode
    {
        public static dynamic ReadText(string text)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return BEncode.ReadStream(stream);
            }
        }

        public static dynamic ReadFile(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return BEncode.ReadStream(stream);
            }
        }

        private static dynamic ReadStream(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                return BEncode.Read(reader);
            }
        }

        private static dynamic Read(BinaryReader reader)
        {
            switch (reader.ReadChar())
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return BEncode.ReadString(reader);
                case 'i':
                    return BEncode.ReadInt(reader);
                case 'l':
                    return BEncode.ReadList(reader);
                case 'd':
                    return BEncode.ReadDictionary(reader);
                default:
                    throw new FormatException();
            }
        }

        private static string ReadString(BinaryReader reader)
        {
            var sb = new StringBuilder();
            reader.BaseStream.Seek(-1, SeekOrigin.Current);
            var length = BEncode.ReadInt(reader, ':');
            
            for (var i = 0; i < length; i++)
            {
                sb.Append(reader.ReadChar());
            }

            return sb.ToString();
        }

        private static int ReadInt(BinaryReader reader, char terminator = 'e')
        {
            var sb = new StringBuilder();
            char? c = null;

            do
            {
                if (c.HasValue)
                {
                    sb.Append(c.Value);
                }

                c = reader.ReadChar();
            }
            while (terminator != c);

            return int.Parse(sb.ToString());
        }

        private static IList<dynamic> ReadList(BinaryReader reader, char terminator = 'e')
        {
            var result = new List<dynamic>();

            if (terminator == reader.PeekChar())
            {
                return result;
            }

            do
            {
                result.Add(BEncode.Read(reader));
            }
            while (terminator != reader.PeekChar());

            return result;
        }

        private static IDictionary<string, dynamic> ReadDictionary(BinaryReader reader, char terminator = 'e')
        {
            var result = new Dictionary<string, dynamic>();

            if (terminator == reader.PeekChar())
            {
                return result;
            }

            do
            {
                result.Add(BEncode.Read(reader), BEncode.Read(reader));
            }
            while (terminator != reader.PeekChar());

            return result;
        }
    }
}
