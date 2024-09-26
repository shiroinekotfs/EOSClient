using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace NAudio.CoreAudioApi.Interfaces;

[StructLayout(LayoutKind.Explicit)]
public struct PropVariant
{
	[FieldOffset(0)]
	private short vt;

	[FieldOffset(2)]
	private short wReserved1;

	[FieldOffset(4)]
	private short wReserved2;

	[FieldOffset(6)]
	private short wReserved3;

	[FieldOffset(8)]
	private sbyte cVal;

	[FieldOffset(8)]
	private byte bVal;

	[FieldOffset(8)]
	private short iVal;

	[FieldOffset(8)]
	private ushort uiVal;

	[FieldOffset(8)]
	private int lVal;

	[FieldOffset(8)]
	private uint ulVal;

	[FieldOffset(8)]
	private int intVal;

	[FieldOffset(8)]
	private uint uintVal;

	[FieldOffset(8)]
	private long hVal;

	[FieldOffset(8)]
	private long uhVal;

	[FieldOffset(8)]
	private float fltVal;

	[FieldOffset(8)]
	private double dblVal;

	[FieldOffset(8)]
	private bool boolVal;

	[FieldOffset(8)]
	private int scode;

	[FieldOffset(8)]
	private DateTime date;

	[FieldOffset(8)]
	private System.Runtime.InteropServices.ComTypes.FILETIME filetime;

	[FieldOffset(8)]
	private Blob blobVal;

	[FieldOffset(8)]
	private IntPtr pointerValue;

	public VarEnum DataType => (VarEnum)vt;

	public object Value
	{
		get
		{
			VarEnum dataType = DataType;
			switch (dataType)
			{
			case VarEnum.VT_I1:
				return bVal;
			case VarEnum.VT_I2:
				return iVal;
			case VarEnum.VT_I4:
				return lVal;
			case VarEnum.VT_I8:
				return hVal;
			case VarEnum.VT_INT:
				return iVal;
			case VarEnum.VT_UI4:
				return ulVal;
			case VarEnum.VT_UI8:
				return uhVal;
			case VarEnum.VT_LPWSTR:
				return Marshal.PtrToStringUni(pointerValue);
			case VarEnum.VT_BLOB:
			case (VarEnum)4113:
				return GetBlob();
			case VarEnum.VT_CLSID:
				return (Guid)Marshal.PtrToStructure(pointerValue, typeof(Guid));
			default:
				throw new NotImplementedException("PropVariant " + dataType);
			}
		}
	}

	public static PropVariant FromLong(long value)
	{
		PropVariant result = default(PropVariant);
		result.vt = 20;
		result.hVal = value;
		return result;
	}

	private byte[] GetBlob()
	{
		byte[] array = new byte[blobVal.Length];
		Marshal.Copy(blobVal.Data, array, 0, array.Length);
		return array;
	}

	public T[] GetBlobAsArrayOf<T>()
	{
		int length = blobVal.Length;
		T val = (T)Activator.CreateInstance(typeof(T));
		int num = Marshal.SizeOf(val);
		if (length % num != 0)
		{
			throw new InvalidDataException($"Blob size {length} not a multiple of struct size {num}");
		}
		int num2 = length / num;
		T[] array = new T[num2];
		for (int i = 0; i < num2; i++)
		{
			array[i] = (T)Activator.CreateInstance(typeof(T));
			Marshal.PtrToStructure(new IntPtr((long)blobVal.Data + i * num), array[i]);
		}
		return array;
	}

	public void Clear()
	{
		PropVariantClear(ref this);
	}

	[DllImport("ole32.dll")]
	private static extern int PropVariantClear(ref PropVariant pvar);
}
