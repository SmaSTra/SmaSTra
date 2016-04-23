using Common.ExtensionMethods;
using System.Runtime.InteropServices;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;
using System;
namespace Common.Collections
{
	public partial class PointerArray<T> : PointerArray, IList<T> where T : struct
	{
		public static PointerArray<T> Create(sbyte[] data, int startOffset, int count)
		{
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			if (startOffset < 0) {
				throw new ArgumentException("Start offset must not be < 0.", "startOffset");
			}
			if (count < 0) {
				throw new ArgumentException("Count must not be < 0.", "count");
			}
			int sizeofTData = sizeof(sbyte);
			if (startOffset + count * SIZEOF_T > data.Length * sizeofTData) {
				throw new IndexOutOfRangeException("The sub-array must be within the confines of the given data array");
			}
			PointerArray<T> result = PointerArray<T>.newInstanceFunction();
			result.dataReference = data;
			unsafe {
				fixed (sbyte* ptr = data) {
					result.pointer = (byte*)ptr + startOffset;
				}
			}
			result.Count = data.Length * sizeofTData / SIZEOF_T;
			return result;
		}
		public static PointerArray<T> Create(sbyte[] data)
		{
			int sizeofTData = sizeof(sbyte);
			int count = data.Length * sizeofTData / SIZEOF_T;
			return Create(data, 0, count);
		}
		public static PointerArray<T> Create(short[] data, int startOffset, int count)
		{
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			if (startOffset < 0) {
				throw new ArgumentException("Start offset must not be < 0.", "startOffset");
			}
			if (count < 0) {
				throw new ArgumentException("Count must not be < 0.", "count");
			}
			int sizeofTData = sizeof(short);
			if (startOffset + count * SIZEOF_T > data.Length * sizeofTData) {
				throw new IndexOutOfRangeException("The sub-array must be within the confines of the given data array");
			}
			PointerArray<T> result = PointerArray<T>.newInstanceFunction();
			result.dataReference = data;
			unsafe {
				fixed (short* ptr = data) {
					result.pointer = (byte*)ptr + startOffset;
				}
			}
			result.Count = data.Length * sizeofTData / SIZEOF_T;
			return result;
		}
		public static PointerArray<T> Create(short[] data)
		{
			int sizeofTData = sizeof(short);
			int count = data.Length * sizeofTData / SIZEOF_T;
			return Create(data, 0, count);
		}
		public static PointerArray<T> Create(ushort[] data, int startOffset, int count)
		{
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			if (startOffset < 0) {
				throw new ArgumentException("Start offset must not be < 0.", "startOffset");
			}
			if (count < 0) {
				throw new ArgumentException("Count must not be < 0.", "count");
			}
			int sizeofTData = sizeof(ushort);
			if (startOffset + count * SIZEOF_T > data.Length * sizeofTData) {
				throw new IndexOutOfRangeException("The sub-array must be within the confines of the given data array");
			}
			PointerArray<T> result = PointerArray<T>.newInstanceFunction();
			result.dataReference = data;
			unsafe {
				fixed (ushort* ptr = data) {
					result.pointer = (byte*)ptr + startOffset;
				}
			}
			result.Count = data.Length * sizeofTData / SIZEOF_T;
			return result;
		}
		public static PointerArray<T> Create(ushort[] data)
		{
			int sizeofTData = sizeof(ushort);
			int count = data.Length * sizeofTData / SIZEOF_T;
			return Create(data, 0, count);
		}
		public static PointerArray<T> Create(int[] data, int startOffset, int count)
		{
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			if (startOffset < 0) {
				throw new ArgumentException("Start offset must not be < 0.", "startOffset");
			}
			if (count < 0) {
				throw new ArgumentException("Count must not be < 0.", "count");
			}
			int sizeofTData = sizeof(int);
			if (startOffset + count * SIZEOF_T > data.Length * sizeofTData) {
				throw new IndexOutOfRangeException("The sub-array must be within the confines of the given data array");
			}
			PointerArray<T> result = PointerArray<T>.newInstanceFunction();
			result.dataReference = data;
			unsafe {
				fixed (int* ptr = data) {
					result.pointer = (byte*)ptr + startOffset;
				}
			}
			result.Count = data.Length * sizeofTData / SIZEOF_T;
			return result;
		}
		public static PointerArray<T> Create(int[] data)
		{
			int sizeofTData = sizeof(int);
			int count = data.Length * sizeofTData / SIZEOF_T;
			return Create(data, 0, count);
		}
		public static PointerArray<T> Create(uint[] data, int startOffset, int count)
		{
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			if (startOffset < 0) {
				throw new ArgumentException("Start offset must not be < 0.", "startOffset");
			}
			if (count < 0) {
				throw new ArgumentException("Count must not be < 0.", "count");
			}
			int sizeofTData = sizeof(uint);
			if (startOffset + count * SIZEOF_T > data.Length * sizeofTData) {
				throw new IndexOutOfRangeException("The sub-array must be within the confines of the given data array");
			}
			PointerArray<T> result = PointerArray<T>.newInstanceFunction();
			result.dataReference = data;
			unsafe {
				fixed (uint* ptr = data) {
					result.pointer = (byte*)ptr + startOffset;
				}
			}
			result.Count = data.Length * sizeofTData / SIZEOF_T;
			return result;
		}
		public static PointerArray<T> Create(uint[] data)
		{
			int sizeofTData = sizeof(uint);
			int count = data.Length * sizeofTData / SIZEOF_T;
			return Create(data, 0, count);
		}
		public static PointerArray<T> Create(long[] data, int startOffset, int count)
		{
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			if (startOffset < 0) {
				throw new ArgumentException("Start offset must not be < 0.", "startOffset");
			}
			if (count < 0) {
				throw new ArgumentException("Count must not be < 0.", "count");
			}
			int sizeofTData = sizeof(long);
			if (startOffset + count * SIZEOF_T > data.Length * sizeofTData) {
				throw new IndexOutOfRangeException("The sub-array must be within the confines of the given data array");
			}
			PointerArray<T> result = PointerArray<T>.newInstanceFunction();
			result.dataReference = data;
			unsafe {
				fixed (long* ptr = data) {
					result.pointer = (byte*)ptr + startOffset;
				}
			}
			result.Count = data.Length * sizeofTData / SIZEOF_T;
			return result;
		}
		public static PointerArray<T> Create(long[] data)
		{
			int sizeofTData = sizeof(long);
			int count = data.Length * sizeofTData / SIZEOF_T;
			return Create(data, 0, count);
		}
		public static PointerArray<T> Create(ulong[] data, int startOffset, int count)
		{
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			if (startOffset < 0) {
				throw new ArgumentException("Start offset must not be < 0.", "startOffset");
			}
			if (count < 0) {
				throw new ArgumentException("Count must not be < 0.", "count");
			}
			int sizeofTData = sizeof(ulong);
			if (startOffset + count * SIZEOF_T > data.Length * sizeofTData) {
				throw new IndexOutOfRangeException("The sub-array must be within the confines of the given data array");
			}
			PointerArray<T> result = PointerArray<T>.newInstanceFunction();
			result.dataReference = data;
			unsafe {
				fixed (ulong* ptr = data) {
					result.pointer = (byte*)ptr + startOffset;
				}
			}
			result.Count = data.Length * sizeofTData / SIZEOF_T;
			return result;
		}
		public static PointerArray<T> Create(ulong[] data)
		{
			int sizeofTData = sizeof(ulong);
			int count = data.Length * sizeofTData / SIZEOF_T;
			return Create(data, 0, count);
		}
		public static PointerArray<T> Create(char[] data, int startOffset, int count)
		{
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			if (startOffset < 0) {
				throw new ArgumentException("Start offset must not be < 0.", "startOffset");
			}
			if (count < 0) {
				throw new ArgumentException("Count must not be < 0.", "count");
			}
			int sizeofTData = sizeof(char);
			if (startOffset + count * SIZEOF_T > data.Length * sizeofTData) {
				throw new IndexOutOfRangeException("The sub-array must be within the confines of the given data array");
			}
			PointerArray<T> result = PointerArray<T>.newInstanceFunction();
			result.dataReference = data;
			unsafe {
				fixed (char* ptr = data) {
					result.pointer = (byte*)ptr + startOffset;
				}
			}
			result.Count = data.Length * sizeofTData / SIZEOF_T;
			return result;
		}
		public static PointerArray<T> Create(char[] data)
		{
			int sizeofTData = sizeof(char);
			int count = data.Length * sizeofTData / SIZEOF_T;
			return Create(data, 0, count);
		}
		public static PointerArray<T> Create(float[] data, int startOffset, int count)
		{
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			if (startOffset < 0) {
				throw new ArgumentException("Start offset must not be < 0.", "startOffset");
			}
			if (count < 0) {
				throw new ArgumentException("Count must not be < 0.", "count");
			}
			int sizeofTData = sizeof(float);
			if (startOffset + count * SIZEOF_T > data.Length * sizeofTData) {
				throw new IndexOutOfRangeException("The sub-array must be within the confines of the given data array");
			}
			PointerArray<T> result = PointerArray<T>.newInstanceFunction();
			result.dataReference = data;
			unsafe {
				fixed (float* ptr = data) {
					result.pointer = (byte*)ptr + startOffset;
				}
			}
			result.Count = data.Length * sizeofTData / SIZEOF_T;
			return result;
		}
		public static PointerArray<T> Create(float[] data)
		{
			int sizeofTData = sizeof(float);
			int count = data.Length * sizeofTData / SIZEOF_T;
			return Create(data, 0, count);
		}
		public static PointerArray<T> Create(double[] data, int startOffset, int count)
		{
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			if (startOffset < 0) {
				throw new ArgumentException("Start offset must not be < 0.", "startOffset");
			}
			if (count < 0) {
				throw new ArgumentException("Count must not be < 0.", "count");
			}
			int sizeofTData = sizeof(double);
			if (startOffset + count * SIZEOF_T > data.Length * sizeofTData) {
				throw new IndexOutOfRangeException("The sub-array must be within the confines of the given data array");
			}
			PointerArray<T> result = PointerArray<T>.newInstanceFunction();
			result.dataReference = data;
			unsafe {
				fixed (double* ptr = data) {
					result.pointer = (byte*)ptr + startOffset;
				}
			}
			result.Count = data.Length * sizeofTData / SIZEOF_T;
			return result;
		}
		public static PointerArray<T> Create(double[] data)
		{
			int sizeofTData = sizeof(double);
			int count = data.Length * sizeofTData / SIZEOF_T;
			return Create(data, 0, count);
		}
		public static PointerArray<T> Create(decimal[] data, int startOffset, int count)
		{
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			if (startOffset < 0) {
				throw new ArgumentException("Start offset must not be < 0.", "startOffset");
			}
			if (count < 0) {
				throw new ArgumentException("Count must not be < 0.", "count");
			}
			int sizeofTData = sizeof(decimal);
			if (startOffset + count * SIZEOF_T > data.Length * sizeofTData) {
				throw new IndexOutOfRangeException("The sub-array must be within the confines of the given data array");
			}
			PointerArray<T> result = PointerArray<T>.newInstanceFunction();
			result.dataReference = data;
			unsafe {
				fixed (decimal* ptr = data) {
					result.pointer = (byte*)ptr + startOffset;
				}
			}
			result.Count = data.Length * sizeofTData / SIZEOF_T;
			return result;
		}
		public static PointerArray<T> Create(decimal[] data)
		{
			int sizeofTData = sizeof(decimal);
			int count = data.Length * sizeofTData / SIZEOF_T;
			return Create(data, 0, count);
		}
		public static PointerArray<T> Create(bool[] data, int startOffset, int count)
		{
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			if (startOffset < 0) {
				throw new ArgumentException("Start offset must not be < 0.", "startOffset");
			}
			if (count < 0) {
				throw new ArgumentException("Count must not be < 0.", "count");
			}
			int sizeofTData = sizeof(bool);
			if (startOffset + count * SIZEOF_T > data.Length * sizeofTData) {
				throw new IndexOutOfRangeException("The sub-array must be within the confines of the given data array");
			}
			PointerArray<T> result = PointerArray<T>.newInstanceFunction();
			result.dataReference = data;
			unsafe {
				fixed (bool* ptr = data) {
					result.pointer = (byte*)ptr + startOffset;
				}
			}
			result.Count = data.Length * sizeofTData / SIZEOF_T;
			return result;
		}
		public static PointerArray<T> Create(bool[] data)
		{
			int sizeofTData = sizeof(bool);
			int count = data.Length * sizeofTData / SIZEOF_T;
			return Create(data, 0, count);
		}
	}
	public class PointerArrayOfSByte : PointerArray<sbyte>
	{
		public override sbyte this[int index]
		{
			get
			{
				unsafe {
					return ((sbyte*)this.pointer)[index];
				}
			}
			set
			{
				unsafe {
					((sbyte*)this.pointer)[index] = value;
				}
			}
		}
	}
	public class PointerArrayOfInt16 : PointerArray<short>
	{
		public override short this[int index]
		{
			get
			{
				unsafe {
					return ((short*)this.pointer)[index];
				}
			}
			set
			{
				unsafe {
					((short*)this.pointer)[index] = value;
				}
			}
		}
	}
	public class PointerArrayOfUInt16 : PointerArray<ushort>
	{
		public override ushort this[int index]
		{
			get
			{
				unsafe {
					return ((ushort*)this.pointer)[index];
				}
			}
			set
			{
				unsafe {
					((ushort*)this.pointer)[index] = value;
				}
			}
		}
	}
	public class PointerArrayOfInt32 : PointerArray<int>
	{
		public override int this[int index]
		{
			get
			{
				unsafe {
					return ((int*)this.pointer)[index];
				}
			}
			set
			{
				unsafe {
					((int*)this.pointer)[index] = value;
				}
			}
		}
	}
	public class PointerArrayOfUInt32 : PointerArray<uint>
	{
		public override uint this[int index]
		{
			get
			{
				unsafe {
					return ((uint*)this.pointer)[index];
				}
			}
			set
			{
				unsafe {
					((uint*)this.pointer)[index] = value;
				}
			}
		}
	}
	public class PointerArrayOfInt64 : PointerArray<long>
	{
		public override long this[int index]
		{
			get
			{
				unsafe {
					return ((long*)this.pointer)[index];
				}
			}
			set
			{
				unsafe {
					((long*)this.pointer)[index] = value;
				}
			}
		}
	}
	public class PointerArrayOfUInt64 : PointerArray<ulong>
	{
		public override ulong this[int index]
		{
			get
			{
				unsafe {
					return ((ulong*)this.pointer)[index];
				}
			}
			set
			{
				unsafe {
					((ulong*)this.pointer)[index] = value;
				}
			}
		}
	}
	public class PointerArrayOfChar : PointerArray<char>
	{
		public override char this[int index]
		{
			get
			{
				unsafe {
					return ((char*)this.pointer)[index];
				}
			}
			set
			{
				unsafe {
					((char*)this.pointer)[index] = value;
				}
			}
		}
	}
	public class PointerArrayOfSingle : PointerArray<float>
	{
		public override float this[int index]
		{
			get
			{
				unsafe {
					return ((float*)this.pointer)[index];
				}
			}
			set
			{
				unsafe {
					((float*)this.pointer)[index] = value;
				}
			}
		}
	}
	public class PointerArrayOfDouble : PointerArray<double>
	{
		public override double this[int index]
		{
			get
			{
				unsafe {
					return ((double*)this.pointer)[index];
				}
			}
			set
			{
				unsafe {
					((double*)this.pointer)[index] = value;
				}
			}
		}
	}
	public class PointerArrayOfDecimal : PointerArray<decimal>
	{
		public override decimal this[int index]
		{
			get
			{
				unsafe {
					return ((decimal*)this.pointer)[index];
				}
			}
			set
			{
				unsafe {
					((decimal*)this.pointer)[index] = value;
				}
			}
		}
	}
	public class PointerArrayOfBoolean : PointerArray<bool>
	{
		public override bool this[int index]
		{
			get
			{
				unsafe {
					return ((bool*)this.pointer)[index];
				}
			}
			set
			{
				unsafe {
					((bool*)this.pointer)[index] = value;
				}
			}
		}
	}
}
