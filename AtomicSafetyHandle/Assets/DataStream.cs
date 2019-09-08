using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;

namespace Unity.Networking.Transport
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct UIntFloat
    {
        [FieldOffset(0)] public float floatValue;

        [FieldOffset(0)] public uint intValue;

        [FieldOffset(0)] public double doubleValue;

        [FieldOffset(0)] public ulong longValue;
    }

    /// <summary>
    /// Data streams can be used to serialize data over the network. The
    /// <c>DataStreamWriter</c> and <c>DataStreamReader</c> classes work together
    /// to serialize data for sending and then to deserialize when receiving.
    /// </summary>
    /// <remarks>
    /// The reader can be used to deserialize the data from a writer, writing data
    /// to a writer and reading it back can be done like this:
    /// <code>
    /// using (var dataWriter = new DataStreamWriter(16, Allocator.Persistent))
    /// {
    ///     dataWriter.Write(42);
    ///     dataWriter.Write(1234);
    ///     // Length is the actual amount of data inside the writer,
    ///     // Capacity is the total amount.
    ///     var dataReader = new DataStreamReader(dataWriter, 0, dataWriter.Length);
    ///     var context = default(DataStreamReader.Context);
    ///     var myFirstInt = dataReader.ReadInt(ref context);
    ///     var mySecondInt = dataReader.ReadInt(ref context);
    /// }
    /// </code>
    ///
    /// The writer needs to be Disposed (here done by wrapping usage in using statement)
    /// because it uses native memory which needs to be freed.
    ///
    /// There are a number of functions for various data types. Each write call
    /// returns a <c>Deferred*</c> variant for that particular type and this can be used
    /// as a marker to overwrite the data later on, this is particularly useful when
    /// the size of the data is written at the start and you want to write it at
    /// the end when you know the value.
    ///
    /// <code>
    /// using (var data = new DataStreamWriter(16, Allocator.Persistent))
    /// {
    ///     // My header data
    ///     var headerSizeMark = data.Write((ushort)0);
    ///     var payloadSizeMark = data.Write((ushort)0);
    ///     data.Write(42);
    ///     data.Write(1234);
    ///     var headerSize = data.Length;
    ///     // Update header size to correct value
    ///     headerSizeMark.Update((ushort)headerSize);
    ///     // My payload data
    ///     byte[] someBytes = Encoding.ASCII.GetBytes("some string");
    ///     data.Write(someBytes, someBytes.Length);
    ///     // Update payload size to correct value
    ///     payloadSizeMark.Update((ushort)(data.Length - headerSize));
    /// }
    /// </code>
    ///
    /// It's possible to get a more direct access to the buffer inside the
    /// reader/writer, in an unsafe way. See <see cref="DataStreamUnsafeUtility"/>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    [NativeContainer]
    public unsafe struct DataStreamWriter : IDisposable
    {
        public bool CheckValid()
        {
            string info = string.Format("uniqueNumber({0}), versionNode({1}), version({2})", m_UniqueNumber, m_Safety.versionNode.ToString(), m_Safety.version);
            UnityEngine.Debug.Log(info);
            AtomicSafetyHandle.GetWriterName(m_Safety);
            return true;
        }

        public struct DeferredByte
        {
            public void Update(byte value)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (m_writer.m_Data->bitIndex != 0)
                    throw new InvalidOperationException("Cannot update a deferred writer without flushing packed writes");
#endif
                int oldOffset = m_writer.m_Data->length;
                m_writer.m_Data->length = m_offset;
                m_writer.Write(value);
                m_writer.m_Data->length = oldOffset;
            }

            internal DataStreamWriter m_writer;
            internal int m_offset;
        }
        public struct DeferredShort
        {
            public void Update(short value)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (m_writer.m_Data->bitIndex != 0)
                    throw new InvalidOperationException("Cannot update a deferred writer without flushing packed writes");
#endif
                int oldOffset = m_writer.m_Data->length;
                m_writer.m_Data->length = m_offset;
                m_writer.Write(value);
                m_writer.m_Data->length = oldOffset;
            }

            internal DataStreamWriter m_writer;
            internal int m_offset;
        }
        public struct DeferredUShort
        {
            public void Update(ushort value)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (m_writer.m_Data->bitIndex != 0)
                    throw new InvalidOperationException("Cannot update a deferred writer without flushing packed writes");
#endif
                int oldOffset = m_writer.m_Data->length;
                m_writer.m_Data->length = m_offset;
                m_writer.Write(value);
                m_writer.m_Data->length = oldOffset;
            }

            internal DataStreamWriter m_writer;
            internal int m_offset;
        }
        public struct DeferredInt
        {
            public void Update(int value)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (m_writer.m_Data->bitIndex != 0)
                    throw new InvalidOperationException("Cannot update a deferred writer without flushing packed writes");
#endif
                int oldOffset = m_writer.m_Data->length;
                m_writer.m_Data->length = m_offset;
                m_writer.Write(value);
                m_writer.m_Data->length = oldOffset;
            }

            internal DataStreamWriter m_writer;
            internal int m_offset;
        }
        public struct DeferredUInt
        {
            public void Update(uint value)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (m_writer.m_Data->bitIndex != 0)
                    throw new InvalidOperationException("Cannot update a deferred writer without flushing packed writes");
#endif
                int oldOffset = m_writer.m_Data->length;
                m_writer.m_Data->length = m_offset;
                m_writer.Write(value);
                m_writer.m_Data->length = oldOffset;
            }

            internal DataStreamWriter m_writer;
            internal int m_offset;
        }
        public struct DeferredFloat
        {
            public void Update(float value)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (m_writer.m_Data->bitIndex != 0)
                    throw new InvalidOperationException("Cannot update a deferred writer without flushing packed writes");
#endif
                int oldOffset = m_writer.m_Data->length;
                m_writer.m_Data->length = m_offset;
                m_writer.Write(value);
                m_writer.m_Data->length = oldOffset;
            }

            internal DataStreamWriter m_writer;
            internal int m_offset;
        }
        public struct DeferredShortNetworkByteOrder
        {
            public void Update(short value)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (m_writer.m_Data->bitIndex != 0)
                    throw new InvalidOperationException("Cannot update a deferred writer without flushing packed writes");
#endif
                int oldOffset = m_writer.m_Data->length;
                m_writer.m_Data->length = m_offset;
                m_writer.WriteNetworkByteOrder(value);
                m_writer.m_Data->length = oldOffset;
            }

            internal DataStreamWriter m_writer;
            internal int m_offset;
        }
        public struct DeferredUShortNetworkByteOrder
        {
            public void Update(ushort value)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (m_writer.m_Data->bitIndex != 0)
                    throw new InvalidOperationException("Cannot update a deferred writer without flushing packed writes");
#endif
                int oldOffset = m_writer.m_Data->length;
                m_writer.m_Data->length = m_offset;
                m_writer.WriteNetworkByteOrder(value);
                m_writer.m_Data->length = oldOffset;
            }

            internal DataStreamWriter m_writer;
            internal int m_offset;
        }
        public struct DeferredIntNetworkByteOrder
        {
            public void Update(int value)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (m_writer.m_Data->bitIndex != 0)
                    throw new InvalidOperationException("Cannot update a deferred writer without flushing packed writes");
#endif
                int oldOffset = m_writer.m_Data->length;
                m_writer.m_Data->length = m_offset;
                m_writer.WriteNetworkByteOrder(value);
                m_writer.m_Data->length = oldOffset;
            }

            internal DataStreamWriter m_writer;
            internal int m_offset;
        }
        public struct DeferredUIntNetworkByteOrder
        {
            public void Update(uint value)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (m_writer.m_Data->bitIndex != 0)
                    throw new InvalidOperationException("Cannot update a deferred writer without flushing packed writes");
#endif
                int oldOffset = m_writer.m_Data->length;
                m_writer.m_Data->length = m_offset;
                m_writer.WriteNetworkByteOrder(value);
                m_writer.m_Data->length = oldOffset;
            }

            internal DataStreamWriter m_writer;
            internal int m_offset;
        }

        internal struct StreamData
        {
            public byte* buffer;
            public int length;
            public int capacity;
            public ulong bitBuffer;
            public int bitIndex;
        }

        [NativeDisableUnsafePtrRestriction] internal StreamData* m_Data;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        public int m_UniqueNumber;
        internal AtomicSafetyHandle m_Safety;
        [NativeSetClassTypeToNullOnSchedule] internal DisposeSentinel m_DisposeSentinel;
#endif

        Allocator m_Allocator;

        public DataStreamWriter(int capacity, Allocator allocator)
        {
            m_UniqueNumber = UnityEngine.Random.Range(1, 10000);
            m_Allocator = allocator;
            m_Data = (StreamData*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<StreamData>(), UnsafeUtility.AlignOf<StreamData>(), m_Allocator);
            m_Data->capacity = capacity;
            m_Data->length = 0;
            m_Data->buffer = (byte*) UnsafeUtility.Malloc(capacity, UnsafeUtility.AlignOf<byte>(), m_Allocator);
            m_Data->bitBuffer = 0;
            m_Data->bitIndex = 0;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 1, m_Allocator);
#endif
            uint test = 1;
            unsafe
            {
                byte* test_b = (byte*) &test;
                IsLittleEndian = test_b[0] == 1;
            }
        }

        private bool IsLittleEndian;

        private static short ByteSwap(short val)
        {
            return (short)(((val & 0xff) << 8) | ((val >> 8)&0xff));
        }
        private static int ByteSwap(int val)
        {
            return (int)(((val & 0xff) << 24) |((val&0xff00)<<8) | ((val>>8)&0xff00) | ((val >> 24)&0xff));
        }

        /// <summary>
        /// True if there is a valid data buffer present. This would be false
        /// if the writer was created with no arguments.
        /// </summary>
        public bool IsCreated
        {
            get { return m_Data != null; }
        }

        /// <summary>
        /// The total size of the data buffer, see <see cref="Length"/> for
        /// the size of space used in the buffer. Capacity can be
        /// changed after the writer has been created.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the given
        /// capacity is smaller than the current buffer usage.</exception>
        public int Capacity
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
                return m_Data->capacity;
            }
            set
            {
                if (m_Data->capacity == value)
                    return;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
                if (m_Data->length + ((m_Data->bitIndex + 7) >> 3) > value)
                    throw new InvalidOperationException("Cannot shrink a data stream to be shorter than the current data in it");
#endif
                byte* newbuf = (byte*) UnsafeUtility.Malloc(value, UnsafeUtility.AlignOf<byte>(), m_Allocator);
                UnsafeUtility.MemCpy(newbuf, m_Data->buffer, m_Data->length);
                UnsafeUtility.Free(m_Data->buffer, m_Allocator);
                m_Data->buffer = newbuf;
                m_Data->capacity = value;
            }
        }

        /// <summary>
        /// The size of the buffer used. See <see cref="Capacity"/> for the total size.
        /// </summary>
        public int Length
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
                return m_Data->length + ((m_Data->bitIndex + 7) >> 3);
            }
        }

        /// <summary>
        /// The writer uses unmanaged memory for its data buffer. Dispose
        /// needs to be called to free this resource.
        /// </summary>
        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#endif
            UnsafeUtility.Free(m_Data->buffer, m_Allocator);
            UnsafeUtility.Free(m_Data, m_Allocator);
            m_Data = (StreamData*) 0;
        }

        public void Flush()
        {
            while (m_Data->bitIndex > 0)
            {
                m_Data->buffer[m_Data->length++] = (byte)m_Data->bitBuffer;
                m_Data->bitIndex -= 8;
                m_Data->bitBuffer >>= 8;
            }

            m_Data->bitIndex = 0;
        }

        /// <summary>
        /// Create a NativeSlice with access to the raw data in the writer, the data size
        /// (start to length) must not exceed the total size of the array or
        /// an exception will be thrown.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        public NativeSlice<byte> GetNativeSlice(int start, int length)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            ValidateSizeParameters(start, length, length);
#endif

            var slice = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<byte>(m_Data->buffer + start, 1,
                length);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeSliceUnsafeUtility.SetAtomicSafetyHandle(ref slice, m_Safety);
#endif
            return slice;
        }

        /// <summary>
        /// Copy data from the writer to the given NativeArray, the data size
        /// (start to length) must not exceed the total size of the array or
        /// an exception will be thrown.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="dest"></param>
        public void CopyTo(int start, int length, NativeArray<byte> dest)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            ValidateSizeParameters(start, length, dest.Length);
#endif

            void* dstPtr = dest.GetUnsafePtr();
            UnsafeUtility.MemCpy(dstPtr, m_Data->buffer + start, length);
        }

        /// <summary>
        /// Copy data from the writer to the given managed byte array, the
        /// data size (start to length) must not exceed the total size of the
        /// byte array or an exception will be thrown.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="dest"></param>
        public void CopyTo(int start, int length, ref byte[] dest)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            ValidateSizeParameters(start, length, dest.Length);
#endif

            fixed (byte* ptr = dest)
            {
                UnsafeUtility.MemCpy(ptr, m_Data->buffer + start, length);
            }
        }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        void ValidateSizeParameters(int start, int length, int dstLength)
        {
            if (start < 0 || length + start > m_Data->length)
                throw new ArgumentOutOfRangeException("start+length",
                    "The sum of start and length can not be larger than the data buffer Length");

            if (length > dstLength)
                throw new ArgumentOutOfRangeException("length", "Length must be <= than the length of the destination");

            if (m_Data->bitIndex > 0)
                throw new InvalidOperationException("Cannot read from a DataStreamWriter when there are pending packed writes, call Flush first");

            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
        }
#endif

        public void WriteBytes(byte* data, int bytes)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
            if (m_Data->length + ((m_Data->bitIndex + 7) >> 3) + bytes > m_Data->capacity)
                throw new System.ArgumentOutOfRangeException();
#endif
            Flush();
            UnsafeUtility.MemCpy(m_Data->buffer + m_Data->length, data, bytes);
            m_Data->length += bytes;
        }

        public DeferredByte Write(byte value)
        {
            var ret = new DeferredByte {m_writer = this, m_offset = m_Data->length + ((m_Data->bitIndex + 7) >> 3)};
            WriteBytes((byte*) &value, sizeof(byte));
            return ret;
        }

        /// <summary>
        /// Copy byte array into the writers data buffer, up to the
        /// given length or the complete size if no length (or -1) is given.
        /// </summary>
        /// <param name="value">Source byte array</param>
        /// <param name="length">Length to copy, omit this to copy all the byte array</param>
        public void Write(byte[] value, int length = -1)
        {
            if (length < 0)
                length = value.Length;

            fixed (byte* p = value)
            {
                WriteBytes(p, length);
            }
        }

        public void WriteUnicodeString(string str)
        {
            byte[] strBytes = System.Text.Encoding.Unicode.GetBytes(str);
            int byteLen = strBytes.Length;

            Write(byteLen);
            fixed (byte* p = strBytes)
            {
                WriteBytes(p, byteLen);
            }

        }

        public DeferredShort Write(short value)
        {
            var ret = new DeferredShort {m_writer = this, m_offset = m_Data->length + ((m_Data->bitIndex + 7) >> 3)};
            WriteBytes((byte*) &value, sizeof(short));
            return ret;
        }

        public DeferredUShort Write(ushort value)
        {
            var ret = new DeferredUShort {m_writer = this, m_offset = m_Data->length + ((m_Data->bitIndex + 7) >> 3)};
            WriteBytes((byte*) &value, sizeof(ushort));
            return ret;
        }

        public DeferredInt Write(int value)
        {
            var ret = new DeferredInt {m_writer = this, m_offset = m_Data->length + ((m_Data->bitIndex + 7) >> 3)};
            WriteBytes((byte*) &value, sizeof(int));
            return ret;
        }

        public DeferredUInt Write(uint value)
        {
            var ret = new DeferredUInt {m_writer = this, m_offset = m_Data->length + ((m_Data->bitIndex + 7) >> 3)};
            WriteBytes((byte*) &value, sizeof(uint));
            return ret;
        }

        public DeferredShortNetworkByteOrder WriteNetworkByteOrder(short value)
        {
            var ret = new DeferredShortNetworkByteOrder {m_writer = this, m_offset = m_Data->length + ((m_Data->bitIndex + 7) >> 3)};
            short netValue = IsLittleEndian ? ByteSwap(value) : value;
            WriteBytes((byte*) &netValue, sizeof(short));
            return ret;
        }

        public DeferredUShortNetworkByteOrder WriteNetworkByteOrder(ushort value)
        {
            var ret = new DeferredUShortNetworkByteOrder {m_writer = this, m_offset = m_Data->length + ((m_Data->bitIndex + 7) >> 3)};
            WriteNetworkByteOrder((short) value);
            return ret;
        }

        public DeferredIntNetworkByteOrder WriteNetworkByteOrder(int value)
        {
            var ret = new DeferredIntNetworkByteOrder {m_writer = this, m_offset = m_Data->length + ((m_Data->bitIndex + 7) >> 3)};
            int netValue = IsLittleEndian ? ByteSwap(value) : value;
            WriteBytes((byte*) &netValue, sizeof(int));
            return ret;
        }

        public DeferredUIntNetworkByteOrder WriteNetworkByteOrder(uint value)
        {
            var ret = new DeferredUIntNetworkByteOrder {m_writer = this, m_offset = m_Data->length + ((m_Data->bitIndex + 7) >> 3)};
            WriteNetworkByteOrder((int)value);
            return ret;
        }

        public DeferredFloat Write(float value)
        {
            var ret = new DeferredFloat {m_writer = this, m_offset = m_Data->length + ((m_Data->bitIndex + 7) >> 3)};
            UIntFloat uf = new UIntFloat();
            uf.floatValue = value;
            Write((int) uf.intValue);
            return ret;
        }

        private void FlushBits()
        {
            while (m_Data->bitIndex >= 8)
            {
                m_Data->buffer[m_Data->length++] = (byte)m_Data->bitBuffer;
                m_Data->bitIndex -= 8;
                m_Data->bitBuffer >>= 8;
            }
        }
        void WriteRawBitsInternal(uint value, int numbits)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (numbits < 0 || numbits > 32)
                throw new ArgumentOutOfRangeException("Invalid number of bits");
            if (value >= (1UL << numbits))
                throw new ArgumentOutOfRangeException("Value does not fit in the specified number of bits");
#endif

            m_Data->bitBuffer |= ((ulong)value << m_Data->bitIndex);
            m_Data->bitIndex += numbits;
        }

        public void WritePackedUInt(uint value/*, NetworkCompressionModel model*/)
        {
            int bucket = 1;// model.CalculateBucket(value);
            uint offset = 1;//model.bucketOffsets[bucket];
            int bits = 1;//model.bucketSizes[bucket];
            ushort encodeEntry = 1;//model.encodeTable[bucket];
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
            if (m_Data->length + ((m_Data->bitIndex + encodeEntry&0xff + bits + 7) >> 3) > m_Data->capacity)
                throw new System.ArgumentOutOfRangeException();
#endif
            WriteRawBitsInternal((uint)(encodeEntry >> 8), encodeEntry & 0xFF);
            WriteRawBitsInternal(value - offset, bits);
            FlushBits();
        }
        public void WritePackedInt(int value/*, NetworkCompressionModel model*/)
        {
            uint interleaved = (uint)((value >> 31) ^ (value << 1));      // interleave negative values between positive values: 0, -1, 1, -2, 2
            WritePackedUInt(interleaved/*, model*/);
        }
        public void WritePackedUIntDelta(uint value, uint baseline/*, NetworkCompressionModel model*/)
        {
            int diff = (int)(baseline - value);
            WritePackedInt(diff/*, model*/);
        }
        public void WritePackedIntDelta(int value, int baseline/*, NetworkCompressionModel model*/)
        {
            int diff = (int)(baseline - value);
            WritePackedInt(diff/*, model*/);
        }
        /// <summary>
        /// Moves the write position to the start of the data buffer used.
        /// </summary>
        public void Clear()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
            m_Data->length = 0;
            m_Data->bitIndex = 0;
            m_Data->bitBuffer = 0;
        }
    }
}

