using System;
using System.Collections.Generic;

namespace NAudio.Utils;

internal class MergeSort
{
	private static void Sort<T>(IList<T> list, int lowIndex, int highIndex, IComparer<T> comparer)
	{
		if (lowIndex >= highIndex)
		{
			return;
		}
		int num = (lowIndex + highIndex) / 2;
		Sort(list, lowIndex, num, comparer);
		Sort(list, num + 1, highIndex, comparer);
		int num2 = num;
		int num3 = num + 1;
		while (lowIndex <= num2 && num3 <= highIndex)
		{
			if (comparer.Compare(list[lowIndex], list[num3]) <= 0)
			{
				lowIndex++;
				continue;
			}
			T value = list[num3];
			for (int num4 = num3 - 1; num4 >= lowIndex; num4--)
			{
				list[num4 + 1] = list[num4];
			}
			list[lowIndex] = value;
			lowIndex++;
			num2++;
			num3++;
		}
	}

	public static void Sort<T>(IList<T> list) where T : IComparable<T>
	{
		Sort(list, 0, list.Count - 1, Comparer<T>.Default);
	}

	public static void Sort<T>(IList<T> list, IComparer<T> comparer)
	{
		Sort(list, 0, list.Count - 1, comparer);
	}
}
