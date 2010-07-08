#region license
/*
Licensed to the Apache Software Foundation (ASF) under one or more
contributor license agreements.  See the NOTICE file distributed with
this work for additional information regarding copyright ownership.
The ASF licenses this file to You under the Apache License, Version 2.0
(the "License"); you may not use this file except in compliance with
the License.  You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using SolrDocument = System.Collections.Generic.IDictionary<string, object>;

namespace SolrNet.Impl {
    /// <summary>
    /// The class is designed to optimaly serialize/deserialize any supported types in Solr response. As we know there are only a limited type of
    /// items this class can do it with very minimal amount of payload and code. There are 15 known types and if there is an
    /// object in the object tree which does not fall into these types, It must be converted to one of these. Implement an
    /// ObjectResolver and pass it over It is expected that this class is used on both end of the pipes. The class has one
    /// read method and one write method for each of the datatypes
    /// </summary>
    /// <remarks>
    /// Never re-use an instance of this class for more than one marshal or unmarshall operation. 
    /// Always create a new instance.
    /// </remarks>
    public class JavaBinCodec {
        public const byte NULL = 0,
                          BOOL_TRUE = 1,
                          BOOL_FALSE = 2,
                          BYTE = 3,
                          SHORT = 4,
                          DOUBLE = 5,
                          INT = 6,
                          LONG = 7,
                          FLOAT = 8,
                          DATE = 9,
                          MAP = 10,
                          SOLRDOC = 11,
                          SOLRDOCLST = 12,
                          BYTEARR = 13,
                          ITERATOR = 14,
                          END = 15, // this is a special tag signals an end. No value is associated with it
                          // types that combine tag + length (or other info) in a single byte
                          TAG_AND_LEN = 1 << 5,
                          STR = 1 << 5,
                          SINT = 2 << 5,
                          SLONG = 3 << 5,
                          ARR = 4 << 5, //
                          ORDERED_MAP = 5 << 5, // SimpleOrderedMap (a NamedList subclass, and more common)
                          NAMED_LST = 6 << 5, // NamedList
                          EXTERN_STRING = 7 << 5;

        private const int epochYears = 1969; // for some reason it's not 1970
        private static readonly byte VERSION = 1;
        private readonly IObjectResolver resolver;
        protected Stream daos;

        public JavaBinCodec() {}

        public JavaBinCodec(IObjectResolver resolver) {
            this.resolver = resolver;
        }

        public void Marshal(Object nl, Stream os) {
            daos = os;
            try {
                daos.WriteByte(VERSION);
                WriteVal(nl);
            } finally {
                daos.Flush();
            }
        }

        private int version;

        public Object Unmarshal(Stream inputStream) {
            var dis = inputStream;
            version = dis.ReadByte();
            if (version != VERSION) {
                throw new Exception("Invalid version or the data in not in 'javabin' format");
            }
            return ReadVal(dis);
        }

        public OrderedDictionary ReadOrderedMap(Stream dis) {
            int sz = ReadSize(dis);
            var nl = new OrderedDictionary();
            for (int i = 0; i < sz; i++) {
                var name = (String) ReadVal(dis);
                Object val = ReadVal(dis);
                nl[name] = val;
            }
            return nl;
        }

        // TODO implement NamedList
        //      public NamedList readNamedList(Stream dis)
        //      {
        //          int sz = ReadSize(dis);
        //          NamedList nl = new NamedList();
        //          for (int i = 0; i < sz; i++)
        //          {
        //              String name = (String)ReadVal(dis);
        //              Object val = ReadVal(dis);
        //              nl.add(name, val);
        //          }
        //          return nl;
        //      }

        //      public void writeNamedList(NamedList nl) {
        //  WriteTag(nl is SimpleOrderedMap ? ORDERED_MAP : NAMED_LST, nl.size());
        //  for (int i = 0; i < nl.size(); i++) {
        //    String name = nl.getName(i);
        //    WriteExternString(name);
        //    Object val = nl.getVal(i);
        //    WriteVal(val);
        //  }
        //}

        public void WriteVal(Object val) {
            if (WriteKnownType(val)) {
                return;
            }
            Object tmpVal = val;
            if (resolver != null) {
                tmpVal = resolver.Resolve(val, this);
                if (tmpVal == null)
                    return; // null means the resolver took care of it fully
                if (WriteKnownType(tmpVal))
                    return;
            }

            WriteVal(val.GetType().Name + ':' + val);
        }

        protected static readonly Object END_OBJ = new Object();

        private int tagByte;

        public Object ReadVal(Stream dis) {
            tagByte = dis.ReadByte();

            // if ((tagByte & 0xe0) == 0) {
            // if top 3 bits are clear, this is a normal tag

            // OK, try type + size in single byte
            var i = usr(tagByte, 5);
            if (i == usr(STR, 5))
                return ReadStr(dis);
            if (i == usr(SINT, 5))
                return ReadSmallInt(dis);
            if (i == usr(SLONG, 5))
                return ReadSmallLong(dis);
            if (i == usr(ARR, 5))
                return ReadArray(dis);
            if (i == usr(ORDERED_MAP, 5))
                return ReadOrderedMap(dis);
            if (i == usr(NAMED_LST, 5))
                throw new NotImplementedException();
            //return readNamedList(dis);
            if (i == usr(EXTERN_STRING, 5))
                return ReadExternString(dis);
            var reader = new BinaryReader(dis);
            switch (tagByte) {
                case NULL:
                    return null;
                case DATE:
                    var ms = ReadLongFromStream(dis);
                    var ticks = TimeSpan.TicksPerMillisecond*ms;
                    return new DateTime(ticks).AddYears(epochYears);
                case INT:
                    return reader.ReadInt32();
                case BOOL_TRUE:
                    return true;
                case BOOL_FALSE:
                    return false;
                case FLOAT:
                    return reader.ReadSingle();
                case DOUBLE:
                    return reader.ReadDouble();
                case LONG:
                    return ReadLongFromStream(dis);
                case BYTE:
                    return (byte)dis.ReadByte();
                case SHORT:
                    return reader.ReadInt16();
                case MAP:
                    return ReadMap(dis);
                case SOLRDOC:
                    return ReadSolrDocument(dis);
                case SOLRDOCLST:
                    return ReadSolrDocumentList(dis);
                case BYTEARR:
                    return ReadByteArray(dis);
                case ITERATOR:
                    return ReadIterator(dis);
                case END:
                    return END_OBJ;
            }

            throw new Exception("Unknown type " + tagByte);
        }

        private static void WriteLongToStream(Stream s, long v) {
            s.WriteByte((byte)usr(v, 56));
            s.WriteByte((byte)usr(v, 48));
            s.WriteByte((byte)usr(v, 40));
            s.WriteByte((byte)usr(v, 32));
            s.WriteByte((byte)usr(v, 24));
            s.WriteByte((byte)usr(v, 16));
            s.WriteByte((byte)usr(v, 8));
            s.WriteByte((byte)v);
        }

        private static long ReadLongFromStream(Stream s) {
            return (((long)s.ReadByte()) << 56)
                       | (((long)s.ReadByte()) << 48)
                       | (((long)s.ReadByte()) << 40)
                       | (((long)s.ReadByte()) << 32)
                       | (((long)s.ReadByte()) << 24)
                       | (s.ReadByte() << 16)
                       | (s.ReadByte() << 8)
                       | (s.ReadByte());
        }

        public bool WriteKnownType(Object val) {
            if (WritePrimitive(val))
                return true;
            //if (val is NamedList) {
            //  writeNamedList((NamedList) val);
            //  return true;
            //}
            //if (val is SolrDocumentList) { // SolrDocumentList is a List, so must come before List check
            //  writeSolrDocumentList((SolrDocumentList) val);
            //  return true;
            //}
            if (val is ICollection) {
                WriteArray((ICollection) val);
                return true;
            }
            if (val is Object[]) {
                WriteArray((Object[]) val);
                return true;
            }
            //if (val is SolrDocument) {
            //  //this needs special treatment to know which fields are to be written
            //  if (resolver == null) {
            //    writeSolrDocument((SolrDocument) val);
            //  } else {
            //    Object retVal = resolver.Resolve(val, this);
            //    if (retVal != null) {
            //      if (retVal is SolrDocument) {
            //        writeSolrDocument((SolrDocument) retVal);
            //      } else {
            //        WriteVal(retVal);
            //      }
            //    }
            //  }
            //  return true;
            //}
            if (val is IDictionary) {
                WriteMap((IDictionary) val);
                return true;
            }
            if (val is IEnumerable) {
                WriteIterator((IEnumerable) val);
                return true;
            }
            return false;
        }

        public void WriteTag(byte tag) {
            daos.WriteByte(tag);
        }

        public void WriteTag(byte tag, int size) {
            if ((tag & 0xe0) != 0) {
                if (size < 0x1f) {
                    daos.WriteByte((byte) (tag | size));
                } else {
                    daos.WriteByte((byte) (tag | 0x1f));
                    WriteVInt(size - 0x1f, daos);
                }
            } else {
                daos.WriteByte(tag);
                WriteVInt(size, daos);
            }
        }

        public void WriteByteArray(byte[] arr, int offset, int len) {
            WriteTag(BYTEARR, len);
            daos.Write(arr, offset, len);
        }

        public byte[] ReadByteArray(Stream dis) {
            return ReadFully(dis, ReadVInt(dis));
        }

        //public void writeSolrDocument(SolrDocument doc)
        //{
        //    writeSolrDocument(doc, null);
        //}

        //public void writeSolrDocument(SolrDocument doc, Set<String> fields) {
        //  int count = 0;
        //  if (fields == null) {
        //    count = doc.getFieldNames().size();
        //  } else {
        //    for (Map.Entry<String, Object> entry : doc) {
        //      if (fields.contains(entry.getKey())) count++;
        //    }
        //  }
        //  WriteTag(SOLRDOC);
        //  WriteTag(ORDERED_MAP, count);
        //  for (Map.Entry<String, Object> entry : doc) {
        //    if (fields == null || fields.contains(entry.getKey())) {
        //      String name = entry.getKey();
        //      WriteExternString(name);
        //      Object val = entry.getValue();
        //      WriteVal(val);
        //    }
        //  }
        //}

        public SolrDocument ReadSolrDocument(Stream dis) {
            var nl = (IDictionary)ReadVal(dis);
            var doc = new Dictionary<string, object>();
            foreach (DictionaryEntry kv in nl)
                doc[(string) kv.Key] = kv.Value;
            return doc;
        }

        public class SolrDocumentList: List<SolrDocument> {
            public long NumFound { get; set; }
            public long Start { get; set; }
            public float? MaxScore { get; set; }
        }

        public SolrDocumentList ReadSolrDocumentList(Stream dis) {
            var solrDocs = new SolrDocumentList();
            var list = (IList)ReadVal(dis);
            solrDocs.NumFound = (long) list[0];
            solrDocs.Start = (long) list[1];
            solrDocs.MaxScore = (float?) list[2];

            var l = (ICollection)ReadVal(dis);
            solrDocs.AddRange(Cast<SolrDocument>(l));
            return solrDocs;
        }

        private static IEnumerable<T> Cast<T>(IEnumerable e) {
            foreach (var i in e)
                yield return (T) i;
        }

        //public void writeSolrDocumentList(SolrDocumentList docs) {
        //  WriteTag(SOLRDOCLST);
        //  List l = new ArrayList(3);
        //  l.add(docs.getNumFound());
        //  l.add(docs.getStart());
        //  l.add(docs.getMaxScore());
        //  WriteArray(l);
        //  WriteArray(docs);
        //}

        public IDictionary ReadMap(Stream dis) {
            int sz = ReadVInt(dis);
            var m = new Hashtable();
            for (int i = 0; i < sz; i++) {
                Object key = ReadVal(dis);
                Object val = ReadVal(dis);
                m[key] = val;
            }
            return m;
        }

        public void WriteIterator(IEnumerable iter) {
            WriteTag(ITERATOR);
            foreach (var i in iter)
                WriteVal(i);
            WriteVal(END_OBJ);
        }

        public IList ReadIterator(Stream fis) {
            var l = new ArrayList();
            while (true) {
                Object o = ReadVal(fis);
                if (o == END_OBJ) break;
                l.Add(o);
            }
            return l;
        }

        public void WriteArray(ICollection l) {
            WriteTag(ARR, l.Count);
            foreach (var i in l)
                WriteVal(i);
        }

        public void WriteArray(Object[] arr) {
            WriteTag(ARR, arr.Length);
            foreach (var i in arr)
                WriteVal(i);
        }

        public IList ReadArray(Stream dis) {
            int sz = ReadSize(dis);
            var l = new ArrayList(sz);
            for (int i = 0; i < sz; i++) {
                l.Add(ReadVal(dis));
            }
            return l;
        }

        /**
         * write the string as tag+length, with length being the number of UTF-16 characters, followed by the string encoded
         * in modified-UTF8
         */

        public void WriteStr(String s) {
            if (s == null) {
                WriteTag(NULL);
                return;
            }
            // Can't use string serialization or toUTF()... it's limited to 64K
            // plus it's bigger than it needs to be for small strings anyway
            int len = s.Length;
            WriteTag(STR, len);
            WriteChars(daos, s, 0, len);
        }


        private char[] charArr;

        public String ReadStr(Stream dis) {
            int sz = ReadSize(dis);
            if (charArr == null || charArr.Length < sz) {
                charArr = new char[sz];
            }
            ReadChars(dis, charArr, 0, sz);
            return new String(charArr, 0, sz);
        }

        public void WriteInt(int val) {
            if (val > 0) {
                var b = (byte) (SINT | (val & 0x0f));

                if (val >= 0x0f) {
                    b |= 0x10;
                    daos.WriteByte(b);
                    WriteVInt(usr(val, 4), daos);
                } else {
                    daos.WriteByte(b);
                }
            } else {
                daos.WriteByte(INT);
                //daos.WriteInt(val);
            }
        }

        public int ReadSmallInt(Stream dis) {
            int v = tagByte & 0x0F;
            if ((tagByte & 0x10) != 0)
                v = (ReadVInt(dis) << 4) | v;
            return v;
        }


        public void WriteLong(long val) {
            if (((ulong)val & 0xff00000000000000L) == 0) {
                var b = (byte) (SLONG | ((byte) val & 0x0f));
                if (val >= 0x0f) {
                    b |= 0x10;
                    daos.WriteByte(b);
                    WriteVLong(val >> 4, daos);
                } else {
                    daos.WriteByte(b);
                }
            } else {
                daos.WriteByte(LONG);
                var writer = new BinaryWriter(daos);
                writer.Write(val);
            }
        }

        public long ReadSmallLong(Stream dis) {
            long v = tagByte & 0x0F;
            if ((tagByte & 0x10) != 0)
                v = (ReadVLong(dis) << 4) | v;
            return v;
        }

        public bool WritePrimitive(Object val) {
            var writer = new BinaryWriter(daos);
            if (val == null) {
                daos.WriteByte(NULL);
                return true;
            }
            if (val is String) {
                WriteStr((String) val);
                return true;
            }
            if (val is int) {
                WriteInt((int) val);
                return true;
            }
            if (val is long) {
                WriteLong((long)val);
                return true;
            }
            if (val is float) {
                daos.WriteByte(FLOAT);
                writer.Write((float) val);
                return true;
            }
            if (val is DateTime) {
                daos.WriteByte(DATE);
                var dt = (DateTime) val;
                var ticks = (dt.AddYears(-epochYears)).Ticks;
                var ms = ticks/TimeSpan.TicksPerMillisecond;
                WriteLongToStream(daos, ms);
                return true;
            }
            if (val is bool) {
                daos.WriteByte((bool) val ? BOOL_TRUE : BOOL_FALSE);
                return true;
            }
            if (val is double) {
                daos.WriteByte(DOUBLE);
                writer.Write((double) val);
                return true;
            }
            if (val is byte) {
                daos.WriteByte(BYTE);
                daos.WriteByte((byte) val);
                return true;
            }
            if (val is short) {
                daos.WriteByte(SHORT);
                writer.Write((short) val);
                return true;
            }
            if (val is byte[]) {
                WriteByteArray((byte[]) val, 0, ((byte[]) val).Length);
                return true;
                //}else if (val is ByteBuffer) {
                //  ByteBuffer buf = (ByteBuffer) val;
                //  WriteByteArray(buf.array(),buf.position(),buf.limit() - buf.position());
                //  return true;
            }
            if (val == END_OBJ) {
                WriteTag(END);
                return true;
            }
            return false;
        }

        public void WriteMap(IDictionary val) {
            WriteTag(MAP, val.Count);
            foreach (DictionaryEntry entry in val) {
                if (entry.Key is string)
                    WriteExternString((string) entry.Key);
                else
                    WriteVal(entry.Key);
                WriteVal(entry.Value);
            }
        }


        public int ReadSize(Stream inputStream) {
            int sz = tagByte & 0x1f;
            if (sz == 0x1f) sz += ReadVInt(inputStream);
            return sz;
        }


        /// <summary>
        /// Special method for variable length int (copied from lucene). Usually used for writing the length of a
        /// collection/array/map In most of the cases the length can be represented in one byte (length < 127) so it saves 3
        /// bytes/object
        /// </summary>
        /// <param name="i"></param>
        /// <param name="outputStream"></param>
        public static void WriteVInt(int i, Stream outputStream) {
            while ((i & ~0x7F) != 0) {
                outputStream.WriteByte((byte) ((i & 0x7f) | 0x80));
                i = usr(i, 7);
            }
            outputStream.WriteByte((byte) i);
        }

        public static int ReadVInt(Stream inputStream) {
            int b = inputStream.ReadByte();
            int i = b & 0x7F;
            for (int shift = 7; (b & 0x80) != 0; shift += 7) {
                b = inputStream.ReadByte();
                i |= (b & 0x7F) << shift;
            }
            return i;
        }


        public static void WriteVLong(long i, Stream outputStream) {
            while ((i & ~0x7F) != 0) {
                outputStream.WriteByte((byte) ((i & 0x7f) | 0x80));
                i = usr(i, 7);
            }
            outputStream.WriteByte((byte) i);
        }

        public static long ReadVLong(Stream inputStream) {
            int b = inputStream.ReadByte();
            long i = b & 0x7F;
            for (int shift = 7; (b & 0x80) != 0; shift += 7) {
                b = inputStream.ReadByte();
                i |= (long) (b & 0x7F) << shift;
            }
            return i;
        }


        /// <summary>
        /// Writes a sequence of UTF-8 encoded characters from a string.
        /// </summary>
        /// <param name="os"></param>
        /// <param name="s"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        public static void WriteChars(Stream os, String s, int start, int length) {
            var bytes = Encoding.UTF8.GetBytes(s.Substring(start, length));
            os.Write(bytes, 0, bytes.Length);
            //int end = start + length;
            //for (int i = start; i < end; i++) {
            //    int code = s[i];
            //    if (code >= 0x01 && code <= 0x7F) {
            //        Write(os, code, BitConverter.GetBytes);
            //    } else if (((code >= 0x80) && (code <= 0x7FF)) || code == 0) {
            //        Write(os, 0xC0 | (code >> 6), BitConverter.GetBytes);
            //        Write(os, 0x80 | (code & 0x3F), BitConverter.GetBytes);
            //    } else {
            //        Write(os, 0xE0 | usr(code, 12), BitConverter.GetBytes);
            //        Write(os, 0x80 | ((code >> 6) & 0x3F), BitConverter.GetBytes);
            //        Write(os, 0x80 | (code & 0x3F), BitConverter.GetBytes);
            //    }
            //}
        }

        /// <summary>
        /// Reads UTF-8 encoded characters into an array.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="buffer">the array to read characters into</param>
        /// <param name="start">the offset in the array to start storing characters</param>
        /// <param name="length">the number of characters to read</param>
        public static void ReadChars(Stream inputStream, char[] buffer, int start, int length) {
            int end = start + length;
            for (int i = start; i < end; i++) {
                int b = inputStream.ReadByte();
                if ((b & 0x80) == 0)
                    buffer[i] = (char) b;
                else if ((b & 0xE0) != 0xE0) {
                    buffer[i] = (char) (((b & 0x1F) << 6)
                                        | (inputStream.ReadByte() & 0x3F));
                } else
                    buffer[i] = (char) (((b & 0x0F) << 12)
                                        | ((inputStream.ReadByte() & 0x3F) << 6)
                                        | (inputStream.ReadByte() & 0x3F));
            }
        }

        private int stringsCount = 0;
        private IDictionary<String, int> stringsMap;
        private List<String> stringsList;

        public void WriteExternString(String s) {
            if (s == null) {
                WriteTag(NULL);
                return;
            }
            int idx = stringsMap == null ? 0 : stringsMap[s];
            WriteTag(EXTERN_STRING, idx);
            if (idx == 0) {
                WriteStr(s);
                if (stringsMap == null)
                    stringsMap = new Dictionary<String, int>();
                stringsMap[s] = ++stringsCount;
            }
        }

        public String ReadExternString(Stream fis) {
            int idx = ReadSize(fis);
            if (idx != 0) {
// idx != 0 is the index of the extern string
                return stringsList[idx - 1];
            } // idx == 0 means it has a string value
            var s = (String) ReadVal(fis);
            if (stringsList == null)
                stringsList = new List<String>();
            stringsList.Add(s);
            return s;
        }

        public interface IObjectResolver {
            Object Resolve(Object o, JavaBinCodec codec);
        }

        /// <summary>
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// From http://www.yoda.arachsys.com/csharp/readbinary.html
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <param name="initialLength">The initial buffer length</param>
        private static byte[] ReadFully(Stream stream, int initialLength) {
            // If we've been passed an unhelpful initial length, just
            // use 32K.
            if (initialLength < 1) {
                initialLength = 32768;
            }

            var buffer = new byte[initialLength];
            int read = 0;

            int chunk;
            while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0) {
                read += chunk;

                // If we've reached the end of our buffer, check to see if there's
                // any more information
                if (read == buffer.Length) {
                    int nextByte = stream.ReadByte();

                    // End of stream? If so, we're done
                    if (nextByte == -1) {
                        return buffer;
                    }

                    // Nope. Resize the buffer, put in the byte we've just
                    // read, and continue
                    var newBuffer = new byte[buffer.Length*2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte) nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }
            // Buffer is now too big. Shrink it.
            var ret = new byte[read];
            Array.Copy(buffer, ret, read);
            return ret;
        }

        /// <summary>
        /// Unsigned right shift
        /// From http://www.eqqon.com/index.php/GitSharp/Non_Trivial_Java_To_CSharp_Conversions
        /// </summary>
        /// <param name="a"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        private static int usr(int a, int shift) {
            return (int) (((uint) a) >> shift);
        }

        private static long usr(long a, int shift) {
            return (long) (((ulong) a) >> shift);
        }

        private delegate R Func<T, R>(T t);

        private static void Write<T>(Stream s, T c, Func<T, byte[]> converter) {
            var buffer = converter(c);
            s.Write(buffer, 0, buffer.Length);
        }
    }
}