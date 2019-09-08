using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;

namespace Unity.Networking.Transport
{
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
    public unsafe struct DataStreamWriter
    {
        public bool CheckValid()
        {
            string info = string.Format("uniqueNumber({0}), versionNode({1}), version({2})", m_UniqueNumber, m_Safety.versionNode.ToString(), m_Safety.version);
            UnityEngine.Debug.Log(info);
            AtomicSafetyHandle.GetWriterName(m_Safety);
            return true;
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
    }
}

